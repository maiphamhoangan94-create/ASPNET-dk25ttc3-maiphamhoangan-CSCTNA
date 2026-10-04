using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    [Table("PostProvinces")]
    public class PostProvince
    {
        public int PostId { get; set; }

        [ForeignKey("PostId")]
        public virtual Post? Post { get; set; }

        public int ProvinceId { get; set; }

        [ForeignKey("ProvinceId")]
        public virtual Province? Province { get; set; }
    }
}
