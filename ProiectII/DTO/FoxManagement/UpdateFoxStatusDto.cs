using System.ComponentModel.DataAnnotations;

namespace ProiectII.DTO.FoxManagement
{
    public class UpdateFoxStatusDto
    {

        public uint FoxId { get; set; }

        [Required(ErrorMessage = "Status ID is mandatory")]
        [Range(1, uint.MaxValue, ErrorMessage = "Status ID must be a positive number !!.")]
        public uint NewStatusId { get; set; }
    }
}
