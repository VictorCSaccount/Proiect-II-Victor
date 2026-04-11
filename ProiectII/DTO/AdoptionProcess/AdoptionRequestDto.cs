namespace ProiectII.DTO.AdoptionProcess
{
    /// <summary>
    /// se trimite de la utilizator cererea de adoptie,,,
    /// </summary>
    public class AdoptionRequestDto
    {
        public uint FoxId { get; set; }
        public string ApplicantMessage { get; set; } = string.Empty;
    }
}
