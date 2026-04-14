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

        //public async Task<ReportDto> CreateReportAsync(CreateReportDto dto, string? userId)
        //{
        //    // 1. Salvarea imaginii pe disc
        //    string imageUrl = await _fileService.SaveFileAsync(dto.ImageFile, "reports");

        //    try
        //    {
        //        // 2. Crearea și salvarea Locației PRIMA DATĂ
        //        var location = new Location
        //        {
        //            Name = "Report Location",
        //            Coordinate = new Coordinate
        //            {
        //                Latitude = (decimal)dto.Latitude,
        //                Longitude = (decimal)dto.Longitude
        //            }
        //        };


        //        await _locationRepository.AddAsync(location);
        //        // CRITIC: Trebuie să salvăm ca să primim location.Id de la MySQL
        //        await _locationRepository.SaveChangesAsync();

        //        // 3. Crearea Raportului
        //        var report = _mapper.Map<Report>(dto);
        //        report.ImageUrl = imageUrl;
        //        report.LocationId = location.Id; // Acum avem un ID real
        //        report.ReporterId = userId;
        //        report.ReportStatus = ReportStatus.Pending;
        //        report.CreatedAt = DateTime.UtcNow;

        //        await _reportRepository.AddAsync(report);
        //        // CRITIC: Salvăm raportul în MySQL
        //        await _reportRepository.SaveChangesAsync();

        //        // Dacă ai o metodă care aduce raportul cu tot cu detalii, o folosești, 
        //        // altfel returnăm direct maparea pe ce am creat
        //        return _mapper.Map<ReportDto>(report);
        //    }
        //    catch (Exception)
        //    {
        //        // Dacă baza de date a crăpat, ștergem poza de pe disc ca să nu ocupe loc degeaba
        //        _fileService.DeleteFile(imageUrl);
        //        throw;
        //    }
        //}


        public async Task<ReportDto> CreateReportAsync(CreateReportDto dto, string? userId)
        {
            // 1. Imaginea
            string imageUrl = await _fileService.SaveFileAsync(dto.ImageFile, "reports");

            // 2. Locația
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
            await _locationRepository.SaveChangesAsync(); // Salvăm ca să avem ID-ul locației

            // 3. MAPAREA (Trebuie să fie prima!)
            // Aceasta curăță obiectul, deci tot ce setăm după ea va rămâne în DB
            var report = _mapper.Map<Report>(dto);

            // 4. SETĂRILE MANUALE (Trebuie să fie ultimele!)
            report.ImageUrl = imageUrl;
            report.LocationId = location.Id;
            report.ReporterId = userId;
            report.ReportStatus = ReportStatus.Pending;
            report.CreatedAt = DateTime.UtcNow;

            // 5. Salvarea Raportului
            await _reportRepository.AddAsync(report);
            await _reportRepository.SaveChangesAsync();

            return _mapper.Map<ReportDto>(report);
        }







        public async Task<IEnumerable<ReportDto>> GetAllActiveReportsAsync()
        {
            // Observație: Asigură-te că metoda GetAllActiveReportsWithDetailsAsync există în ReportRepository
            var reports = await _reportRepository.GetAllActiveReportsWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReportDto>>(reports);
        }

        public async Task<ReportDto?> GetReportByIdAsync(uint id)
        {
            // Observație: Asigură-te că metoda GetByIdWithDetailsAsync există în ReportRepository
            var report = await _reportRepository.GetByIdWithDetailsAsync(id);
            if (report == null) return null;

            return _mapper.Map<ReportDto>(report);
        }

        public async Task<bool> UpdateReportStatusAsync(uint reportId, UpdateReportStatusDto dto)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return false;

            // Convertim string-ul din DTO (ex: "Investigated") în Enum. 
            // Dacă trimiti ID (uint) din frontend, faci cast (ReportStatus)dto.Status
            if (Enum.TryParse<ReportStatus>(dto.Status, true, out var newStatus))
            {
                report.ReportStatus = newStatus;
                // Dacă ai un LastUpdateAt în model, acum e momentul să-l modifici
                // report.LastUpdateAt = DateTime.UtcNow; 

                _reportRepository.Update(report);
                return await _reportRepository.SaveChangesAsync();
            }

            return false; // Status invalid trimis de UI
        }

        public async Task<bool> DeleteReportAsync(uint reportId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return false;

            // Curățenie industrială: Ștergem și fișierul fizic, nu doar rândul din MySQL
            if (!string.IsNullOrEmpty(report.ImageUrl))
            {
                _fileService.DeleteFile(report.ImageUrl);
            }

            _reportRepository.Delete(report);
            return await _reportRepository.SaveChangesAsync();
        }
    }
}