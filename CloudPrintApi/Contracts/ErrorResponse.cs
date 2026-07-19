using System.Text.Json.Serialization;

namespace CloudPrintAPI.Contracts
{
    /// <summary>
    /// Represents the contract for an API error response,
    /// typically used for non-2xx HTTP status codes like 400, 401, 403, 500, etc.
    /// </summary>
    internal class ErrorResponse
    {
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }

    }
}