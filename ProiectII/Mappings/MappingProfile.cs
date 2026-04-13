using AutoMapper;
using ProiectII.DTO;
using ProiectII.DTO.AdoptionProcess;
using ProiectII.DTO.AuthAccount;
using ProiectII.DTO.CommentsReport;
using ProiectII.DTO.FoxManagement;
using ProiectII.Models;

namespace ProiectII.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            {
                // ==========================================
                // GET: Model -> DTO (Ieșire date)
                // ==========================================

                // 1. FoxSummaryDto (Liste, tabele)
                CreateMap<Fox, FoxSummaryDto>()
                    .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name));

                // 2. FoxDetailsDto (Vizualizare detaliată / Hartă)
                CreateMap<Fox, FoxDetailsDto>()
                    .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name))
                    // Navigăm prin relații pentru a extrage coordonatele. 
                    // AutoMapper tratează automat cazurile în care LastSeenLocation este null.
                    .ForMember(dest => dest.LastSeenLatitude, opt => opt.MapFrom(src => src.LastSeenLocation.Coordinate.Latitude))
                    .ForMember(dest => dest.LastSeenLongitude, opt => opt.MapFrom(src => src.LastSeenLocation.Coordinate.Longitude));


                //// ==========================================
                //// POST/PUT: DTO -> Model (Intrare date)
                //// ==========================================

                // 3. CreateFoxDto
                CreateMap<CreateFoxDto, Fox>()
                    // Ignorăm imaginea. Salvarea imaginii și generarea ImageUrl se va face manual în FoxService.
                    .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                    // Folosim ForPath pentru a mapa dintr-o proprietate plată într-un obiect imbricat (Owned Type)
                    .ForPath(dest => dest.FirstSeenLocation.Coordinate.Latitude, opt => opt.MapFrom(src => src.FirstSeenLatitude))
                    .ForPath(dest => dest.FirstSeenLocation.Coordinate.Longitude, opt => opt.MapFrom(src => src.FirstSeenLongitude))
                    // Logică de business: La creare, ultima locație cunoscută este aceeași cu prima
                    .ForPath(dest => dest.LastSeenLocation.Coordinate.Latitude, opt => opt.MapFrom(src => src.FirstSeenLatitude))
                    .ForPath(dest => dest.LastSeenLocation.Coordinate.Longitude, opt => opt.MapFrom(src => src.FirstSeenLongitude));

                //// 4. UpdateFoxDto
                //CreateMap<UpdateFoxDto, Fox>()
                //    // Un update de coordonate actualizează doar LastSeenLocation
                //    .ForPath(dest => dest.LastSeenLocation.Coordinate.Latitude, opt => opt.MapFrom(src => src.Latitude))
                //    .ForPath(dest => dest.LastSeenLocation.Coordinate.Longitude, opt => opt.MapFrom(src => src.Longitude))
                //    // Protejăm datele inițiale: FirstSeenLocation nu trebuie suprascris din greșeală la un update
                //    .ForMember(dest => dest.FirstSeenLocationId, opt => opt.Ignore())
                //    .ForMember(dest => dest.FirstSeenLocation, opt => opt.Ignore());

                //// 5. UpdateFoxStatusDto
                //CreateMap<UpdateFoxStatusDto, Fox>()
                //    .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.NewStatusId));
            }














            //CreateMap<Fox, FoxDetailsDto>()
            //    .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name))
            //    .ForMember(dest => dest.LastSeenLatitude, opt => opt.MapFrom(src => src.LastSeenLocation.Coordinate.Latitude))
            //    .ForMember(dest => dest.LastSeenLongitude, opt => opt.MapFrom(src => src.LastSeenLocation.Coordinate.Longitude));

            //// La creare, DTO-ul vine cu coordonate. Ignorăm Id-ul și alte câmpuri pe care le generează sistemul.
            //CreateMap<CreateFoxDto, Fox>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());







            //// ==========================================
            //// 2. ADOPTION PROCESS (Adopții)
            //// ==========================================
            //CreateMap<Adoption, AdoptionDto>()
            //    .ForMember(dest => dest.FoxName, opt => opt.MapFrom(src => src.Fox.Name))
            //    // Transformăm enum-ul în string pentru UI
            //    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.AdoptionStatus.ToString()))
            //    // Aici rezolvăm problema de context: combinăm numele userului
            //    .ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            //// ==========================================
            //// 3. REPORTS & COMMENTS (Rapoarte și Comentarii)
            //// ==========================================
            //CreateMap<Report, ReportDto>()
            //    .ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => $"{src.Reporter.FirstName} {src.Reporter.LastName}"))
            //    .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.ReportStatus.ToString()))
            //    .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Coordinate.Latitude))
            //    .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.Coordinate.Longitude));

            //CreateMap<Comment, CommentDto>()
            //    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            //// ==========================================
            //// 4. USERS & SYSTEM (Utilizatori)
            //// ==========================================
            //CreateMap<ApplicationUser, UserDto>();
        }
    }
}