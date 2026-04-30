using ProiectII.DTO.AdoptionProcess;

namespace ProiectII.Interfaces
{
    public interface IAdoptionService
    {
        Task<AdoptionDto> CreateAdoptionRequestAsync(string userId, AdoptionRequestDto dto);
        Task<IEnumerable<AdoptionDto>> GetAllAdoptionsAsync();
        Task<IEnumerable<AdoptionDto>> GetUserAdoptionsAsync(string userId);
        Task<bool> ProcessAdoptionAsync(ProcessAdoptionDto dto);
    }
}