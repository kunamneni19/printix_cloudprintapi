using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using CloudPrintAPI.DTO;
using RestSharp;

namespace CloudPrintAPI.Utils
{
    internal class CPAuthorizationRest
    {
        // Thread-safe RestClient instance shared to maximize performance.
        private static readonly RestClient client = new RestClient();

        /// <summary>
        /// Requests a Cloud Print Authorization Token asynchronously.
        /// </summary>
        internal async Task<CloudPrintAuthTokenResponse> GetCloudPrintAuthTokenFullResponseAsync(string url, string cpClientId, string cpClientSecret)
        {
            var request = new RestRequest(url, Method.Post);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", cpClientId);
            request.AddParameter("client_secret", cpClientSecret);

            // Execute directly targeting your exact DTO schema
            var response = await client.ExecuteAsync<CloudPrintAuthTokenResponse>(request);

            // Extract the deserialized data model or create an empty fallback container
            var responseModel = response.Data ?? new CloudPrintAuthTokenResponse();

            // Map structural HTTP transport metadata cleanly
            responseModel.StatusCode = response.StatusCode;
            responseModel.ResponseContent = response.Content;

            if (!response.IsSuccessful && response.ErrorException != null)
            {
                responseModel.ResponseContent = $"HTTP error: {response.ErrorException.Message}";
            }

            return responseModel;
        }

        /// <summary>
        /// Refreshes an expired authentication context asynchronously based on regional tenant parameters.
        /// </summary>
        internal async Task<RefreshTokenResponse> RefreshTokenAsync(string url, string env, string clientId, string refreshToken)
        {
            var responseModel = new RefreshTokenResponse();
            string domainSuffix = (env?.StartsWith("dev") == true) ? "dev" : "net";
            string tokenEndpoint;

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
                    // Return raw bad status info directly for the test layer to evaluate
                    responseModel.StatusCode = HttpStatusCode.BadRequest;
                    responseModel.ResponseBody = "Wrong environment specified.";
                    return responseModel;
            }

            var request = new RestRequest(tokenEndpoint, Method.Post);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("client_id", clientId);
            request.AddParameter("refresh_token", refreshToken);

            var response = await client.ExecuteAsync<RefreshTokenResponse>(request);

            responseModel = response.Data ?? new RefreshTokenResponse();
            responseModel.StatusCode = response.StatusCode;
            responseModel.ResponseBody = response.Content;

            if (!response.IsSuccessful && response.ErrorException != null)
            {
                responseModel.ResponseBody = $"HTTP error: {response.ErrorException.Message}";
            }

            return responseModel;
        }
    }
}