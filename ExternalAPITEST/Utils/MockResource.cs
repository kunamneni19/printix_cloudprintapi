using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ExternalAPI
{
    internal class MockResource
    {
        internal async Task<string> CreateMockPrinter(string apiEndpoint, string apiKey, string tenantId,string networkId,
            string vendor, string printerName, string printerModel, string snNumber,string ipAddress, string macAddress)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Prepare the request body
                var requestBody = new[]
{
                    new
                    {
                    vendor = vendor,
                    model = printerModel,
                    printerName = printerName,
                    networkUid = networkId,
                    IP = ipAddress,
                    MAC = macAddress,
                    SN = snNumber,
                    PDLs = new string[] { }, 
                    colorSupport = false, 
                    duplexSupport = false, 
                    ieee1284 = (string)null 
                    }
                };
                var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Send the API request to create the mock printer
                var apiUrl = $"{apiEndpoint.TrimEnd('/')}/{tenantId}/printers?list";
                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if ((int)response.StatusCode < 300)
                {   
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to create mock printer. Status Code: {response.StatusCode}, Error: {errorMessage}");
                }

            }
        }

    }
}