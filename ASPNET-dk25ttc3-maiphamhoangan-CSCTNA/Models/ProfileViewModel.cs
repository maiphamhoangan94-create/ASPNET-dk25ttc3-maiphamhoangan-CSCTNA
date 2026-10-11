namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; } = new();

        public bool IsOwner { get; set; }

        public List<ProfilePostViewModel> Posts { get; set; } = new();

        public int TotalPosts => Posts.Count;

        public int ApprovedPosts => Posts.Count(post => post.Status == 1);

        public int PendingPosts => Posts.Count(post => post.Status == 0);

        public int RejectedPosts => Posts.Count(post => post.Status == 2);

        public int TotalComments => Posts.Sum(post => post.CommentCount);

        public int TotalRatings => Posts.Sum(post => post.RatingCount);

        public double AverageRating => TotalRatings > 0
            ? Posts.Sum(post => post.RatingScoreTotal) / TotalRatings
            : 0;
    }

    public class ProfilePostViewModel
    {
        public Post Post { get; set; } = new();

        public int CommentCount { get; set; }

        public int RatingCount { get; set; }

        public int RatingScoreTotal { get; set; }

        public double AverageRating => RatingCount > 0
            ? RatingScoreTotal / (double)RatingCount
            : 0;

        public int Status => Post.Status;
    }
}
