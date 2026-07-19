using System.Net;
using System.Text.Json.Serialization;
namespace CloudPrintAPI.Contracts
{
    internal class AuthenticationResponse
    {
        [JsonPropertyName("access_token")] 
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int? ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonIgnore] // Ignore this property during JSON deserialization
        public HttpStatusCode StatusCode { get; set; }

        [JsonIgnore] // Ignore this property during JSON deserialization
        public string? ResponseContent { get; set; }

        [JsonIgnore] // Ignore this property during JSON deserialization
        public bool IsSuccessStatusCode => StatusCode == HttpStatusCode.OK;
    }
}
