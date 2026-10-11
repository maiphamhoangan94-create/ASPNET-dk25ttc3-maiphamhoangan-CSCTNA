using System.ComponentModel.DataAnnotations;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Tên đăng nhập hoặc Email")]
        [Display(Name = "Tên đăng nhập / Email")]
        public string UsernameOrEmail { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }
}
