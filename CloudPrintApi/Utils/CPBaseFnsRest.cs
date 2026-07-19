using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using RestSharp;

namespace CloudPrintAPI
{
    internal class CPBaseFnsRest
    {
        // Shared, thread-safe RestClient instance for performance and connection pooling
        private static readonly RestClient client = new RestClient();

        /// <summary>
        /// Executes a synchronous GET request using RestClient.
        /// </summary>
        internal RestResponse ApiGetMethod(string url, string auth_token)
        {
            try
            {
                // Use new Uri(url) for absolute dynamic routing safety
                var request = new RestRequest(new Uri(url), Method.Get);
                request.AddHeader("Authorization", $"Bearer {auth_token}");
                request.AddHeader("Content-Type", "application/json");

                RestResponse response = client.Execute(request);

                // Replicating your original Debug.Assert requirement
                Debug.Assert(response.StatusCode == HttpStatusCode.OK, $"Get Request status must be OK, status code = {(int)response.StatusCode}");

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Executes a synchronous POST request containing raw byte data using RestClient.
        /// </summary>
        internal RestResponse ApiPostMethod(string url, string auth_token, byte[] postData)
        {
            try
            {
                var request = new RestRequest(new Uri(url), Method.Post);
                request.AddHeader("Authorization", $"Bearer {auth_token}");

                // RestSharp automatically manages Content-Length when adding bytes/bodies
                request.AddBody(postData, "application/json");

                RestResponse response = client.Execute(request);

                // If the response failed (e.g., 400, 500 error codes or transport level exceptions)
                if (!response.IsSuccessful)
                {
                    if (response.ErrorException != null)
                    {
                        Console.WriteLine($"Network error on POST: {response.ErrorException.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"HTTP Error on POST: {(int)response.StatusCode} - {response.StatusDescription}");
                        Console.WriteLine($"Error Response Body: {response.Content}");
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Executes a synchronous DELETE request using RestClient.
        /// </summary>
        internal RestResponse ApiDeleteMethod(string url, string auth_token)
        {
            try
            {
                var request = new RestRequest(new Uri(url), Method.Delete);
                request.AddHeader("Authorization", $"Bearer {auth_token}");
                request.AddHeader("Content-Type", "application/json");

                RestResponse response = client.Execute(request);

                if (!response.IsSuccessful)
                {
                    if (response.ErrorException != null)
                    {
                        Console.WriteLine($"Network error on DELETE: {response.ErrorException.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"HTTP Error on DELETE: {(int)response.StatusCode} - {response.StatusDescription}");
                        Console.WriteLine($"Error Response Body: {response.Content}");
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}