using Microsoft.EntityFrameworkCore;

namespace ProiectII.Models
{
    public class Coordinate
    {
        [Precision(18, 10)]
        public decimal Latitude { get; set; }
        [Precision(18, 10)]
        public decimal Longitude { get; set; }
        [Precision(18, 10)]
        public decimal? Altitude { get; set; } = null;
        public bool IsValid()
        {
            return Latitude >= -90 && Latitude <= 90 &&
                   Longitude >= -180 && Longitude <= 180;
        }

    }
}
