namespace ProiectII.DTO.MapTracking
{
    public class EnclosurePointDto
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Order { get; set; } // Ordinea în care se unesc punctele (1, 2, 3...)
    }
}
