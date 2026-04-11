namespace ProiectII.DTO.MapTracking
{
    public class EnclosureMapDto
    {
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;

        //  HEX color format: "#FF5733'
        public string HexColor { get; set; } = "#00FF00";

        // Lista de puncte care formează conturul pe hartă
        public List<CoordinateDto> Points { get; set; } = new();

        // Nivelul de transparență (0.0 - 1.0)
        public double Opacity { get; set; } = 0.5;

        
        public CoordinateDto Center { get; set; } = new CoordinateDto();

    }
}
