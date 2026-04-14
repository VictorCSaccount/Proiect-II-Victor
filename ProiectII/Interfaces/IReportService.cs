using ProiectII.DTO.CommentsReport;

namespace ProiectII.Interfaces
{
    public interface IReportService
    {
        Task<ReportDto> CreateReportAsync(CreateReportDto dto, string? userId);
        Task<IEnumerable<ReportDto>> GetAllActiveReportsAsync();
        Task<ReportDto?> GetReportByIdAsync(uint id);
        Task<bool> UpdateReportStatusAsync(uint reportId, UpdateReportStatusDto dto);
        Task<bool> DeleteReportAsync(uint reportId);
    }



}
