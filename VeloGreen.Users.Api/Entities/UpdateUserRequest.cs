using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VeloGreen.Users.Api.Entities
{
    public class UpdateUserRequest
    {
        [Required]
        public Guid Id { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CurrentPassword { get; set; }
        
        public string NewPassword { get; set; }
    }
}
