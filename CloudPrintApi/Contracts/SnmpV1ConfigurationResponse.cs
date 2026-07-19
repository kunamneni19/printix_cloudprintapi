using System.Text.Json.Serialization; 
using System.Collections.Generic;    

namespace CloudPrintAPI.Contracts
{
    /// <summary>
    /// Response body contract for a created SNMP V1 configuration
    /// </summary>
    internal class SnmpV1ConfigurationResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("getCommunityName")]
        public string? GetCommunityName { get; set; }

        [JsonPropertyName("setCommunityName")]
        public string? SetCommunityName { get; set; }

        [JsonPropertyName("tenantDefault")]
        public bool TenantDefault { get; set; }

        [JsonPropertyName("securityLevel")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SnmpV1SecurityLevel? SecurityLevel { get; set; } // Uses enum from SnmpEnums.cs

        [JsonPropertyName("version")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SnmpVersion? Version { get; set; } // Uses enum from SnmpEnums.cs

        [JsonPropertyName("username")]
        public string? Username { get; set; } 

        [JsonPropertyName("contextName")]
        public string? ContextName { get; set; } 

        [JsonPropertyName("authentication")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SnmpV1Authentication? Authentication { get; set; } 

        [JsonPropertyName("privacy")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SnmpV1Privacy? Privacy { get; set; } 

        [JsonPropertyName("_links")]
        public SnmpLinks? Links { get; set; } 
    }

    /// <summary>
    /// Represents the _links object within the SNMP V1 configuration response.
    /// Defined internally to be specific to this response.
    /// </summary>
    internal class SnmpLinks
    {
        [JsonPropertyName("self")]
        public Link? Self { get; set; }

        [JsonPropertyName("networks")]
        public Link? Networks { get; set; }
    }

    /// <summary>
    /// Represents a generic link object with an href.
    /// Defined internally within SnmpLinks for tight coupling.
    /// </summary>
    internal class Link
    {
        [JsonPropertyName("href")]
        public string? Href { get; set; }
    }

}