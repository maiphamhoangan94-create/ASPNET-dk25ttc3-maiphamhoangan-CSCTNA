namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    public class RecipeListViewModel
    {
        public List<Post> Recipes { get; set; } = new();

        public List<Category> Categories { get; set; } = new();

        public int? SelectedCategoryId { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalRecipes { get; set; }

        public int PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
