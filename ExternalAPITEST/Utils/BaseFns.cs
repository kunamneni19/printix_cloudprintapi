using System.Diagnostics;
using System.Net;

namespace ExternalAPI
{
    internal class BaseFns
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

        internal HttpWebResponse ApiDeleteMethod(string  url, string auth_token)
        {
            HttpWebResponse DelResponse;
            try
            {
                HttpWebRequest Delrequest = (HttpWebRequest)WebRequest.Create(url);
                Delrequest.Method = "DELETE";
                Delrequest.ContentType = "application/json";
                Delrequest.Headers.Add("Authorization", "Bearer " + auth_token);
                DelResponse = (HttpWebResponse)Delrequest.GetResponse();
                //Debug.Assert(DelResponse.StatusCode == HttpStatusCode.OK, "Delete Request stauts must be OK, status code = 200");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return DelResponse;
        }

    }
}
