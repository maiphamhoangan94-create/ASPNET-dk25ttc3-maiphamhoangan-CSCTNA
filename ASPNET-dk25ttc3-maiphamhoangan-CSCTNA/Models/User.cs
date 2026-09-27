using System;
using System.ComponentModel.DataAnnotations;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}