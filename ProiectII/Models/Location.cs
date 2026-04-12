using System.ComponentModel.DataAnnotations;

namespace ProiectII.Models
{
    public class Location
    {
        [Key]
        public uint Id { get; set; }

        // place name or description of the location, can be used to provide more information about the location of the foxes, such as the name of the park, the street, or any other relevant information that can help users understand where the foxes are located.
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        // geolocation coordinates of the foxes, can be used to show the location of the foxes on a map, or to provide information about the location of the foxes in relation to other points of interest, such as parks, streets, or other landmarks. The coordinates can be used to calculate distances between the foxes and other locations, or to provide directions to the location of the foxes.
        [Required]
        public Coordinate Coordinate { get; set; }
        

        // sometimes the coordinates of the foxes are not exact, so we can use this property to define a radius around the coordinates, in which the fox is located. This is useful for the cases when we have only approximate coordinates for the foxes, or when we want to show a general area where the fox is located, instead of a specific point on the map.
        public double PrecisionRadius { get; set; } // precision in meters , defines the area/circle on the map

        public double GetDistanceTo(Location otherLocation)
        {
            // Implement the Haversine formula to calculate the distance between two locations based on their coordinates
            double R = 6371e3; // Earth radius in meters
            double lat1 = Coordinate.Latitude * Math.PI / 180;
            double lat2 = otherLocation.Coordinate.Latitude * Math.PI / 180;
            double deltaLat = (otherLocation.Coordinate.Latitude - Coordinate.Latitude) * Math.PI / 180;
            double deltaLon = (otherLocation.Coordinate.Longitude - Coordinate.Longitude) * Math.PI / 180;
            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // distance in meters
        }
    }
}
