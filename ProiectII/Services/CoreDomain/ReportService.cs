using AutoMapper;
using ProiectII.DTO.CommentsReport;
using ProiectII.Interfaces;
using ProiectII.Models;

namespace ProiectII.Services.CoreDomain
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IFileStorageService _fileService;
        private readonly IMapper _mapper;

        public ReportService(
            IReportRepository reportRepository,
            ILocationRepository locationRepository,
            IFileStorageService fileService,
            IMapper mapper)
        {
            _reportRepository = reportRepository;
            _locationRepository = locationRepository;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<ReportDto> CreateReportAsync(CreateReportDto dto, string? userId)
        {
            string? imageUrl = null;

            // 1. Salvarea imaginii DOAR dacă a fost trimisă fizic
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                imageUrl = await _fileService.SaveFileAsync(dto.ImageFile, "reports");
            }

            try
            {
                // 2. Crearea Locației
                var location = new Location
                {
                    Name = "Report Location",
                    Coordinate = new Coordinate
                    {
                        Latitude = (decimal)dto.Latitude,
                        Longitude = (decimal)dto.Longitude
                    }
                };

                await _locationRepository.AddAsync(location);
                await _locationRepository.SaveChangesAsync(); // Extragem ID-ul locației

                // 3. Crearea Raportului
                var report = _mapper.Map<Report>(dto);

                report.ImageUrl = imageUrl; // Va fi null dacă nu a fost urcată poză
                report.LocationId = location.Id;
                report.ReporterId = string.IsNullOrWhiteSpace(userId) ? null : userId;
                report.ReportStatus = ReportStatus.Pending;
                report.CreatedAt = DateTime.UtcNow;

                await _reportRepository.AddAsync(report);
                await _reportRepository.SaveChangesAsync();

                var completeReport = await _reportRepository.GetByIdWithDetailsAsync(report.Id);

                if (completeReport == null)
                    throw new Exception("Eroare severă: Raportul a fost salvat, dar nu a putut fi citit înapoi.");

                return _mapper.Map<ReportDto>(completeReport);
            }
            catch (Exception)
            {
                // 4. Rollback manual DOAR dacă a existat o imagine salvată
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    _fileService.DeleteFile(imageUrl);
                }
                throw;
            }
        }

        public async Task<IEnumerable<ReportDto>> GetAllActiveReportsAsync()
        {
            var reports = await _reportRepository.GetAllActiveReportsWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReportDto>>(reports);
        }

        public async Task<ReportDto?> GetReportByIdAsync(uint id)
        {
            var report = await _reportRepository.GetByIdWithDetailsAsync(id);
            return report == null ? null : _mapper.Map<ReportDto>(report);
        }

        public async Task<bool> UpdateReportStatusAsync(uint reportId, UpdateReportStatusDto dto)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return false;

            // Tratăm posibilitatea ca UI-ul să trimită un status invalid
            if (Enum.TryParse<ReportStatus>(dto.Status, true, out var newStatus))
            {
                report.ReportStatus = newStatus;

                _reportRepository.Update(report);
                return await _reportRepository.SaveChangesAsync();
            }

            return false;
        }

        public async Task<bool> DeleteReportAsync(uint reportId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return false;

            if (!string.IsNullOrEmpty(report.ImageUrl))
            {
                _fileService.DeleteFile(report.ImageUrl);
            }

            _reportRepository.Delete(report);
            return await _reportRepository.SaveChangesAsync();
        }
    }
}