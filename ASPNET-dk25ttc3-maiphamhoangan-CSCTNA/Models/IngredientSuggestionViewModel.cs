namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    public class IngredientSuggestionViewModel
    {
        public List<string> AvailableIngredients { get; set; } = new();

        public List<IngredientUsage> IngredientUsage { get; set; } = new();

        public List<string> SelectedIngredients { get; set; } = new();

        public List<RecipeIngredientSuggestion> Suggestions { get; set; } = new();
    }

    public class IngredientUsage
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class RecipeIngredientSuggestion
    {
        public Post Recipe { get; set; } = new();

        public List<string> RecipeIngredients { get; set; } = new();

        public List<string> MatchedIngredients { get; set; } = new();

        public int MissingIngredientCount => Math.Max(RecipeIngredients.Count - MatchedIngredients.Count, 0);

        public int MatchPercent => RecipeIngredients.Count == 0
            ? 0
            : (int)Math.Round(MatchedIngredients.Count * 100d / RecipeIngredients.Count);
    }
}
