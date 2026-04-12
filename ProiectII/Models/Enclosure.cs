using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ProiectII.Models
{
    public class Enclosure // defineste tarcul sau o anumita zona pe o harta, care poate fi asociata cu o locatie sau cu o zona geografica specifica. Acesta poate fi utilizat pentru a delimita o zona in care se afla vulpile sau pentru a defini o zona de interes pe harta.
    {
        [Key]
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$")] // Validare format HEX pe DB
        [StringLength(7)] // force the format to be a 7 digits format 
        public string ColorMaskHex { get; set; } // format "#FF0000"

        [Range(0.0, 1.0)]
        public double Opacity { get; set; } // format 0.5 ( 50% transparency)



        public uint CenterLocationId { get; set; }
        [ForeignKey("CenterLocationId")]

        public virtual Location CenterLocation { get; set; }


        public List<EnclosurePoint> PolygonPoints { get; set; } = new List<EnclosurePoint>();

        public bool ContainsPoint()
        {  return PolygonPoints.Count > 0; 
        }


        public Coordinate GetCenter()
        {
            if (PolygonPoints.Count == 0) return null;
            decimal avgLat = PolygonPoints.Average(p => p.Coordinate.Latitude);
            decimal avgLon = PolygonPoints.Average(p => p.Coordinate.Longitude);
            return new Coordinate { Latitude = avgLat, Longitude = avgLon };
        }

        public double GetArea()
        {
            if (PolygonPoints.Count < 3) return 0; // not a valid polygon
            double area = 0;
            for (int i = 0; i < PolygonPoints.Count; i++)
            {
                var p1 = PolygonPoints[i].Coordinate;
                var p2 = PolygonPoints[(i + 1) % PolygonPoints.Count].Coordinate;
                area += p1.Longitude * p2.Latitude - p2.Longitude * p1.Latitude;
            }
            return Math.Abs(area / 2.0);
        }

    }
}
