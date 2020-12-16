using System;
using System.ComponentModel.DataAnnotations;

namespace VeloGreen.Users.Api.Entities
{
    /// <summary>
    /// Request for updating a user
    /// </summary>
    public class UpdateUserRequest
    {
        /// <summary>
        /// User identifier
        /// </summary>
        [Required]
        public Guid Id { get; set; }
        
        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Current password. Must be set to change password. Leave empty if not changing password
        /// </summary>
        public string CurrentPassword { get; set; }
        
        /// <summary>
        /// New password. Must be set to change password. Leave empty if not changing password
        /// </summary>
        public string NewPassword { get; set; }
    }
}
