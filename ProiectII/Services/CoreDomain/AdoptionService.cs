using ProiectII.DTO.AdoptionProcess;
using ProiectII.Interfaces;
using ProiectII.Models;

namespace ProiectII.Services.CoreDomain
{
    public class AdoptionService(IAdoptionRepository adoptionRepository, IFoxRepository foxRepository) : IAdoptionService
    {
        public async Task<AdoptionDto> CreateAdoptionRequestAsync(string userId, AdoptionRequestDto dto)
        {
            var adoption = new Adoption
            {
                FoxId = dto.FoxId,
                UserId = userId,
                RequestDate = DateTime.UtcNow,
                Reason = dto.ApplicantMessage,
                AdoptionStatus = (AdoptionStatus)1 
            };

            await adoptionRepository.AddAsync(adoption);
            await adoptionRepository.SaveChangesAsync();

            
            var adoptions = await adoptionRepository.GetAllWithDetailsAsync();
            var completeAdoption = adoptions.FirstOrDefault(a => a.Id == adoption.Id);

            return MapToDtoList(new[] { completeAdoption ?? adoption }).First();
        }

        public async Task<IEnumerable<AdoptionDto>> GetAllAdoptionsAsync()
        {
            var adoptions = await adoptionRepository.GetAllWithDetailsAsync();
            return MapToDtoList(adoptions);
        }

        public async Task<IEnumerable<AdoptionDto>> GetUserAdoptionsAsync(string userId)
        {
            var adoptions = await adoptionRepository.GetAllWithDetailsAsync();
            var userAdoptions = adoptions.Where(a => a.UserId == userId);
            return MapToDtoList(userAdoptions);
        }

        public async Task<bool> ProcessAdoptionAsync(ProcessAdoptionDto dto)
        {
            var adoption = await adoptionRepository.GetByIdAsync(dto.AdoptionId);
            if (adoption == null) return false;

            AdoptionStatus noulStatus = dto.Status.ToLower() == "approved"
                ? (AdoptionStatus)2 // Approved
                : (AdoptionStatus)3; // Rejected

            adoption.AdoptionStatus = noulStatus;
            adoption.AdminComment = dto.AdminComment;

            adoptionRepository.Update(adoption);

            if (noulStatus == (AdoptionStatus)2)
            {
                var fox = await foxRepository.GetByIdAsync(adoption.FoxId);
                if (fox != null)
                {
                    fox.StatusId = 3; // 3 = Adoptată
                    foxRepository.Update(fox); // Sincron în RAM
                }
            }

            return await adoptionRepository.SaveChangesAsync();
        }

        private static IEnumerable<AdoptionDto> MapToDtoList(IEnumerable<Adoption> adoptions)
        {
            return adoptions.Select(a => new AdoptionDto
            {
                Id = a.Id,
                FoxId = a.FoxId,
                FoxName = a.Fox?.Name ?? "Necunoscut",
                Status = a.AdoptionStatus == (AdoptionStatus)1 ? "Pending" :
                         (a.AdoptionStatus == (AdoptionStatus)2 ? "Approved" : "Rejected"),
                SubmittedAt = a.RequestDate,
                ApplicantMessage = a.Reason,
                AdminComment = a.AdminComment,
                ApplicantName = $"{a.User?.FirstName} {a.User?.LastName}".Trim(),
                ApplicantEmail = a.User?.Email ?? "Necunoscut"
            });
        }
    }
}