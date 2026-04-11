namespace ProiectII.DTO.AdoptionProcess
{
    public class ProcessAdoptionDto
    {
        public uint AdoptionId { get; set; }

        //se trimit valorile standard pentru status( din enum...) sub forma de string
        public string Status { get; set; } = string.Empty;
        public string AdminComment { get; set; } = string.Empty;
    }
}
