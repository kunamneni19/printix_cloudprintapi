using Newtonsoft.Json;
using Newtonsoft.Json.Converters; 
using System.Runtime.Serialization; 

namespace CloudPrintAPI.DTO
{
    /// <summary>
    /// Represents the request body structure for the /submit endpoint (API Version 1.1).
    /// </summary>
    public class SubmitJobRequestV1_1
    {
        /// <summary>
        /// Sets whether the document should be printed in color. Possible values are true or false.
        /// </summary>
        [JsonProperty("color", Required = Required.Always)]
        public bool Color { get; set; }

        /// <summary>
        /// Sets what kind of duplex to use to print the document, if any.
        /// Possible values are NONE, SHORT_EDGE or LONG_EDGE.
        /// </summary>
        [JsonProperty("duplex", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public DuplexTypeV1_1 Duplex { get; set; }

        /// <summary>
        /// Sets the page orientation. Possible values are PORTRAIT, LANDSCAPE or AUTO.
        /// </summary>
        [JsonProperty("page_orientation", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PageOrientationTypeV1_1 PageOrientation { get; set; }

        /// <summary>
        /// A positive integer, representing the number of copies to print.
        /// </summary>
        [JsonProperty("copies", Required = Required.Always)]
        public int Copies { get; set; }

        /// <summary>
        /// Sets the page dimensions for the document to be printed. (e.g., "A4")
        /// </summary>
        [JsonProperty("media_size", Required = Required.Always)]
        public string MediaSize { get; set; } // Matches the "Paper size values" (e.g., "A4")

        /// <summary>
        /// Determines how the job should be scaled if at all; if set the document will be scaled to the paper size.
        /// Possible values are: NOSCALE, SHRINK, FIT. (Optional)
        /// </summary>
        [JsonProperty("scaling")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ScalingType? Scaling { get; set; }

        /// <summary>
        /// Optional field. When the field is specified, the API will assign the job to the user matching the mapping.
        /// The field should contain an object with a "key" and a "value" field.
        /// </summary>
        [JsonProperty("userMapping")]
        public UserMapping UserMapping { get; set; }
    }

    // Nested DTOs and Enums required by SubmitJobRequestV1_1:

    /// <summary>
    /// Represents the structure for the userMapping field in SubmitJobRequestV1_1.
    /// </summary>
    public class UserMapping
    {
        [JsonProperty("key", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public UserMappingKey Key { get; set; }

        [JsonProperty("value", Required = Required.Always)]
        public string Value { get; set; }
    }

    /// <summary>
    /// Possible values for the 'key' field within UserMapping.
    /// </summary>
    public enum UserMappingKey
    {
        [EnumMember(Value = "AzureObjectId")]
        AzureObjectId,
        [EnumMember(Value = "AzureUPN")]
        AzureUPN, // Note: Your doc had AzureUPN twice, assuming it's a typo and only one is intended
        [EnumMember(Value = "SAMAccountName")]
        SAMAccountName,
        [EnumMember(Value = "OnPremImmutableId")]
        OnPremImmutableId,
        [EnumMember(Value = "OnPremUpn")]
        OnPremUpn,
        [EnumMember(Value = "Email")]
        Email
    }

    /// <summary>
    /// Possible values for the 'duplex' field in SubmitJobRequestV1_1.
    /// </summary>
    public enum DuplexTypeV1_1
    {
        [EnumMember(Value = "NONE")]
        NONE,
        [EnumMember(Value = "SHORT_EDGE")]
        SHORT_EDGE,
        [EnumMember(Value = "LONG_EDGE")]
        LONG_EDGE
    }

    /// <summary>
    /// Possible values for the 'page_orientation' field in SubmitJobRequestV1_1.
    /// </summary>
    public enum PageOrientationTypeV1_1
    {
        [EnumMember(Value = "PORTRAIT")]
        PORTRAIT,
        [EnumMember(Value = "LANDSCAPE")]
        LANDSCAPE,
        [EnumMember(Value = "AUTO")]
        AUTO
    }

    /// <summary>
    /// Possible values for the 'scaling' field in SubmitJobRequestV1_1.
    /// </summary>
    public enum ScalingType
    {
        [EnumMember(Value = "NOSCALE")]
        NOSCALE,
        [EnumMember(Value = "SHRINK")]
        SHRINK,
        [EnumMember(Value = "FIT")]
        FIT
    }
}