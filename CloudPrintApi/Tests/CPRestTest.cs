using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CloudPrintAPI.DTO;
using CloudPrintAPI.Utils;
using CommonUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudPrintAPI
{
    [TestClass]
    public class CPRestTest
    {
        public string CP_AUTH_TOKEN = String.Empty;
        public static string ADMIN_USER = String.Empty;
        public static string ADMIN_PASS = String.Empty;
        public static string ENV = String.Empty;
        public static string TENANT_ID = String.Empty;
        public static string IP = String.Empty;
        public static string IN_NETWORK_ID = String.Empty;
        public static string MAC = String.Empty;
        public static string NETWORK_NAME = String.Empty;
        public static string IN_NETWORK_NAME = String.Empty;
        public static string networkID = String.Empty;
        public static string WORKFLOW_NAME = String.Empty;
        public static string WORKFLOW_ID = String.Empty;
        public static string EMAIL_TO = String.Empty;
        public static string IN_WORKFLOW_NAME = String.Empty;
        public static string IN_WORKFLOW_ID = String.Empty;
        public static string GROUP_ID = String.Empty;
        public static string UPLOAD_LINK_WF = String.Empty;
        public static string FÍNISH_UPLOAD_LINK_WF = String.Empty;
        public static string TENANT = String.Empty;
        public static string CP_AUTH_TOKEN_URL = String.Empty;
        public static string CP_GRANT_TYPE = String.Empty;
        public static string CP_CLIENT_ID = String.Empty;
        public static string CP_CLIENT_SECRET = String.Empty;
        public static string CP_URL = String.Empty;
        public static string Session_Environment;
        public static string CPAPI_URL = String.Empty;
        public static string NORMAL_USER = String.Empty;
        public static string USER_PASS = String.Empty;
        public static string dummyPrinterName = String.Empty;

        // FIXED: Switched worker engine context cleanly to CPAuthorizationRest
        static CPAuthorizationRest AuthObj;
        static CPUsers? usersObj;
        static CloudPrint? cloudprintObj;

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestInitialize]
        public async Task Init()
        {
            Session_Environment = System.Environment.GetEnvironmentVariable("environment") ?? "dev01";
            CommonUtil.ValidateEnvironmentVariable(Session_Environment);
            TestContext.Properties["environment"] = Session_Environment;

            string binPath = AppDomain.CurrentDomain.BaseDirectory;
            string repoRoot = Path.GetFullPath(Path.Combine(binPath, @"..\..\..\"));
            string dataPath = Path.Combine(repoRoot, "APITest.Utils", "Data");
            string path_out = CommonUtil.env_setup(Session_Environment, dataPath);

            var testdata = CommonUtil.ReadAndValidateJsonData<TestData>($"{path_out}");
            ENV = Session_Environment;
            ADMIN_USER = testdata.admin.admin_username;
            ADMIN_PASS = testdata.admin.admin_pass;
            TENANT_ID = testdata.admin.tenant_id;
            IP = testdata.admin.ip;
            MAC = testdata.admin.mac;
            NETWORK_NAME = testdata.admin.network_name;
            IN_NETWORK_NAME = testdata.admin.network_name;
            IN_NETWORK_ID = testdata.admin.network_id;
            EMAIL_TO = testdata.admin.email_to;
            IN_WORKFLOW_NAME = testdata.admin.workflow_name;
            IN_WORKFLOW_ID = testdata.admin.workflow_id;
            GROUP_ID = testdata.admin.group_id;
            CP_AUTH_TOKEN_URL = testdata.admin.cp_auth_token_url;
            CP_GRANT_TYPE = testdata.admin.cp_grant_type;
            CP_CLIENT_SECRET = testdata.admin.cp_client_secret;
            CP_CLIENT_ID = testdata.admin.cp_client_id;
            TENANT = testdata.admin.tenant;
            NORMAL_USER = testdata.admin.user_username;
            USER_PASS = testdata.admin.user_pass;
            CPAPI_URL = testdata.admin.cp_url;

            // FIXED: Instantiating correct RestSharp wrapper utility class
            AuthObj = new CPAuthorizationRest();
            CloudPrintAuthTokenResponse authResponse = await AuthObj.GetCloudPrintAuthTokenFullResponseAsync(CP_AUTH_TOKEN_URL, CP_CLIENT_ID, CP_CLIENT_SECRET);

            CP_AUTH_TOKEN = authResponse.AccessToken;
            Assert.AreEqual(HttpStatusCode.OK, authResponse.StatusCode, "Failed to authenticate during initialization.");
            Assert.IsFalse(string.IsNullOrEmpty(authResponse.AccessToken), "Access token received is null or empty.");

            usersObj = new CPUsers();
            cloudprintObj = new CloudPrint();
        }

        [TestMethod, TestCategory("smoke"), TestCategory("Regression"), TestCategory("Release")]
        public void Get_Cloud_Authorization_Token()
        {
            try
            {
                Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
                CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);

                Assert.IsTrue(CP_URL.Contains(TENANT_ID), $"Cloud print api url failed to extract for environment : {Session_Environment}");
                Console.WriteLine($"Test executed successfully {testContextInstance.TestName} :{CP_AUTH_TOKEN}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail($"Test Failed: {testContextInstance.TestName}");
            }
        }

        [TestMethod, TestCategory("smoke"), TestCategory("Regression"), TestCategory("Release")]
        public void Get_Cloud_Print_API_URL()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);

                Assert.IsTrue(CP_URL.Contains(TENANT_ID), $"Cloud print api url failed to extract for environment : {Session_Environment}");
                Console.WriteLine($"Test executed successfully {testContextInstance.TestName} :{CP_URL}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail($"Test Failed: {testContextInstance.TestName}");
            }
        }

        [TestMethod, TestCategory("smoke"), TestCategory("Regression"), TestCategory("Release")]
        public void Get_Cloud_Print_Active_Printers_List()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                CP_URL = cloudprintObj.GetCloudPrintURI(ENV, CP_AUTH_TOKEN, TENANT);

                Assert.IsTrue(CP_URL.Contains(TENANT_ID), $"Cloud print api url failed to extract for environment : {Session_Environment}");
                string[] Printers_List = cloudprintObj.GetActivePrinterList(CP_URL, CP_AUTH_TOKEN);

                Assert.AreNotEqual(0, Printers_List.Length, $"Tenant Environment found no active printers; list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of Active printers:");
                foreach (string name in Printers_List)
                {
                    Console.WriteLine(name);
                }
                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail($"Test Failed: {testContextInstance.TestName}");
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Cloud_Print_Normal_Document_in_Active_Printer()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Assert.IsTrue(CP_URL.Contains(TENANT_ID), $"Cloud print api url failed to extract for environment : {Session_Environment}");
                }
                string[] Printers_id_List = cloudprintObj.GetActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, Printers_id_List.Length, $"Tenant Environment found no active printers; ID list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }

                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, Printers_cr_job_List.Length, $"Tenant Environment found no active printer creation profiles in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                string[] job_url_list = cloudprintObj.Create_Print_Job(Printers_cr_job_List[0], CP_AUTH_TOKEN);
                Assert.IsNotNull(job_url_list, $"No Create Job URLs found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("JOB URL Details");
                foreach (string jurl in job_url_list)
                {
                    Console.WriteLine(jurl);
                }

                CommonUtil resourceFile = new CommonUtil();
                string fileName = resourceFile.GetResourceFilePath("filePNG.png");
                string fileType = "png";
                string upload_status = cloudprintObj.UploadFile(job_url_list[0], fileName, fileType);
                Assert.AreEqual("Created", upload_status, $"Failed to upload PNG {fileName} through Cloud API service.");

                string job_status_url = cloudprintObj.FinishUploadFile(job_url_list[1], CP_AUTH_TOKEN, fileType);
                Assert.IsNotNull(job_status_url, $"Failed to Finish upload valid file through cloud api service.");

                string job_status = cloudprintObj.GetJobStatus(job_status_url, CP_AUTH_TOKEN);
                Assert.AreEqual("True", job_status, $"Print job status evaluation failed in cloud api service.");

                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail($"Test Failed: {testContextInstance.TestName}");
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Cloud_Print_to_Multiple_Printers()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Assert.IsTrue(CP_URL.Contains(TENANT_ID), $"Cloud print api url failed to extract for environment : {Session_Environment}");
                }
                string[] Printers_id_List = cloudprintObj.GetActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, Printers_id_List.Length, $"Tenant Environment found no active printers; list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }

                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, Printers_cr_job_List.Length, $"No Active Printers found in Tenant in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                foreach (string jobid in Printers_cr_job_List)
                {
                    string[] job_url_list = cloudprintObj.Create_Print_Job(jobid, CP_AUTH_TOKEN);
                    Assert.AreNotEqual(0, job_url_list.Length, $"No Create Job URLs {jobid} found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");

                    Console.WriteLine("JOB URL Details");
                    foreach (string jurl in job_url_list)
                    {
                        Console.WriteLine(jurl);
                    }
                    CommonUtil resourceFile = new CommonUtil();
                    string fileName = resourceFile.GetResourceFilePath("filePNG.png");
                    string fileType = "png";
                    string upload_status = cloudprintObj.UploadFile(job_url_list[0], fileName, fileType);
                    Assert.AreEqual("Created", upload_status, $"Failed to upload PNG {fileName} through Cloud API service.");

                    string job_status_url = cloudprintObj.FinishUploadFile(job_url_list[1], CP_AUTH_TOKEN, fileType);
                    Assert.IsNotNull(job_status_url, $"Failed to Finish upload valid file through cloud api service.");

                    string job_status = cloudprintObj.GetJobStatus(job_status_url, CP_AUTH_TOKEN);
                    Assert.AreEqual("True", job_status, $"Print job status evaluation failed in cloud api service.");
                }
                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail($"Test Failed: {testContextInstance.TestName}");
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Cloud_Print_Big_Document_in_Active_Printer()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Assert.IsTrue(CP_URL.Contains(TENANT_ID), $"Cloud print api url failed to extract for environment : {Session_Environment}");
                }
                string[] Printers_id_List = cloudprintObj.GetActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, Printers_id_List.Length, $"Tenant Environment found no active printers; list is empty in environment: {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }

                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, Printers_cr_job_List.Length, $"No Active Printers found in Tenant in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                string[] job_url_list = cloudprintObj.Create_Print_Job(Printers_cr_job_List[0], CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, job_url_list.Length, $"No Create Job URLs found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("JOB URL Details");
                foreach (string jurl in job_url_list)
                {
                    Console.WriteLine(jurl);
                }
                CommonUtil resourceFile = new CommonUtil();
                string fileName = resourceFile.GetResourceFilePath("too-big.png");
                string fileType = "png";
                string upload_status = cloudprintObj.UploadFile(job_url_list[0], fileName, fileType);
                Assert.AreEqual("Created", upload_status, $"Failed to upload big PNG {fileName} through Cloud API service.");

                string job_status_url = cloudprintObj.FinishUploadFile(job_url_list[1], CP_AUTH_TOKEN, fileType);
                Assert.AreNotEqual(string.Empty, job_status_url, $"Failed to Finish upload valid file through cloud api service.");

                string job_status = cloudprintObj.GetJobStatus(job_status_url, CP_AUTH_TOKEN);
                Assert.AreEqual("True", job_status, $"Print job status evaluation failed in cloud api service.");

                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail($"Test Failed: {testContextInstance.TestName}");
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Cloud_Print_Black_Document_in_Active_Printer()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Assert.IsTrue(CP_URL.Contains(TENANT_ID), $"Cloud print api url failed to extract for environment : {Session_Environment}");
                }
                string[] Printers_id_List = cloudprintObj.GetActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Assert.IsNotNull(Printers_id_List, $"Tenant Environment found no active printers; list is null in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }

                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Assert.IsNotNull(Printers_cr_job_List, $"No Active Printers found in Tenant in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                string[] job_url_list = cloudprintObj.Create_Print_Job(Printers_cr_job_List[0], CP_AUTH_TOKEN);
                Assert.IsNotNull(job_url_list, $"No Create Job URLs found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("JOB URL Details");
                foreach (string jurl in job_url_list)
                {
                    Console.WriteLine(jurl);
                }
                CommonUtil resourceFile = new CommonUtil();
                string fileName = resourceFile.GetResourceFilePath("blackPDF.pdf");
                string fileType = "pdf";
                string upload_status = cloudprintObj.UploadFile(job_url_list[0], fileName, fileType);
                Assert.AreEqual("Created", upload_status, $"Failed to upload PDF {fileName} through Cloud API service.");

                string job_status_url = cloudprintObj.FinishUploadFile(job_url_list[1], CP_AUTH_TOKEN, fileType);
                Assert.IsNotNull(job_status_url, $"Failed to Finish upload valid file through cloud api service.");

                string job_status = cloudprintObj.GetJobStatus(job_status_url, CP_AUTH_TOKEN);
                Assert.AreEqual("True", job_status, $"Print job status evaluation failed in cloud api service.");

                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail($"Test Failed: {testContextInstance.TestName}");
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Cloud_Print_Delete()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Assert.IsTrue(CP_URL.Contains(TENANT_ID), $"Cloud print api url failed to extract for environment : {Session_Environment}");
                }
                string[] Printers_id_List = cloudprintObj.GetActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, Printers_id_List.Length, $"Tenant Environment found no active printers; list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }

                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, Printers_cr_job_List.Length, $"No Active Printers found in Tenant in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                string[] job_url_list = cloudprintObj.Create_Print_Job(Printers_cr_job_List[0], CP_AUTH_TOKEN);
                Assert.IsNotNull(job_url_list, $"No Create Job URLs found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("JOB URL Details");
                foreach (string jurl in job_url_list)
                {
                    Console.WriteLine(jurl);
                }
                Thread.Sleep(1000);
                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);

                Assert.AreEqual("OK", job_status, $"Failed to Delete print job in Cloud api service: {job_url_list[3]}");
                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail($"Test Failed: {testContextInstance.TestName}");
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Get_Cloud_Print_Active_Jobs_List()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Assert.IsTrue(CP_URL.Contains(TENANT_ID), $"Cloud print api url failed to extract for environment : {Session_Environment}");
                }

                string[] Active_Jobs_List = cloudprintObj.GetActivePrinterJobList(CP_URL, CP_AUTH_TOKEN);
                Assert.IsNotNull(Active_Jobs_List, $"Cloud Active Jobs List failed to extract in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of Active printers:");
                foreach (string name in Active_Jobs_List)
                {
                    Console.WriteLine(name);
                }
                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail($"Test Failed: {testContextInstance.TestName}");
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Cloud_Print_Upload_Document_to_Not_Active_Printer()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Assert.IsTrue(CP_URL.Contains(TENANT_ID), $"Cloud print api url failed to extract for environment : {Session_Environment}");
                }
                string[] Printers_id_List = cloudprintObj.Get_Not_ActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, Printers_id_List.Length, $"Tenant Environment found no non-active printers; list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of Not Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }

                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, Printers_cr_job_List.Length, $"Tenant Environment found no active printer создание jobs in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                string[] job_url_list = cloudprintObj.Create_Print_Job(Printers_cr_job_List[0], CP_AUTH_TOKEN);
                Assert.IsNotNull(job_url_list, $"No Create Job URLs found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("JOB URL Details");
                foreach (string jurl in job_url_list)
                {
                    Console.WriteLine(jurl);
                }
                CommonUtil resourceFile = new CommonUtil();
                string fileName = resourceFile.GetResourceFilePath("filePNG.png");
                string fileType = "png";
                string upload_status = cloudprintObj.UploadFile(job_url_list[0], fileName, fileType);

                Assert.AreEqual("Created", upload_status, $"Failed to upload PNG {fileName} through Cloud API service.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail($"Test Failed: {testContextInstance.TestName}");
            }
        }
    }
}