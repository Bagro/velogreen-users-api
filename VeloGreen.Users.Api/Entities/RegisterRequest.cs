using System.ComponentModel.DataAnnotations;

namespace VeloGreen.Users.Api.Entities
{
    public class RegisterRequest
    {
        [Required]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
