using System.ComponentModel.DataAnnotations;

namespace VeloGreen.Users.Api.Entities
{
    /// <summary>
    /// Request for authenticating a user
    /// </summary>
    public class AuthenticationRequest
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
