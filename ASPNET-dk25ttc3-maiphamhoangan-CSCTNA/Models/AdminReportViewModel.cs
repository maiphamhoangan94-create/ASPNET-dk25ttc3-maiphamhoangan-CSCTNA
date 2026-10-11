namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    public class AdminReportViewModel
    {
        public int TotalRecipes { get; set; }
        public int TotalKitchenCorners { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCategories { get; set; }
        public int PendingPosts { get; set; }
        public int ApprovedPosts { get; set; }
        public int RejectedPosts { get; set; }
        public int TotalComments { get; set; }
        public int TotalRatings { get; set; }
        public double AverageRating { get; set; }
        public List<ChartDataPoint> PostsByCategory { get; set; } = new();
        public List<ChartDataPoint> PostsByStatus { get; set; } = new();
        public List<ChartDataPoint> MonthlyPosts { get; set; } = new();
        public List<ChartDataPoint> EngagementSummary { get; set; } = new();
    }

    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
    }
}
