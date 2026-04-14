using ProiectII.DTO.FoxManagement;
using ProiectII.Models;



namespace ProiectII.Interfaces
{
    public interface IFoxService
    {
        Task<IEnumerable<FoxSummaryDto>> GetAllFoxesAsync();
        Task<FoxDetailsDto?> GetFoxByIdAsync(uint id);
        Task<FoxDetailsDto> CreateFoxAsync(CreateFoxDto dto);
    }
}
