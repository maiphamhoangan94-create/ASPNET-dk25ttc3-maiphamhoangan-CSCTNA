using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    [Table("Posts")]
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không vượt quá 200 ký tự")]
        [Display(Name = "Tiêu đề bài viết")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Nội dung")]
        public string? Content { get; set; }

        [StringLength(500)]
        [Display(Name = "Ảnh đại diện")]
        public string? ImageUrl { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Loại bài viết")]
        public string PostType { get; set; } = PostTypes.Recipe;

        [Display(Name = "Bộ ảnh")]
        public string? GalleryImageUrls { get; set; }

        [Display(Name = "Trạng thái")]
        public int Status { get; set; }= 0; // Mặc định: 0 - Chờ duyệt, 1 - Đã duyệt, 2 - Từ chối

        [StringLength(500)]
        [Display(Name = "Lý do từ chối")]
        public string? RejectionReason { get; set; }

        [Display(Name = "Danh mục")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [Required]
        [Display(Name = "Tác giả")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Display(Name = "Người duyệt")]
        public int? ApprovedBy { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User? AdminApprover { get; set; }

        [Display(Name = "Thời gian duyệt")]
        public DateTime? ApprovedAt { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<PostProvince> PostProvinces { get; set; } = new List<PostProvince>();

        public virtual ICollection<PostComment> Comments { get; set; } = new List<PostComment>();

        public virtual ICollection<PostRating> Ratings { get; set; } = new List<PostRating>();

        [NotMapped]
        [Display(Name = "Tỉnh thành")]
        public List<int> SelectedProvinceIds { get; set; } = new();

        [NotMapped]
        [Display(Name = "Nguyên liệu")]
        public string? Ingredients { get; set; }

        [NotMapped]
        public List<string> GalleryImages => (GalleryImageUrls ?? string.Empty)
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(imageUrl => imageUrl.Trim())
            .Where(imageUrl => !string.IsNullOrWhiteSpace(imageUrl))
            .ToList();
    }

    public static class PostTypes
    {
        public const string Recipe = "Recipe";
        public const string KitchenCorner = "KitchenCorner";
    }
}
