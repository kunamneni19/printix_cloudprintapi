using System.Text.Json;
using System.Text.Json.Serialization; // Required for JsonStringEnumConverter

namespace CloudPrintAPI.Utils
{
    /// <summary>
    /// Provides consistent JsonSerializerOptions for use across the application,
    /// especially for API request/response handling.
    /// This static class ensures all parts of your application use the same JSON serialization/deserialization rules.
    /// </summary>
    public static class JsonOptions
    {
        /// <summary>
        /// Gets a standard set of JsonSerializerOptions configured for API communication.
        /// These options include:
        /// - PropertyNameCaseInsensitive = false (respects JsonPropertyName attributes for strict matching)
        /// - AllowTrailingCommas = false (prevents trailing commas in JSON)
        /// - ReadCommentHandling = JsonCommentHandling.Disallow (prevents comments in JSON)
        /// - Converters = { new JsonStringEnumConverter() } (converts enums to/from string values)
        /// </summary>
        /// <returns>A JsonSerializerOptions instance configured for the API.</returns>
        public static JsonSerializerOptions GetDefaultApiOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false, 
                AllowTrailingCommas = false,       
                ReadCommentHandling = JsonCommentHandling.Disallow,
                Converters = { new JsonStringEnumConverter() } 
            };
        }
    }
}