using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Role { get; set; } = "User";
        public virtual ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
        public virtual ICollection<PostRating> Ratings { get; set; } = new List<PostRating>();
    }
}
