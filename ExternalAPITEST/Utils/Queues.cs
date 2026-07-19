using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;

namespace ExternalAPI
{
    internal class Queues
    {
        BaseFns bfns = new BaseFns();
        internal string[] GetQueuesList(string url, string authtoken, string tenantID)
        {
            string[] QueueNamesList;
            HttpWebResponse postResponse;
            try
            {
                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/queues";
                }
                //Apply PostMethod+
                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(url);
                PostRequest.Method = "POST";
                PostRequest.ContentType = "application/json";
                PostRequest.Headers.Add("Authorization", "Bearer " + authtoken);
                using (var streamWriter = new StreamWriter(PostRequest.GetRequestStream()))
                {
                    string data = "{}";
                    streamWriter.Write(data);
                    streamWriter.Flush();
                }

                postResponse = (HttpWebResponse)PostRequest.GetResponse();
                Debug.Assert(postResponse.StatusCode == HttpStatusCode.OK, $"Fetch Queues List is failed, Actual response :{postResponse.StatusCode}");
                using (StreamReader reader = new StreamReader(postResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var token_value = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)token_value.Root["_embedded"]["px:printerQueueResources"];
                    int length = jArray.Count;
                    QueueNamesList = new string[length];
                    var temp = new List<string>();
                    for (int i = 0; i < length; i++)
                    {
                        QueueNamesList[i] = jArray[i]["name"].ToString();
                       
                    }
                    QueueNamesList = QueueNamesList.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetQueuesList(): {ex.Message}");
                throw;
            }
            return QueueNamesList;
        }

        internal string AddQueue(string url, string authtoken, string tenantID, string networkID)
        {
            string NewQueueName=String.Empty;
            try
            {
                // add code here  build POST method URL + JSON Data
                //networkurl = url/tenantID/networks/networkID
                //printerurl = url/tenantID/queues
                //Data = method: POST  {"ipAddress":"172.17.22.164", "type":"NETWORK",  "networkId":"url/tenantID/networks/networkID", "active":true}


            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute AddQueue(): {ex.Message}");
                throw;
            }
            return NewQueueName;
        }

    }
}
