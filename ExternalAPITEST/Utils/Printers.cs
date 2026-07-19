using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;

namespace ExternalAPI
{
    internal class Printers
    {
        BaseFns bfns = new BaseFns();
        internal string[] GetPrintersList(string url, string authtoken, string tenantID)
        {
            string[] PrinterNamesList;
            try
            {
                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/printers";
                }
                HttpWebResponse GetResponse = bfns.ApiGetMethod(url, authtoken);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var token_value = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)token_value.Root["_embedded"]["printers"];
                    int length = jArray.Count;
                    PrinterNamesList = new string[length];

                    for (int i = 0; i < length; i++)
                    {
                        PrinterNamesList[i] = jArray[i]["_embedded"]["printerModel"]["modelName"].ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetPrintersList(): {ex.Message}");
                throw;
            }
            return PrinterNamesList;
        }

        internal string AddPrinter(string url, string authtoken, string tenantID, string networkID, string IP)
        {
            string NewPrinterName=String.Empty;
            try
            {
                // add code here  build POST method URL + JSON Data
                //networkurl = url/tenantID/networks/networkID
                //printerurl = url/tenantID/printers
                //Data = method: POST  {"ipAddress":"172.17.22.164", "type":"NETWORK",  "networkId":"url/tenantID/networks/networkID", "active":true}


            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute AddPrinter(): {ex.Message}");
                throw;
            }
            return NewPrinterName;
        }

        internal void DeletePrinter(string url, string authtoken, string tenantID, string networkID, string IP)
        {
            try
            {
                // add code here  build POST method URL + JSON Data
                //networkurl = url/tenantID/networks/networkID
                //printerurl = url/tenantID/printers
                //Data = method: POST  {"ipAddress":"172.17.22.164", "type":"NETWORK",  "networkId":"url/tenantID/networks/networkID", "active":true}

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute DeletePrinter(): {ex.Message}");
                throw;
            }
        }

        internal async Task<string> getPrinter(string url, string authtoken,string tenantId, string printerId)
        {
            string[] printerData;
            try
            {
                if (tenantId != null)
                {
                    url = url + "/" + tenantId + "/printers/"+printerId;
                }
                using (HttpClient client = new HttpClient())
                {
                    // Add Authorization header
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authtoken}");

                    // Send GET request
                    var response = await client.GetAsync(url);

                    // Ensure the request was successful
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to retrieve printer data. Status Code: {response.StatusCode}");
                    }

                    // Read and return the response content
                    return await response.Content.ReadAsStringAsync();
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while getting printer data: {ex.Message}");
            }

        }

        internal async Task<string> getPrinterViaCloud(string url, string authtoken)
        {
            string[] printerData;
            try
            {
                
                using (HttpClient client = new HttpClient())
                {
                    // Add Authorization header
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authtoken}");

                    // Send GET request
                    var response = await client.GetAsync(url);

                    // Ensure the request was successful
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to retrieve printer data. Status Code: {response.StatusCode}");
                    }

                    // Read and return the response content
                    return await response.Content.ReadAsStringAsync();
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while getting printer data: {ex.Message}");
            }


        }

    }
}
