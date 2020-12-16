using System;
using System.Text.Json.Serialization;

namespace VeloGreen.Users.Api.Entities
{
    /// <summary>
    /// User
    /// </summary>
    public class User
    {
        /// <summary>
        /// User identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}
