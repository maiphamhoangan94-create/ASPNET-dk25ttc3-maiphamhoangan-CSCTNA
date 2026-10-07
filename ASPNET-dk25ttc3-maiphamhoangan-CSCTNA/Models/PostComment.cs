using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    [Table("PostComments")]
    public class PostComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [ForeignKey("PostId")]
        public virtual Post? Post { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public int? ParentCommentId { get; set; }

        [ForeignKey("ParentCommentId")]
        public virtual PostComment? ParentComment { get; set; }

        public virtual ICollection<PostComment> Replies { get; set; } = new List<PostComment>();

        [Required(ErrorMessage = "Vui lòng nhập nội dung bình luận.")]
        [StringLength(1000, ErrorMessage = "Bình luận không vượt quá 1000 ký tự.")]
        [Display(Name = "Nội dung bình luận")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Ngày bình luận")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
