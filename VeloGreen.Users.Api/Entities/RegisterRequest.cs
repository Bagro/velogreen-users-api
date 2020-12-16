using System.ComponentModel.DataAnnotations;

namespace VeloGreen.Users.Api.Entities
{
    /// <summary>
    /// Request for registering a user
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Email also used as username for logins
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
