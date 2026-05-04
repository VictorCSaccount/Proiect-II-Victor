using ProiectII.DTO.FoxManagement;
using ProiectII.Models;



namespace ProiectII.Interfaces
{
    public interface IFoxService
    {
        Task<IEnumerable<FoxSummaryDto>> GetAllFoxesAsync();
        Task<FoxDetailsDto?> GetFoxByIdAsync(uint id);
        Task<FoxDetailsDto> CreateFoxAsync(CreateFoxDto dto);
        Task<bool> ArchiveFoxAsync(uint id);
        Task<bool> UpdateFoxStatusAsync(uint id, UpdateFoxStatusDto dto);
        Task<bool> UpdateFoxAsync(uint id, UpdateFoxDto dto);

        Task<IEnumerable<FoxMapMarkerDto>> GetMapMarkersAsync();
    }
}
