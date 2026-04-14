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
            ///==========================================
            ///FOX MANAGEMENT
            ///==========================================

            {
                //iesire spre UI: Model -> DTO

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

                //intrarea din UI în Model: DTO -> Model

                // 3. CreateFoxDto

                CreateMap<CreateFoxDto, Fox>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                    .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()) // Îl pui tu manual în Service după upload

                    // 2. Ignorăm obiectele de navigare (pentru că folosim StatusId și EnclosureId din DTO)
                    .ForMember(dest => dest.Status, opt => opt.Ignore())
                    .ForMember(dest => dest.FoxEnclosure, opt => opt.Ignore()) // În screenshot-ul tău apare "FoxEnclosure", pune numele exact din clasa Fox

                    // 3. Ignorăm ID-urile locațiilor (pentru că le vom crea noi prin coordonate)
                    .ForMember(dest => dest.FirstSeenLocationId, opt => opt.Ignore())
                    .ForMember(dest => dest.LastSeenLocationId, opt => opt.Ignore())

                    // 4. Maparea coordonatelor (ceea ce am scris anterior)
                    .ForPath(dest => dest.FirstSeenLocation.Coordinate.Latitude, opt => opt.MapFrom(src => src.FirstSeenLatitude))
                    .ForPath(dest => dest.FirstSeenLocation.Coordinate.Longitude, opt => opt.MapFrom(src => src.FirstSeenLongitude))
                    .ForPath(dest => dest.LastSeenLocation.Coordinate.Latitude, opt => opt.MapFrom(src => src.FirstSeenLatitude))
                    .ForPath(dest => dest.LastSeenLocation.Coordinate.Longitude, opt => opt.MapFrom(src => src.FirstSeenLongitude))
                    .ForMember(dest => dest.Comments, opt => opt.Ignore())
                    .ForMember(dest => dest.Reports, opt => opt.Ignore())
                    .ForMember(dest => dest.Adoptions, opt => opt.Ignore());

                //// 4. UpdateFoxDto

                CreateMap<UpdateFoxDto, Fox>()
                    // 1. Maparea datelor care vin din DTO
                    .ForPath(dest => dest.LastSeenLocation.Coordinate.Latitude, opt => opt.MapFrom(src => src.Latitude))
                    .ForPath(dest => dest.LastSeenLocation.Coordinate.Longitude, opt => opt.MapFrom(src => src.Longitude))

                    // 2. IGNORĂM câmpurile de sistem și ID-urile (nu se schimbă la update)
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                    .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())

                    // 3. IGNORĂM datele inițiale (cele pe care ai zis că vrei să le protejezi)
                    .ForMember(dest => dest.FirstSeenLocationId, opt => opt.Ignore())
                    .ForMember(dest => dest.FirstSeenLocation, opt => opt.Ignore())

                    // 4. IGNORĂM restul obiectelor de navigare care apar în eroarea ta
                    .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                    .ForMember(dest => dest.Status, opt => opt.Ignore())
                    .ForMember(dest => dest.LastSeenLocationId, opt => opt.Ignore())
                    .ForMember(dest => dest.FoxEnclosure, opt => opt.Ignore()) // Verifică dacă în model e "Enclosure" sau "FoxEnclosure"
                    .ForMember(dest => dest.Comments, opt => opt.Ignore())
                    .ForMember(dest => dest.Reports, opt => opt.Ignore())
                    .ForMember(dest => dest.Adoptions, opt => opt.Ignore());

                //// 5. UpdateFoxStatusDto
                CreateMap<UpdateFoxStatusDto, Fox>()
                    // 1. Singura mapare activă
                    .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.NewStatusId))

                    // 2. IGNORĂM absolut tot restul (cele 14 câmpuri rămase)
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Name, opt => opt.Ignore())
                    .ForMember(dest => dest.Description, opt => opt.Ignore())
                    .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())

                    // Obiecte de navigare și ID-uri de locație
                    .ForMember(dest => dest.Status, opt => opt.Ignore())
                    .ForMember(dest => dest.FirstSeenLocationId, opt => opt.Ignore())
                    .ForMember(dest => dest.FirstSeenLocation, opt => opt.Ignore())
                    .ForMember(dest => dest.LastSeenLocationId, opt => opt.Ignore())
                    .ForMember(dest => dest.LastSeenLocation, opt => opt.Ignore())
                    .ForMember(dest => dest.EnclosureId, opt => opt.Ignore())
                    .ForMember(dest => dest.FoxEnclosure, opt => opt.Ignore())

                    // Colecțiile pe care tocmai le-am adăugat
                    .ForMember(dest => dest.Adoptions, opt => opt.Ignore())
                    .ForMember(dest => dest.Comments, opt => opt.Ignore())
                    .ForMember(dest => dest.Reports, opt => opt.Ignore());

            }

            // ==========================================
            //ADOPTION PROCESS
            // ==========================================   

            {

                // 1. Ieșire: Model -> DTO (Pentru vizualizare)
                CreateMap<Adoption, AdoptionDto>()
                    // Navigăm prin relații pentru datele vulpii și ale utilizatorului
                    .ForMember(dest => dest.FoxName, opt => opt.MapFrom(src => src.Fox.Name))
                    .ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                    .ForMember(dest => dest.ApplicantEmail, opt => opt.MapFrom(src => src.User.Email))
                    // Convertim Enum-ul în String
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.AdoptionStatus.ToString()))
                    // Sincronizăm diferențele de nume dintre Model și DTO
                    .ForMember(dest => dest.SubmittedAt, opt => opt.MapFrom(src => src.RequestDate))
                    .ForMember(dest => dest.ApplicantMessage, opt => opt.MapFrom(src => src.Reason));
                // Notă: dest.AdminComment va fi mapat automat DOAR dacă adaugi public string? AdminComment {get;set;} în clasa Adoption!

                // 2. Intrare: DTO -> Model (Când un user cere o adopție)
                CreateMap<AdoptionRequestDto, Adoption>()
                    // Mapare manuală pentru câmpul cu nume diferit
                    .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.ApplicantMessage))

                    // Ignorăm câmpurile de sistem și obiectele de navigare (FOARTE IMPORTANT)
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Fox, opt => opt.Ignore())
                    .ForMember(dest => dest.User, opt => opt.Ignore())
                    .ForMember(dest => dest.AdminComment, opt => opt.Ignore())

                    // Setăm valorile de sistem
                    .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.AdoptionStatus, opt => opt.MapFrom(src => AdoptionStatus.Pending))

                    // Securitate: Ignorăm UserId (îl punem manual în Service)
                    .ForMember(dest => dest.UserId, opt => opt.Ignore());




            }



            // ===========================
            // Commments DTO
            // ===========================
            {

                // 1. Maparea de la Entitate la DTO (pentru afișare)
                CreateMap<Comment, CommentDto>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                    .ForMember(dest => dest.EditedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                    .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => src.UpdatedAt != null));

                // 2. Maparea de la DTO la Entitate (pentru creare)
                CreateMap<CreateCommentDto, Comment>()
                    // Ignorăm tot ce setează baza de date sau Service-ul manual
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                    .ForMember(dest => dest.IsVisible, opt => opt.Ignore())
                    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                    .ForMember(dest => dest.UserId, opt => opt.Ignore())
                    .ForMember(dest => dest.User, opt => opt.Ignore())
                    .ForMember(dest => dest.Fox, opt => opt.Ignore());








            }

            //// ===================
            /// Reports
            /// =====================

            {




                // 1. De la DTO la Entitate (Creare)
                CreateMap<CreateReportDto, Report>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.ReporterId, opt => opt.Ignore())
                    .ForMember(dest => dest.Reporter, opt => opt.Ignore())
                    .ForMember(dest => dest.LocationId, opt => opt.Ignore())
                    .ForMember(dest => dest.Location, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                    .ForMember(dest => dest.ReportStatus, opt => opt.Ignore())
                    .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
                  

                // 2. De la Entitate la DTO (Afișare)
                CreateMap<Report, ReportDto>()
                    .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.ReportStatus.ToString()))
                    .ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => src.Reporter != null ? src.Reporter.UserName : "Guest"))
     
                    .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => (double)src.Location.Coordinate.Latitude))
                    .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => (double)src.Location.Coordinate.Longitude));
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