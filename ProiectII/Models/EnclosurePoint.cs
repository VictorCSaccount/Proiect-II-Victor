using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProiectII.Models
{
    public class EnclosurePoint
    {
        [Key]
        public uint Id { get; set; }
        // Importante pentru ordinea in care se trasează liniile poligonului/tarcului pe hartă

        [Required]
        public int DrawOrder { get; set; } // tehonically not needed, but it can be useful for the frontend to know in which order to connect the points to draw the enclosure on the map

        [Required]
        public uint EnclosureId { get; set; }

        
        [ForeignKey("EnclosureId")] 
        public bool IsActive { get; set; }
        [MaxLength(200)]
        public string? Note { get; set; } = null;

        [Required]
        public Coordinate Coordinate { get; set; }

    }
}
