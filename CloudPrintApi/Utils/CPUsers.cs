using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using CloudPrintAPI.DTO;
using System.Text;

namespace CloudPrintAPI.Utils
{
    internal class CPUsers
    {
        CPBaseFns bfns = new CPBaseFns();
        internal string[] GetCloudUsersList(string url, string authtoken, string tenantID, string optionalQuery = "")
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

                    try
                    {
                        var token_value = JsonConvert.DeserializeObject<JObject>(json);

                        if (token_value["users"] is JArray usersArray)
                        {
                            List<string> userNamesList = new List<string>(); // Use a List<string>

                            foreach (var user in usersArray) // Iterate directly over the array
                            {
                                string fullName = user["fullName"]?.ToString() ?? "";
                                string email = user["email"]?.ToString() ?? "";
                                string id = user["id"]?.ToString() ?? "";

                                userNamesList.Add($"{fullName} ({email}) - id: {id}"); // Add to the List
                                Console.WriteLine($"User: {userNamesList[userNamesList.Count - 1]}"); // Print the last added user
                            }

                            UsersNamesList = userNamesList.ToArray(); // Convert to array if needed

                        }
                        else
                        {
                            Console.WriteLine("Error: 'users' array not found or is not an array in JSON.");
                            UsersNamesList = new string[0]; // Initialize to empty array
                        }

                    }
                    catch (JsonReaderException ex)
                    {
                        Console.WriteLine("Error parsing JSON: " + ex.Message + ", Raw JSON: " + json);
                        UsersNamesList = new string[0];
                    }
                    catch (NullReferenceException ex)
                    {
                        Console.WriteLine("NullReferenceException: " + ex.Message + ", Raw JSON: " + json);
                        UsersNamesList = new string[0];
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An unexpected error occurred: " + ex.Message + ", Raw JSON: " + json);
                        UsersNamesList = new string[0];
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

        internal string[] GetGuestUsersList(string url, string authtoken, string tenantID, string optionalQuery = "")
        {
            List<string> allGuestUsersList = new List<string>();
            int pageNumber = 0;
            int pageSize = 10;
            bool morePages = true;

            if (!string.IsNullOrEmpty(optionalQuery))
            {
                string fullUrl = $"{url}{tenantID}/users{optionalQuery}";
                HttpWebResponse GetResponse = bfns.ApiGetMethod(fullUrl, authtoken);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))
                {
                    var json = reader.ReadToEnd();
                    var response = JsonConvert.DeserializeObject<GuestUserResponse>(json);

                    List<GuestUser> userList = response.Users;
                    if (userList != null)
                    {
                        foreach (GuestUser user in userList)
                        {
                            if (!string.IsNullOrEmpty(user.FullName))
                            {
                                allGuestUsersList.Add(user.FullName);
                            }
                        }
                    }
                }
                return allGuestUsersList.ToArray();
            }
            try
            {
                while (morePages)
                {
                    string nextUrl = $"{url}{tenantID}/users?page={pageNumber}&pageSize={pageSize}";

                    HttpWebResponse GetResponse = bfns.ApiGetMethod(nextUrl, authtoken);
                    using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))
                    {
                        var json = reader.ReadToEnd();
                        var response = JsonConvert.DeserializeObject<GuestUserResponse>(json);

                        List<GuestUser> userList = response.Users;
                        if (userList != null)
                        {
                            foreach (GuestUser user in userList)
                            {
                                if (!string.IsNullOrEmpty(user.FullName))
                                {
                                    allGuestUsersList.Add(user.FullName);
                                }
                            }
                        }

                        // Check if we have more pages to fetch based on the API response's page info
                        if (response?.Page != null && response.Page.Number < response.Page.TotalPages - 1)
                        {
                            pageNumber++;
                            Console.WriteLine($"Next page found. Fetching page: {pageNumber}");
                        }
                        else
                        {
                            morePages = false;
                            Console.WriteLine("No more pages to fetch.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetGuestUsersList(): {ex.Message}, {ex.StackTrace}");
                throw;
            }

            return allGuestUsersList.ToArray();
        }
    }    
}
