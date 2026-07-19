using System.Net.Http.Json; 
using System.Text.Json;

using CloudPrintAPI.Contracts; 

namespace CloudPrintAPI.Utils
{
    /// <summary>
    /// Client for interacting with SNMP V1 configuration API endpoints.
    /// Does NOT implement IDisposable. The HttpClient instance will not be explicitly disposed by this class.
    /// </summary>
    internal class SnmpV1
    {
        private readonly HttpClient _httpClient;
         private bool _isDisposed = false; 

        /// <summary>
        /// Initializes a new instance of the SnmpV1 client with its own HttpClient.
        /// </summary>
        public SnmpV1()
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5) 
            };
            _httpClient = new HttpClient(handler);
        }

        /// <summary>
        /// Initializes a new instance of the SnmpV1 client with a provided HttpClient.
        /// Useful for dependency injection or testing where HttpClient is managed externally.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance to use.</param>
        public SnmpV1(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Sends a POST request to create an SNMP V1 configuration and validates the response.
        /// </summary>
        /// <param name="url">The full URL for the SNMP V1 configurations endpoint
        /// <param name="accessToken">The Bearer access token for authorization.</param>
        /// <param name="requestBody">The SnmpV1ConfigurationRequest object containing the configuration details.</param>
        /// <returns>An SnmpV1ConfigurationResponse object on success.</returns>
        /// <exception cref="HttpRequestException">Thrown for non-success HTTP status codes (4xx, 5xx).</exception>
        /// <exception cref="Exception">Thrown if response JSON does not match the SnmpV1ConfigurationResponse contract, 
        /// or for other unexpected errors during response processing.</exception>
        public async Task<SnmpV1ConfigurationResponse> CreateSnmpV1ConfigurationAsync(
            string url, string accessToken, SnmpV1ConfigurationRequest requestBody)
        {
            // Set Authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            // Ensure Accept header is set for JSON responses
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // Use consistent JSON serialization options from the helper
            var jsonSerializerOptions = JsonOptions.GetDefaultApiOptions();

            // Send POST request with JSON body
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, requestBody, jsonSerializerOptions);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Delegate validation and deserialization to the dedicated validator class
                return SnmpV1ResponseValidator.ValidateAndDeserialize(responseContent);
            }
            else 
            {
                SnmpApiErrorResponse? snmpErrorDetails = null;
               
                try
                {
                    // Attempt to deserialize error response using consistent options
                    snmpErrorDetails = JsonSerializer.Deserialize<SnmpApiErrorResponse>(responseContent, jsonSerializerOptions);
                }
                catch (JsonException ex)
                {
                    throw new HttpRequestException(
                        $"API returned non-success status ({response.StatusCode}) but error response JSON could not be deserialized into SnmpApiErrorResponse. " +
                        $"Deserialization error: {ex.Message}. Full response JSON: {responseContent}",
                        ex, // Pass the original JsonException as inner exception
                        response.StatusCode
                    );
                }

                // Throw HttpRequestException for non-success status codes
                throw new HttpRequestException(
                    $"Failed to create SNMP V1 configuration. Status: {response.StatusCode}. " +
                    $"Error Code: {snmpErrorDetails?.ErrorCode ?? "N/A"}. " +
                    $"Error Text: {snmpErrorDetails?.ErrorText ?? "N/A"}. " +
                    $"Message: {snmpErrorDetails?.Message ?? "N/A"}. " +
                    $"Full response JSON: {responseContent}",
                    null, 
                    response.StatusCode 
                );
            }
        }

    }
}