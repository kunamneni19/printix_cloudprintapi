using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;
using System.Globalization;
using System.Data;
using CloudPrintAPI.DTO;

namespace CloudPrintAPI
{
    internal class CloudPrint
    {
        CPBaseFns bfns = new CPBaseFns();

        internal string GetCloudPrintURI(string environment, string cp_auth_token, string tenantname)
        {
            HttpWebResponse GetResponse;
            string Cloud_Print_API_URL;
            string url = String.Empty;
            string tenant;
            try
            {
                switch (environment)
                {

                    case "prodenv":
                        url = "https://api.printix.net/cloudprint";
                        break;
                    case "prodenv.us":
                        url = "https://api.us.printix.net/cloudprint";
                        break;
                    case "testenv.us":
                    case "devenv.us":
                    case "devenv":
                    case "testenv":
                        url = "https://api." + environment + ".printix.net/cloudprint";
                        break;
                    case "dev01":
                    case "dev02":
                    case "dev03":
                    case "dev04":
                        url = "https://api." + environment + ".printix.dev/cloudprint";
                        break;
                    default:
                        Assert.Fail($"invalid environment from input : {environment}");
                        break;
                }
                GetResponse = bfns.ApiGetMethod(url, cp_auth_token);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    switch (environment)
                    {
                        case "devenv":
                        case "testenv":
                        case "devenv.us":
                        case "testenv.us":
                            tenant = $"{tenantname}.{environment}.printix.net";
                            url = (string)json_response.Root["_links"][tenant]["href"].ToString().Split("?page=0")[0];
                            break;
                        case "dev01":
                        case "dev02":
                        case "dev03":
                        case "dev04":
                            tenant = $"{tenantname}.{environment}.printix.dev";
                            url = (string)json_response.Root["_links"][tenant]["href"].ToString().Split("?page=0")[0];
                            break;
                        case "prod":
                        case "prodenv":
                            tenant = $"{tenantname}.printix.net";
                            url = (string)json_response.Root["_links"][tenant]["href"].ToString().Split("?page=0")[0];
                            break;
                        case "prodenv.us":
                        case "prod.us":
                            tenant = $"{tenantname}.us.printix.net";
                            url = (string)json_response.Root["_links"][tenant]["href"].ToString().Split("?page=0")[0];
                            break;

                    }

                }
                Cloud_Print_API_URL = url;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetCloudPrintURI(): {ex.Message}");
                throw;
            }
            return Cloud_Print_API_URL;
        }

        internal string[] GetActivePrinterList(string cp_url, string cp_auth_token)
        {
            string[] ActivePrintersList;
            HttpWebResponse GetResponse;
            try
            {
                GetResponse = bfns.ApiGetMethod(cp_url, cp_auth_token);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)json_response.Root["printers"];
                    int length = jArray.Count;
                    Console.WriteLine($"Total Printers Find on this Tenant network for Cloud API: {length}");

                    ActivePrintersList = new string[length];
                    var temp = new List<string>();
                    int j = 0;
                    for (int i = 0; i < length; i++)
                    {
                        if (jArray[i]["connectionStatus"].ToString() == "ONLINE" || jArray[i]["connectionStatus"].ToString() == "WARNING" || jArray[i]["connectionStatus"].ToString() == "UNKNOWN")
                        {
                            ActivePrintersList[j] = jArray[i]["name"].ToString();
                            j++;
                        }

                    }
                    ActivePrintersList = ActivePrintersList.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetCloudPrintURI(): {ex.Message}");
                throw;
            }
            return ActivePrintersList;
        }

        internal string[] GetActivePrinter_ID_List(string cp_url, string cp_auth_token)
        {
            string[] ActivePrintersIDList;
            HttpWebResponse GetResponse;
            try
            {
                GetResponse = bfns.ApiGetMethod(cp_url, cp_auth_token);
                Thread.Sleep(1000);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)json_response.Root["printers"];
                    int length = jArray.Count;
                    Console.WriteLine($"Total Printers Find on this Tenant network for Cloud API: {length}");

                    ActivePrintersIDList = new string[length];
                    var temp = new List<string>();
                    int j = 0;
                    for (int i = 0; i < length; i++)
                    {
                        if (jArray[i]["connectionStatus"].ToString() == "ONLINE" || jArray[i]["connectionStatus"].ToString() == "WARNING" || jArray[i]["connectionStatus"].ToString() == "UNKNOWN")
                        {
                            ActivePrintersIDList[j] = jArray[i]["id"].ToString();
                            j++;
                        }

                    }
                    ActivePrintersIDList = ActivePrintersIDList.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetCloudPrintURI(): {ex.Message}");
                throw;
            }
            return ActivePrintersIDList;
        }

        internal string[] Get_Create_Job_URL_List(string cp_url, string cp_auth_token)
        {
            string[] CreateJobURLList;
            HttpWebResponse GetResponse;
            try
            {
                GetResponse = bfns.ApiGetMethod(cp_url, cp_auth_token);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)json_response.Root["printers"];
                    int length = jArray.Count;
                    Console.WriteLine($"Total Printers Find on this Tenant network for Cloud API: {length}");

                    CreateJobURLList = new string[length];
                    var temp = new List<string>();
                    int j = 0;
                    for (int i = 0; i < length; i++)
                    {
                        if (jArray[i]["connectionStatus"].ToString() == "ONLINE" || jArray[i]["connectionStatus"].ToString() == "WARNING" || jArray[i]["connectionStatus"].ToString() == "UNKNOWN")
                        {
                            CreateJobURLList[j] = (jArray[i]["_links"]["submit"]["href"].ToString().Split("{title}")[0] + "KofaxPrint_job" + i);
                            j++;
                        }

                    }
                    CreateJobURLList = CreateJobURLList.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute Create_Job_URL_List(): {ex.Message}");
                throw;
            }
            return CreateJobURLList;
        }

        internal string[] Create_Print_Job(string cp_url, string cp_auth_token)
        {
            string[] Create_Job_links = new string[4];
            /*
             Create_Job_links[0]  : Upload Document
             Create_Job_links[1]  : Finnish Upload
             Create_Job_links[2]  : Current Job ID 
            */
            HttpWebResponse postResponse;
            try
            {

                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(cp_url);
                PostRequest.Method = "POST";
                PostRequest.ContentType = "application/json";
                PostRequest.Headers.Add("Authorization", "Bearer " + cp_auth_token);
                PostRequest.Headers.Add("version", "1.1");

                using (var streamWriter = new StreamWriter(PostRequest.GetRequestStream()))
                {
                    string data = "{\"color\":false, \"duplex\":\"NONE\", \"page_orientation\":\"AUTO\", \"copies\":1,\"media_size\":\"A4\", \"scaling\":\"NOSCALE\"}";
                    streamWriter.Write(data);
                    streamWriter.Flush();
                }

                postResponse = (HttpWebResponse)PostRequest.GetResponse();
                Debug.Assert(postResponse.StatusCode == HttpStatusCode.OK, $"Workflow create stauts must be Created, but actual response :{postResponse.StatusCode}");

                using (StreamReader reader = new StreamReader(postResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    Create_Job_links[0] = (string)json_response.Root["uploadLinks"][0]["url"];
                    Create_Job_links[1] = (string)json_response.Root["_links"]["uploadCompleted"]["href"];
                    Create_Job_links[2] = ((string)json_response.Root["job"]["_links"]["self"]["href"]).Split("/jobs/")[1];
                    Create_Job_links[3] = ((string)json_response.Root["job"]["_links"]["self"]["href"]);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to genrate Upload link to the Workflow() check work flow properties ex: source must be Mobile: {ex.Message}");
                throw;

            }
            return Create_Job_links;
        }

        internal string[] Get_Job_URL_List(string cp_url, string cp_auth_token)
        {
            string[] JobList;
            HttpWebResponse GetResponse;
            try
            {
                GetResponse = bfns.ApiGetMethod(cp_url, cp_auth_token);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)json_response.Root["printers"];
                    int length = jArray.Count;
                    Console.WriteLine($"Total Printers Find on this Tenant network for Cloud API: {length}");

                    JobList = new string[length];
                    var temp = new List<string>();
                    int j = 0;
                    for (int i = 0; i < length; i++)
                    {
                        if (jArray[i]["connectionStatus"].ToString() == "ONLINE")
                        {
                            JobList[j] = (jArray[i]["_links"]["jobs"]["href"].ToString().Split("{?page}")[0]);
                            j++;
                        }

                    }
                    JobList = JobList.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute Get_Job_URL_List(): {ex.Message}");
                throw;
            }
            return JobList;
        }

        internal string UploadFile(string url, string filePath, string filetype)
        {
            HttpWebResponse PutResponse;
            string status = "Failed";
            try
            {
                HttpWebRequest PutRequest = (HttpWebRequest)WebRequest.Create(url);
                PutRequest.Method = "PUT";
                switch (filetype)
                {
                    case "PDF":
                        PutRequest.ContentType = "application/pdf";
                        break;
                    case "excel":
                        PutRequest.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    case "word":
                        PutRequest.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    case "png":
                        PutRequest.ContentType = "application/png";
                        break;
                    default:
                        PutRequest.ContentType = "application/pdf";
                        break;
                }

                byte[] byteArray = File.ReadAllBytes(@$"{filePath}");
                PutRequest.ContentLength = byteArray.Length;
                PutRequest.Headers.Add("x-ms-blob-type", "BlockBlob");
                Stream dataStream = PutRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                PutResponse = (HttpWebResponse)PutRequest.GetResponse();
                //Debug.Assert(PutResponse.StatusCode == HttpStatusCode.Created, $"Failed to upload a file to Workflow:{url} actual response :{PutResponse.StatusCode}");
                status = "Created";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute UploadFile() ,  status is : {status}: {ex.Message}");

                throw;
            }
            return status;
        }

        internal string FinishUploadFile(string url, string authtoken, string filetype)
        {
            HttpWebResponse PostResponse;
            string job_status;
            try
            {
                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(url);
                PostRequest.Method = "POST";
                PostRequest.ContentType = "application/json";
                PostRequest.Headers.Add("Authorization", "Bearer " + authtoken);
                using (var streamWriter = new StreamWriter(PostRequest.GetRequestStream()))
                {
                    string data = "{\"fileSize\": 100000,\"mimeType\": \"image/" + filetype + "\",\"pageCount\": 1,\"startProcess\": true}";
                    streamWriter.Write(data);
                    streamWriter.Flush();
                }

                PostResponse = (HttpWebResponse)PostRequest.GetResponse();
                Debug.Assert(PostResponse.StatusCode == HttpStatusCode.OK, $"Upload File is not Accepted :actual response :{PostResponse.StatusCode}");

                using (StreamReader reader = new StreamReader(PostResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    job_status = (string)json_response.Root["jobs"][0]["_links"]["self"]["href"];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute FinnishUploadFile() in Cloupd print api: {ex.Message}");

                throw;
            }
            return job_status;
        }

        internal string GetJobStatus(string JobStatusUrl, string cp_auth_token)
        {
            HttpWebResponse GetResponse;
            string status = "False";
            try
            {
                GetResponse = bfns.ApiGetMethod(JobStatusUrl, cp_auth_token);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    status = (string)json_response.Root["success"];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetCloudPrintURI(): {ex.Message}");
                throw;
            }
            return status;
        }

        internal string DeleteJobStatus(string JobStatusUrl, string cp_auth_token)
        {
            string status = "Fail";
            try
            {
                JobStatusUrl = JobStatusUrl + "/delete";

                HttpWebRequest DeleteRequest = (HttpWebRequest)WebRequest.Create(JobStatusUrl);
                DeleteRequest.Method = "POST";
                DeleteRequest.ContentType = "application/json";
                DeleteRequest.Headers.Add("Authorization", "Bearer " + cp_auth_token);
                using (var streamWriter = new StreamWriter(DeleteRequest.GetRequestStream()))
                {
                    string data = "{}";
                    streamWriter.Write(data);
                    streamWriter.Flush();
                }

                HttpWebResponse DeleteResponse = (HttpWebResponse)DeleteRequest.GetResponse();
                Debug.Assert(DeleteResponse.StatusCode == HttpStatusCode.OK, $"Delete print Job is failed :actual response :{DeleteResponse.StatusCode}");
                status = "OK";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute DeleteCloudPrintJob(): {ex.Message}");
                throw;
            }
            return status;
        }

        internal string GetJobOwnerId(string JobStatusUrl, string cp_auth_token)
        {
            HttpWebResponse GetResponse;
            string ownerId = "False";
            try
            {
                GetResponse = bfns.ApiGetMethod(JobStatusUrl, cp_auth_token);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    var jobs = json_response.Root["jobs"];
                    ownerId = (string)jobs.First["ownerId"];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetCloudPrintURI(): {ex.Message},{ex.StackTrace}");
                throw;
            }
            return ownerId;
        }

        /// <summary>
        /// Creates a GUEST_USER using DTOs for request and response.
        /// The CreateGuestUserRequest DTO should be fully populated by the caller.
        /// </summary>
        /// <param name="CPURL">The base URL for the Cloud Print API.</param>
        /// <param name="tenantId">The ID of the tenant.</param>
        /// <param name="cp_auth_token">The authentication token.</param>
        /// <param name="request">The CreateGuestUserRequest DTO containing user details, fully prepared for the API call.</param>
        /// <returns>A GuestUserResponse DTO representing the API's response.</returns>
        public GuestUserResponse CreateGuestUser(string CPURL, string tenantId, string cp_auth_token, CreateGuestUserRequest request)
        {
            GuestUserResponse responseDto = new GuestUserResponse();

            try
            {
                string fullUrl = $"{CPURL}tenants/{tenantId}/users/create";
                Console.WriteLine($"Create Users URL: {fullUrl} ");

                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(fullUrl);
                PostRequest.Method = "POST";
                PostRequest.ContentType = "application/json";
                PostRequest.Headers.Add("Authorization", "Bearer " + cp_auth_token);

                // Serialize the CreateGuestUserRequest DTO to JSON
                string jsonRequest = JsonConvert.SerializeObject(request);

                using (var streamWriter = new StreamWriter(PostRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonRequest);
                    streamWriter.Flush();
                }

                HttpWebResponse PostResponse = (HttpWebResponse)PostRequest.GetResponse();
                responseDto.StatusCode = PostResponse.StatusCode;
                responseDto.IsSuccessStatusCode = (int)PostResponse.StatusCode >= 200 && (int)PostResponse.StatusCode < 300;

                Console.WriteLine("Response status code: " + PostResponse.StatusCode);

                using (StreamReader reader = new StreamReader(PostResponse.GetResponseStream()))
                {
                    string jsonResponseBody = reader.ReadToEnd();
                    responseDto.ResponseBody = jsonResponseBody; // Store raw response body for debugging

                    var apiResponse = JsonConvert.DeserializeObject<GuestUserResponse>(jsonResponseBody);

                    if (apiResponse != null)
                    {
                        responseDto.Success = apiResponse.Success;
                        responseDto.Message = apiResponse.Message;
                        responseDto.TenantId = apiResponse.TenantId;
                        responseDto.SortOrder = apiResponse.SortOrder;
                        responseDto.Users = apiResponse.Users;
                        responseDto.Page = apiResponse.Page;
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
                if (errorResponse != null)
                {
                    responseDto.StatusCode = errorResponse.StatusCode;
                    responseDto.IsSuccessStatusCode = false;

                    using (StreamReader reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        string errorResponseBody = reader.ReadToEnd();
                        responseDto.ResponseBody = errorResponseBody;
                        Console.WriteLine($"Error Response Body: {errorResponseBody}");

                        try
                        {
                            var errorJObject = JsonConvert.DeserializeObject<JObject>(errorResponseBody);
                            responseDto.Message = errorJObject?["message"]?.ToString();
                        }
                        catch (JsonSerializationException)
                        {
                            responseDto.Message = $"API error: {errorResponse.StatusDescription}";
                        }
                    }

                }
                else
                {
                    responseDto.IsSuccessStatusCode = false;
                    responseDto.Success = false;
                    responseDto.Message = $"WebException occurred without an HTTP response: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to execute Create Guest User: {ex.Message}, {ex.StackTrace}");
                responseDto.IsSuccessStatusCode = false;
                responseDto.Success = false;
                responseDto.Message = $"An unexpected error occurred: {ex.Message}";
            }

            return responseDto;
        }

        public List<string> DeleteUser(string deleteUsersURL, string tenantId, string cp_auth_token, string userId)
        {
            List<string>  UserDataList = new List<string>();
            HttpWebResponse PostResponse = null;
            try
            {
                // /users/ID/delete
                deleteUsersURL += "tenants/" + tenantId + @"/users/" + userId + @"/delete".Trim();

                Console.WriteLine($"Delete Users URL: {deleteUsersURL} ");

                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(deleteUsersURL);
                PostRequest.Method = "POST";
                PostRequest.Headers.Add("Authorization", "Bearer " + cp_auth_token);
                PostRequest.ContentType = "application/json-patch+json";

                PostResponse = (HttpWebResponse)PostRequest.GetResponse();

                Console.WriteLine("response status code: " + PostResponse.StatusCode);
                using (StreamReader reader = new StreamReader(PostResponse.GetResponseStream()))
                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    string status = (string)json_response.Root["success"].ToString();
                    if (status != "True")
                    {
                        string errtcode = (string)json_response.Root["errorCode"].ToString();
                        string errtext = (string)json_response.Root["parameterizedErrorText"].ToString();
                        UserDataList.Add(status);
                        UserDataList.Add(errtcode);
                        UserDataList.Add(errtext);
                    }
                    else
                    {

                        UserDataList.Add(status);

                    }
                }
            }
            catch (WebException ex)
            {
                PostResponse = ex.Response as HttpWebResponse;
                if (PostResponse != null)
                {
                    Console.WriteLine($"Received a non-2xx status code: {PostResponse.StatusCode}");

                    // Read the error response body for detailed information
                    using (StreamReader reader = new StreamReader(PostResponse.GetResponseStream()))
                    {
                        var errorJson = reader.ReadToEnd();
                        var json_response = JsonConvert.DeserializeObject<JObject>(errorJson);

                        // The API returns a 'success: false' and specific error details on failure.
                        string successStatus = json_response.SelectToken("success")?.ToString() ?? "False";
                        string errCode = json_response.SelectToken("errorCode")?.ToString() ?? PostResponse.StatusCode.ToString();
                        string errText = json_response.SelectToken("parameterizedErrorText")?.ToString() ?? PostResponse.StatusDescription;

                        // Add the collected information to the list for the test to use
                        UserDataList.Add(successStatus);
                        UserDataList.Add(errCode);
                        UserDataList.Add(errText);

                        // We are testing negative cases, so these are expected behaviors.
                        Console.WriteLine($"Error Response Body: {errorJson}");
                    }
                }
                else
                {
                    // Handle cases where there's no HTTP response (e.g., network timeout)
                    Console.WriteLine("WebException occurred without an HTTP response: " + ex.Message);
                    UserDataList.Add("False");
                    UserDataList.Add("NETWORK_ERROR");
                    UserDataList.Add(ex.Message);
                }
            }
            finally
            {
                if (PostResponse != null)
                {
                    PostResponse.Close();
                }
            }

            return UserDataList;

        }
        public ChangeOwnerResponse ChangeOwner(string changeOwnerURL, string cp_auth_token, string newOwnerEmail, bool emailInQuery = false)
        {
            ChangeOwnerResponse responseDto = new ChangeOwnerResponse(); // Initialize your DTO

            string requestData = "{}";
            string userEmailParam = "";

            try
            {
                changeOwnerURL = changeOwnerURL + "/changeOwner";
                if (emailInQuery)
                {
                    changeOwnerURL += "?userEmail=" + newOwnerEmail;
                }
                else
                {
                    userEmailParam = "userEmail=" + newOwnerEmail;
                }

                HttpWebRequest ChangeOwnerRequest = (HttpWebRequest)WebRequest.Create(changeOwnerURL);
                ChangeOwnerRequest.Method = "POST";
                ChangeOwnerRequest.Headers.Add("Authorization", "Bearer " + cp_auth_token);
                ChangeOwnerRequest.Headers.Add("version", "1.1");

                if (!emailInQuery)
                {
                    ChangeOwnerRequest.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    requestData = userEmailParam;
                    using (var streamWriter = new StreamWriter(ChangeOwnerRequest.GetRequestStream()))
                    {
                        streamWriter.Write(requestData);
                        streamWriter.Flush();
                    }
                }

                HttpWebResponse ChangeOwnerResponse = (HttpWebResponse)ChangeOwnerRequest.GetResponse();

                // Populate StatusCode and IsSuccessStatusCode from the HttpWebResponse
                responseDto.StatusCode = ChangeOwnerResponse.StatusCode;
                responseDto.IsSuccessStatusCode = (int)ChangeOwnerResponse.StatusCode >= 200 && (int)ChangeOwnerResponse.StatusCode < 300;

                using (StreamReader reader = new StreamReader(ChangeOwnerResponse.GetResponseStream()))
                {
                    var json = reader.ReadToEnd();
                    responseDto.ResponseBody = json; // Store the raw JSON response

                    // Deserialize the JSON directly into your ChangeOwnerResponse DTO
                    ChangeOwnerResponse deserializedResponse = JsonConvert.DeserializeObject<ChangeOwnerResponse>(json);

                    // Copy properties from the deserialized object to the responseDto
                    responseDto.TenantId = deserializedResponse.TenantId;
                    responseDto.SortOrder = deserializedResponse.SortOrder;
                    responseDto.Success = deserializedResponse.Success;
                    responseDto.Message = deserializedResponse.Message;
                    responseDto.Jobs = deserializedResponse.Jobs;
                    responseDto.Page = deserializedResponse.Page;
                }
            }
            catch (WebException webEx)
            {
                Console.WriteLine($"Failed to execute ChangeOwner() - WebException: {webEx.Message}");
                if (webEx.Response is HttpWebResponse errorResponse)
                {
                    responseDto.StatusCode = errorResponse.StatusCode;
                    responseDto.IsSuccessStatusCode = false;
                    using (StreamReader reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        responseDto.ResponseBody = reader.ReadToEnd();
                        try
                        {
                            var errorDto = JsonConvert.DeserializeObject<ChangeOwnerResponse>(responseDto.ResponseBody);
                            if (errorDto != null)
                            {
                                responseDto.Message = errorDto.Message ?? "Unknown API error";
                                responseDto.Success = false;
                            }
                        }
                        catch (JsonException)
                        {
                            responseDto.Message = "Error response was not in expected ChangeOwnerResponse format.";
                        }
                    }
                }
                else
                {
                    responseDto.Message = $"An unexpected network error occurred: {webEx.Message}";
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to execute ChangeOwner(): {ex.Message}");
                responseDto.Success = false;
                responseDto.Message = ex.Message;
                throw;
            }
            return responseDto;
        }
       
        internal string[] GetActivePrinterJobList(string cp_url, string cp_auth_token)
        {
            string[] ActivePrintersJobList;
            HttpWebResponse GetResponse;
            try
            {
                GetResponse = bfns.ApiGetMethod(cp_url, cp_auth_token);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)json_response.Root["printers"];
                    int length = jArray.Count;
                    Console.WriteLine($"Total Printers Find on this Tenant network for Cloud API: {length}");

                    ActivePrintersJobList = new string[length];
                    var temp = new List<string>();
                    int j = 0;
                    for (int i = 0; i < length; i++)
                    {
                        if (jArray[i]["connectionStatus"].ToString() == "ONLINE")
                        {
                            ActivePrintersJobList[j] = jArray[i]["_links"]["jobs"]["href"].ToString().Split("{?page")[0];
                            j++;
                        }

                    }
                    ActivePrintersJobList = ActivePrintersJobList.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetCloudPrintURI(): {ex.Message}");
                throw;
            }
            return ActivePrintersJobList;
        }

        internal string[] Get_Not_ActivePrinter_ID_List(string cp_url, string cp_auth_token)
        {
            string[] ActivePrintersIDList;
            HttpWebResponse GetResponse;
            try
            {
                GetResponse = bfns.ApiGetMethod(cp_url, cp_auth_token);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)json_response.Root["printers"];
                    int length = jArray.Count;
                    Console.WriteLine($"Total Printers Find on this Tenant network for Cloud API: {length}");

                    ActivePrintersIDList = new string[length];
                    var temp = new List<string>();
                    int j = 0;
                    for (int i = 0; i < length; i++)
                    {
                        if (jArray[i]["connectionStatus"].ToString() != "ONLINE")
                        {
                            ActivePrintersIDList[j] = jArray[i]["id"].ToString();
                            j++;
                        }

                    }
                    ActivePrintersIDList = ActivePrintersIDList.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetCloudPrintURI(): {ex.Message}");
                throw;
            }
            return ActivePrintersIDList;
        }

        internal string[] OwnersSetupHelper(string env, string cp_url, string tenant_id, string cp_auth_token, string tenant)
        {
            string[] job_url_list;
            try
            {
                if (!cp_url.Contains(tenant_id))
                {
                    cp_url = GetCloudPrintURI(env, cp_auth_token, tenant);
                    Console.WriteLine($"cp_url is : {cp_url}");
                    Debug.Assert(cp_url.Contains(tenant_id), $"Cloud print api url is failed to extract for environment : {env}");
                }

                string[] Printers_id_List = GetActivePrinter_ID_List(cp_url, cp_auth_token);
                Debug.Assert(Printers_id_List.Length != 0,
                    $"Tenant Environment not found any active printers, so list is empty in environment : {env} & tenant {tenant_id}");
                Console.WriteLine("List of Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }

                //Get Create JOB URL
                string[] Printers_cr_job_List = Get_Create_Job_URL_List(cp_url, cp_auth_token);
                Debug.Assert(Printers_id_List.Length != 0,
                    $"No Active Printers found in Tenant in environment : {env} & tenant {tenant_id}");
                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                //Create a Job to active printers.
                job_url_list = Create_Print_Job(Printers_cr_job_List[0], cp_auth_token);
                Debug.Assert(job_url_list != null,
                    $"No Create Job URLs found to extract in environment : {env} & tenant {tenant_id}");
                Console.WriteLine("JOB URL Details");
                foreach (var jurl in job_url_list)
                {
                    Console.WriteLine(jurl);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute OwnersSetupHelper(): {tenant}{ex.Message}{ex.StackTrace}");
                throw;
            }

            return job_url_list;
        }

        public bool FindGuestUser(string cp_url, string cp_auth_token, string tenantID, string fullname)
        {
            bool isFound = false; string uname = String.Empty;
            try
            {
                if (tenantID != null)
                {
                    cp_url = cp_url + "tenants/" + tenantID + "/users" + @"?page=0&pageSize=10";
                }

                HttpWebResponse GetResponse = bfns.ApiGetMethod(cp_url, cp_auth_token);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var token_value = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)token_value.Root["users"];
                    int length = jArray.Count;
                    for (int i = 0; i < length; i++)
                    {
                        uname = (string?)jArray[i]["fullName"];

                        if (uname.Contains(fullname))
                        {
                            Console.WriteLine($"user is found :{uname}");
                            isFound = true;
                            break;
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to find Guest user: {ex.Message}{ex.StackTrace}");
                throw;
            }
            return isFound;
        }

        public List<string> ListGuestUsers(string CPURL, string cp_auth_token, string tenantId, string fullname)
        {
            List<string> UserDataList; string uname = String.Empty;

            try
            {
                CPURL = CPURL + "tenants/" + tenantId + @"/users" + @"?page=0&pageSize=10".Trim();
                Console.WriteLine($"Create Users URL: {CPURL} ");
                UserDataList = new List<string>();
                HttpWebResponse GetResponse = bfns.ApiGetMethod(CPURL, cp_auth_token);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var token_value = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)token_value.Root["users"];
                    int length = jArray.Count;
                    for (int i = 0; i < length; i++)
                    {
                        uname = (string?)jArray[i]["fullName"];

                        if (uname.Contains(fullname))
                        {
                            Console.WriteLine($"user is found :{uname}");
                            UserDataList.Add((string?)jArray[i]["id"]);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute Create Guest User: {ex.Message},{ex.StackTrace}");
                throw;
            }

            return UserDataList;
        }
    }
}

