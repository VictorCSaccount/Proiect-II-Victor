using AutoMapper;
using ProiectII.DTO.FoxManagement;
using ProiectII.Interfaces;
using ProiectII.Models;


namespace ProiectII.Services.CoreDomain
{
    public class FoxService : IFoxService
    {
        private readonly IFoxRepository _foxRepository;
        private readonly IMapper _mapper;

        public FoxService(IFoxRepository foxRepository, IMapper mapper)
        {
            _foxRepository = foxRepository;
            _mapper = mapper;
        }

        public async Task<FoxDetailsDto> CreateFoxAsync(CreateFoxDto dto)
        {


            ////////////////cred ca maitrebuie modificiat
            var foxEntity = _mapper.Map<Fox>(dto);

            foxEntity.IsDeleted = false;

            await _foxRepository.AddAsync(foxEntity);

            return _mapper.Map<FoxDetailsDto>(foxEntity);
        }


        public async Task<IEnumerable<FoxSummaryDto>> GetAllFoxesAsync()
        {
            // 1. Iei datele brute din baza de date
            var foxes = await _foxRepository.GetAllAsync();

            // 2. Le transformi în DTO-uri (traducerea)
            return _mapper.Map<IEnumerable<FoxSummaryDto>>(foxes);
        }

        public async Task<FoxDetailsDto?> GetFoxByIdAsync(uint id)
        {
            var fox = await _foxRepository.GetByIdAsync(id);
            if (fox == null) return null;

            return _mapper.Map<FoxDetailsDto>(fox);
        }
    }
}
