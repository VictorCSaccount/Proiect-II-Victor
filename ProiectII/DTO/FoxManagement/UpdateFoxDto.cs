namespace ProiectII.DTO.FoxManagement
{
    public class UpdateFoxDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public uint EnclosureId { get; set; }
    }
}
