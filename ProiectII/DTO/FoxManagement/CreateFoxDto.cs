using System.ComponentModel.DataAnnotations;

namespace ProiectII.DTO.FoxManagement
{
    public class CreateFoxDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public uint EnclosureId { get; set; }

        public double FirstSeenLatitude { get; set; }
        public double FirstSeenLongitude     { get; set; }

        // incarcare fisier vulpe!! ( trebuie verificat salvarea imginii..)
        public IFormFile? Image { get; set; }

        public uint StatusId { get; set; }
    }
}
