using System.ComponentModel.DataAnnotations;

namespace ad_jwt_api.Entities
{
    public class AuthRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
