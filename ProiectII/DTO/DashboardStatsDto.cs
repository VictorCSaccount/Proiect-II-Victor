namespace ProiectII.DTO
{
    public class DashboardStatsDto
    {
        public int TotalFoxes { get; set; }
        public int PendingAdoptions { get; set; }
        public int ActiveReports { get; set; }
        public int TotalUsers { get; set; }
        public List<RecentActivityDto> RecentActivities { get; set; } = [];
    }
}
