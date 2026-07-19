using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;

namespace ExternalAPI
{
    internal class Users
    {
        BaseFns bfns = new BaseFns();
        internal string[] GetUsersList(string url, string authtoken, string tenantID, string optionalQuery = "")
        {
            string[] UsersNamesList;
            try
            {
                if (tenantID != null)
                {
                    if (!url.EndsWith("/"))
                    {
                        url += "/";
                    }
                    url += tenantID + "/users" + optionalQuery;
                }

                HttpWebResponse GetResponse = bfns.ApiGetMethod(url, authtoken);


                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var token_value = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)token_value.Root["_embedded"]["px:tenantUserResources"];
                    int length = jArray.Count;
                    UsersNamesList = new string[length];

                    for (int i = 0; i < length; i++)
                    {
                        var userId = jArray[i]["_links"]["self"]["href"].ToString();
                        var userIdParsed = userId.Split(@"/users/");
                        UsersNamesList[i] = jArray[i]["_embedded"]["user"]["name"] + " id: " + userIdParsed[1];
                        Console.WriteLine("User[" + i + "]:" + UsersNamesList[i]);
                    }

                }



            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetUsersList(): {ex.Message}, {ex.StackTrace}");
                throw;
            }
            return UsersNamesList;
        }

    }    
}
