namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    public class ProvinceExploreViewModel
    {
        public List<ProvinceFoodGroup> ProvinceGroups { get; set; } = new();

        public int? SelectedProvinceId { get; set; }

        public int TotalProvinceCount { get; set; }

        public int TotalRecipeCount { get; set; }

        public string? SelectedProvinceName { get; set; }
    }

    public class ProvinceFoodGroup
    {
        public Province Province { get; set; } = new();

        public List<Post> Posts { get; set; } = new();
    }
}
