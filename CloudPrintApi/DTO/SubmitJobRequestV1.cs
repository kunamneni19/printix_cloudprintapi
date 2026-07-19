using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data;

namespace CloudPrintAPI.DTO
{
    public class SubmitJobRequestV1
    {
        [JsonProperty("title", Required = Required.Always)] // Mandatory URL query param
        public string Title { get; set; }

        [JsonProperty("user")] // Optional URL query param
        public string User { get; set; }

        [JsonProperty("PDL")] // Optional URL query param
        [JsonConverter(typeof(StringEnumConverter))]
        public PdlType? PDL { get; set; }

        [JsonProperty("releaseImmediately")]
        public bool? ReleaseImmediately { get; set; } 

        [JsonProperty("vendor_ticket_item")]
        public List<VendorTicketItem> VendorTicketItem { get; set; } = new List<VendorTicketItem>();

        [JsonProperty("color", Required = Required.Always)]
        public ColorV1 Color { get; set; }

        [JsonProperty("duplex", Required = Required.Always)]
        public DuplexV1 Duplex { get; set; }

        [JsonProperty("page_orientation", Required = Required.Always)]
        public PageOrientationV1 PageOrientation { get; set; }

        [JsonProperty("copies", Required = Required.Always)]
        public CopiesV1 Copies { get; set; }

        [JsonProperty("media_size", Required = Required.Always)]
        public MediaSizeV1 MediaSize { get; set; }
    }

    // Nested DTOs required by SubmitJobRequestV1:

    public class VendorTicketItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class ColorV1
    {
        [JsonProperty("vendor_id")]
        public object VendorId { get; set; } // Can be null based on example

        [JsonProperty("type", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public StandardColorType Type { get; set; }
    }

    public enum StandardColorType
    {
        STANDARD_COLOR
    }

    public class DuplexV1
    {
        [JsonProperty("type", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public DuplexTypeV1 Type { get; set; }
    }

    public enum DuplexTypeV1
    {
        NO_DUPLEX
    }

    public class PageOrientationV1
    {
        [JsonProperty("type", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PageOrientationTypeV1 Type { get; set; }
    }

    public enum PageOrientationTypeV1
    {
        AUTO
    }

    public class CopiesV1
    {
        [JsonProperty("copies", Required = Required.Always)]
        public int Copies { get; set; }
    }

    public class MediaSizeV1
    {
        [JsonProperty("mediaSize", Required = Required.Always)]
        public string MediaSize { get; set; } // e.g., "A4"
    }

    /// <summary>
    /// Possible values for the Printer Document Language (PDL) parameter.
    /// </summary>
    public enum PdlType
    {
        PCL5,
        PCLXL,
        POSTSCRIPT,
        UFRII,
        TEXT,
        XPS
    }
}