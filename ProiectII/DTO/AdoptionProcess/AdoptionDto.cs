namespace ProiectII.DTO.AdoptionProcess
{
    /// Obiectul de iesire ce contine detaliile cererii.
    public class AdoptionDto
    {
        public uint Id { get; set; }
        public uint FoxId { get; set; }
        public string FoxName { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime SubmittedAt { get; set; }
        public string ApplicantMessage { get; set; } = string.Empty;
        public string? AdminComment { get; set; }

        public string ApplicantName { get; set; }

        public string ApplicantEmail { get; set;}

    }
}
