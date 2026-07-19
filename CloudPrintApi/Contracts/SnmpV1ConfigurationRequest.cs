using System.Text.Json.Serialization; 
using System.Collections.Generic;   

namespace CloudPrintAPI.Contracts
{
    /// <summary>
    /// Represents the request body contract for creating an SNMP V1 configuration.
    /// </summary>
    internal class SnmpV1ConfigurationRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty; 

        [JsonPropertyName("getCommunityName")]
        public string GetCommunityName { get; set; } = string.Empty; 

        [JsonPropertyName("setCommunityName")]
        public string SetCommunityName { get; set; } = string.Empty; 

        [JsonPropertyName("tenantDefault")]
        public bool TenantDefault { get; set; } 

        [JsonPropertyName("securityLevel")]
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SnmpV1SecurityLevel SecurityLevel { get; set; } // Uses enum from SnmpEnums.csd

        [JsonPropertyName("version")]
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SnmpVersion Version { get; set; } = SnmpVersion.V1; // Uses enum from SnmpEnums.cs, 

        [JsonPropertyName("username")]
        public string? Username { get; set; } 

        [JsonPropertyName("contextName")]
        public string? ContextName { get; set; } 

        [JsonPropertyName("authentication")]
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SnmpV1Authentication Authentication { get; set; } 

        [JsonPropertyName("authenticationKey")]
        public string? AuthenticationKey { get; set; } 

        [JsonPropertyName("privacy")]
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SnmpV1Privacy Privacy { get; set; } 

        [JsonPropertyName("privacyKey")]
        public string? PrivacyKey { get; set; } 

        [JsonPropertyName("networkIds")]
        public List<string> NetworkIds { get; set; } = new List<string>(); 
    }
}