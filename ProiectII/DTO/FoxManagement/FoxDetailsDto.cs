
namespace ProiectII.DTO.FoxManagement
{
    public class FoxDetailsDto
    {
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double LastSeenLatitude { get; set; }
        public double LastSeenLongitude { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
    }
}
