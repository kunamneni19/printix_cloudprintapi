using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;

namespace ExternalAPI
{
    public class Capture
    {
        BaseFns bfns = new BaseFns();

        internal string CreateWorkFlow(string name, string url, string authtoken, string tenantID, string email_to, string source)
        {
            HttpWebResponse postResponse;
            string workflowID;
            try
            {
                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/workflows";
                }
                DataConversion OutjsonData  = new DataConversion();
                //work flow create parameters:
                /*name = workflow_name
                 active = true or false
                 orientation= PORTRAIT or LANDSCAPE
                 filetype = GENERATE_SEARCHABLE_PDF   or GENERATE_DOCX
                 email_to =  it must be with printix.net other you need to register the email before yuo pass it here.
                 source =  MFP or PHONE
                 */
                string DataJson = OutjsonData.Create_Workflow_Email_Data_Json(name, "true", "PORTRAIT", email_to, "GENERATE_SEARCHABLE_PDF", source);

                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(url);
                PostRequest.Method = "POST";
                PostRequest.ContentType = "application/json";
                PostRequest.Headers.Add("Authorization", "Bearer " + authtoken);
                using (var streamWriter = new StreamWriter(PostRequest.GetRequestStream()))
                {
                   string data = DataJson;
                   streamWriter.Write(data);
                   streamWriter.Flush();
                }

                postResponse = (HttpWebResponse)PostRequest.GetResponse();
                Debug.Assert(postResponse.StatusCode == HttpStatusCode.Created, $"Workflow create stauts must be Created, but actual response :{postResponse.StatusCode}");
                using (StreamReader reader = new StreamReader(postResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    string workflowData = (string)json_response.Root["_embedded"]["workflowActivities"][0]["_links"]["self"]["href"];
                    string workflowIdContent = workflowData.Split("workflows")[1];
                    string aworkflowID = workflowIdContent.Split("/activities")[0];
                    workflowID = aworkflowID.Split("/")[1];
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute CreateWorkFlow(): {ex.Message}");
                throw;
            }
            return workflowID;
        }

        internal string CreateWorkFlow_Group(string name, string url, string authtoken, string tenantID, string groupid, string email_to, string source)
        {
            HttpWebResponse postResponse;
            string workflowID;
            try
            {
                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/workflows";
                }
                DataConversion OutjsonData = new DataConversion();
                //work flow create parameters:
                /*name = workflow_name
                 active = true or false
                 orientation= PORTRAIT or LANDSCAPE
                 filetype = GENERATE_SEARCHABLE_PDF   or GENERATE_DOCX
                 email_to =  it must be with printix.net other you need to register the email before yuo pass it here.
                 source =  MFP or PHONE
                 */
                string DataJson = OutjsonData.Create_Workflow_Email_Data_Json(name, "true", "PORTRAIT", email_to, "GENERATE_SEARCHABLE_PDF", source);

                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(url);
                PostRequest.Method = "POST";
                PostRequest.ContentType = "application/json";
                PostRequest.Headers.Add("Authorization", "Bearer " + authtoken);
                using (var streamWriter = new StreamWriter(PostRequest.GetRequestStream()))
                {
                    string data = DataJson;
                    streamWriter.Write(data);
                    streamWriter.Flush();
                }

                postResponse = (HttpWebResponse)PostRequest.GetResponse();
                Debug.Assert(postResponse.StatusCode == HttpStatusCode.Created, $"Workflow create stauts must be Created, but actual response :{postResponse.StatusCode}");
                using (StreamReader reader = new StreamReader(postResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    string workflowData = (string)json_response.Root["_embedded"]["workflowActivities"][0]["_links"]["self"]["href"];
                    string workflowIdContent = workflowData.Split("workflows")[1];
                    string aworkflowID = workflowIdContent.Split("/activities")[0];
                    workflowID = aworkflowID.Split("/")[1];
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute CreateWorkFlow(): {ex.Message}");
                throw;
            }
            return workflowID;
        }

        internal string[] GetWorkFlowIDList(string url, string authtoken, string tenantID, string workflowName)
        {
            string[] WorkflowIDList;
            try
            {
                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/workflows?match="+ workflowName;
                }

                HttpWebResponse GetResponse = bfns.ApiGetMethod(url, authtoken);
                using (StreamReader reader = new StreamReader(GetResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var token_value = JsonConvert.DeserializeObject<JObject>(json);
                    JArray jArray = (JArray)token_value.Root["_embedded"]["px:workflowResources"];
                    int length = jArray.Count;
                    WorkflowIDList = new string[length];
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
                            string workflowContent = jArray[i]["name"].ToString();
                            string Bworkflowid = jArray[i]["_links"]["self"]["href"].ToString();
                            string workflowid = Bworkflowid.Split("/workflows/")[1];
                            
                            if (workflowContent == workflowName)
                            {
                                WorkflowIDList[j] = workflowid;
                                j++;
                            }
                        }
                    }
                    WorkflowIDList = WorkflowIDList.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetWorkflowIDList(): {ex.Message}");
                throw;
            }
            return WorkflowIDList;
        }

        internal string[] CreateUploadLink(string url, string authtoken, string tenantID, string workflowID)
        {
            string[] uploadlink = new string[2];
            HttpWebResponse postResponse;
            try
            {
                url = url + "/" + tenantID + "/" + "workflows" + "/" + workflowID + "/" + "scans";
                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(url);
                PostRequest.Method = "POST";
                PostRequest.ContentType ="application/json";
                PostRequest.Headers.Add("Authorization", "Bearer " + authtoken);
                using (var streamWriter = new StreamWriter(PostRequest.GetRequestStream()))
                {
                    string data = "{}";
                    streamWriter.Write(data);
                    streamWriter.Flush();
                } 

                postResponse = (HttpWebResponse)PostRequest.GetResponse();
                Debug.Assert(postResponse.StatusCode == HttpStatusCode.Created, $"Workflow create stauts must be Created, but actual response :{postResponse.StatusCode}");
                using (StreamReader reader = new StreamReader(postResponse.GetResponseStream()))

                {
                    var json = reader.ReadToEnd();
                    var json_response = JsonConvert.DeserializeObject<JObject>(json);
                    uploadlink[0] = (string)json_response.Root["currentActivity"]["_embedded"]["uploadFiles"][0]["uploadLink"];
                    uploadlink[1] = (string)json_response.Root["_embedded"]["workflowActivities"][0]["_embedded"]["uploadFiles"][0]["_links"]["px:finishUploads"]["href"];
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to genrate Upload link to the Workflow() check work flow properties ex: source must be Mobile: {ex.Message}, {ex.StackTrace}");
                throw;

            }
            return uploadlink;
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
                Console.WriteLine($"failed to execute UploadFile() ,  status is : {status}: {ex.Message},{ex.StackTrace}");
             
                throw;
            }
            return status;
        }

        /*  DataAccessMethod format  
        {
          "fileSize": 3415855,
          "mimeType": "image/jpeg",
          "pageCount": 1,
          "startProcess": true
        }*/

        internal string FinishUploadFile(string url, string authtoken, string filetype)
        {
            HttpWebResponse PostResponse;
            string status = "Failed";
            try
            {
                HttpWebRequest PostRequest = (HttpWebRequest)WebRequest.Create(url);
                PostRequest.Method = "POST";
                PostRequest.ContentType = "application/json";
                PostRequest.Headers.Add("Authorization", "Bearer " + authtoken);
                using (var streamWriter = new StreamWriter(PostRequest.GetRequestStream()))
                {
                    string data = "{\"fileSize\": 100000,\"mimeType\": \"image/jpeg\",\"pageCount\": 1,\"startProcess\": true}";
                    streamWriter.Write(data);
                    streamWriter.Flush();
                }

                PostResponse = (HttpWebResponse)PostRequest.GetResponse();
                Debug.Assert(PostResponse.StatusCode == HttpStatusCode.Accepted || PostResponse.StatusCode == HttpStatusCode.OK, $"Upload File is not Accepted :actual response :{PostResponse.StatusCode}");
                status = "Accepted";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute UploadFile() ,  status is : {status}: {ex.Message} ,{ex.StackTrace}");

                throw;
            }
            return status;
        }

        internal void DeleteWorkflow(string url, string authtoken, string tenantID, string workflowid)
        {
            try
            {
                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/workflows/" + workflowid;
                }
                HttpWebResponse DeleteResponse = bfns.ApiDeleteMethod(url, authtoken);
                Debug.Assert(DeleteResponse.StatusCode == HttpStatusCode.NoContent, $"Delete request is failed {DeleteResponse.StatusCode}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute DeleteWorkflow(): {ex.Message},{ex.StackTrace}");
                throw;
            }

        }


        internal string ModifyWorkFlow(string name, string workflow_id, string url, string authtoken, string tenantID, string email_to, string source="PHONE", string active="true", string orientation= "PORTRAIT", string Ftype= "GENERATE_SEARCHABLE_PDF", string pdfversion= "PDF_1_6")
        {
            HttpWebResponse PutResponse;
            try
            {

                if (tenantID != null)
                {
                    url = url + "/" + tenantID + "/workflows" + "/" + workflow_id;
                }
                DataConversion OutjsonData = new DataConversion();
                
                string DataJson = OutjsonData.Modify_Workflow_Email_Data_Json(name, active, orientation, email_to, source, Ftype, pdfversion);

                HttpWebRequest PutRequest = (HttpWebRequest)WebRequest.Create(url);
                PutRequest.Method = "PUT";
                PutRequest.ContentType = "application/json";
                PutRequest.Headers.Add("Authorization", "Bearer " + authtoken);
                using (var streamWriter = new StreamWriter(PutRequest.GetRequestStream()))
                {
                    string data = DataJson;
                    streamWriter.Write(data);
                    streamWriter.Flush();
                }

                PutResponse = (HttpWebResponse)PutRequest.GetResponse();
                Debug.Assert(PutResponse.StatusCode == HttpStatusCode.Accepted, $"Workflow Modify stauts must be Accepted, but actual response :{PutResponse.StatusCode}");
                

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute CreateWorkFlow(): {ex.Message},{ex.StackTrace}");
                throw;
            }
            return "Accepted";
        }



    }

}
