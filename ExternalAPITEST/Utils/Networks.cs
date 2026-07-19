using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;


namespace ExternalAPI
{
    //!! Define all network related methods !!//
    //Duplicate request for post method : pass value as param status = "Duplicate"
    internal class Networks
    {
        BaseFns bfns = new BaseFns();
        internal string[] GetNetworksList(string url,string authtoken,string tenantID)
        {
            string[] NetworkNamesList;
            try
            {
                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/networks";
                }

                HttpWebResponse GetResponse = bfns.ApiGetMethod(url, authtoken);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var token_value = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)token_value.Root["_embedded"]["px:networkResources"];
                    int length = jArray.Count;
                    NetworkNamesList = new string[length];

                    for (int i = 0; i < length; i++)
                    {
                        NetworkNamesList[i] = jArray[i]["name"].ToString();
                    }

                }
            }catch(Exception ex)
            {
                Console.WriteLine($"failed to execute GetauthcodeFns(): {ex.Message}");
                throw;
            }
            return NetworkNamesList;
        }

        internal string AddNetworkName(string url, string authtoken, string tenantID, string networkName)
        {
            string networkID;
            try
            {
                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/networks";
                }
                
                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(url);
                PostRequest.Method = "POST";
                PostRequest.ContentType = "application/json";
                PostRequest.Headers.Add("Authorization", "Bearer " + authtoken);

                using (var streamWriter = new StreamWriter(PostRequest.GetRequestStream()))
                {
                    string json = "{ \"name\" : \""+ networkName+"\"}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
                HttpWebResponse PostResponse = (HttpWebResponse)PostRequest.GetResponse();
                Console.WriteLine("response status code: " + PostResponse.StatusCode);
                using (StreamReader reader = new StreamReader(PostResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    string networkData = (string)json_response.Root["_links"]["self"]["href"];
                    networkID = networkData.Split("networks/")[1];
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine($"failed to execute AddNetworkName(): {ex.Message}");
                throw;

            }
            return networkID;
        }

        internal string AddNetworkGateway(string url, string authtoken, string tenantID, string networkId, string ipData, string macData)
        {
            //Add Network IP & MAC to the network name. 
            //
            HttpWebResponse PatchResponse;
            try
            {
                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/networks/"+ networkId;
                }

                HttpWebRequest PatchRequest = (HttpWebRequest)WebRequest.Create(url);
                PatchRequest.Method = "Patch";
                PatchRequest.ContentType = "application/json";
                PatchRequest.Headers.Add("Authorization", "Bearer " + authtoken);

                using (var streamWriter = new StreamWriter(PatchRequest.GetRequestStream()))
                {
                    string json = "{\"ip\":\"" + ipData + "\"," +
               "\"mac\":\"" + macData + "\"}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
                PatchResponse = (HttpWebResponse)PatchRequest.GetResponse();
                Debug.Assert(PatchResponse.StatusCode == HttpStatusCode.OK, "Patch Request stauts must be OK, status code = 200");
                         
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLowerInvariant().Contains("409")){
                    return "Conflict";
                }
                else {
                    Console.WriteLine($"Conflict status to execute AdddNetworkGateway(): {ex.Message}");
                    throw;

                }



            }
            return "OK";
        }

        // Get list of network-id's based on network-name
        internal string[] GetNetworksIDList(string url, string authtoken, string tenantID, string networkName)
        {
            string[] NetworkIDList;
            try
            {
                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/networks";
                }

                HttpWebResponse GetResponse = bfns.ApiGetMethod(url, authtoken);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var token_value = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)token_value.Root["_embedded"]["px:networkResources"];
                    int length = jArray.Count;
                    NetworkIDList = new string[length];
                    var temp = new List<string>();
                    int j = 0;
                    for (int i = 0; i < length; i++)
                    {
                        if (jArray[i]["name"] == null)
                        {
                            jArray[i]["name"] = null;
                        }
                        else
                        {
                            string networkContent = jArray[i]["name"].ToString();
                            string networkid = jArray[i]["_links"]["px:discoverEnvironment"]["href"].ToString();
                            string Bnetwork = networkid.Split("/networks/")[1];
                            string Anetwork = Bnetwork.Split("/discoverEnvironment")[0];
                            
                            if (networkContent == networkName)
                            {
                                NetworkIDList[j] = Anetwork;
                                j++;
                            }
                        }
                    }
                    NetworkIDList = NetworkIDList.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetNetworkIDList(): {ex.Message}");
                throw;
            }
            return NetworkIDList;
        }

        internal void DeleteNetwork(string url, string authtoken, string tenantID, string networkid)
        {
            try
            {
                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/networks"+"/"+networkid;
                }

                HttpWebResponse DeleteResponse = bfns.ApiDeleteMethod(url, authtoken);
                Debug.Assert(DeleteResponse.StatusCode == HttpStatusCode.OK, $"Delete request is failed {DeleteResponse.StatusCode}");
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute DeleteNetwork(): {ex.Message}");
                throw;
            }
 
        }

        // Generate a random IP address
        internal string GenerateRandomIpAddress()
        {
            Random random = new Random();
            return $"{random.Next(192, 193)}.{random.Next(10, 256)}.{random.Next(10, 256)}.{random.Next(10, 100)}";
        }

        // Generate a random MAC address
        internal string GenerateRandomMacAddress()
        {
            Random random = new Random();
            byte[] macAddr = new byte[6];
            random.NextBytes(macAddr);

            macAddr[0] = (byte)(macAddr[0] & (byte)254);
            macAddr[0] = (byte)(macAddr[0] | (byte)2);

            return string.Join(":", macAddr.Select(b => b.ToString("X2")));
        }
    }
}
