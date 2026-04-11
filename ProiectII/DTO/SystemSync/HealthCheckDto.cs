namespace ProiectII.DTO.SystemSync
{
    public class HealthCheckDto
    {
        
        public string Status { get; set; } = string.Empty;

        public string Component { get; set; } = string.Empty;

        public int LatencyMs { get; set; }
    }
}
