namespace ProiectII.DTO.CommentsReport
{
    public class ReportDto
    {
        public uint Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImageUrl { get; set; } = string.Empty; // adresa imaginii -- salvata local sau pe server( daca in viitor facem pe un azure etc..
        public string Status { get; set; } = "Pending"; //il consider dinstart pending.. de vazut daca e bine!!
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public string ReporterName { get; set; }  
    }
}
