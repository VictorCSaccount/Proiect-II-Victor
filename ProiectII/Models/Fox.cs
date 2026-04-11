using System.ComponentModel.DataAnnotations;

namespace ProiectII.Models
{
    public class Fox
    {
        [Key]
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } //description of the fox, can be used to provide more information about the fox, such as its
        public string ImageUrl { get; set; }
        public bool isDeleted { get; set; }
        public bool IsAdopted { get; set; }

        //first known location of the fox, can be used to provide more information about the origin of the fox, or to show the location of the fox on a map. This property can be set when the fox is first reported, and it can help users understand where the fox was first seen, and how it has moved over time.
        public uint? FirstSeenLocationId { get; set; }
        public Location FirstSeenLocation { get; set; }

        // Last known location of the fox, can be used to provide more information about the current location of the fox, or to show the location of the fox on a map. This property can be updated when new sightings of the fox are reported, and it can help users track the movement of the fox over time.
        public uint? LastSeenLocationId { get; set; }
        public Location LastSeenLocation { get; set; }

        public uint? EnclosureId { get; set; } // tarcul sau zona geografica asociata cu vulpea, poate fi null daca vulpea nu este asociata cu niciun tarc sau zona geografica specifica
        public Enclosure? FoxEnclosure { get; set; }


        public void Adopt()
        {
            IsAdopted = true;
        }

        public bool isHealthy()
        {

            return true; // presupunem ca toate vulpile sunt sanatoase pentru simplificare
        }

    }

 }

