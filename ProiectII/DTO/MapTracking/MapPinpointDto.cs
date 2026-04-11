namespace ProiectII.DTO.MapTracking
{
    public class MapPinpointDto
    {
        public uint Id { get; set; }

        // example Fox, Report Staff
        public string Type { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // detalii suplimentare
        public string Label { get; set; } = string.Empty;
    }
}
