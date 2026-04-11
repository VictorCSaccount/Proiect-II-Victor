using System.ComponentModel.DataAnnotations;
namespace ProiectII.Models
{
    public class EnclosurePoint
    {
        [Key]
        public uint Id { get; set; }
        // Importante pentru ordinea in care se trasează liniile poligonului/tarcului pe hartă
        public uint DrawOrder { get; set; } // tehonically not needed, but it can be useful for the frontend to know in which order to connect the points to draw the enclosure on the map
        public uint EnclosureId { get; set; }
        public required Enclosure Enclosure { get; set; }

        public bool IsActive { get; set; }

        public string? Note { get; set; } = null;

        public Coordinate Coordinate { get; set; }

    }
}
