namespace ProiectII.DTO.MapTracking
{
    public class CreateEnclosureDto
    {
        public string Name { get; set; } = string.Empty;

        public string HexColor { get; set; } = "#FFFFFF";

        public List<CoordinateDto> Points { get; set; } = new();

        public double Opacity { get; set; } = 0.5;

        public CoordinateDto Center { get; set; } = new();
    }
}
