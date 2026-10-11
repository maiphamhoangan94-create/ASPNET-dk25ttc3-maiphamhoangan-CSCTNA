namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    public class HomeIndexViewModel
    {
        public List<Category> TasteCategories { get; set; } = new();

        public List<Post> SuggestedPosts { get; set; } = new();

        public Post? FeaturedKitchenCorner { get; set; }

        public Post? FeaturedRecipe { get; set; }

        public double FeaturedKitchenAverageRating { get; set; }

        public double FeaturedRecipeAverageRating { get; set; }

        public double FeaturedAuthorRecipeAverageRating { get; set; }
    }
}
