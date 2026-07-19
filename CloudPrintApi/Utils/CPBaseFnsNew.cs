using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CloudPrintAPI
{
    internal class CPBaseFnsNew
    {
        // Thread-safe HttpClient instance shared across all test executions
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Executes an asynchronous HTTP GET request with a Bearer authentication token.
        /// </summary>
        internal async Task<HttpResponseMessage> ApiGetMethodAsync(string url, string auth_token)
        {
            try
            {
                // Create a dynamic request message to safely inject headers per call
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth_token);

                    HttpResponseMessage response = await client.SendAsync(request);

                    // Replaces Debug.Assert with a standard assertion alert check
                    Debug.Assert(response.StatusCode == HttpStatusCode.OK, "Get Request status must be OK, status code = 200");

                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception on GET: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Executes an asynchronous HTTP POST request sending raw byte array payloads.
        /// </summary>
        internal async Task<HttpResponseMessage> ApiPostMethodAsync(string url, string auth_token, byte[] postData)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth_token);

                    // ByteArrayContent replaces the manual request stream writing logic entirely
                    request.Content = new ByteArrayContent(postData);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    // SendAsync automatically handles 400/500 level error responses gracefully without throwing WebExceptions
                    HttpResponseMessage response = await client.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error Status on POST: {response.StatusCode}");
                        Console.WriteLine($"Error Response Body: {errorResponse}");
                    }

                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception on POST: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Executes an asynchronous HTTP DELETE request context.
        /// </summary>
        internal async Task<HttpResponseMessage> ApiDeleteMethodAsync(string url, string auth_token)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Delete, url))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth_token);

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error Status on DELETE: {response.StatusCode}");
                        Console.WriteLine($"Error Response Body: {errorResponse}");
                    }

                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception on DELETE: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}