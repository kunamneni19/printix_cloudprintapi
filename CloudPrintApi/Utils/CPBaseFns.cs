using System.Diagnostics;
using System.Net;

namespace CloudPrintAPI
{
    internal class CPBaseFns
    {
        internal HttpWebResponse ApiGetMethod(string url, string auth_token)
        {
            HttpWebResponse GetResponse;
            try
            {
                //var h1Client = new HttpClient();
                HttpWebRequest Getrequest = (HttpWebRequest)WebRequest.Create(url);
                Getrequest.Method = "GET";
                Getrequest.ContentType = "application/json";
                Getrequest.Headers.Add("Authorization", "Bearer " + auth_token);
                GetResponse = (HttpWebResponse)Getrequest.GetResponse();
                Debug.Assert(GetResponse.StatusCode == HttpStatusCode.OK, "Get Request stauts must be OK, status code = 200");               
               
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
            return GetResponse;
        }

        internal HttpWebResponse ApiPostMethod(string url, string auth_token, byte[] postData)
        {
            HttpWebResponse PostResponse;
            try
            {
                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(url);
                PostRequest.Method = "POST";
                PostRequest.ContentType = "application/json"; 
                PostRequest.Headers.Add("Authorization", "Bearer " + auth_token);
                PostRequest.ContentLength = postData.Length; 

                // Write the request body
                using (Stream requestStream = PostRequest.GetRequestStream())
                {
                    requestStream.Write(postData, 0, postData.Length);
                }

                PostResponse = (HttpWebResponse)PostRequest.GetResponse();
            }
            catch (WebException webEx) // Use WebException for HTTP-specific errors
            {
                // Capture the response in case of HTTP errors (e.g., 400 Bad Request)
                if (webEx.Response != null)
                {
                    Console.WriteLine($"WebException on POST: {webEx.Message}");
                    using (StreamReader reader = new StreamReader(webEx.Response.GetResponseStream()))
                    {
                        string errorResponse = reader.ReadToEnd();
                        Console.WriteLine($"Error Response Body: {errorResponse}");
                    }
                    return (HttpWebResponse)webEx.Response; // Return the actual error response
                }
                else
                {
                    Console.WriteLine($"WebException (non-HTTP response): {webEx.Message}");
                    throw; // Re-throw if no response was received
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
            return PostResponse;
        }
        internal HttpWebResponse ApiDeleteMethod(string url, string auth_token)
        {
            HttpWebResponse DeleteResponse;
            try
            {
                HttpWebRequest DeleteRequest = (HttpWebRequest)WebRequest.Create(url);
                DeleteRequest.Method = "DELETE";
                DeleteRequest.ContentType = "application/json"; 
                DeleteRequest.Headers.Add("Authorization", "Bearer " + auth_token);

                DeleteResponse = (HttpWebResponse)DeleteRequest.GetResponse();
            }
            catch (WebException webEx) 
            {
                if (webEx.Response != null)
                {
                    Console.WriteLine($"WebException on DELETE: {webEx.Message}");
                    using (StreamReader reader = new StreamReader(webEx.Response.GetResponseStream()))
                    {
                        string errorResponse = reader.ReadToEnd();
                        Console.WriteLine($"Error Response Body: {errorResponse}");
                    }
                    return (HttpWebResponse)webEx.Response; // Return the actual error response
                }
                else
                {
                    Console.WriteLine($"WebException (non-HTTP response): {webEx.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
            return DeleteResponse;
        }
    }

}
