namespace ProiectII.DTO.MapTracking
{
    public class CreateEnclosureDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ColorMaskHex { get; set; } = "#FFFFFF";
        public double Opacity { get; set; } = 0.5;
        public uint CenterLocationId { get; set; }

        // Lista de puncte care formează poligonul
        public List<EnclosurePointDto> Points { get; set; } = new List<EnclosurePointDto>();
    }
}
