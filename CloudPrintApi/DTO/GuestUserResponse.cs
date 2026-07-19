using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CloudPrintAPI.DTO
{
    public class GuestUserResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? ResponseBody { get; set; }
        public bool IsSuccessStatusCode { get; set; }

        [JsonProperty("tenantId")]
        public string? TenantId { get; set; }

        [JsonProperty("sortOrder")]
        public string? SortOrder { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; } 

        [JsonProperty("users")]
        public List<GuestUser>? Users { get; set; }

        [JsonProperty("page")]
        public GuestUserPaginationInfo? Page { get; set; }
        
    }

    public class GuestUser
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("tenantId")]
        public string? TenantId { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("fullName")]
        public string? FullName { get; set; }

        [JsonProperty("role")]
        public string? Role { get; set; }

        [JsonProperty("pin")]
        public string? Pin { get; set; }

        [JsonProperty("expirationTimestamp")]
        public string? ExpirationTimestamp { get; set; }

        [JsonProperty("sendWelcomeEmail")]
        public bool SendWelcomeEmail { get; set; }

        [JsonProperty("sendExpirationEmail")]
        public bool SendExpirationEmail { get; set; }

        [JsonProperty("dateAdded")]
        public string? DateAdded { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }
    }

    public class GuestUserPaginationInfo
    {
        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("totalElements")]
        public int TotalElements { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }
    }
}
