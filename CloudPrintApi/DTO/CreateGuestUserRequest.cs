using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace CloudPrintAPI.DTO
{
    internal class CreateGuestUserRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty; // Non-nullable, will be assigned

        [JsonProperty("fullName")]
        public string FullName { get; set; } = string.Empty; // Non-nullable, will be assigned

        [JsonProperty("role")]
        public string? Role { get; set; } // Can be nullable if optional, or has a default on API side

        [JsonProperty("pin")]
        public string? Pin { get; set; } // Nullable

        [JsonProperty("password")]
        public string? Password { get; set; } // Nullable

        [JsonProperty("expirationTimestamp")]
        public string? ExpirationTimestamp { get; set; } // Nullable, or DateTime? if you prefer object representation

        [JsonProperty("sendWelcomeEmail")]
        public bool SendWelcomeEmail { get; set; }

        [JsonProperty("sendExpirationEmail")]
        public bool SendExpirationEmail { get; set; }

        [JsonProperty("welcomeEmailContent")]
        public string? WelcomeEmailContent { get; set; } // Nullable

    }
}
