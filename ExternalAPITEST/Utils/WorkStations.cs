using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using ExternalAPI.DTO;

namespace ExternalAPI
{
    internal class WorkStations
    {
        BaseFns bfns = new BaseFns();
        Networks _networksUtility = new Networks();

        internal string[] GetWSList(string url, string authtoken, string tenantID)
        {
            string[] wsNamesList;
            try
            {
                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/workstations";
                }
                HttpWebResponse GetResponse = bfns.ApiGetMethod(url, authtoken);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var token_value = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)token_value.Root["_embedded"]["workstations"];
                    int length = jArray.Count;
                    wsNamesList = new string[length];

                    for (int i = 0; i < length; i++)
                    {
                        wsNamesList[i] = jArray[i]["name"].ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetauthcodeFns(): {ex.Message}");
                throw;
            }
            return wsNamesList;
        }

        internal async Task<WsResponseDto> CreateWS(string url, string authtoken, string tenantID, CreateWsRequestDto requestBody)
        {
            WsResponseDto workstationResponse = null;

            if (requestBody == null)
            {
                throw new ArgumentNullException(nameof(requestBody), "Request body for workstation creation cannot be null.");
            }

            string requestUrl = url;
            if (tenantID != null)
            {
                requestUrl = $"{url}/{tenantID}/workstations"; // Corrected URL construction
            }
            else
            {
                throw new ArgumentException("tenantID cannot be null for workstation creation.");
            }

            HttpWebRequest postRequest = (HttpWebRequest)WebRequest.Create(requestUrl); // Use requestUrl here
            postRequest.Method = "POST";
            postRequest.ContentType = "application/json";
            postRequest.Headers.Add("Authorization", "Bearer " + authtoken);

            // Manually construct the JSON request body
            string jsonRequestBody = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                Formatting = Formatting.None // Or Formatting.Indented for readability in logs
            });

            try
            {
                // Asynchronously get the request stream to write the JSON body
                using (Stream stream = await Task.Factory.FromAsync(
                    postRequest.BeginGetRequestStream,
                    postRequest.EndGetRequestStream,
                    null))
                {
                    using (StreamWriter streamWriter = new StreamWriter(stream))
                    {
                        await streamWriter.WriteAsync(jsonRequestBody); // Write JSON asynchronously
                    }
                }

                // Asynchronously get the response
                using (HttpWebResponse postResponse = (HttpWebResponse)await Task.Factory.FromAsync(
                    postRequest.BeginGetResponse,
                    postRequest.EndGetResponse,
                    null))
                {
                    Console.WriteLine("Response status code: " + (int)postResponse.StatusCode);

                    using (StreamReader reader = new StreamReader(postResponse.GetResponseStream()))
                    {
                        string json = await reader.ReadToEndAsync(); // Read response asynchronously
                        Console.WriteLine("Response body: " + json);

                        // Deserialize the JSON response to JObject
                        var json_response_jobject = JsonConvert.DeserializeObject<JObject>(json);

                        // Map JObject to WsResponseDto, including networking and userIds
                        workstationResponse = new WsResponseDto
                        {
                            Name = (string)json_response_jobject["name"],
                            Active = (bool)json_response_jobject["active"],
                            Type = (string)json_response_jobject["type"],
                            Version = (int)json_response_jobject["version"],
                            OperatingSystem = (string)json_response_jobject["operatingSystem"],
                            UserIds = json_response_jobject["userIds"]?.ToObject<List<string>>() ?? new List<string>(),
                            // Add other properties of WsResponseDto as needed
                        };
                    }
                }
            }
            catch (WebException ex)
            {
                // Handle HTTP errors, especially status codes like 400, 401, 404, 500
                if (ex.Response != null)
                {
                    using (var errorStream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(errorStream))
                    {
                        string errorResponse = await reader.ReadToEndAsync();
                        Console.WriteLine($"API Error Response: {errorResponse}");
                    }
                }
                Console.WriteLine($"WebException in CreateWS: {ex.Message}");
                throw; // Re-throw to allow test to catch/fail
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON deserialization error in CreateWS: {ex.Message}");
                throw; // Re-throw
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred in CreateWS: {ex.Message}");
                throw; // Re-throw
            }

            return workstationResponse;
        }

       
        internal CreateWsRequestDto createWsBody()
        {
            string ipAddress1 = _networksUtility.GenerateRandomIpAddress();
            string ipAddress2 = _networksUtility.GenerateRandomIpAddress();
            string gatewayIp = _networksUtility.GenerateRandomIpAddress();
            string gatewayMac = _networksUtility.GenerateRandomMacAddress();

            var requestBody = new CreateWsRequestDto
            {
                Name = GenerateRandomWsName(),
                IpAddresses = new List<string> { ipAddress1, ipAddress2 },
                Networking = new NetworkRequestDto
                {
                    Adapters = new List<AdapterRequestDto>
                    {
                        new AdapterRequestDto
                        {
                            Gateway = new GatewayRequestDto
                            {
                                Ip = gatewayIp,
                                Mac = gatewayMac
                            },
                            Id = "adapter-1",
                            Ipv4Address = null,
                            Ipv6Address = null,
                            Name = null,
                            Ssid = null
                        }
                    }
                }
            };
            return requestBody;
        }

        internal  string GenerateRandomWsName()
        {   Random _random = new Random();

            string[] prefixes = { "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Omega", "Cyber" };
            string[] suffixes = { "Workstation", "Desktop", "Client", "Node", "Station", "Machine", "Host" };
            string[] adjs = { "Primary", "Secondary", "Remote", "Local", "Secure", "Cloud" };
            string[] numbers = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "A", "B", "C", "D", "E", "F", "G", "H" };

            string prefix = prefixes[_random.Next(prefixes.Length)];
            string suffix = suffixes[_random.Next(suffixes.Length)];
            string adj = adjs[_random.Next(adjs.Length)];
            string number = numbers[_random.Next(numbers.Length)];

            return $"{adj}-{prefix}-{suffix}-{number}";
        }
    }
}
