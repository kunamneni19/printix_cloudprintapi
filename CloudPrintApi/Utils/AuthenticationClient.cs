using System.Text.Json;
using CloudPrintAPI.Contracts;

namespace CloudPrintAPI.Utils
{
    internal class AuthenticationClient 
    {
        private readonly HttpClient _httpClient;
        private bool _isDisposed = false;

        public AuthenticationClient()
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5)
            };
            _httpClient = new HttpClient(handler);
        }


        /// <summary>
        /// Sends a POST request to obtain an authentication token and performs contract validation.
        /// </summary>
        /// <param name="url">The URL of the token endpoint.</param>
        /// <param name="clientId">The client ID for authentication.</param>
        /// <param name="clientSecret">The client secret for authentication.</param>
        /// <returns>An AuthenticationResponse object containing token details and response metadata.</returns>
        /// <exception cref="Exception">Throws an exception if the API call or JSON contract validation fails.</exception>
        public async Task<AuthenticationResponse> GetAuthenticationTokenAsync(
            string url, string clientId, string clientSecret)
        {
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            });

            HttpResponseMessage response = await _httpClient.PostAsync(url, requestContent);
            string responseContent = await response.Content.ReadAsStringAsync();

            var authResponse = new AuthenticationResponse
            {
                StatusCode = response.StatusCode,
                ResponseContent = responseContent
            };

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    //  Contract Validation
                    var expectedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "access_token",
                        "expires_in",
                        "refresh_token", 
                        "token_type"    
                    };

                    // Extract JSON body & Check for unexpected extra fields before deserialization.
                    using JsonDocument doc = JsonDocument.Parse(responseContent);
                    var root = doc.RootElement;

                    // Validate JSON body versus contract.
                    foreach (JsonProperty property in root.EnumerateObject())
                    {
                        if (!expectedFields.Contains(property.Name))
                        {
                            throw new JsonException($"Contract Violation: Unexpected field '{property.Name}' found in the response.");
                        }
                    }

                    // Configure JsonSerializerOptions. PropertyNameCaseInsensitive should be false
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = false,
                        AllowTrailingCommas = false,
                        ReadCommentHandling = JsonCommentHandling.Disallow,
                    };

                    // Deserialize the JSON into AuthenticationResponse contract.
                    var deserializedObject = JsonSerializer.Deserialize<AuthenticationResponse>(responseContent, jsonSerializerOptions);

                    // Assign the deserialized values to the AuthenticationResponse instance.
                    authResponse.AccessToken = deserializedObject?.AccessToken;
                    authResponse.ExpiresIn = deserializedObject?.ExpiresIn;
                    authResponse.RefreshToken = deserializedObject?.RefreshToken; 
                    authResponse.TokenType = deserializedObject?.TokenType;

                    // Explicit checks for required fields that are defined as nullable
                    if (string.IsNullOrEmpty(authResponse.AccessToken))
                    {
                        throw new JsonException("Contract Violation: 'access_token' is missing or empty in the response.");
                    }
                    if (!authResponse.ExpiresIn.HasValue)
                    {
                        throw new JsonException("Contract Violation: 'expires_in' is missing or null in the response.");
                    }
                   
                    if (authResponse.ExpiresIn.Value != 3659)
                    {
                        throw new JsonException($"Contract Violation: 'expires_in' must be 3599, but was {authResponse.ExpiresIn.Value}.");
                    }

                    if (string.IsNullOrEmpty(authResponse.RefreshToken))
                    {
                        throw new JsonException("Contract Violation: 'refresh_token' is missing or empty in the response.");
                    }
                    if (string.IsNullOrEmpty(authResponse.TokenType))
                    {
                        throw new JsonException("Contract Violation: 'token_type' is missing or empty in the response.");
                    }
                    if (authResponse.TokenType != "Bearer")
                    {
                        throw new JsonException($"Contract Violation: 'token_type' must be 'Bearer', but was '{authResponse.TokenType}'.");
                    }
                }
                catch (JsonException ex)
                {
                    throw new Exception($"{Environment.NewLine}API response contract validation failed.{Environment.NewLine}Details: {ex.Message}. " +
                                        $"{Environment.NewLine}Full response JSON: {responseContent}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"{Environment.NewLine}An unexpected error occurred during API response processing: {ex.Message}. {Environment.NewLine}Raw response: {responseContent}", ex);
                }
            }

            return authResponse;
        }

        /// <summary>
        /// Sends a POST request to obtain an authentication token but expects an error response.
        /// </summary>
        /// <param name="url">The URL of the token endpoint.</param>
        /// <param name="clientId">The client ID for authentication (expected to be invalid).</param>
        /// <param name="clientSecret">The client secret for authentication (expected to be invalid).</param>
        /// <returns>An AuthenticationResponse1 object containing status code, raw response, and parsed error details.</returns>
        public async Task<AuthenticationResponse> GetAuthenticationErrorResponseAsync(
            string url, string clientId, string clientSecret)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(AuthenticationClient), "Cannot use a disposed AuthenticationClient instance.");
            }

            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            });

            HttpResponseMessage response = await _httpClient.PostAsync(url, requestContent);
            string responseContent = await response.Content.ReadAsStringAsync();

            var authResponse = new AuthenticationResponse
            {
                StatusCode = response.StatusCode,
                ResponseContent = responseContent
            };

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false,
                AllowTrailingCommas = false,
                ReadCommentHandling = JsonCommentHandling.Disallow,
            };

            // Attempt to deserialize the error response content into the ErrorResponse contract
            try
            {
                var errorDetails = JsonSerializer.Deserialize<ErrorResponse>(responseContent, jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                throw new Exception(
                    $"{Environment.NewLine}Error response JSON could not be deserialized into '{nameof(ErrorResponse)}' contract. " +
                    $"{Environment.NewLine}Deserialization error: {ex.Message}. {Environment.NewLine}Full response JSON: {responseContent}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"An unexpected error occurred while processing the error response: {ex.Message}. " +
                    $"Full response JSON: {responseContent}", ex);
            }

            return authResponse; 
        }

    }
}