using Newtonsoft.Json;

namespace CloudPrintAPI.DTO
{
    // --- Main Response DTO for Change Owner ---
    public class ChangeOwnerResponse
    {
        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        [JsonProperty("sortOrder")]
        public object SortOrder { get; set; } // Can be null, or a specific type if known

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("jobs")]
        public List<Job> Jobs { get; set; } // List of Job objects

        [JsonProperty("page")]
        public JobPaginationInfo Page { get; set; } 
            
        [JsonIgnore]
        public System.Net.HttpStatusCode StatusCode { get; set; }

        [JsonIgnore]
        public bool IsSuccessStatusCode { get; set; }

        [JsonIgnore]
        public string ResponseBody { get; set; } 
    }

    // --- Job DTO (nested within ChangeOwnerResponse) ---
    public class Job
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("createTime")]
        public string CreateTimeRaw { get; set; }

        [JsonIgnore]
        public DateTime CreateTime => DateTimeOffset.Parse(CreateTimeRaw).LocalDateTime;

        [JsonProperty("updateTime")]
        public string UpdateTimeRaw { get; set; }

        // Helper property to get DateTime
        [JsonIgnore]
        public DateTime UpdateTime => DateTimeOffset.Parse(UpdateTimeRaw).LocalDateTime;


        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("ownerId")]
        public string OwnerId { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; } 
    }

    // --- Links DTO (nested within Job) ---
    public class Links
    {
        [JsonProperty("self")]
        public Link Self { get; set; } 

        [JsonProperty("printer")]
        public Link Printer { get; set; } 

        [JsonProperty("changeOwner")]
        public ChangeOwnerLink ChangeOwner { get; set; } 
    }

    // --- Generic Link DTO 
    public class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }

    // --- Specific ChangeOwnerLink DTO 
    public class ChangeOwnerLink : Link
    {
        [JsonProperty("templated")]
        public bool Templated { get; set; }
    }

    // --- Page DTO (nested within ChangeOwnerResponse) ---
    public class JobPaginationInfo
    {
        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("totalElements")]
        public int TotalElements { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }
    }
}