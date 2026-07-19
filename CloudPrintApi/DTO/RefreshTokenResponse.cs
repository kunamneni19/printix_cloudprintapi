using System.Net;
using System.Text.Json.Serialization; // Required for JSON mapping attributes

namespace CloudPrintAPI.DTO
{
    internal class RefreshTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        // Mapped to "refresh_token" as typically returned by OAuth token refresh endpoints
        [JsonPropertyName("refresh_token")]
        public string? NewRefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int? ExpiresIn { get; set; }

        // These properties are populated manually in your utility class, so they don't need JSON attributes:
        public HttpStatusCode StatusCode { get; set; }
        public string? ResponseBody { get; set; }
        public bool IsSuccessStatusCode => StatusCode == HttpStatusCode.OK;
    }
}