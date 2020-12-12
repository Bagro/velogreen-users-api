using System.ComponentModel.DataAnnotations;

namespace VeloGreen.Users.Api.Entities
{
    public class AuthenticationRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
