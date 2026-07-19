using CloudPrintAPI.Utils;
using System.Diagnostics;
using System.Net;
using CloudPrintAPI.DTO;
using CloudprintAPI;
using CommonUtils;
using System.Globalization;

namespace CloudPrintAPI
{

    [TestClass]
    public class CloudPrintTest
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
        public static string NORMAL_USER = String.Empty; public static string USER_PASS = String.Empty;
        public static string dummyPrinterName = String.Empty;
        
        static CPAuthorization AuthObj;
        static CPUsers? usersObj;
        static CloudPrint? cloudprintObj;

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestInitialize]
        public void Init()
        {
            Session_Environment = System.Environment.GetEnvironmentVariable("environment")??"dev01";
            CommonUtil.ValidateEnvironmentVariable(Session_Environment);
            TestContext.Properties["environment"] = Session_Environment;
            string binPath = AppDomain.CurrentDomain.BaseDirectory;
            string repoRoot = Path.GetFullPath(Path.Combine(binPath, @"..\..\..\"));
            string dataPath = Path.Combine(repoRoot, "APITest.Utils", "Data");
            string path_out = CommonUtil.env_setup(Session_Environment, dataPath);
            //path_out = CommonUtil.env_setup(Session_Environment, path);
            var testdata = CommonUtil.ReadAndValidateJsonData<TestData>($"{path_out}");
            ENV = Session_Environment;
            ADMIN_USER = testdata.admin.admin_username; ADMIN_PASS = testdata.admin.admin_pass;
            TENANT_ID = testdata.admin.tenant_id;
            IP = testdata.admin.ip; MAC = testdata.admin.mac;
            NETWORK_NAME = testdata.admin.network_name; IN_NETWORK_NAME = testdata.admin.network_name;
            IN_NETWORK_ID = testdata.admin.network_id; EMAIL_TO = testdata.admin.email_to;
            IN_WORKFLOW_NAME = testdata.admin.workflow_name;
            IN_WORKFLOW_ID = testdata.admin.workflow_id; GROUP_ID = testdata.admin.group_id;
            CP_AUTH_TOKEN_URL = testdata.admin.cp_auth_token_url;
            CP_GRANT_TYPE = testdata.admin.cp_grant_type;
            CP_CLIENT_SECRET = testdata.admin.cp_client_secret;
            CP_CLIENT_ID = testdata.admin.cp_client_id;
            TENANT = testdata.admin.tenant;
            NORMAL_USER = testdata.admin.user_username;
            USER_PASS = testdata.admin.user_pass;

            CPAPI_URL = testdata.admin.cp_url;
            AuthObj = new CPAuthorization();
            CloudPrintAuthTokenResponse authResponse = AuthObj.GetCloudPrintAuthTokenFullResponse(CP_AUTH_TOKEN_URL, CP_CLIENT_ID, CP_CLIENT_SECRET);
            CP_AUTH_TOKEN = authResponse.AccessToken;
            Assert.AreEqual(HttpStatusCode.OK, authResponse.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(authResponse.AccessToken));

            //objects creation for resources
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
                Debug.Assert(CP_URL.Contains(TENANT_ID), $"Cloud print api url is failed to extract for environment : {Session_Environment}");
                Console.WriteLine($"Test executed successfully {testContextInstance.TestName} :{CP_AUTH_TOKEN}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }

        }

        [TestMethod, TestCategory("smoke"), TestCategory("Regression"), TestCategory("Release")]
        public void Get_Cloud_Print_API_URL()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                Debug.Assert(CP_URL.Contains(TENANT_ID), $"Cloud print api url is failed to extract for environment : {Session_Environment}");
                Console.WriteLine($"Test executed successfully {testContextInstance.TestName} :{CP_URL}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

        }

        [TestMethod, TestCategory("smoke"), TestCategory("Regression"), TestCategory("Release")]
        public void Get_Cloud_Print_Active_Printers_List()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                // Code must be placed here ENV  is devenv,  testenv, prodenv, devenv.us, testenv.us 
                CP_URL = cloudprintObj.GetCloudPrintURI(ENV, CP_AUTH_TOKEN, TENANT);
                Debug.Assert(CP_URL.Contains(TENANT_ID), $"Cloud print api url is failed to extract for environment : {Session_Environment}");
                string[] Printers_List = cloudprintObj.GetActivePrinterList(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_List.Length != 0, $"Teanant Environment not found any active printers, so list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");
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
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Cloud_Print_Normal_Document_in_Active_Printer()
        {

            /*
             1. Get list of Active printer Create JOB URLs 
             2. Create a Print Job using the URL from above step.
             3. Upload the Docuemnt from the Step 2 output
             4. Perform Finnish Upload from Step 2.
             5. Validate Job state after Finnish upload.
            */
            try
            {

                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Debug.Assert(CP_URL.Contains(TENANT_ID), $"Cloud print api url is failed to extract for environment : {Session_Environment}");
                }
                string[] Printers_id_List = cloudprintObj.GetActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_id_List.Length != 0, $"Teanant Environment not found any active printers, so list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }
                //1.Get Create JOB URL

                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_id_List.Length != 0, $"Teanant Environment not found any active printers, so list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                //2.Create a Job to active printers.
                //output:
                //  a1. Get Upload Document Link
                //  a2. Finnish Upload Document

                string[] job_url_list = cloudprintObj.Create_Print_Job(Printers_cr_job_List[0], CP_AUTH_TOKEN);
                Debug.Assert(job_url_list != null, $"No Create Job URLs found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("JOB URL Details");
                foreach (string jurl in job_url_list)
                {
                    Console.WriteLine(jurl);
                }
                //3.Upload Documents
                CommonUtil resourceFile = new CommonUtil();
                string fileName = resourceFile.GetResourceFilePath("filePNG.png");
                string fileType = "png";    // "word"    or "excel"   or  "PDF" or "png"
                string upload_status = cloudprintObj.UploadFile(job_url_list[0], fileName, fileType);
                Debug.Assert(upload_status == "Created", $"Failed to upload PNG {fileName} through Cloud API service");
                //4.FinishUpload url
                string job_status_url = cloudprintObj.FinishUploadFile(job_url_list[1], CP_AUTH_TOKEN, fileType);
                Debug.Assert(job_status_url != null, $"Failed to Finish upload valid file thru cloud api service:{job_status_url}");
                //5.Get Job status
                string job_status = cloudprintObj.GetJobStatus(job_status_url, CP_AUTH_TOKEN);
                Debug.Assert(job_status == "True", $"Failed to Job status is failed in cloud api service:{job_status}");

                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Cloud_Print_to_Multiple_Printers()
        {
            /*
             1. Get list of Active printer Create JOB URLs 
             2. Create a Print Job using the URL from above step.
             3. Upload the Docuemnt from the Step 2 output
             4. Perform Finnish Upload from Step 2.
             5. Validate Job state after Finnish upload.
            */
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Debug.Assert(CP_URL.Contains(TENANT_ID), $"Cloud print api url is failed to extract for environment : {Session_Environment}");
                }
                string[] Printers_id_List = cloudprintObj.GetActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_id_List.Length != 0, $"Teanant Environment not found any active printers, so list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }
                //1.Get Create JOB URL

                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_id_List.Length != 0, $"No Active Printers found in Tenant in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                //2.Create a Job to active printers.
                //output:
                //  a1. Get Upload Document Link
                //  a2. Finnish Upload Document
                foreach (string jobid in Printers_cr_job_List)
                {
                    string[] job_url_list = cloudprintObj.Create_Print_Job(jobid, CP_AUTH_TOKEN);
                    Debug.Assert(job_url_list.Length != 0, $"No Create Job URLs  {jobid} found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");
                    Console.WriteLine("JOB URL Details");
                    foreach (string jurl in job_url_list)
                    {
                        Console.WriteLine(jurl);
                    }
                    //3.Upload Documents
                    CommonUtil resourceFile = new CommonUtil();
                    string fileName = resourceFile.GetResourceFilePath("filePNG.png");
                    string fileType = "png";    // "word"    or "excel"   or  "PDF" or "png"
                    string upload_status = cloudprintObj.UploadFile(job_url_list[0], fileName, fileType);
                    Debug.Assert(upload_status == "Created", $"Failed to upload PNG {fileName} through Cloud API service");
                    //4.FinishUpload url
                    string job_status_url = cloudprintObj.FinishUploadFile(job_url_list[1], CP_AUTH_TOKEN, fileType);
                    Debug.Assert(job_status_url != null, $"Failed to Finish upload valid file thru cloud api service:{job_status_url}");
                    //5.Get Job status
                    string job_status = cloudprintObj.GetJobStatus(job_status_url, CP_AUTH_TOKEN);
                    Debug.Assert(job_status == "True", $"Failed to Job status is failed in cloud api service:{job_status}");
                }
                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }


        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Cloud_Print_Big_Document_in_Active_Printer()
        {
            /*
             1. Get list of Active printer Create JOB URLs 
             2. Create a Print Job using the URL from above step.
             3. Upload the Docuemnt from the Step 2 output
             4. Perform Finnish Upload from Step 2.
             5. Validate Job state after Finnish upload.
            */
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Debug.Assert(CP_URL.Contains(TENANT_ID), $"Cloud print api url is failed to extract for environment : {Session_Environment}");
                }
                string[] Printers_id_List = cloudprintObj.GetActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_id_List.Length != 0, $"Teanant Environment not found any active printers, so list is empty in environment: {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }
                //1.Get Create JOB URL

                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_id_List.Length != 0, $"No Active Printers found in Tenant in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                //2.Create a Job to active printers.
                //output:
                //  a1. Get Upload Document Link
                //  a2. Finnish Upload Document

                string[] job_url_list = cloudprintObj.Create_Print_Job(Printers_cr_job_List[0], CP_AUTH_TOKEN);
                Debug.Assert(job_url_list.Length != 0, $"No Create Job URLs found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("JOB URL Details");
                foreach (string jurl in job_url_list)
                {
                    Console.WriteLine(jurl);
                }
                //3.Upload Documents
                CommonUtil resourceFile = new CommonUtil();
                string fileName = resourceFile.GetResourceFilePath("too-big.png");
                string fileType = "png";    // "word"    or "excel"   or  "PDF" or "png"
                string upload_status = cloudprintObj.UploadFile(job_url_list[0], fileName, fileType);
                Debug.Assert(upload_status == "Created", $"Failed to upload PNG {fileName} through Cloud API service");
                //4.FinishUpload url
                string job_status_url = cloudprintObj.FinishUploadFile(job_url_list[1], CP_AUTH_TOKEN, fileType);
                Debug.Assert(job_status_url != string.Empty, $"Failed to Finish upload valid file thru cloud api service:{job_status_url}");
                //5.Get Job status
                string job_status = cloudprintObj.GetJobStatus(job_status_url, CP_AUTH_TOKEN);
                Debug.Assert(job_status == "True", $"Failed to Job status is failed in cloud api service:{job_status}");

                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }


        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Cloud_Print_Black_Document_in_Active_Printer()
        {
            /*
             1. Get list of Active printer Create JOB URLs 
             2. Create a Print Job using the URL from above step.
             3. Upload the Docuemnt from the Step 2 output
             4. Perform Finnish Upload from Step 2.
             5. Validate Job state after Finnish upload.
            */
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Debug.Assert(CP_URL.Contains(TENANT_ID), $"Cloud print api url is failed to extract for environment : {Session_Environment}");
                }
                string[] Printers_id_List = cloudprintObj.GetActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_id_List != null, $"Teanant Environment not found any active printers, so list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }
                //1.Get Create JOB URL

                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_id_List != null, $"No Active Printers found in Tenant in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                //2.Create a Job to active printers.
                //output:
                //  a1. Get Upload Document Link
                //  a2. Finnish Upload Document

                string[] job_url_list = cloudprintObj.Create_Print_Job(Printers_cr_job_List[0], CP_AUTH_TOKEN);
                Debug.Assert(job_url_list != null, $"No Create Job URLs found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("JOB URL Details");
                foreach (string jurl in job_url_list)
                {
                    Console.WriteLine(jurl);
                }
                //3.Upload Documents
                CommonUtil resourceFile = new CommonUtil();
                string fileName = resourceFile.GetResourceFilePath("blackPDF.pdf");
                string fileType = "pdf";    // "word"    or "excel"   or  "PDF" or "png"
                string upload_status = cloudprintObj.UploadFile(job_url_list[0], fileName, fileType);
                Debug.Assert(upload_status == "Created", $"Failed to upload PNG {fileName} through Cloud API service");
                //4.FinishUpload url
                string job_status_url = cloudprintObj.FinishUploadFile(job_url_list[1], CP_AUTH_TOKEN, fileType);
                Debug.Assert(job_status_url != null, $"Failed to Finish upload valid file thru cloud api service:{job_status_url}");
                //5.Get Job status
                string job_status = cloudprintObj.GetJobStatus(job_status_url, CP_AUTH_TOKEN);
                Debug.Assert(job_status == "True", $"Failed to Job status is failed in cloud api service:{job_status}");

                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }


        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Cloud_Print_Delete()
        {
            /*
             1. Get list of Active printer Create JOB URLs 
             2. Create a Print Job using the URL from above step.
             3. Delete printJob.
            */
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Debug.Assert(CP_URL.Contains(TENANT_ID), $"Cloud print api url is failed to extract for environment : {Session_Environment}");
                }
                string[] Printers_id_List = cloudprintObj.GetActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_id_List.Length != 0, $"Teanant Environment not found any active printers, so list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }
                //1.Get Create JOB URL

                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_id_List.Length != 0, $"No Active Printers found in Tenant in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                //2.Create a Job to active printers.
                //output:"created Job URL".

                string[] job_url_list = cloudprintObj.Create_Print_Job(Printers_cr_job_List[0], CP_AUTH_TOKEN);
                Debug.Assert(job_url_list != null, $"No Create Job URLs found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("JOB URL Details");
                foreach (string jurl in job_url_list)
                {
                    Console.WriteLine(jurl);
                }
                Thread.Sleep(1000);
                //3.Delete Print Job
                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);
                Debug.Assert(job_status == "OK", $"Failed to Delete print job in Cloud api service:{job_url_list[3]}");
                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

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
                    Debug.Assert(CP_URL.Contains(TENANT_ID), $"Cloud print api url is failed to extract for environment : {Session_Environment}");
                }

                string[] Active_Jobs_List = cloudprintObj.GetActivePrinterJobList(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Active_Jobs_List != null, $"Cloud Active Jobs List is failed to extract in environment : {Session_Environment} & tenant {TENANT_ID}");
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
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }


        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void Cloud_Print_Upload_Document_to_Not_Active_Printer()
        {
            /*
             1. Get list of Active printer Create JOB URLs 
             2. Create a Print Job using the URL from above step.
             3. Upload the Docuemnt from the Step 2 output
             4. Perform Finnish Upload from Step 2.
             5. Validate Job state after Finnish upload.
            */

            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                if (CP_URL == string.Empty)
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                    Debug.Assert(CP_URL.Contains(TENANT_ID), $"Cloud print api url is failed to extract for environment : {Session_Environment}");
                }
                string[] Printers_id_List = cloudprintObj.Get_Not_ActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_id_List.Length != 0, $"Teanant Environment not found any active printers, so list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of Not Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }
                //1.Get Create JOB URL

                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Debug.Assert(Printers_id_List.Length != 0, $"Teanant Environment not found any active printers, so list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                //2.Create a Job to active printers.
                //output:
                //  a1. Get Upload Document Link
                //  a2. Finnish Upload Document

                string[] job_url_list = cloudprintObj.Create_Print_Job(Printers_cr_job_List[0], CP_AUTH_TOKEN);
                Debug.Assert(job_url_list != null, $"No Create Job URLs found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");
                Console.WriteLine("JOB URL Details");
                foreach (string jurl in job_url_list)
                {
                    Console.WriteLine(jurl);
                }
                //3.Upload Documents
                CommonUtil resourceFile = new CommonUtil();
                string fileName = resourceFile.GetResourceFilePath("filePNG.png");
                string fileType = "png";    // "word"    or "excel"   or  "PDF" or "png"
                string upload_status = cloudprintObj.UploadFile(job_url_list[0], fileName, fileType);
                Debug.Assert(upload_status == "Created", $"Failed to upload PNG {fileName} through Cloud API service");
                //4.FinishUpload url
                string job_status_url = cloudprintObj.FinishUploadFile(job_url_list[1], CP_AUTH_TOKEN, fileType);
                Debug.Assert(job_status_url != null, $"Failed to Finish upload valid file thru cloud api service:{job_status_url}");
                //5.Get Job status
                string job_status = cloudprintObj.GetJobStatus(job_status_url, CP_AUTH_TOKEN);
                Debug.Assert(job_status == "True", $"Failed to Job status is failed in cloud api service:{job_status}");

                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")] 
        public void TestGetListGuestUsers()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            /* create guest user  */
            var randomUserSuffix = new Random();
            var uniqueUserId = randomUserSuffix.Next(999999);
            var uniqueFullUserName = "GUser" + uniqueUserId;
            var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";
            string GUser = uniqueFullUserName; 

            // Make sure you have a valid role, pin, password, and expiration timestamp for creation
            string defaultRole = "GUEST_USER"; 
            string defaultPin = "1980";
            string defaultPassword = "EeGahqu7";
            DateTime expiryDate = DateTime.Now.AddDays(30);
            string formattedExpiryDate = expiryDate.ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture);

            bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, GUser);
            if (!isFound)
            {
                // 1. Create the CreateGuestUserRequest DTO
                var createRequest = new CreateGuestUserRequest
                {
                    Email = uniqueUserEmail,
                    FullName = uniqueFullUserName,
                    Role = defaultRole,
                    Pin = defaultPin,
                    Password = defaultPassword,
                    ExpirationTimestamp = formattedExpiryDate,
                    SendWelcomeEmail = false, 
                    SendExpirationEmail = false, 
                    WelcomeEmailContent = null 
                };

                // 2. Call the new CreateGuestUser method and get GuestUserResponse
                GuestUserResponse createResponse = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

                // 3. Update assertions to check the GuestUserResponse DTO
                Assert.IsTrue(createResponse.IsSuccessStatusCode,
                              $"API call to create user was not successful. Status: {createResponse.StatusCode}, Message: {createResponse.Message}. Response Body: {createResponse.ResponseBody}");
                Assert.IsTrue(createResponse.Success,
                              $"API reported failure for user creation: {createResponse.Message}. Response Body: {createResponse.ResponseBody}");
                Assert.IsNotNull(createResponse.Users, "Response Users list is null after creation.");
                Assert.AreEqual(1, createResponse.Users.Count, "Expected 1 user in the response after creation.");
                Assert.AreEqual(uniqueFullUserName, createResponse.Users[0].FullName, "Created user's full name does not match.");
            }

            /*Fetch Guest users List */

            if (string.IsNullOrEmpty(CP_URL)) // Use String.IsNullOrEmpty for robust check
            {
                CP_URL = cloudprintObj.GetCloudPrintURI(ENV, CP_AUTH_TOKEN, TENANT);
                Debug.Assert(CP_URL.Contains(TENANT_ID),
                     $"Cloud print api url is failed to extract for environment : {ENV}");
            }

            string baseUrlForUsersList = CP_URL.Split(new[] { TENANT_ID }, StringSplitOptions.None)[0];

            var userNames = usersObj.GetGuestUsersList(baseUrlForUsersList, CP_AUTH_TOKEN, TENANT_ID);

            // Use Assert for testing conditions, not Debug.Assert in release code
            Assert.IsNotNull(userNames, "GetCloudUsersList returned null.");
            Assert.IsTrue(userNames.Length > 0, "No Guest user registered in this tenant, so Users List should not be Empty");

            Assert.IsTrue(Array.Exists(userNames, name => name == uniqueFullUserName),
                $"Newly created user with name '{uniqueFullUserName}' was not found in the fetched user list.");

        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void ListUsers_InvalidAuthentication()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            String cpToken = "";
            if (CP_URL == string.Empty)
            {
                CP_URL = cloudprintObj.GetCloudPrintURI(ENV, CP_AUTH_TOKEN, TENANT);
                Debug.Assert(CP_URL.Contains(TENANT_ID),
                    $"Cloud print api url is failed to extract for environment : {ENV}");
            }

            try
            {
                CP_URL = CP_URL.Split(TENANT_ID)[0];
                var users = usersObj.GetGuestUsersList(CP_URL, cpToken, TENANT_ID, @"?page=0&pageSize=10");

            }
            catch (Exception e)
            {
                if (e.Message.Contains("The remote server returned an error: (401)"))
                {
                    Debug.Assert(true, "Unexpected Error Message, Expecting 401, actual Message: " + e.Message);
                }
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void ListUsers_First10Query()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            if (CP_URL == string.Empty)
            {
                CP_URL = cloudprintObj.GetCloudPrintURI(ENV, CP_AUTH_TOKEN, TENANT);
                Debug.Assert(CP_URL.Contains(TENANT_ID),
                    $"Cloud print api url is failed to extract for environment : {ENV}");
            }

            try
            {
                CP_URL = CP_URL.Split(TENANT_ID)[0];
                var users = usersObj.GetGuestUsersList(CP_URL, CP_AUTH_TOKEN, TENANT_ID, @"?page=0&pageSize=10");
                Debug.Assert(users.Length <= 10);
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void ListUsers_First1Query()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            if (CP_URL == string.Empty)
            {
                CP_URL = cloudprintObj.GetCloudPrintURI(ENV, CP_AUTH_TOKEN, TENANT);
                Debug.Assert(CP_URL.Contains(TENANT_ID),
                    $"Cloud print api url is failed to extract for environment : {ENV}");
            }

            try
            {
                CP_URL = CP_URL.Split(TENANT_ID)[0];
                var users = usersObj.GetGuestUsersList(CP_URL, CP_AUTH_TOKEN, TENANT_ID, @"?page=0&pageSize=2");
                Debug.Assert(users.Length <= 2);
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void ListUsers_InvalidQuery()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            if (CP_URL == string.Empty)
            {
                CP_URL = cloudprintObj.GetCloudPrintURI(ENV, CP_AUTH_TOKEN, TENANT);
                Debug.Assert(CP_URL.Contains(TENANT_ID),
                    $"Cloud print api url is failed to extract for environment : {ENV}");
            }

            try
            {
                CP_URL = CP_URL.Split(TENANT_ID)[0];
                var users = usersObj.GetGuestUsersList(CP_URL, CP_AUTH_TOKEN, TENANT_ID, @"?page=0&pageSize=A");
                Debug.Assert(false, "Expecting 400");
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("The remote server returned an error: (400)"))
                {
                    Debug.Assert(false, "Unexpected Error Message, Expecting 400, actual Message: " + e.Message);
                }
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release")]
        public void ListUsers_InvalidTenantId()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            if (CP_URL == string.Empty)
            {
                CP_URL = cloudprintObj.GetCloudPrintURI(ENV, CP_AUTH_TOKEN, TENANT);
                Debug.Assert(CP_URL.Contains(TENANT_ID),
                    $"Cloud print api url is failed to extract for environment : {ENV}");
            }

            try
            {
                CP_URL = CP_URL.Split(TENANT_ID)[0];
                usersObj.GetGuestUsersList(CP_URL, CP_AUTH_TOKEN, TENANT_ID + "INVALID");
                throw new Exception("Expected Error 400");
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("The remote server returned an error: (400)"))
                {
                    Debug.Assert(false, "Unexpected Error Message, Expecting 400, actual Message: " + e.Message);
                }
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsers()
        {
            Console.WriteLine($"Test Execution: {testContextInstance.TestName}");

            var random = new Random();
            var uniqueUserId = random.Next(999999);
            var uniqueFullUserName = "GUser" + uniqueUserId;
            var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";

            // Define default values for your test scenario
            string defaultRole = "GUEST_USER";
            string defaultPin = "1980";
            string defaultPassword = "EeGahqu7";
            bool sendWelcomeEmail = false;
            bool sendExpirationEmail = false;
            string welcomeEmailContent = null;
            
            // Calculate default expiration date (current date + 30 days)
            DateTime expiryDate = DateTime.Now.AddDays(30);
            string formattedExpiryDate = expiryDate.ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture);

            // 1. Check if user already exists
            bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, uniqueFullUserName);

            if (!isFound)
            {
               // 2. Prepare the request DTO with ALL necessary parameters
               var createRequest = new CreateGuestUserRequest
               {
                   Email = uniqueUserEmail,
                   FullName = uniqueFullUserName,
                   Role = defaultRole,
                   Pin = defaultPin,
                   Password = defaultPassword,
                   ExpirationTimestamp = formattedExpiryDate,
                   SendWelcomeEmail = sendWelcomeEmail,
                   SendExpirationEmail = sendExpirationEmail,
                   WelcomeEmailContent = welcomeEmailContent
               };

               // 3. Call the API to create the guest user
               GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

               // 4. Assertions based on the response DTO
               Assert.IsTrue(response.IsSuccessStatusCode, $"API call was not successful. Status: " +
                   $"{response.StatusCode}, Message: {response.Message}. Response Body: {response.ResponseBody}");
               Assert.IsTrue(response.Success, $"API reported failure: {response.Message}. Response Body: {response.ResponseBody}");

               Assert.IsNotNull(response.Users, "Response Users list is null.");
               Assert.AreEqual(1, response.Users.Count, "Expected 1 user in the response.");

               var createdUser = response.Users[0];

               Assert.AreEqual(uniqueFullUserName, createdUser.FullName, "Created user's full name does not match.");
               Assert.IsTrue(uniqueUserEmail.Equals(createdUser.Email, StringComparison.OrdinalIgnoreCase), 
                   "Created user's email does not match.");
                    Assert.AreEqual("OK", response.Message, "Overall API response message is not 'OK'.");

                    Console.WriteLine($"Successfully created guest user: {uniqueFullUserName}");
            }
            
            Console.WriteLine($"Successfully executed: {testContextInstance.TestName}");
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsersWithoutPassword()
        {
            Console.WriteLine($"Test Execution: {testContextInstance.TestName}");

            var random = new Random();
            var uniqueUserId = random.Next(999999);
            var uniqueFullUserName = "GUser" + uniqueUserId;
            var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";

            // Define default values for your test scenario
            string defaultRole = "GUEST_USER";
            string defaultPin = "1980";
            bool sendWelcomeEmail = false;
            bool sendExpirationEmail = false;
            string welcomeEmailContent = null;
            
            // Calculate default expiration date (current date + 30 days)
            DateTime expiryDate = DateTime.Now.AddDays(30);
            string formattedExpiryDate = expiryDate.ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture);
            
            // 1. Check if user already exists
            bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, uniqueFullUserName);

            if (!isFound)
            {
               // 2. Prepare the request DTO with ALL necessary parameters
               var createRequest = new CreateGuestUserRequest
               {
                  Email = uniqueUserEmail,
                  FullName = uniqueFullUserName,
                  Role = defaultRole,
                  Pin = defaultPin,
                  ExpirationTimestamp = formattedExpiryDate,
                  SendWelcomeEmail = sendWelcomeEmail,
                  SendExpirationEmail = sendExpirationEmail,
                  WelcomeEmailContent = welcomeEmailContent
               };

               // 3. Call the API to create the guest user
               GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);
                
               // 4. Assertions based on the response DTO
               Assert.IsTrue(response.IsSuccessStatusCode, $"API call was not successful. Status: " +
                   $"{response.StatusCode}, Message: {response.Message}. Response Body: {response.ResponseBody}");
               Assert.IsTrue(response.Success, $"API reported failure: {response.Message}. Response Body: {response.ResponseBody}");

               Assert.IsNotNull(response.Users, "Response Users list is null.");
               Assert.AreEqual(1, response.Users.Count, "Expected 1 user in the response.");

               var createdUser = response.Users[0];

               Assert.AreEqual(uniqueFullUserName, createdUser.FullName, "Created user's full name does not match.");
               Assert.IsTrue(uniqueUserEmail.Equals(createdUser.Email, StringComparison.OrdinalIgnoreCase), "Created user's email does not match.");
               Assert.AreEqual("OK", response.Message, "Overall API response message is not 'OK'.");
                
            }
            Console.WriteLine($"Successfully created guest user: {uniqueFullUserName}");

        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsersWithoutPin()
        {
            Console.WriteLine($"Test Execution: {testContextInstance.TestName}");

            var random = new Random();
            var uniqueUserId = random.Next(999999);
            var uniqueFullUserName = "GUser" + uniqueUserId;
            var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";

            // Define default values for your test scenario
            string defaultRole = "GUEST_USER";
            bool sendWelcomeEmail = false;
            bool sendExpirationEmail = false;
            string welcomeEmailContent = null;

            // Calculate default expiration date (current date + 30 days)
            DateTime expiryDate = DateTime.Now.AddDays(30);
            string formattedExpiryDate = expiryDate.ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture);

            // 1. Check if user already exists
            bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, uniqueFullUserName);

            if (!isFound)
            {
              // 2. Prepare the request DTO with ALL necessary parameters
              var createRequest = new CreateGuestUserRequest
              {
                 Email = uniqueUserEmail,
                 FullName = uniqueFullUserName,
                 Role = defaultRole,
                 ExpirationTimestamp = formattedExpiryDate,
                 SendWelcomeEmail = sendWelcomeEmail,
                 SendExpirationEmail = sendExpirationEmail,
                 WelcomeEmailContent = welcomeEmailContent
              };

              // 3. Call the API to create the guest user
              GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

              // 4. Assertions based on the response DTO
              Assert.IsTrue(response.IsSuccessStatusCode, $"API call was not successful. Status: {response.StatusCode}, Message: {response.Message}. Response Body: {response.ResponseBody}");
              Assert.IsTrue(response.Success, $"API reported failure: {response.Message}. Response Body: {response.ResponseBody}");

              Assert.IsNotNull(response.Users, "Response Users list is null.");
              Assert.AreEqual(1, response.Users.Count, "Expected 1 user in the response.");

              var createdUser = response.Users[0];

              Assert.AreEqual(uniqueFullUserName, createdUser.FullName, "Created user's full name does not match.");
              Assert.IsTrue(uniqueUserEmail.Equals(createdUser.Email, StringComparison.OrdinalIgnoreCase), "Created user's email does not match.");
              Assert.AreEqual("OK", response.Message, "Overall API response message is not 'OK'.");
            }
            Console.WriteLine($"Successfully created guest user: {uniqueFullUserName}");
        }

        [DataTestMethod]
        [DataRow(null)]
        [TestCategory("Regression"), TestCategory("Negative"), TestCategory("GuestUser")]
        public void CreateGuestUser_WithoutFullName_Fails(string invalidFullName)
        {
            Console.WriteLine($"Test Execution: {testContextInstance.TestName} with FullName: '{invalidFullName ?? "null"}'");

             var random = new Random();
             var uniqueUserId = random.Next(999999);
             // Even though we expect it to fail, we still need a unique email
             var uniqueUserEmail = $"invaliduser{uniqueUserId}@SQA.com";

             // Prepare the request DTO with the invalid full name
             var createRequest = new CreateGuestUserRequest
             {

                 Email = uniqueUserEmail,
                 FullName = invalidFullName,
                 Role = "GUEST_USER",
                 Pin = "1980",
                 Password = "EeGahqu7",
                 ExpirationTimestamp = DateTime.Now.AddDays(30).ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture),
                 SendWelcomeEmail = false,
                 SendExpirationEmail = false,
                 WelcomeEmailContent = null
             };

             // Call the API to create the guest user
             GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

            // Assertions for a failed creation
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode,
                $"Expected a 'Bad Request' status code, but got '{response.StatusCode}'. Response Body: {response.ResponseBody}");

            Assert.IsFalse(response.Success,
                $"API unexpectedly reported success. Response Body: {response.ResponseBody}");
            Assert.IsNull(response.Users, "Users list should be null for a failed creation.");

            Assert.IsTrue(response.ResponseBody?.Contains("\"errorCode\":\"VALIDATION_FAILED\"") == true &&
                                   response.ResponseBody?.Contains("The request was not valid.") == true,
                                  $"Response body did not indicate 'VALIDATION_FAILED' error code and message. Actual Body: " +
                                  $"{response.ResponseBody}");

            Console.WriteLine($"Successfully confirmed failure for FullName: '{invalidFullName ?? "null"}'");
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsers_InvalidTenantID()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            try
            {
                var random = new Random();
                var uniqueUserId = random.Next(999999);
                var uniqueFullUserName = "GUser" + uniqueUserId;
                var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";

                bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, uniqueFullUserName);

                if (!isFound)
                {
                    var createRequest = new CreateGuestUserRequest
                    {
                        Email = uniqueUserEmail,
                        FullName = uniqueFullUserName,
                        Role = "GUEST_USER",
                        Pin = "1980",
                        Password = "EeGahqu7",
                        ExpirationTimestamp = DateTime.Now.AddDays(30).ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture)
                    };

                    // Intentionally use an invalid TENANT_ID in the API call
                    GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID + "INVALID", CP_AUTH_TOKEN, createRequest);

                    Assert.IsFalse(response.IsSuccessStatusCode, "API call should not be successful for invalid tenant ID.");
                    Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, $"Expected status code BadRequest, but got {response.StatusCode}");
                    Assert.IsFalse(response.Success, "API should report failure for invalid tenant ID.");
                    Assert.IsTrue(response.Message?.Contains("Failed to convert 'tenant'") == true ||
                          response.ResponseBody?.Contains("Failed to convert 'tenant'") == true,
                          $"Response message/body did not indicate expected tenant conversion failure: {response.Message}, Body: {response.ResponseBody}");

                }
                Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{uniqueFullUserName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsers_InvalidEmail()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            try
            {
                var random = new Random();
                var uniqueUserId = random.Next(999999);
                var uniqueFullUserName = "GUser" + uniqueUserId;
                var uniqueUserEmail = uniqueFullUserName; // Intentionally invalid email format
                string GUser = uniqueFullUserName;

                bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, GUser);

                if (!isFound)
                {
                    var createRequest = new CreateGuestUserRequest
                    {
                        Email = uniqueUserEmail, // Invalid email here
                        FullName = uniqueFullUserName,
                        Role = "GUEST_USER",
                        Pin = "1980",
                        Password = "EeGahqu7",
                        ExpirationTimestamp = DateTime.Now.AddDays(30).ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture)
                    };

                    GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

                    Assert.IsFalse(response.IsSuccessStatusCode, "API call should not be successful for invalid email.");
                    Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, $"Expected status code BadRequest, but got {response.StatusCode}");
                    Assert.IsFalse(response.Success, "API should report failure for invalid email.");
                    Assert.IsTrue(response.ResponseBody?.Contains("\"errorCode\":\"VALIDATION_FAILED\"") == true &&
                                   response.ResponseBody?.Contains("The request was not valid.") == true,
                                  $"Response body did not indicate 'VALIDATION_FAILED' error code and message. Actual Body: " +
                                  $"{response.ResponseBody}");
                }
                Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{GUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsers_InvalidAuthentication()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            try
            {
                var random = new Random();
                var uniqueUserId = random.Next(999999);
                var uniqueFullUserName = "GUser" + uniqueUserId;
                var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";
                string GUser = uniqueFullUserName;

                bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, GUser);

                if (!isFound)
                {
                    var createRequest = new CreateGuestUserRequest
                    {
                        Email = uniqueUserEmail,
                        FullName = uniqueFullUserName,
                        Role = "GUEST_USER",
                        Pin = "1980",
                        Password = "EeGahqu7",
                        ExpirationTimestamp = DateTime.Now.AddDays(30).ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture)
                    };

                    // Intentionally use an invalid AUTH_TOKEN
                    GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN + "INVALID", createRequest);

                    Assert.IsFalse(response.IsSuccessStatusCode, "API call should not be successful for invalid authentication.");
                    Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, $"Expected status code Unauthorized, but got {response.StatusCode}");
                    Assert.IsFalse(response.Success, "API should report failure for invalid authentication.");

                }
                Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{GUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsers_InvalidRole()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            try
            {
                var random = new Random();
                var uniqueUserId = random.Next(999999);
                var uniqueFullUserName = "GUser" + uniqueUserId;
                var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";
                string GUser = uniqueFullUserName;

                bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, GUser);

                if (!isFound)
                {
                    var createRequest = new CreateGuestUserRequest
                    {
                        Email = uniqueUserEmail,
                        FullName = uniqueFullUserName,
                        Role = "GUEST_USER123", // Intentionally invalid role
                        Pin = "1980",
                        Password = "EeGahqu7",
                        ExpirationTimestamp = DateTime.Now.AddDays(30).ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture)
                    };

                    GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

                    Assert.IsFalse(response.IsSuccessStatusCode, "API call should not be successful for invalid role.");
                    Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, $"Expected status code Forbidden, but got {response.StatusCode}");
                    Assert.IsFalse(response.Success, "API should report failure for invalid role.");
                    Assert.IsTrue(response.Message?.Contains("Requested role is not allowed") == true || response.ResponseBody?.Contains("Requested role is not allowed") == true, $"Response message/body did not indicate 'Requested role is not allowed': {response.Message}, Body: {response.ResponseBody}");
                }
                Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{GUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsers_InvalidPIN()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            try
            {
                var random = new Random();
                var uniqueUserId = random.Next(999999);
                var uniqueFullUserName = "GUser" + uniqueUserId;
                var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";
                string GUser = uniqueFullUserName;

                bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, GUser);

                if (!isFound)
                {
                    var createRequest = new CreateGuestUserRequest
                    {
                        Email = uniqueUserEmail,
                        FullName = uniqueFullUserName,
                        Role = "GUEST_USER",
                        Pin = "12345678", // Intentionally invalid PIN (e.g., too long)
                        Password = "EeGahqu7",
                        ExpirationTimestamp = DateTime.Now.AddDays(30).ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture)
                    };

                    GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

                    Assert.IsFalse(response.IsSuccessStatusCode, "API call should not be successful for invalid PIN.");
                    Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, $"Expected status code BadRequest, but got {response.StatusCode}");
                    Assert.IsFalse(response.Success, "API should report failure for invalid PIN.");
                    Assert.IsTrue(response.ResponseBody?.Contains("\"errorCode\":\"VALIDATION_FAILED\"") == true &&
                                   response.ResponseBody?.Contains("The request was not valid.") == true,
                                  $"Response body did not indicate 'VALIDATION_FAILED' error code and message. Actual Body: " +
                                  $"{response.ResponseBody}");
                }
                Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{GUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsers_InvalidPassword()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            try
            {
                var random = new Random();
                var uniqueUserId = random.Next(999999);
                var uniqueFullUserName = "GUser" + uniqueUserId;
                var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";
                string GUser = uniqueFullUserName;

                bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, GUser);

                if (!isFound)
                {
                    var createRequest = new CreateGuestUserRequest
                    {
                        Email = uniqueUserEmail,
                        FullName = uniqueFullUserName,
                        Role = "GUEST_USER",
                        Pin = "1980",
                        Password = "1678", // Intentionally invalid password
                        ExpirationTimestamp = DateTime.Now.AddDays(30).ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture)
                    };

                    GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

                    Assert.IsFalse(response.IsSuccessStatusCode, "API call should not be successful for invalid password.");
                    Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, $"Expected status code BadRequest, but got {response.StatusCode}");
                    Assert.IsFalse(response.Success, "API should report failure for invalid password.");
                    Assert.IsTrue(response.ResponseBody?.Contains("\"errorCode\":\"VALIDATION_FAILED\"") == true &&
                                   response.ResponseBody?.Contains("The request was not valid.") == true,
                                  $"Response body did not indicate 'VALIDATION_FAILED' error code and message. Actual Body: " +
                                  $"{response.ResponseBody}");
                }
                Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{GUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsers_InvalidExpirationTimestamp()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            try
            {
                var random = new Random();
                var uniqueUserId = random.Next(999999);
                var uniqueFullUserName = "GUser" + uniqueUserId;
                var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";
                string GUser = uniqueFullUserName;

                bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, GUser);

                if (!isFound)
                {
                    var createRequest = new CreateGuestUserRequest
                    {
                        Email = uniqueUserEmail,
                        FullName = uniqueFullUserName,
                        Role = "GUEST_USER",
                        Pin = "1980",
                        Password = "EeGahqu7",
                        ExpirationTimestamp = DateTime.Now.AddDays(14).ToString() // Using default ToString() which might be invalid
                    };

                    GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

                    Assert.IsFalse(response.IsSuccessStatusCode, "API call should not be successful for invalid expiration timestamp.");
                    Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, $"Expected status code BadRequest, but got {response.StatusCode}");
                    Assert.IsFalse(response.Success, "API should report failure for invalid expiration timestamp.");
                    Assert.IsTrue(response.Message?.Contains("Could not process the request body") == true ||
                       response.ResponseBody?.Contains("Could not process the request body") == true,
                       $"Response message/body did not indicate request body processing failure. Actual message: '{response.Message}', Body: {response.ResponseBody}");

                }
                Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{GUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsers_SendWelcomeEmail()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            try
            {
                var random = new Random();
                var uniqueUserId = random.Next(999999);
                var uniqueFullUserName = "GUser" + uniqueUserId;
                var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";
                string GUser = uniqueFullUserName;

                bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, GUser);

                if (!isFound)
                {
                    var createRequest = new CreateGuestUserRequest
                    {
                        Email = uniqueUserEmail,
                        FullName = uniqueFullUserName,
                        Role = "GUEST_USER",
                        Pin = "1980",
                        Password = "EeGahqu7",
                        ExpirationTimestamp = DateTime.Now.AddDays(30).ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture),
                        SendWelcomeEmail = true // Test specific parameter
                    };

                    GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

                    Assert.IsTrue(response.IsSuccessStatusCode, $"API call was not successful. IsSuccessStatusCode: {response.IsSuccessStatusCode}, Status: {response.StatusCode}, Message: {response.Message}. Response Body: {response.ResponseBody}");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Expected status code OK, but got {response.StatusCode}");
                    Assert.IsTrue(response.Success, $"API reported failure: Success={response.Success}, Message={response.Message}. Response Body: {response.ResponseBody}");
                    Assert.AreEqual("OK", response.Message, "Overall API response message is not 'OK'.");

                    Assert.IsNotNull(response.Users, "Response Users list is null.");
                    Assert.AreEqual(1, response.Users.Count, "Expected 1 user in the response.");
                    var createdUser = response.Users[0];

                    Assert.AreEqual(uniqueFullUserName, createdUser.FullName, "Created user's full name does not match.");
                    Assert.IsTrue(uniqueUserEmail.Equals(createdUser.Email, StringComparison.OrdinalIgnoreCase), "Created user's email does not match.");
                    // Potentially assert on email sending status if your response DTO provides it
                }
                Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{GUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsers_SendExpirationEmail()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            try
            {
                var random = new Random();
                var uniqueUserId = random.Next(999999);
                var uniqueFullUserName = "GUser" + uniqueUserId;
                var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";
                string GUser = uniqueFullUserName;

                bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, GUser);

                if (!isFound)
                {
                    var createRequest = new CreateGuestUserRequest
                    {
                        Email = uniqueUserEmail,
                        FullName = uniqueFullUserName,
                        Role = "GUEST_USER",
                        Pin = "1980",
                        Password = "EeGahqu7",
                        ExpirationTimestamp = DateTime.Now.AddDays(30).ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture),
                        SendWelcomeEmail = true, // Common to send welcome if sending expiration
                        SendExpirationEmail = true // Test specific parameter
                    };

                    GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

                    Assert.IsTrue(response.IsSuccessStatusCode, $"API call was not successful. IsSuccessStatusCode: {response.IsSuccessStatusCode}, Status: {response.StatusCode}, Message: {response.Message}. Response Body: {response.ResponseBody}");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Expected status code OK, but got {response.StatusCode}");
                    Assert.IsTrue(response.Success, $"API reported failure: Success={response.Success}, Message={response.Message}. Response Body: {response.ResponseBody}");
                    Assert.AreEqual("OK", response.Message, "Overall API response message is not 'OK'.");

                    Assert.IsNotNull(response.Users, "Response Users list is null.");
                    Assert.AreEqual(1, response.Users.Count, "Expected 1 user in the response.");
                    var createdUser = response.Users[0];

                    Assert.AreEqual(uniqueFullUserName, createdUser.FullName, "Created user's full name does not match.");
                    Assert.IsTrue(uniqueUserEmail.Equals(createdUser.Email, StringComparison.OrdinalIgnoreCase), "Created user's email does not match.");
                    // Potentially assert on email sending status if your response DTO provides it
                }
                Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{GUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void CreateGuestUsers_SendEmailValidContent()
        {
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            try
            {
                var random = new Random();
                var uniqueUserId = random.Next(999999);
                var uniqueFullUserName = "GUser" + uniqueUserId;
                var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";
                string GUser = uniqueFullUserName;

                bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, GUser);

                if (!isFound)
                {
                    var createRequest = new CreateGuestUserRequest
                    {
                        Email = uniqueUserEmail,
                        FullName = uniqueFullUserName,
                        Role = "GUEST_USER",
                        Pin = "1980",
                        Password = "EeGahqu7",
                        ExpirationTimestamp = DateTime.Now.AddDays(30).ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture),
                        SendWelcomeEmail = true,
                        SendExpirationEmail = true,
                        WelcomeEmailContent = "Welcome to RPintix !!!!" // Test specific parameter
                    };

                    GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

                    Assert.IsTrue(response.IsSuccessStatusCode, $"API call was not successful. IsSuccessStatusCode: {response.IsSuccessStatusCode}, Status: {response.StatusCode}, Message: {response.Message}. Response Body: {response.ResponseBody}");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Expected status code OK, but got {response.StatusCode}");
                    Assert.IsTrue(response.Success, $"API reported failure: Success={response.Success}, Message={response.Message}. Response Body: {response.ResponseBody}");
                    Assert.AreEqual("OK", response.Message, "Overall API response message is not 'OK'.");

                    Assert.IsNotNull(response.Users, "Response Users list is null.");
                    Assert.AreEqual(1, response.Users.Count, "Expected 1 user in the response.");
                    var createdUser = response.Users[0];

                    Assert.AreEqual(uniqueFullUserName, createdUser.FullName, "Created user's full name does not match.");
                    Assert.IsTrue(uniqueUserEmail.Equals(createdUser.Email, StringComparison.OrdinalIgnoreCase), "Created user's email does not match.");
                    // Potentially assert on the content of the sent email if you have a way to verify it (e.g., email service mock)
                }
                Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{GUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }
        }
        //Delete Guest user Tests

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void DeleteGuestUsersInvalidTenant()
        {
           Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
           List<string> userDeleted;
           string GUserID = "e6ac8543-bcf8-4b9d-92ce-2578b0babcde";

           userDeleted = cloudprintObj.DeleteUser(CPAPI_URL, TENANT_ID + "INVALID", CP_AUTH_TOKEN, GUserID);
                                
           Assert.AreEqual("False", userDeleted[0], "The second element should be the success flag 'False'.");
           Assert.AreEqual("BadRequest", userDeleted[1], "The first element should be 'BadRequest'.");
           Assert.AreEqual("", userDeleted[2], "The third element should be the error code 'BadRequest'.");
                
           Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{GUserID}");
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void DeleteGuestUsers()
        {
            try
            {
                // get list of user ids based on "GUser"
                Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
                List<string> usersID;
                List<string> userDeleted;
                usersID = cloudprintObj.ListGuestUsers(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, "GUser");
                if (usersID.Count > 0)
                {
                    Console.WriteLine(usersID);
                }
                // call delete user for all those GUsers.
                foreach (string userID in usersID)
                {
                    userDeleted = cloudprintObj.DeleteUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, userID);
                    Assert.IsTrue(userDeleted[0].Equals("True", StringComparison.OrdinalIgnoreCase), $"Delete user should fail:{userID}");
                    Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{userID}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }

        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
        public void DeleteGuestUsersInvalidUserID()
        {
            //call delete user with invalid USER ID, test status should  be false.
            Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
            List<string> userDeleted;
            string GUserID = "e6ac8543-bcf8-4b9d-92ce-2578b0babcde";
            userDeleted = cloudprintObj.DeleteUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, GUserID);
            Assert.AreEqual("False", userDeleted[0], "The 'success' status should be False for an invalid user ID.");
            Assert.AreEqual("RESOURCE_NOT FOUND", userDeleted[1], "The API should return a 'RESOURCE_NOT_FOUND' error code.");
            string expectedErrorMessage = "The user with ID {0} could not be found";
            Assert.AreEqual(expectedErrorMessage, userDeleted[2], "The error message should match the expected content.");
            Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{GUserID}");
        }

        //change owner tests
        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("ChangeOwner")]
        public void Change_Owner_ByContent()
        {
            Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            var job_url_list = cloudprintObj.OwnersSetupHelper(ENV, CP_URL, TENANT_ID, CP_AUTH_TOKEN, TENANT);
            var oldJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);

            try
            {
                // Change Owner - Now expecting ChangeOwnerResponse back
                ChangeOwnerResponse changeOwnerResponse = cloudprintObj.ChangeOwner(job_url_list[3], CP_AUTH_TOKEN, ADMIN_USER, false);

                // Assert that the change was successful based on the response
                Debug.Assert(changeOwnerResponse.Success, $"Change owner API call was not successful: {changeOwnerResponse.Message}");
                Debug.Assert(changeOwnerResponse.Jobs != null && changeOwnerResponse.Jobs.Count > 0, "No jobs returned in ChangeOwnerResponse.");

                // Get the new job owner ID from the response
                string newJobOwnerId = changeOwnerResponse.Jobs[0].OwnerId;

                Console.WriteLine($"old 'ownerId' was : {oldJobOwnerId}");
                Console.WriteLine($"new 'ownerId' is : {newJobOwnerId}");

                // Assert that the owner ID has indeed changed
                Debug.Assert(oldJobOwnerId != newJobOwnerId, $"OwnerId was NOT changed. Old: {oldJobOwnerId}, New: {newJobOwnerId}");
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
            }
            finally
            {
                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);
                Debug.Assert(job_status == "OK", $"Failed to Delete print job in Cloud api service:{job_url_list[3]}");
                Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            }
        }

        /// <summary>
        /// ChangeOwner by Content with an Invalid Authentication code
        /// Expecting 401
        /// </summary>
        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("ChangeOwner")]
        public void Change_Owner_ByContentInvalidAuthentication()
        {
            Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            var job_url_list = cloudprintObj.OwnersSetupHelper(ENV, CP_URL, TENANT_ID, CP_AUTH_TOKEN, TENANT);
            var oldJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);

            ChangeOwnerResponse changeOwnerResponse = null; // Declare outside try-catch to access in finally

            try
            {
                // Change Owner with invalid token. The helper method will throw a WebException.
                changeOwnerResponse = cloudprintObj.ChangeOwner(job_url_list[3], CP_AUTH_TOKEN + "INVALID", ADMIN_USER, false);

                // If the line above doesn't throw, it's an unexpected success, so fail the test.
                Debug.Assert(false, "Expected a WebException (e.g., 401 Unauthorized), but the API call succeeded.");
            }
            catch (WebException webEx) // Catch WebException specifically
            {
                Console.WriteLine($"Caught expected WebException: {webEx.Message}");
                if (webEx.Response is HttpWebResponse httpResponse)
                {
                    // Assert on the HTTP status code directly
                    Debug.Assert(httpResponse.StatusCode == HttpStatusCode.Unauthorized,
                                 $"Expected HTTP 401 Unauthorized, but got: {httpResponse.StatusCode}");

                    // You can optionally try to deserialize the error response body here
                    // if the API returns specific error JSON for 401 and you want to assert its content.
                    // However, for just checking the status code, the above is sufficient.
                }
                else
                {
                    Debug.Assert(false, $"Expected HttpWebResponse with 401 status, but got a different WebException type: {webEx.GetType().Name}");
                }
            }
            catch (Exception e) // Catch any other unexpected exceptions
            {
                Debug.Assert(false, $"Caught unexpected exception type: {e.GetType().Name}. Message: {e.Message}");
            }
            finally
            {
                // Check to make sure ownerId was NOT changed. This part remains similar.
                // The oldJobOwnerId should be compared with the ownerId fetched *after* the failed attempt.
                var newJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);
                Console.WriteLine($"old 'ownerId' was : {oldJobOwnerId}");
                Console.WriteLine($"new 'ownerId' is : {newJobOwnerId}");
                Debug.Assert(oldJobOwnerId == newJobOwnerId, $"OwnerId WAS changed, but it shouldn't have been. Old: {oldJobOwnerId}, New: {newJobOwnerId}");

                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);
                Debug.Assert(job_status == "OK", $"Failed to Delete print job in Cloud api service:{job_url_list[3]}");
                Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            }
        }

        /// <summary>
        /// ChangeOwner by Content with an Invalid Owner
        /// Expecting 404
        /// </summary>
        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("ChangeOwner")]
        public void Change_Owner_ByContentInvalidOwner()
        {
            Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            var job_url_list = cloudprintObj.OwnersSetupHelper(ENV, CP_URL, TENANT_ID, CP_AUTH_TOKEN, TENANT);
            var oldJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);

            ChangeOwnerResponse changeOwnerResponse = null; // Declare outside try-catch for potential inspection

            try
            {
                // Change Owner with an invalid owner email. The helper method should throw a WebException.
                changeOwnerResponse = cloudprintObj.ChangeOwner(job_url_list[3], CP_AUTH_TOKEN, "INVALID" + ADMIN_USER, false);

                // If the line above doesn't throw, it's an unexpected success or different error, so fail.
                Debug.Assert(false, "Expected a WebException (e.g., 404 Not Found), but the API call succeeded or returned a different error.");
            }
            catch (WebException webEx) // Catch WebException specifically for HTTP errors
            {
                Console.WriteLine($"Caught expected WebException: {webEx.Message}");
                if (webEx.Response is HttpWebResponse httpResponse)
                {
                    // Assert on the HTTP status code directly for a 404 Not Found
                    Debug.Assert(httpResponse.StatusCode == HttpStatusCode.NotFound,
                                 $"Expected HTTP 404 Not Found, but got: {httpResponse.StatusCode}");

                    // Optionally, if the API returns a specific error message in the body for 404,
                    // you could read and assert on that too.
                    // Example:
                    // using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
                    // {
                    //     string errorBody = reader.ReadToEnd();
                    //     // Assert based on errorBody content if needed
                    // }
                }
                else
                {
                    Debug.Assert(false, $"Expected HttpWebResponse with 404 status, but got a different WebException type: {webEx.GetType().Name}");
                }
            }
            catch (Exception e) // Catch any other unexpected exceptions
            {
                Debug.Assert(false, $"Caught unexpected exception type: {e.GetType().Name}. Message: {e.Message}");
            }
            finally
            {
                // Check to make sure ownerId was NOT changed.
                var newJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);
                Console.WriteLine($"old 'ownerId' was : {oldJobOwnerId}");
                Console.WriteLine($"new 'ownerId' is : {newJobOwnerId}");
                Debug.Assert(oldJobOwnerId == newJobOwnerId, $"OwnerId WAS changed, but it shouldn't have been. Old: {oldJobOwnerId}, New: {newJobOwnerId}");

                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);
                Debug.Assert(job_status == "OK", $"Failed to Delete print job in Cloud api service:{job_url_list[3]}");
                Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            }
        }

        /// <summary>
        /// ChangeOwner an Invalid Tenant Id
        /// Expecting 400
        /// </summary>
        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("ChangeOwner")]
        public void Change_Owner_ByContentInvalidTenantId()
        {
            Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            var job_url_list = cloudprintObj.OwnersSetupHelper(ENV, CP_URL, TENANT_ID, CP_AUTH_TOKEN, TENANT);
            var oldJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);

            // Set the Tenant invalid while everything else in the URL is valid
            // Assuming the original URL contains a GUID-like tenant ID and '2880' is genuinely invalid format or value.
            var invalidTenantUrl = job_url_list[3].Replace(TENANT_ID, "2880" + Guid.NewGuid().ToString());

            ChangeOwnerResponse changeOwnerResponse = null;

            try
            {
                // Change Owner with the invalid tenant ID in the URL.
                changeOwnerResponse = cloudprintObj.ChangeOwner(invalidTenantUrl, CP_AUTH_TOKEN, ADMIN_USER, false);

                // If the line above doesn't throw, it's an unexpected success or different error.
                Debug.Assert(false, "Expected a WebException (e.g., 400 Bad Request), but the API call succeeded or returned a different error.");
            }
            catch (WebException webEx) // Catch WebException specifically for HTTP errors
            {
                Console.WriteLine($"Caught expected WebException: {webEx.Message}");
                if (webEx.Response is HttpWebResponse httpResponse)
                {
                    // Assert on the HTTP status code directly for a 400 Bad Request
                    Debug.Assert(httpResponse.StatusCode == HttpStatusCode.BadRequest,
                                 $"Expected HTTP 400 Bad Request, but got: {httpResponse.StatusCode}");

                }
                else
                {
                    Debug.Assert(false, $"Expected HttpWebResponse with 400 status, but got a different WebException type: {webEx.GetType().Name}");
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false, $"Caught unexpected exception type: {e.GetType().Name}. Message: {e.Message}");
            }
            finally
            {
                // Check to make sure ownerId was NOT changed. This assertion is critical.
                var newJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN); // Use the original job_url_list[3] for comparison
                Console.WriteLine($"old 'ownerId' was : {oldJobOwnerId}");
                Console.WriteLine($"new 'ownerId' is : {newJobOwnerId}");
                Debug.Assert(oldJobOwnerId == newJobOwnerId, $"OwnerId WAS changed, but it shouldn't have been. Old: {oldJobOwnerId}, New: {newJobOwnerId}");

                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);
                Debug.Assert(job_status == "OK", $"Failed to Delete print job in Cloud api service:{job_url_list[3]}");
                Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            }
        }

        /// <summary>
        /// ChangeOwner an Invalid Tenant URL
        /// Expecting 404 Not Found (or potentially 400 Bad Request depending on API routing)
        /// </summary>
        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("ChangeOwner")]
        public void Change_Owner_ByContentInvalidURL()
        {
            Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            var job_url_list = cloudprintObj.OwnersSetupHelper(ENV, CP_URL, TENANT_ID, CP_AUTH_TOKEN, TENANT);
            var oldJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);

            // Create an invalid URL by appending extra characters to a valid job URL
            var invalidJobUrl = job_url_list[3] + "INVALID_PATH_SEGMENT";

            ChangeOwnerResponse changeOwnerResponse = null;

            try
            {
                // Attempt to change owner with the invalid URL.
                changeOwnerResponse = cloudprintObj.ChangeOwner(invalidJobUrl, CP_AUTH_TOKEN, ADMIN_USER, false);

                Debug.Assert(false, "Expected a WebException (e.g., 404 Not Found or 400 Bad Request), but the API call succeeded or returned a different error.");
            }
            catch (WebException webEx) 
            {
                Console.WriteLine($"Caught expected WebException: {webEx.Message}");
                if (webEx.Response is HttpWebResponse httpResponse)
                {
                    // Assert that the status code is either 404 Not Found or 400 Bad Request.
                    Debug.Assert(httpResponse.StatusCode == HttpStatusCode.NotFound ||
                                 httpResponse.StatusCode == HttpStatusCode.BadRequest,
                                 $"Expected HTTP 404 Not Found or 400 Bad Request, but got: {httpResponse.StatusCode}");

                }
                else
                {
                           Debug.Assert(false, $"Expected HttpWebResponse with 4xx status, but got a different WebException type or no HTTP response: {webEx.GetType().Name}");
                }
            }
            catch (Exception e) // Catch any other unexpected exceptions
            {
                Debug.Assert(false, $"Caught unexpected exception type: {e.GetType().Name}. Message: {e.Message}");
            }
            finally
            {
                // Crucially, check that the ownerId was NOT changed on the ORIGINAL job URL.
                // The invalid URL should not have affected the actual job.
                var newJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);
                Console.WriteLine($"old 'ownerId' was : {oldJobOwnerId}");
                Console.WriteLine($"new 'ownerId' is : {newJobOwnerId}");
                Debug.Assert(oldJobOwnerId == newJobOwnerId, $"OwnerId WAS changed, but it shouldn't have been. Old: {oldJobOwnerId}, New: {newJobOwnerId}");

                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);
                Debug.Assert(job_status == "OK", $"Failed to Delete print job in Cloud api service:{job_url_list[3]}");
                Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            }
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("ChangeOwner")]
        public void Change_Owner_ByURL()
        {
            Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            var job_url_list = cloudprintObj.OwnersSetupHelper(ENV, CP_URL, TENANT_ID, CP_AUTH_TOKEN, TENANT);
            var oldJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);

            try
            {
                // Change Owner - This time, the 'emailInQuery' parameter is true,
                // meaning the new owner's email will be passed in the URL query string.
                ChangeOwnerResponse changeOwnerResponse = cloudprintObj.ChangeOwner(job_url_list[3], CP_AUTH_TOKEN, ADMIN_USER, true);

                // Assert that the change was successful based on the response DTO
                Debug.Assert(changeOwnerResponse.IsSuccessStatusCode, $"Change owner API call returned non-success HTTP status: {changeOwnerResponse.StatusCode}");
                Debug.Assert(changeOwnerResponse.Success, $"Change owner API call reported 'Success' as false: {changeOwnerResponse.Message}");
                Debug.Assert(changeOwnerResponse.Jobs != null && changeOwnerResponse.Jobs.Count > 0, "No jobs returned in ChangeOwnerResponse.");

                // Get the new job owner ID directly from the response DTO
                string newJobOwnerId = changeOwnerResponse.Jobs[0].OwnerId;

                Console.WriteLine($"old 'ownerId' was : {oldJobOwnerId}");
                Console.WriteLine($"new 'ownerId' is : {newJobOwnerId}");

                // Assert that the owner ID has indeed changed
                Debug.Assert(oldJobOwnerId != newJobOwnerId, $"OwnerId was NOT changed. Old: {oldJobOwnerId}, New: {newJobOwnerId}");

            }
            catch (Exception e)
            {
                Debug.Assert(false, $"An unexpected error occurred during Change_Owner_ByURL: {e.Message}");
            }
            finally
            {
                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);
                Debug.Assert(job_status == "OK", $"Failed to Delete print job in Cloud api service:{job_url_list[3]}");
                Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            }
        }

        /// <summary>
        /// ChangeOwner by URL with an Invalid Authentication code
        /// Expecting 401
        /// </summary>
        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("ChangeOwner")]
        public void Change_Owner_ByURLInvalidAuthentication()
        {
            Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            var job_url_list = cloudprintObj.OwnersSetupHelper(ENV, CP_URL, TENANT_ID, CP_AUTH_TOKEN, TENANT);
            var oldJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);

            ChangeOwnerResponse changeOwnerResponse = null; 

            try
            {
                // Change Owner with invalid token, passing new owner email in URL query string (emailInQuery = true).
                // The helper method will throw a WebException for 401.
                changeOwnerResponse = cloudprintObj.ChangeOwner(job_url_list[3], CP_AUTH_TOKEN + "INVALID", ADMIN_USER, true);

                // If the line above doesn't throw, it's an unexpected success, so fail the test.
                Debug.Assert(false, "Expected a WebException (e.g., 401 Unauthorized), but the API call succeeded.");
            }
            catch (WebException webEx) // Catch WebException specifically for HTTP errors
            {
                Console.WriteLine($"Caught expected WebException: {webEx.Message}");
                if (webEx.Response is HttpWebResponse httpResponse)
                {
                    // Assert on the HTTP status code directly for 401 Unauthorized
                    Debug.Assert(httpResponse.StatusCode == HttpStatusCode.Unauthorized,
                                 $"Expected HTTP 401 Unauthorized, but got: {httpResponse.StatusCode}");
                }
                else
                {
                    Debug.Assert(false, $"Expected HttpWebResponse with 401 status, but got a different WebException type: {webEx.GetType().Name}");
                }
            }
            catch (Exception e) // Catch any other unexpected exceptions
            {
                Debug.Assert(false, $"Caught unexpected exception type: {e.GetType().Name}. Message: {e.Message}");
            }
            finally
            {
                // Check to make sure ownerId was NOT changed. This assertion is crucial.
                var newJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);
                Console.WriteLine($"old 'ownerId' was : {oldJobOwnerId}");
                Console.WriteLine($"new 'ownerId' is : {newJobOwnerId}");
                Debug.Assert(oldJobOwnerId == newJobOwnerId, $"OwnerId WAS changed, but it shouldn't have been. Old: {oldJobOwnerId}, New: {newJobOwnerId}");

                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);
                Debug.Assert(job_status == "OK", $"Failed to Delete print job in Cloud api service:{job_url_list[3]}");
                Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            }
        }

        /// <summary>
        /// ChangeOwner by URL with an Invalid Owner
        /// Expecting 404
        /// </summary>
        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("ChangeOwner")]
        public void Change_Owner_ByURLInvalidOwner()
        {
            Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            var job_url_list = cloudprintObj.OwnersSetupHelper(ENV, CP_URL, TENANT_ID, CP_AUTH_TOKEN, TENANT);
            var oldJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);

            ChangeOwnerResponse changeOwnerResponse = null; 

            try
            {
                // Change Owner with an invalid new owner email, passing it in the URL query string.
                // The helper method should throw a WebException.
                changeOwnerResponse = cloudprintObj.ChangeOwner(job_url_list[3], CP_AUTH_TOKEN, "INVALID" + ADMIN_USER, true);

                // If the call doesn't throw, it's an unexpected success or different error, so fail the test.
                Debug.Assert(false, "Expected a WebException (e.g., 404 Not Found), but the API call succeeded or returned a different error.");
            }
            catch (WebException webEx) // Catch WebException specifically for HTTP errors
            {
                Console.WriteLine($"Caught expected WebException: {webEx.Message}");
                if (webEx.Response is HttpWebResponse httpResponse)
                {
                    // Assert on the HTTP status code directly for a 404 Not Found.
                    Debug.Assert(httpResponse.StatusCode == HttpStatusCode.NotFound,
                                 $"Expected HTTP 404 Not Found, but got: {httpResponse.StatusCode}");

                }
                else
                {
                    Debug.Assert(false, $"Expected HttpWebResponse with 404 status, but got a different WebException type: {webEx.GetType().Name}");
                }
            }
            catch (Exception e) 
            {
                Debug.Assert(false, $"Caught unexpected exception type: {e.GetType().Name}. Message: {e.Message}");
            }
            finally
            {
                // Check to make sure ownerId was NOT changed. This is critical for negative tests.
                var newJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);
                Console.WriteLine($"old 'ownerId' was : {oldJobOwnerId}");
                Console.WriteLine($"new 'ownerId' is : {newJobOwnerId}");
                Debug.Assert(oldJobOwnerId == newJobOwnerId, $"OwnerId WAS changed, but it shouldn't have been. Old: {oldJobOwnerId}, New: {newJobOwnerId}");

                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);
                Debug.Assert(job_status == "OK", $"Failed to Delete print job in Cloud api service:{job_url_list[3]}");
                Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            }
        }

        /// <summary>
        /// ChangeOwner by URL with an Invalid Tenant Id
        /// Expecting 400 Bad Request (or possibly 404 Not Found)
        /// </summary>
        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("ChangeOwner")]
        public void Change_Owner_ByURLInvalidTenantId()
        {
            Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            var job_url_list = cloudprintObj.OwnersSetupHelper(ENV, CP_URL, TENANT_ID, CP_AUTH_TOKEN, TENANT);
            var oldJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);

            var invalidTenantUrl = job_url_list[3].Replace(@"/tenants/", @"/tenants/2880" + Guid.NewGuid().ToString().Substring(4));
            ChangeOwnerResponse changeOwnerResponse = null; 

            try
            {
                // Attempt to change owner with the invalid tenant ID in the URL, passing email via query string.
                // The helper method should throw a WebException for 400 or 404.
                changeOwnerResponse = cloudprintObj.ChangeOwner(invalidTenantUrl, CP_AUTH_TOKEN, ADMIN_USER, true);

                // If the call doesn't throw, it's an unexpected success or a different error.
                Debug.Assert(false, "Expected a WebException (e.g., 400 Bad Request or 404 Not Found), but the API call succeeded or returned a different error.");
            }
            catch (WebException webEx) // Catch WebException specifically for HTTP errors
            {
                Console.WriteLine($"Caught expected WebException: {webEx.Message}");
                if (webEx.Response is HttpWebResponse httpResponse)
                {
                    // Assert that the status code is either 400 Bad Request or 404 Not Found.
                    // 400 is common for invalid format/parameters; 404 if the resource path (tenant) simply doesn't exist.
                    Debug.Assert(httpResponse.StatusCode == HttpStatusCode.BadRequest ||
                                 httpResponse.StatusCode == HttpStatusCode.NotFound,
                                 $"Expected HTTP 400 Bad Request or 404 Not Found, but got: {httpResponse.StatusCode}");
                }
                else
                {
       
                    Debug.Assert(false, $"Expected HttpWebResponse with 4xx status, but got a different WebException type or no HTTP response: {webEx.GetType().Name}");
                }
            }
            catch (Exception e)             {
                Debug.Assert(false, $"Caught unexpected exception type: {e.GetType().Name}. Message: {e.Message}");
            }
            finally
            {
                // Crucially, check that the ownerId was NOT changed on the ORIGINAL job URL.
                // The invalid tenant ID should not have affected the actual job.
                var newJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);
                Console.WriteLine($"old 'ownerId' was : {oldJobOwnerId}");
                Console.WriteLine($"new 'ownerId' is : {newJobOwnerId}");
                Debug.Assert(oldJobOwnerId == newJobOwnerId, $"OwnerId WAS changed, but it shouldn't have been. Old: {oldJobOwnerId}, New: {newJobOwnerId}");

                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);
                Debug.Assert(job_status == "OK", $"Failed to Delete print job in Cloud api service:{job_url_list[3]}");
                Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            }
        }

        /// <summary>
        /// Attempt to ChangeOwner with a malformed/invalid job URL.
        /// Expecting 404 Not Found or 400 Bad Request.
        /// </summary>
        [TestMethod, TestCategory("Regression"), TestCategory("Release"), TestCategory("ChangeOwner")]
        public void Change_Owner_ByURLInvalidURL()
        {
            Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            var job_url_list = cloudprintObj.OwnersSetupHelper(ENV, CP_URL, TENANT_ID, CP_AUTH_TOKEN, TENANT);
            var oldJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);

            // Create an invalid URL by appending extra characters to a valid job URL
            var invalidJobUrl = job_url_list[3] + "_INVALID_SUFFIX_XYZ";

            ChangeOwnerResponse changeOwnerResponse = null; 

            try
            {
                // Attempt to change owner with the invalid URL, passing email via query string.
                // The helper method should throw a WebException.
                changeOwnerResponse = cloudprintObj.ChangeOwner(invalidJobUrl, CP_AUTH_TOKEN, ADMIN_USER, true);
                Debug.Assert(false, "Expected a WebException (e.g., 400 Bad Request or 404 Not Found), but the API call succeeded or returned a different error.");
            }
            catch (WebException webEx) 
            {
                Console.WriteLine($"Caught expected WebException: {webEx.Message}");
                if (webEx.Response is HttpWebResponse httpResponse)
                {
                    // Assert that the status code is either 404 Not Found or 400 Bad Request.
                    // 404 is most common for non-existent paths, 400 for malformed syntax.
                    Debug.Assert(httpResponse.StatusCode == HttpStatusCode.NotFound ||
                                 httpResponse.StatusCode == HttpStatusCode.BadRequest,
                                 $"Expected HTTP 404 Not Found or 400 Bad Request, but got: {httpResponse.StatusCode}");

                    Console.WriteLine($"Actual HTTP Status Code: {httpResponse.StatusCode}");
                }
                else
                {
                    // This catches scenarios where WebException is thrown but without an HttpWebResponse (e.g., network issues).
                    Debug.Assert(false, $"Expected HttpWebResponse with 4xx status, but got a different WebException type or no HTTP response: {webEx.GetType().Name}");
                }
            }
            catch (Exception e) 
            {
                Debug.Assert(false, $"Caught unexpected exception type: {e.GetType().Name}. Message: {e.Message}");
            }
            finally
            {
                // Crucially, check that the ownerId was NOT changed on the ORIGINAL job URL.
                // The invalid URL should not have affected the actual job.
                var newJobOwnerId = cloudprintObj.GetJobOwnerId(job_url_list[3], CP_AUTH_TOKEN);
                Console.WriteLine($"old 'ownerId' was : {oldJobOwnerId}");
                Console.WriteLine($"new 'ownerId' is : {newJobOwnerId}");
                Debug.Assert(oldJobOwnerId == newJobOwnerId, $"OwnerId WAS changed, but it shouldn't have been. Old: {oldJobOwnerId}, New: {newJobOwnerId}");

                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);
                Debug.Assert(job_status == "OK", $"Failed to Delete print job in Cloud api service:{job_url_list[3]}");
                Console.WriteLine($"Test executed successfully: " + testContextInstance.TestName);
            }

            [TestMethod]
            [TestCategory("Regression"), TestCategory("Release")]
            void Cloud_Print_Delete()
            {
                /*
                 1. Get list of Active printer Create JOB URLs 
                 2. Create a Print Job using the URL from above step.
                 3. Delete printJob.
                */
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");

                if (string.IsNullOrEmpty(CP_URL))
                {
                    CP_URL = cloudprintObj.GetCloudPrintURI(Session_Environment, CP_AUTH_TOKEN, TENANT);
                }
                Assert.IsTrue(CP_URL.Contains(TENANT_ID), $"Cloud print api url failed to extract for environment : {Session_Environment}");

                string[] Printers_id_List = cloudprintObj.GetActivePrinter_ID_List(CP_URL, CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, Printers_id_List.Length, $"Tenant Environment not found any active printers, so list is empty in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of Active printers id's:");
                foreach (string id in Printers_id_List)
                {
                    Console.WriteLine(id);
                }

                // 1. Get Create JOB URL
                string[] Printers_cr_job_List = cloudprintObj.Get_Create_Job_URL_List(CP_URL, CP_AUTH_TOKEN);
                Assert.AreNotEqual(0, Printers_id_List.Length, $"No Active Printers found in Tenant in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("List of create job URL List is :");
                foreach (string job in Printers_cr_job_List)
                {
                    Console.WriteLine(job);
                }

                // 2. Create a Job to active printers.
                string[] job_url_list = cloudprintObj.Create_Print_Job(Printers_cr_job_List[0], CP_AUTH_TOKEN);
                Assert.IsNotNull(job_url_list, $"No Create Job URLs found to extract in environment : {Session_Environment} & tenant {TENANT_ID}");

                Console.WriteLine("JOB URL Details");
                foreach (string jurl in job_url_list)
                {
                    Console.WriteLine(jurl);
                }

                Thread.Sleep(1000);

                // 3. Delete Print Job
                Console.WriteLine($"deleting Job is...........: {job_url_list[3]}");
                string job_status = cloudprintObj.DeleteJobStatus(job_url_list[3], CP_AUTH_TOKEN);
                Assert.AreEqual("OK", job_status, $"Failed to Delete print job in Cloud api service:{job_url_list[3]}");

                Console.WriteLine($"Test executed successfully {testContextInstance.TestName}");
            }


            [TestMethod]
            [TestCategory("Regression"), TestCategory("Release"), TestCategory("GuestUser")]
            void CreateGuestUsers_InvalidRole()
            {
                Console.WriteLine($"Test Execution:{testContextInstance.TestName}");

                var random = new Random();
                var uniqueUserId = random.Next(999999);
                var uniqueFullUserName = "GUser" + uniqueUserId;
                var uniqueUserEmail = $"{uniqueFullUserName}@SQA.com";

                bool isFound = cloudprintObj.FindGuestUser(CPAPI_URL, CP_AUTH_TOKEN, TENANT_ID, uniqueFullUserName);

                if (!isFound)
                {
                    var createRequest = new CreateGuestUserRequest
                    {
                        Email = uniqueUserEmail,
                        FullName = uniqueFullUserName,
                        Role = "INVALID_ROLE_TYPE", // Intentionally invalid role payload
                        Pin = "1980",
                        Password = "EeGahqu7",
                        ExpirationTimestamp = DateTime.Now.AddDays(30).ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture)
                    };

                    GuestUserResponse response = cloudprintObj.CreateGuestUser(CPAPI_URL, TENANT_ID, CP_AUTH_TOKEN, createRequest);

                    Assert.IsFalse(response.IsSuccessStatusCode, "API call should fail for an unexpected role allocation.");
                    Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, $"Expected bad request syntax for invalid role, instead received: {response.StatusCode}");
                    Assert.IsFalse(response.Success, "API metadata context reported false positive configuration context.");
                }
                Console.WriteLine($"successfully executed:{testContextInstance.TestName}, user :{uniqueFullUserName}");
            }

        }



        [TestCleanup()]
        public void Teardown()
        {
            // This method will be called after each MSTest test method has completed
        }

    }
}