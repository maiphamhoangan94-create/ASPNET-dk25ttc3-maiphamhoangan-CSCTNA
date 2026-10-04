using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    [Table("Provinces")]
    public class Province
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tỉnh thành")]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<PostProvince> PostProvinces { get; set; } = new List<PostProvince>();
    }
}
