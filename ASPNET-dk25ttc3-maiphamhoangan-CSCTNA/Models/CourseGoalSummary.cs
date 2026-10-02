namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    public class CourseGoalSummary
    {
        public int UserCount { get; set; }
        public int CategoryCount { get; set; }
        public int PostCount { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }
}
