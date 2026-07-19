using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;

namespace ExternalAPI
{
    internal class Authorization
    {
        static string auth_code = String.Empty;
        static string token_to = String.Empty;
        static string auth02url = String.Empty;
        static string access_token = String.Empty;
        static string auth4_url = String.Empty;

        internal string GetJWT_Token(string tenant, string environment, string authtokenurl)
        {
            string jwt_Token=String.Empty;
            string url = authtokenurl;

            try
            {
                //Cookie Container to store cookies (Crucial: Create it ONCE)
                CookieContainer cookies = new CookieContainer();

                HttpWebRequest JWT_Request = (HttpWebRequest)WebRequest.Create(url);
                JWT_Request.Method = "Get";
                JWT_Request.AllowAutoRedirect = false;
                JWT_Request.CookieContainer = cookies;
                HttpWebResponse JWT_Response = (HttpWebResponse)JWT_Request.GetResponse();

                Debug.Assert(JWT_Response.StatusCode == HttpStatusCode.Found, "Get Request stauts must be Found, status code = 302");
                String jwt_response = JWT_Response.Headers["Location"].ToString();
                if (jwt_response.Contains("jwt="))
                {
                    jwt_Token = JWT_Response.Headers["Location"].Split("&jwt=")[1].ToString();
                }
                
            }catch (Exception ex)
            {
                Console.WriteLine($"failed to Generate JWT Token: GetJWT_Toke(): {ex.Message}");
                throw;

            }
            return jwt_Token;
        }

        internal string tenantLogin(string environment, string jwt, string Username, string Password, string auto_redirect = "true")
        {
            HttpWebResponse LoginResponse;
            string status = "failed";
            string url = null;
            try
            {
                switch (environment)
                {
                    case "prodenv":
                        url = "https://auth.printix.net/login";
                        break;
                    case "prodenv.us":
                        url = "https://auth.us.printix.net/login";
                        break;
                    case "dev01":
                    case "dev02":
                    case "dev03":
                    case "dev04":
                        url = "https://auth." + environment + ".printix.dev/login";
                        break;
                    default:
                        url = "https://auth." + environment + ".printix.net/login";
                        break;
                }

                var postData = "username=" + Uri.EscapeDataString($"{Username}");
                postData += "&password=" + Uri.EscapeDataString($"{Password}");
                postData += "&jwt=" + Uri.EscapeDataString($"{jwt}");

                var data = Encoding.ASCII.GetBytes(postData);
                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(url);
                PostRequest.Method = "POST";
                PostRequest.ContentType = "application/x-www-form-urlencoded";
                if (auto_redirect == "false")
                {
                    PostRequest.AllowAutoRedirect = false;
                }
                else
                {
                    PostRequest.AllowAutoRedirect = true;
                }

                PostRequest.ContentLength = data.Length;

                using (var stream = PostRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                LoginResponse = (HttpWebResponse)PostRequest.GetResponse();
                if (LoginResponse.StatusCode == HttpStatusCode.OK)
                {
                    status = "OK";
                }
                else if (LoginResponse.StatusCode == HttpStatusCode.Found)
                {
                    if (LoginResponse.Headers["Location"].Contains("JWT_TOKEN_EXPIRED"))
                    {
                        status = "Expired";

                    }
                    else if (LoginResponse.Headers["Location"].Contains("client_secre"))
                    {
                        status = "Approved";

                    }
                }

            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())

                    if (ex.Message.Contains("401"))
                    {
                        status = "Unauthorised";
                    }
                    else
                    {
                        status = "Failed";

                    }

            }
            return status;

        }

        internal string redirect_url(string tenant, string redirect_uri, string environment, string response_type, string client_id, string client_secret, string JWT = "", string token = "false")
        {
            string url = string.Empty;
            string status = "Failed";

            try
            {

                if (JWT != "")
                {
                    if (environment == "prodenv")
                    {
                        url = "https://auth.printix.net/oauth/authorize/tenant/" + tenant + ".printix.net?response_type=" + response_type + "&client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&client_secret=" + client_secret + "&jwt=" + JWT;
                    }
                    else if (environment == "prodenv.us")
                    {
                        url = "https://auth.us.printix.net/oauth/authorize/tenant/" + tenant + ".us.printix.net?response_type=" + response_type + "&client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&client_secret=" + client_secret + "&jwt=" + JWT;
                    }
                    else if (environment == "dev01" || environment == "dev02" || environment == "dev03" || environment == "dev04")
                    {
                        url = "https://auth." + environment + ".printix.dev/oauth/authorize/tenant/" + tenant + "." + environment + ".printix.dev?response_type=" + response_type + "&client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&client_secret=" + client_secret + "&jwt=" + JWT;
                    }
                    else
                    {
                        url = "https://auth." + environment + ".printix.net/oauth/authorize/tenant/" + tenant + "." + environment + ".printix.net?response_type=" + response_type + "&client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&client_secret=" + client_secret + "&jwt=" + JWT;

                    }

                }
                else
                {
                    if (environment == "prodenv")
                    {
                        url = "https://auth.printix.net/oauth/authorize/tenant/" + tenant + ".printix.net?response_type=" + response_type + "&client_id=" + client_id + "&client_secret=" + client_secret + "&redirect_uri=" + redirect_uri;
                    }
                    else if (environment == "prodenv.us")
                    {
                        url = "https://auth.us.printix.net/oauth/authorize/tenant/" + tenant + ".us.printix.net?response_type=" + response_type + "&client_id=" + client_id + "&client_secret=" + client_secret + "&redirect_uri=" + redirect_uri;
                    }
                    else if (environment == "dev01" || environment == "dev02" || environment == "dev03" || environment == "dev04")
                    {
                        url = "https://auth." + environment + ".printix.dev/oauth/authorize/tenant/" + tenant + "." + environment + ".printix.dev?response_type=" + response_type + "&client_id=" + client_id + "&client_secret=" + client_secret + "&redirect_uri=" + redirect_uri;
                    }
                    else
                    {
                        url = "https://auth." + environment + ".printix.net/oauth/authorize/tenant/" + tenant + "." + environment + ".printix.net?response_type=" + response_type + "&client_id=" + client_id + "&client_secret=" + client_secret + "&redirect_uri=" + redirect_uri;

                    }

                }

                HttpWebRequest redirect_Request = (HttpWebRequest)WebRequest.Create(url);
                redirect_Request.Method = "Get";
                redirect_Request.AllowAutoRedirect = false;
                HttpWebResponse redirect_Response = (HttpWebResponse)redirect_Request.GetResponse();

                Debug.Assert(redirect_Response.StatusCode == HttpStatusCode.Found, "Get Request stauts must be Found, status code = 302");
                status = "Found";

                if (token == "true")
                {
                    String tokenResponse = redirect_Response.Headers["Location"];
                    Debug.Assert(redirect_Response.Headers["Location"].Contains("invalid_request"), "Failed to found expired token");
                    status = "Failed";
                }

            }catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())

                    if (ex.Message.Contains("400"))
                    {
                        status = "Bad Request";
                    }
                    else if (ex.Message.Contains("302"))
                    {
                        Console.WriteLine($"failed to find EXPIRED Token in LOCATION:redirect_url(): {ex.Message}");
                        throw;

                    }
                    else if (ex.Message.Contains("401"))
                    {
                        status = "Unauthorised";
                    }
                    else
                    {
                        status = "Failed";

                    }
            }

            return status;
        }

        internal string redirect_url_missing_tenant(string redirect_uri, string environment = "devenv")
        {
            string response_type = string.Empty; string client_id = string.Empty; string client_secret = string.Empty; string url = string.Empty; string status = "Failed";
            try
            {
                //1.Generate Random UUID and concate with url.

                Guid Random_uuid = Guid.NewGuid();
                string UUID = Random_uuid.ToString();
                if (environment == "prodenv")
                {
                    url = "https://auth.printix.net/oauth/authorize/tenant/" + UUID + ".printix.net?response_type=" + response_type + "&client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&client_secret=" + client_secret;
                }
                else if (environment == "prodenv.us")
                {
                    url = "https://auth.us.printix.net/oauth/authorize/tenant/" + UUID + ".printix.net?response_type=" + response_type + "&client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&client_secret=" + client_secret;
                }
                else if (environment == "dev01" || environment == "dev02" || environment == "dev03" || environment == "dev04")
                {
                    url = "https://auth." + environment + ".printix.dev/oauth/authorize/tenant/" + UUID + "." + environment + ".printix.dev?response_type=" + response_type + "&client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&client_secret=" + client_secret;
                }
                else
                {
                    url = "https://auth." + environment + ".printix.net/oauth/authorize/tenant/" + UUID + "." + environment + ".printix.net?response_type=" + response_type + "&client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&client_secret=" + client_secret;
                }


                HttpWebRequest redirect_Request = (HttpWebRequest)WebRequest.Create(url);
                redirect_Request.Method = "Get";
                redirect_Request.AllowAutoRedirect = false;
                HttpWebResponse redirect_Response = (HttpWebResponse)redirect_Request.GetResponse();


            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())

                    if (ex.Message.Contains("404"))
                    {
                        status = "BAD URI";
                    }
                    else
                    {
                        status = "Failed";

                    }
            }

            return status;
        }

        internal string signin_context(string environment, string jwt = "")
        {
            string status = "failed"; string url;
            HttpWebResponse Context_Response = null;
            try
            {
                switch (environment)
                {
                    case "prodenv":
                        url = "https://auth.printix.net/signin/context";
                        break;
                    case "prodenv.us":
                        url = "https://auth.us.printix.net/signin/context";
                        break;
                    case "dev01":
                    case "dev02":
                    case "dev03":
                    case "dev04":
                        url = "https://auth." + environment + ".printix.dev/signin/context";
                        break;
                    default:
                        url = "https://auth." + environment + ".printix.net/signin/context";
                        break;
                }
                HttpWebRequest Context_Request = (HttpWebRequest)WebRequest.Create(url);
                Context_Request.Method = "Get";

                if (jwt != "")
                {
                    Context_Request.Headers.Add("x-jwt", jwt);
                    Context_Request.Headers.Add("Accept", "application/json");
                }

                Context_Response = (HttpWebResponse)Context_Request.GetResponse();

            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())

                    if (ex.Message.Contains("401"))
                    {
                        status = "Unauthorised";

                    }
                    else if (ex.Message.Contains("409"))
                    {
                        status = "Conflict";

                    }
                    else
                    {
                        status = "Failed";

                    }

            }
            return status;
        }

        internal string[] Get_Tenant_Info(string environment = "devenv", string tenant = "apitest", string auth_token = "")
        {
            string url;
            HttpWebResponse Tenant_Response;
            string[] response = new string[2];
            try
            {
                switch (environment)
                {
                    case "prodenv":
                        url = "https://api.printix.net/v1/tenants/" + tenant + ".printix.net";
                        break;
                    case "prodenv.us":
                        url = "https://api.us.printix.net/v1/tenants/" + tenant + ".us.printix.net";
                        break;
                    case "dev01":
                    case "dev02":
                    case "dev03":
                    case "dev04":
                        url = "https://api." + environment + ".printix.dev/v1/tenants/" + tenant + "." + environment + ".printix.dev";
                        break;
                    default:
                        url = "https://api." + environment + ".printix.net/v1/tenants/" + tenant + "." + environment + ".printix.net";
                        break;
                }
                HttpWebRequest Tenant_Request = (HttpWebRequest)WebRequest.Create(url);
                Tenant_Request.Method = "Get";
                Tenant_Request.ContentType = "application/json";

                if (auth_token != "")
                {
                    Tenant_Request.Headers.Add("Authorization", "Bearer " + auth_token);
                }
                Tenant_Response = (HttpWebResponse)Tenant_Request.GetResponse();
                Debug.Assert(Tenant_Response.StatusCode == HttpStatusCode.OK, $"Tenant info status is failed: {Tenant_Response.StatusCode}");

                using (StreamReader reader = new StreamReader(Tenant_Response.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    Console.WriteLine($"token_value is: {json.ToString()}");
                    var token_value = JsonConvert.DeserializeObject<JObject>(json);

                    var createdByNameToken = token_value?.Root["name"];
                    string createdByName = createdByNameToken != null ? createdByNameToken.ToString() : string.Empty;
                    response[0] = createdByName;

                    var createdByEmailToken = token_value?.Root["hostingDomains"];
                    string createdByEmail = createdByEmailToken != null ? createdByEmailToken.ToString() : string.Empty;
                    response[1] = createdByEmail;

                }

            }catch (Exception ex){
                Console.WriteLine($"failed to execute getCodefn(): {ex.Message}");
                throw;
            }
            return response;
        }

        internal string GenerateNewToken(string url, string env, string username, string password, string grant_type, string client_id, string client_secret, string response_type, string redirect_uri)
        {
            String auth02Url = String.Empty;
            String auth03Url = String.Empty;
            String authUrl04 = String.Empty;
            var queryParm = String.Empty;
            var codeParm = String.Empty;
            var realCode = String.Empty;
            String code = String.Empty;
            string accessToken = String.Empty;
            String refreshToken = String.Empty;
            String domainSuffix = "net";
            String baseUrl = String.Empty;
            try
            {
                if (env == "dev01" || env == "dev02" || env == "dev03" || env == "dev04")
                {
                    domainSuffix = "dev";
                }
                //Cookie Container to store cookies (Crucial: Create it ONCE)
                CookieContainer cookies = new CookieContainer();
                
                // 1. Initial Request (GET)
                HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(url);
                request1.Method = "GET";
                request1.AllowAutoRedirect = false;
                request1.CookieContainer = cookies;

                using (HttpWebResponse resp1 = (HttpWebResponse)request1.GetResponse())
                {
                    if (resp1.StatusCode != HttpStatusCode.Redirect)
                    {
                        throw new Exception($"Unexpected status code for initial request: {resp1.StatusCode}");
                    }
                    string auth0 = resp1.Headers["Location"];

                    if (string.IsNullOrEmpty(auth0)) // Check if Location header is null or empty
                    {
                        throw new Exception("Location header is missing.");
                    }
                    else if (auth0.Contains("error=invalid_request"))
                    {
                        throw new Exception(" failed with request 1, response containse error=invalid_request");
                    }

                    if (auth0.Contains("?code="))
                    {
                        ExtractCodeFromUrl(auth0);  // Extract code if it present in the response.
                    }
                    else if (auth0.Contains("&jwt="))
                    {
                        String jwtToken;
                        jwtToken = resp1.Headers["Location"].Split("&jwt=")[1];
                        if (env.Contains("prodenv"))
                        {
                            baseUrl= env.Equals("prodenv.us")
                                ? $"https://auth.us.printix.{domainSuffix}/login?jwt={jwtToken}"
                                : $"https://auth.printix.{domainSuffix}/login?jwt={jwtToken}";
                        }
                        else
                        {
                            baseUrl= $"https://auth.{env}.printix.{domainSuffix}/login?jwt={jwtToken}";
                        }
                    }
                    else
                    {
                        String[] domainParts = auth0.Split(".");
                        switch (domainParts.Length)
                        {
                            case 4:
                                baseUrl = $"https://auth.{domainParts[1]}.{domainParts[2]}.{domainSuffix}/login?client_id={client_id}";
                                break;
                            case 5:
                                baseUrl = $"https://auth.{domainParts[1]}.{domainParts[2]}.{domainParts[3]}.{domainSuffix}/login?client_id={client_id}";
                                break;
                            default:
                                if (env.Contains("prodenv"))
                                {
                                    baseUrl = $"https://auth.printix.{domainSuffix}/login?client_id={client_id}";
                                }
                                else {
                                    baseUrl = $"https://auth.{env}.printix.{domainSuffix}/login?client_id={client_id}";
                                }
                                break;
                        }

                    }

                }
                // request 2   Send the POST request  with adminuser and password.

                auth02Url = baseUrl;
                var postData2 = $"username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password)}";

                var data2 = Encoding.ASCII.GetBytes(postData2);

                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(auth02Url);
                request2.Method = "POST";
                request2.ContentType = "application/x-www-form-urlencoded";
                request2.ContentLength = data2.Length;
                request2.AllowAutoRedirect = false;
                request2.CookieContainer = cookies;

                using (var stream = request2.GetRequestStream())
                {
                    stream.Write(data2, 0, data2.Length);
                }

                using (HttpWebResponse resp2 = (HttpWebResponse)request2.GetResponse())
                {

                    auth03Url = resp2.Headers["Location"]; // Get redirect URL
                                                           // Check if the URL ends with '&continue' and remove it
                    if (auth03Url.Contains("AUTHN_BAD_CREDENTIALS"))
                    {
                        throw new Exception(" failed with request 2");
                    }

                    if (auth03Url.EndsWith("&continue"))
                    {
                        auth03Url = auth03Url.Substring(0, auth03Url.Length - "&continue".Length);
                    }
                    else
                    {
                        if (env.Contains("prodenv") && auth03Url.Contains("jwt="))
                        {
                            String jwtToken = resp2.Headers["Location"].Split("jwt=")[1];
                            baseUrl = env.Equals("prodenv.us")
                                ? $"https://auth.us.printix.{domainSuffix}:443/oauth/authorize/tenant/"
                                : $"https://auth.printix.{domainSuffix}:443/oauth/authorize/tenant/"; // Common base URL

                            auth03Url = $"{baseUrl}{redirect_uri.Substring(8)}?response_type={response_type}&client_id={client_id}&client_secret={client_secret}&redirect_uri={redirect_uri}&jwt={jwtToken}"; // Combined string construction
                        }
                        else if (auth03Url.Contains("jwt=")) // Check this condition only once
                        {
                            String jwtToken = resp2.Headers["Location"].Split("jwt=")[1];
                            auth03Url = $"https://auth.{env}.printix.{domainSuffix}:443/oauth/authorize/tenant/{redirect_uri.Split("https://")[1]}?response_type={response_type}&client_id={client_id}&client_secret={client_secret}&redirect_uri={redirect_uri}&jwt={jwtToken}";
                        }
                    }

                    Console.WriteLine("Modified request URL3: " + auth03Url);
                }
                //request 3 
                HttpWebRequest request3 = (HttpWebRequest)WebRequest.Create(auth03Url);
                request3.Method = "GET";
                request3.AllowAutoRedirect = false;
                request3.CookieContainer = cookies;

                using (HttpWebResponse resp3 = (HttpWebResponse)request3.GetResponse()) // Corrected variable name
                {
                    if (resp3.StatusCode != HttpStatusCode.Found) // Check for OK
                    {
                        throw new Exception($"Unexpected status code for third request: {resp3.StatusCode}");
                    }
                    string location = resp3.Headers["Location"];
                    if (string.IsNullOrEmpty(location))
                    {
                        throw new Exception("Location header is missing from the third response.");
                    }
                    Uri locationUri = new Uri(location);
                    string query = locationUri.Query;
                    var queryParams = HttpUtility.ParseQueryString(query);
                    if (queryParams.AllKeys.Contains("code"))
                    {
                        code = queryParams["code"];
                    }
                    else
                    {
                        throw new Exception("failed to get the code from request 3");
                    }
                }

                //request 4
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
                        authUrl04 = $"https://auth.{env}.printix.{domainSuffix}/oauth/token";
                        break;
                    case "prodenv":
                        authUrl04 = $"https://auth.printix.{domainSuffix}/oauth/token";
                        break;
                    case "prodenv.us":
                        authUrl04 = $"https://auth.us.printix.{domainSuffix}/oauth/token";
                        break;
                    default: throw new Exception("wrong environment to generate token");
                }

                var postData = $"grant_type={Uri.EscapeDataString(grant_type)}&code={Uri.EscapeDataString(code)}&redirect_uri={Uri.EscapeDataString(redirect_uri)}&client_id={Uri.EscapeDataString(client_id)}&client_secret={Uri.EscapeDataString(client_secret)}";

                var data4 = Encoding.ASCII.GetBytes(postData);

                HttpWebRequest request4 = (HttpWebRequest)WebRequest.Create(authUrl04);
                request4.Method = "POST";
                request4.ContentType = "application/x-www-form-urlencoded";
                request4.ContentLength = data4.Length;

                using (var stream = request4.GetRequestStream())
                {
                    stream.Write(data4, 0, data4.Length);
                }

                using (HttpWebResponse resp4 = (HttpWebResponse)request4.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(resp4.GetResponseStream()))
                    {
                        var json = reader.ReadToEnd();

                        try
                        {
                            var tokenValue = JsonConvert.DeserializeObject<JObject>(json);
                            accessToken = (string)tokenValue["access_token"];
                            refreshToken = (string)tokenValue["refresh_token"];

                        }
                        catch (Exception ex)
                        {
                            throw new Exception("failed no token is generated from request 4");
                        }
                    }
                }

            }catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw; // Re-throw the exception after logging
            }
            return accessToken;
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