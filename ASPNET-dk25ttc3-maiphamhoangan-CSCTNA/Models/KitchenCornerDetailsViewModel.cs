namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    public class KitchenCornerDetailsViewModel
    {
        public Post KitchenCorner { get; set; } = new();

        public List<Post> SharedRecipes { get; set; } = new();
    }
}
