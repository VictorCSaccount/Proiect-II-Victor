namespace ProiectII.Models
{
    public class Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }

        public bool IsValid()
        {
            return Latitude >= -90 && Latitude <= 90 &&
                   Longitude >= -180 && Longitude <= 180;
        }

    }
}
