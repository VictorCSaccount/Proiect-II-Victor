using AutoMapper;
using ProiectII.DTO.AdoptionProcess;
using ProiectII.DTO.CommentsReport;
using ProiectII.DTO.FoxManagement;
using ProiectII.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProiectII.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ==========================================
            // 1. FOX MANAGEMENT
            // ==========================================

            // Ieșire spre UI
            CreateMap<Fox, FoxSummaryDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name));

            CreateMap<Fox, FoxDetailsDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dest => dest.LastSeenLatitude, opt => opt.MapFrom(src => src.LastSeenLocation != null ? src.LastSeenLocation.Coordinate.Latitude : 0))
                .ForMember(dest => dest.LastSeenLongitude, opt => opt.MapFrom(src => src.LastSeenLocation != null ? src.LastSeenLocation.Coordinate.Longitude : 0));

            // Intrare din UI
            CreateMap<CreateFoxDto, Fox>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.FoxEnclosure, opt => opt.Ignore())
                .ForMember(dest => dest.FirstSeenLocationId, opt => opt.Ignore())
                .ForMember(dest => dest.LastSeenLocationId, opt => opt.Ignore())
                .ForPath(dest => dest.FirstSeenLocation.Coordinate.Latitude, opt => opt.MapFrom(src => src.FirstSeenLatitude))
                .ForPath(dest => dest.FirstSeenLocation.Coordinate.Longitude, opt => opt.MapFrom(src => src.FirstSeenLongitude))
                .ForPath(dest => dest.LastSeenLocation.Coordinate.Latitude, opt => opt.MapFrom(src => src.FirstSeenLatitude))
                .ForPath(dest => dest.LastSeenLocation.Coordinate.Longitude, opt => opt.MapFrom(src => src.FirstSeenLongitude))
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.Reports, opt => opt.Ignore())
                .ForMember(dest => dest.Adoptions, opt => opt.Ignore());

            CreateMap<UpdateFoxDto, Fox>()
                .ForPath(dest => dest.LastSeenLocation.Coordinate.Latitude, opt => opt.MapFrom(src => src.Latitude))
                .ForPath(dest => dest.LastSeenLocation.Coordinate.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.FirstSeenLocationId, opt => opt.Ignore())
                .ForMember(dest => dest.FirstSeenLocation, opt => opt.Ignore())
                .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.LastSeenLocationId, opt => opt.Ignore())
                .ForMember(dest => dest.FoxEnclosure, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.Reports, opt => opt.Ignore())
                .ForMember(dest => dest.Adoptions, opt => opt.Ignore());

            CreateMap<UpdateFoxStatusDto, Fox>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.NewStatusId))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.FirstSeenLocationId, opt => opt.Ignore())
                .ForMember(dest => dest.FirstSeenLocation, opt => opt.Ignore())
                .ForMember(dest => dest.LastSeenLocationId, opt => opt.Ignore())
                .ForMember(dest => dest.LastSeenLocation, opt => opt.Ignore())
                .ForMember(dest => dest.EnclosureId, opt => opt.Ignore())
                .ForMember(dest => dest.FoxEnclosure, opt => opt.Ignore())
                .ForMember(dest => dest.Adoptions, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.Reports, opt => opt.Ignore());


            CreateMap<Fox, FoxMapMarkerDto>()
                .ForMember(dest => dest.FoxId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : "Necunoscut"))

                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src =>
                    src.LastSeenLocation != null ? (double)src.LastSeenLocation.Coordinate.Latitude :
                    (src.FirstSeenLocation != null ? (double)src.FirstSeenLocation.Coordinate.Latitude : 0)))

                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src =>
                    src.LastSeenLocation != null ? (double)src.LastSeenLocation.Coordinate.Longitude :
                    (src.FirstSeenLocation != null ? (double)src.FirstSeenLocation.Coordinate.Longitude : 0)));




            // ==========================================
            // 2. ADOPTION PROCESS
            // ==========================================
            CreateMap<Adoption, AdoptionDto>()
                .ForMember(dest => dest.FoxName, opt => opt.MapFrom(src => src.Fox.Name))
                .ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.ApplicantEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.AdoptionStatus.ToString()))
                .ForMember(dest => dest.SubmittedAt, opt => opt.MapFrom(src => src.RequestDate))
                .ForMember(dest => dest.ApplicantMessage, opt => opt.MapFrom(src => src.Reason));

            CreateMap<AdoptionRequestDto, Adoption>()
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.ApplicantMessage))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Fox, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.AdminComment, opt => opt.Ignore())
                .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.AdoptionStatus, opt => opt.MapFrom(src => AdoptionStatus.Pending))
                .ForMember(dest => dest.UserId, opt => opt.Ignore());


            // ==========================================
            // 3. COMMENTS
            // ==========================================
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.EditedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => src.UpdatedAt != null));

            CreateMap<CreateCommentDto, Comment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsVisible, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Fox, opt => opt.Ignore());


            // ==========================================
            // 4. REPORTS
            // ==========================================
            CreateMap<CreateReportDto, Report>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ReporterId, opt => opt.Ignore())
                .ForMember(dest => dest.Reporter, opt => opt.Ignore())
                .ForMember(dest => dest.LocationId, opt => opt.Ignore())
                .ForMember(dest => dest.Location, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ReportStatus, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<Report, ReportDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.ReportStatus.ToString()))
                .ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => src.Reporter != null ? src.Reporter.UserName : "Guest"))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location != null ? (double)src.Location.Coordinate.Latitude : 0))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location != null ? (double)src.Location.Coordinate.Longitude : 0));
        }
    }
}