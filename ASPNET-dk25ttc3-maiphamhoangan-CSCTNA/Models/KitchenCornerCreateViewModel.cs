using System.ComponentModel.DataAnnotations;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    public class KitchenCornerCreateViewModel
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không vượt quá 200 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Câu chuyện góc bếp")]
        public string? Content { get; set; }

        [Required(ErrorMessage = "Vui lòng tải lên từ 1 đến 5 hình ảnh")]
        [Display(Name = "Hình ảnh góc bếp")]
        public List<IFormFile> Images { get; set; } = new();
    }
}
