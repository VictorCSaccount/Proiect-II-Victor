using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectII.Models
{
    public class Fox
    {
        [Key]
        public uint Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        //[Text]
        [Column(TypeName = "text")]
        public string Description { get; set; } //description of the fox, can be used to provide more information about the fox, such as its
        [MaxLength(512)]
        public string ImageUrl { get; set; }

        [Required]
        public uint StatusId { get; set; } // (FK)
        [ForeignKey("StatusId")]
        public Status Status { get; set; } // Proprietatea de navigare

        public bool IsDeleted { get; set; }

        //first known location of the fox, can be used to provide more information about the origin of the fox, or to show the location of the fox on a map. This property can be set when the fox is first reported, and it can help users understand where the fox was first seen, and how it has moved over time.
        
        public uint? FirstSeenLocationId { get; set; }
        [ForeignKey("FirstSeenLocationId")]
        public Location? FirstSeenLocation { get; set; }

        // Last known location of the fox, can be used to provide more information about the current location of the fox, or to show the location of the fox on a map. This property can be updated when new sightings of the fox are reported, and it can help users track the movement of the fox over time.
        public uint? LastSeenLocationId { get; set; }

        [ForeignKey("LastSeenLocationId")]
        public Location LastSeenLocation { get; set; }

        public uint? EnclosureId { get; set; } // tarcul sau zona geografica asociata cu vulpea, poate fi null daca vulpea nu este asociata cu niciun tarc sau zona geografica specifica
        public Enclosure? FoxEnclosure { get; set; }


        public virtual ICollection<Adoption>? Adoptions { get; set; } = new List<Adoption>(); 
        public virtual ICollection<Comment>? Comments { get; set; } = new List<Comment>();

        public virtual ICollection<Report>? Reports { get; set; } = new List<Report>();



        public void Adopt(uint adoptedStatusId)
        {
            this.StatusId = adoptedStatusId;
        }


        public bool IsHealthy()
        {

            return true; // presupunem ca toate vulpile sunt sanatoase pentru simplificare
        }

        public bool UpdateLocation(double lat, double lon)
        {
            // Logica pentru a crea o nouă locație sau a updata LastSeen
            return true;
        }

        public int GetDaysInShelter()
        {
            // Aici ai nevoie de un câmp "CreatedAt" sau "EntryDate" (lipsește din model!)
            return 0;
        }

        public void ChangeStatus(uint newStatusId)
        {
            this.StatusId = newStatusId;
        }


    }

 }

