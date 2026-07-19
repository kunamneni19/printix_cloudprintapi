
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace CloudPrintAPI.Contracts
{
    public class SnmpApiErrorResponse 
    {
        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }

        [JsonPropertyName("parameterizedErrorText")]
        public string? ParameterizedErrorText { get; set; }

        [JsonPropertyName("parameters")]
        public List<string>? Parameters { get; set; } 

        [JsonPropertyName("errorText")]
        public string? ErrorText { get; set; }

        [JsonPropertyName("printix-errorId")]
        public string? PrintixErrorId { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; } 
    }
}