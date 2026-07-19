using System.Net;
using System.Text.Json.Serialization;

namespace CloudPrintAPI.DTO
{
    internal class CloudPrintAuthTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int? ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        // These are set manually by your utility class, so they don't need JSON attributes:
        public HttpStatusCode StatusCode { get; set; }
        public string? ResponseContent { get; set; }
        public bool IsSuccessStatusCode => StatusCode == HttpStatusCode.OK;

    }
}
