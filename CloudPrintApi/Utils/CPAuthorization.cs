using CloudPrintAPI.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;

namespace CloudprintAPI
{
    public class CPAuthorization
    {
        static string auth_code = String.Empty;
        static string token_to = String.Empty;
        static string auth02url = String.Empty;
        static string access_token = String.Empty;
        static string auth4_url = String.Empty;

        internal CloudPrintAuthTokenResponse GetCloudPrintAuthTokenFullResponse(string url, string cp_client_id, string cp_client_secret)
        {
            var response = new CloudPrintAuthTokenResponse
            {
                StatusCode = HttpStatusCode.InternalServerError // Default in case of unhandled error
            };

            try
            {
                var postData = "grant_type=" + Uri.EscapeDataString("client_credentials");
                postData += "&client_id=" + Uri.EscapeDataString(cp_client_id);
                postData += "&client_secret=" + Uri.EscapeDataString(cp_client_secret);

                var data = Encoding.ASCII.GetBytes(postData);
                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(url);
                PostRequest.Method = "POST";
                PostRequest.ContentType = "application/x-www-form-urlencoded";
                PostRequest.ContentLength = data.Length;

                using (var stream = PostRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse PostResponse = (HttpWebResponse)PostRequest.GetResponse())
                {
                    response.StatusCode = PostResponse.StatusCode;
                    using (StreamReader reader = new StreamReader(PostResponse.GetResponseStream()))
                    {
                        response.ResponseContent = reader.ReadToEnd();
                        var token_value = JsonConvert.DeserializeObject<JObject>(response.ResponseContent);
                        response.AccessToken = (string?)token_value?["access_token"];
                        response.RefreshToken = (string?)token_value?["refresh_token"];
                        response.ExpiresIn = (int?)token_value?["expires_in"];
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse errorResponse)
                {
                    response.StatusCode = errorResponse.StatusCode;
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        response.ResponseContent = reader.ReadToEnd();
                        Console.WriteLine($"Error getting Cloud Print Auth Token: {response.StatusCode} - {response.ResponseContent}");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to execute getCloudPrintAuthToken (non-HTTP error): {ex.Message}");
                    response.StatusCode = HttpStatusCode.ServiceUnavailable; // Or another appropriate code
                    response.ResponseContent = $"Non-HTTP error: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to execute getCloudPrintAuthToken: {ex.Message}");
                response.StatusCode = HttpStatusCode.InternalServerError; // General catch-all
                response.ResponseContent = $"Unexpected error: {ex.Message}";
            }
            return response;
        }

        // Updated method to return RefreshTokenResponse object
        internal RefreshTokenResponse RefreshToken(string url, string env, string clientId, string refreshToken)
        {
            var response = new RefreshTokenResponse
            {
                StatusCode = HttpStatusCode.InternalServerError // Default in case of unhandled error
            };

            string domainSuffix = "net";
            string tokenEndpoint = String.Empty;

            try
            {
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
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.ResponseBody = "Wrong environment specified.";
                        Console.WriteLine("Wrong environment to refresh token.");
                        return response; // Return early for invalid environment
                }

                var postData = $"grant_type=refresh_token&client_id={Uri.EscapeDataString(clientId)}&refresh_token={Uri.EscapeDataString(refreshToken)}";
                var data = Encoding.ASCII.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse()) // Renamed to httpResponse to avoid conflict with `response` variable
                {
                    response.StatusCode = httpResponse.StatusCode;
                    using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        response.ResponseBody = reader.ReadToEnd();
                        var tokenValue = JsonConvert.DeserializeObject<JObject>(response.ResponseBody);
                        response.AccessToken = (string?)tokenValue?["access_token"];
                        response.NewRefreshToken = (string?)tokenValue?["refresh_token"];
                        response.ExpiresIn = (int?)tokenValue?["expires_in"];
                    }
                }
            }
            catch (WebException wex)
            {
                if (wex.Response is HttpWebResponse errorResponse)
                {
                    response.StatusCode = errorResponse.StatusCode;
                    using (StreamReader reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        response.ResponseBody = reader.ReadToEnd();
                        Console.WriteLine($"Token refresh failed: {response.StatusCode} - {response.ResponseBody}");
                    }
                }
                else
                {
                    Console.WriteLine($"Error refreshing token (non-HTTP error): {wex.Message}");
                    response.StatusCode = HttpStatusCode.ServiceUnavailable;
                    response.ResponseBody = $"Non-HTTP error: {wex.Message}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during token refresh: {ex.Message}");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ResponseBody = $"Unexpected error: {ex.Message}";
            }

            return response;
        }

        private String ExtractCodeFromUrl(String auth0)
        {
            String code= String.Empty;
            var queryParm = auth0.Split("?")[1].Split("&")[0];
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