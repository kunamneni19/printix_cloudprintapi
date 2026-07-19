using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CloudPrintAPI.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CloudPrintAPI.Utils
{
    internal class CPAuthorizationNew
    {
        // Thread-safe HttpClient instance shared across all test executions to maximize performance
        private static readonly HttpClient client = new HttpClient();

        private static string auth_code = string.Empty;
        private static string token_to = string.Empty;
        private static string auth02url = string.Empty;
        private static string access_token = string.Empty;
        private static string auth4_url = string.Empty;

        /// <summary>
        /// Requests a Cloud Print Authorization Token asynchronously using client credentials via HttpClient.
        /// </summary>
        internal async Task<CloudPrintAuthTokenResponse> GetCloudPrintAuthTokenFullResponseAsync(string url, string cp_client_id, string cp_client_secret)
        {
            var responseModel = new CloudPrintAuthTokenResponse
            {
                StatusCode = HttpStatusCode.InternalServerError // Catch-all default state
            };

            try
            {
                // HttpClient cleanly manages form data and URL encoding automatically
                var postData = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", cp_client_id },
                    { "client_secret", cp_client_secret }
                };

                using (var content = new FormUrlEncodedContent(postData))
                {
                    HttpResponseMessage httpResponse = await client.PostAsync(url, content);

                    responseModel.StatusCode = httpResponse.StatusCode;
                    responseModel.ResponseContent = await httpResponse.Content.ReadAsStringAsync();

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var tokenValue = JsonConvert.DeserializeObject<JObject>(responseModel.ResponseContent);
                        responseModel.AccessToken = (string?)tokenValue?["access_token"];
                        responseModel.RefreshToken = (string?)tokenValue?["refresh_token"];
                        responseModel.ExpiresIn = (int?)tokenValue?["expires_in"];
                    }
                    else
                    {
                        Console.WriteLine($"Error getting Cloud Print Auth Token: {responseModel.StatusCode} - {responseModel.ResponseContent}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network HTTP Error during token acquisition: {ex.Message}");
                responseModel.StatusCode = ex.StatusCode ?? HttpStatusCode.ServiceUnavailable;
                responseModel.ResponseContent = $"HTTP error: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to execute getCloudPrintAuthToken: {ex.Message}");
                responseModel.StatusCode = HttpStatusCode.InternalServerError;
                responseModel.ResponseContent = $"Unexpected error: {ex.Message}";
            }

            return responseModel;
        }

        /// <summary>
        /// Refreshes an expired authentication context asynchronously based on regional tenant parameters.
        /// </summary>
        internal async Task<RefreshTokenResponse> RefreshTokenAsync(string url, string env, string clientId, string refreshToken)
        {
            var responseModel = new RefreshTokenResponse
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            string domainSuffix = "net";
            string tokenEndpoint = string.Empty;

            try
            {
                // Maintain your environmental business logic transitions (.dev vs .net)
                if (env == "dev01" || env == "dev02" || env == "dev03" || env == "dev04")
                {
                    domainSuffix = "dev";
                }

                switch (env)
                {
                    case "dev01":
                    case "dev02":
                    case "dev03":
                    case "dev04":
                    case "devenv":
                    case "testenv":
                    case "devenv.us":
                    case "testenv.us":
                        tokenEndpoint = $"https://auth.{env}.printix.{domainSuffix}/oauth/token";
                        break;
                    case "prodenv":
                        tokenEndpoint = $"https://auth.printix.{domainSuffix}/oauth/token";
                        break;
                    case "prodenv.us":
                        tokenEndpoint = $"https://auth.us.printix.{domainSuffix}/oauth/token";
                        break;
                    default:
                        responseModel.StatusCode = HttpStatusCode.BadRequest;
                        responseModel.ResponseBody = "Wrong environment specified.";
                        Console.WriteLine("Wrong environment to refresh token.");
                        return responseModel;
                }

                var postData = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "client_id", clientId },
                    { "refresh_token", refreshToken }
                };

                using (var content = new FormUrlEncodedContent(postData))
                {
                    HttpResponseMessage httpResponse = await client.PostAsync(tokenEndpoint, content);

                    responseModel.StatusCode = httpResponse.StatusCode;
                    responseModel.ResponseBody = await httpResponse.Content.ReadAsStringAsync();

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var tokenValue = JsonConvert.DeserializeObject<JObject>(responseModel.ResponseBody);
                        responseModel.AccessToken = (string?)tokenValue?["access_token"];
                        responseModel.NewRefreshToken = (string?)tokenValue?["refresh_token"];
                        responseModel.ExpiresIn = (int?)tokenValue?["expires_in"];
                    }
                    else
                    {
                        Console.WriteLine($"Token refresh failed: {responseModel.StatusCode} - {responseModel.ResponseBody}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network HTTP Error during token refresh: {ex.Message}");
                responseModel.StatusCode = ex.StatusCode ?? HttpStatusCode.ServiceUnavailable;
                responseModel.ResponseBody = $"HTTP error: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during token refresh: {ex.Message}");
                responseModel.StatusCode = HttpStatusCode.InternalServerError;
                responseModel.ResponseBody = $"Unexpected error: {ex.Message}";
            }

            return responseModel;
        }

        /// <summary>
        /// Safely parses query attributes from redirect parameters to capture authorization codes.
        /// </summary>
        private string ExtractCodeFromUrl(string auth0)
        {
            string code = string.Empty;
            Uri locationUri = new Uri(auth0);
            string query = locationUri.Query;
            var queryParams = HttpUtility.ParseQueryString(query);

            if (queryParams.AllKeys.Contains("code"))
            {
                code = queryParams["code"];
                Console.WriteLine("Authorization Code: " + code);
            }
            return code;
        }
    }
}