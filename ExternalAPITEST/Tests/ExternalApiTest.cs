using System.Diagnostics;
using Newtonsoft.Json;
using System;
using CommonUtils;
using ExternalAPI.DTO;

namespace ExternalAPI
{

    [TestClass]
    public class ExternalApiTest
    {
        public static string AUTH_TOKEN = String.Empty; 
        public static string AUTH_URL = String.Empty; 
        public static string TOKEN_URL = String.Empty;
        public static string AUTH_TOKEN_URL = String.Empty; 
        public static string ADMIN_USER = String.Empty; public static string ADMIN_PASS = String.Empty; 
        public static string RESPONSE_TYPE = String.Empty;
        public static string CLIENT_ID = String.Empty; 
        public static string CLIENT_SECRET = String.Empty; 
        public static string GRANT_TYPE = String.Empty; 
        public static string ENV = String.Empty;
        public static string TENANT_ID = String.Empty; 
        public static string IP = String.Empty; public static string IN_NETWORK_ID = String.Empty;
        public static string MAC = String.Empty; public static string NETWORK_NAME = String.Empty; public static string IN_NETWORK_NAME = String.Empty;
        public static string networkID = String.Empty; public static string WORKFLOW_NAME = String.Empty; public static string WORKFLOW_ID = String.Empty;
        public static string EMAIL_TO = String.Empty; public static string IN_WORKFLOW_NAME = String.Empty; public static string REDIRECT_URI = String.Empty;
        public static string IN_WORKFLOW_ID = String.Empty; public static string GROUP_ID = String.Empty; public static string UPLOAD_LINK_WF = String.Empty; public static string FÍNISH_UPLOAD_LINK_WF = String.Empty;
        public static string TENANT = String.Empty; 
        public static string INVALID_B_JWT = String.Empty; public static string VALID_EXP_B_JWT = String.Empty; public static string Session_Environment;

        public static string NORMAL_USER = String.Empty; public static string USER_PASS = String.Empty;
        public static string TEMP_TENANT = String.Empty; public static string dummyPrinterName = String.Empty;

        static Authorization? AuthObj; 
        static Networks? networkObj; 
        static Users? usersObj; 
        static WorkStations? wsObj;
        static Capture? captureObj;
        static Printers? printerObj; 
        static Queues? queueObj;


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestInitialize]
        public void Initialize()
        {
            Session_Environment = System.Environment.GetEnvironmentVariable("environment");
            CommonUtil.ValidateEnvironmentVariable(Session_Environment);
            TestContext.Properties["environment"] = Session_Environment;
            string binPath = AppDomain.CurrentDomain.BaseDirectory;
            string repoRoot = Path.GetFullPath(Path.Combine(binPath, @"..\..\..\"));
            string dataPath = Path.Combine(repoRoot, "APITest.Utils", "Data");
            string path_out = CommonUtil.env_setup(Session_Environment, dataPath);
            var testdata = CommonUtil.ReadAndValidateJsonData<TestData>($"{path_out}");
            AUTH_URL = testdata.admin.auth_url; ENV = Session_Environment; AUTH_TOKEN_URL = testdata.admin.auth_token_url;
            TOKEN_URL = testdata.admin.token_url; ADMIN_USER = testdata.admin.admin_username; ADMIN_PASS = testdata.admin.admin_pass;
            TENANT_ID = testdata.admin.tenant_id; TOKEN_URL = testdata.admin.token_url; RESPONSE_TYPE = testdata.admin.response_type;
            IP = testdata.admin.ip; MAC = testdata.admin.mac; GRANT_TYPE = testdata.admin.grant_type; CLIENT_ID = testdata.admin.client_id;
            CLIENT_SECRET = testdata.admin.client_secret; NETWORK_NAME = testdata.admin.network_name; IN_NETWORK_NAME = testdata.admin.network_name;
            IN_NETWORK_ID = testdata.admin.network_id; EMAIL_TO = testdata.admin.email_to; IN_WORKFLOW_NAME = testdata.admin.workflow_name;
            IN_WORKFLOW_ID = testdata.admin.workflow_id; GROUP_ID = testdata.admin.group_id; 
            TENANT = testdata.admin.tenant; INVALID_B_JWT = testdata.admin.InvalidSessionBeforeJWT;
            VALID_EXP_B_JWT = testdata.admin.validButExpiredSessionBeforeCredentials;
            NORMAL_USER = testdata.admin.user_username; USER_PASS = testdata.admin.user_pass;
            REDIRECT_URI = testdata.admin.redirect_uri;
            TEMP_TENANT = testdata.admin.temp_tenant;
            
            AuthObj = new Authorization();
            AUTH_TOKEN = AuthObj.GenerateNewToken(AUTH_TOKEN_URL, Session_Environment, ADMIN_USER, ADMIN_PASS, GRANT_TYPE, CLIENT_ID, CLIENT_SECRET, RESPONSE_TYPE,REDIRECT_URI);
            //objects creation for resources
            networkObj = new Networks(); usersObj = new Users(); wsObj = new WorkStations(); queueObj = new Queues();
            captureObj = new Capture(); printerObj = new Printers();
        }

        [TestMethod, TestCategory("smoke"), TestCategory("Regression"), TestCategory("Release")]
        
        public void Get_Authorization_Token()
        {
            try
            {
                Console.WriteLine($"Test Execution:{testContextInstance.TestName}, environment: {Session_Environment}");
                Debug.Assert(AUTH_TOKEN != null, "Auth Token must be generated");
                Console.WriteLine($"Successfully tested {testContextInstance.TestName}, new Token : {AUTH_TOKEN}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }
            
        }

        [TestMethod, TestCategory("Regression"), TestCategory("user"), TestCategory("Release")]
        public void GetTenantUsersList()
        {
            try {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string[] userNames = usersObj.GetUsersList(AUTH_URL, AUTH_TOKEN, TENANT_ID);
                Debug.Assert(userNames.Length != 0, "Users List should not Empty");
                Console.WriteLine($"List of Teant users:");
                foreach (string name in userNames)
                {
                    Console.WriteLine(name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);
            }

            // Code must be placed here 
           
        }

        [TestMethod, TestCategory("Regression"), TestCategory("ws"), TestCategory("Release")]
        public void GetTenantWorkstationsList()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}"); 
                string[] wsNames = wsObj.GetWSList(AUTH_URL, AUTH_TOKEN, TENANT_ID);
                Debug.Assert(wsNames.Length != 0, "workstations List should not Empty");
                Console.WriteLine($"List of Teant workstations:");
                foreach (string name in wsNames)
                {
                    Console.WriteLine(name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
            
        }

        [TestMethod, TestCategory("Regression"), TestCategory("network"), TestCategory("Release")]
        public void GetTenantNetworksList()
        {
            try
            {
                // Code must be placed here 
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string[] networkNames = networkObj.GetNetworksList(AUTH_URL, AUTH_TOKEN, TENANT_ID);
                Debug.Assert(networkNames.Length != 0, $"Netowrks List should not Empty for tenant: {TENANT_ID}");
                Console.WriteLine($"List of Teant Networks:");
                foreach (string name in networkNames)
                {
                    Console.WriteLine(name);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
        }

        [TestMethod, TestCategory("smoke"), TestCategory("network"), TestCategory("Regression"), TestCategory("Release")]
        public void AddNewNetwork()
        {

            try
            {
                // Create network  
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                networkID = networkObj.AddNetworkName(AUTH_URL, AUTH_TOKEN, TENANT_ID, IN_NETWORK_NAME);
                Debug.Assert(networkID.Length != 0, $"Netowrk ID should not Empty for Network : {IN_NETWORK_NAME}");

                // ADD Network GateWay.
                string status = networkObj.AddNetworkGateway(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID, IP, MAC);
                Debug.Assert(status == "OK", $"Failed to add network gateway, network name : {IN_NETWORK_NAME}  & network_id : {networkID}");
                Console.WriteLine($"Network Gateway is added successfully : {IN_NETWORK_NAME} & Network ID is : {networkID} , ip: {IP}, mac : {MAC}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
            
        }

        [TestMethod, TestCategory("network"), TestCategory("Regression"), TestCategory("Release")]
        public void AddMultipleNetworkGateWays_sameIP()
        {
            try
            {
                // Create network  name , new ip & new MAC
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                Random rnd = new Random();
                string newNETWORK_NAME = NETWORK_NAME + "1A";
                string newNETWORK_NAME1 = NETWORK_NAME + "1B";
                string newMAC = "001727101112";
                string newMAC1 = "001727101113";
                networkID = networkObj.AddNetworkName(AUTH_URL, AUTH_TOKEN, TENANT_ID, newNETWORK_NAME);
                Debug.Assert(networkID.Length != 0, $"Netowrk ID should not Empty for network name : {newNETWORK_NAME}");

                // ADD Network GateWay.
                string status = networkObj.AddNetworkGateway(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID, IP, newMAC);
                Debug.Assert(status == "OK", $"network gateway failed to add successfully, netowrk name : {newNETWORK_NAME},& Network ID is : {networkID}, ip: {IP}, mac: {newMAC}");
                Console.WriteLine($"First Network Gateway is added successfully : {newNETWORK_NAME} & Network ID is : {networkID} , ip: {IP}, mac : {newMAC}");

                string networkID1 = networkObj.AddNetworkName(AUTH_URL, AUTH_TOKEN, TENANT_ID, newNETWORK_NAME1);
                Debug.Assert(networkID.Length != 0, $"Netowrk ID should not Empty for network name {newNETWORK_NAME1}");

                // ADD Network GateWay.
                string status1 = networkObj.AddNetworkGateway(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID1, IP, newMAC1);
                Debug.Assert(status1 == "OK", $"network gateway failed to add successfully, netowrk name : {newNETWORK_NAME1},& Network ID is : {networkID1}, ip: {IP}, mac: {newMAC1}");
                Console.WriteLine($"Network Gateway is added successfully : {newNETWORK_NAME1} & Network ID is : {networkID} , ip: {IP}, new mac : {newMAC1}");


                //delete first network
                networkObj.DeleteNetwork(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID);
                //delete second network
                networkObj.DeleteNetwork(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID1);



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }



        }

        [TestMethod, TestCategory("network"), TestCategory("Regression"), TestCategory("Release")]
        public void AddMultipleNetworkGateWays_sameMAC()
        {
            try
            {
                // Create network  
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string newNETWORK_NAME = NETWORK_NAME + "1C";
                string newNETWORK_NAME1 = NETWORK_NAME + "1D";
                string newIP = "192.192.19.10";
                string newIP1 = "192.192.19.11";
                networkID = networkObj.AddNetworkName(AUTH_URL, AUTH_TOKEN, TENANT_ID, newNETWORK_NAME);
                Debug.Assert(networkID.Length != 0, $"Netowrk ID should not Empty for network : {newNETWORK_NAME}");

                // ADD Network GateWay.

                string status = networkObj.AddNetworkGateway(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID, newIP, MAC);
                Debug.Assert(status == "OK", $"network gateway failed to add successfully, netowrk name : {newNETWORK_NAME},& Network ID is : {networkID}, ip: {newIP}, mac: {MAC} ");
                Console.WriteLine($"First Network Gateway is added successfully : {newNETWORK_NAME} & Network ID is : {networkID} , ip: {newIP}, mac : {MAC}");

                // add new network name & gateway 
                string networkID1 = networkObj.AddNetworkName(AUTH_URL, AUTH_TOKEN, TENANT_ID, newNETWORK_NAME1);
                Debug.Assert(networkID1.Length != 0, "Netowrk ID should not Empty");

                // ADD Network GateWay.
                string status1 = networkObj.AddNetworkGateway(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID1, newIP1, MAC);
                Debug.Assert(status1 == "OK", $"network gateway failed to add successfully, netowrk name : {newNETWORK_NAME1},& Network ID is : {networkID1}, ip: {newIP1}, mac: {MAC}");
                Console.WriteLine($"Network Gateway is added successfully : {newNETWORK_NAME1} & Network ID is : {networkID1} , ip: {newIP1}, new mac : {MAC}");

                //delete first network
                networkObj.DeleteNetwork(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID);
                //delete second network
                networkObj.DeleteNetwork(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID1);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

        }

        [TestMethod, TestCategory("network"), TestCategory("Regression")]
        [TestProperty("ExecutionOrder", "8")]
        public void AddDuplicateNetworkGateWays()
        {
            try
            {
                // Create network name & add IP & MAC
                // use same network name to add same IP & MAC  & expected result must be Forbidden  403.
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                Random rnd = new Random();
                int number = rnd.Next();
                string newNETWORK_NAME = NETWORK_NAME + "1E";
                string newNETWORK_NAME1 = NETWORK_NAME + "1F";
                string newMAC = "001727101113";
                string newIP = "192.192.20.13";


                networkID = networkObj.AddNetworkName(AUTH_URL, AUTH_TOKEN, TENANT_ID, newNETWORK_NAME);
                Debug.Assert(networkID.Length != 0, "Netowrk ID should not Empty");

                // ADD Network GateWay.
                string status = networkObj.AddNetworkGateway(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID, newIP, newMAC);
                Debug.Assert(status == "OK", "Network gateway should ADD and status must be OK, actual status :{status}");

                Console.WriteLine($"First Network Gateway is added successfully : {newNETWORK_NAME} & Network ID is : {networkID} , ip: {newIP}, mac : {newMAC}");
                // ADD Network name &  GateWay for duplicate.
                string networkID1 = networkObj.AddNetworkName(AUTH_URL, AUTH_TOKEN, TENANT_ID, newNETWORK_NAME1);
                Debug.Assert(networkID1.Length != 0, $"Netowrk ID should not Empty for neotwork : {newNETWORK_NAME1}");
                //add network gateway
                string status1 = networkObj.AddNetworkGateway(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID1, newIP, newMAC);
                Debug.Assert(status1 == "Conflict", $"Patch Request stauts must be Conflict, status code = 409, network : {newNETWORK_NAME1} & Network ID is : {networkID1} , ip: {newIP}, new mac : {newMAC}");
                Console.WriteLine($"Network Gateway is not added due to duplicate gateway : {newNETWORK_NAME1} & Network ID is : {networkID1} , ip: {newIP}, new mac : {newMAC}");

                //delete first network
                networkObj.DeleteNetwork(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID);
                //delete second network
                networkObj.DeleteNetwork(AUTH_URL, AUTH_TOKEN, TENANT_ID, networkID1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
           

        }

        [TestMethod, TestCategory("smoke"), TestCategory("network"), TestCategory("Regression"), TestCategory("Release")]
        [TestProperty("ExecutionOrder", "30")]
        public void DeleteNetwork()
        {
            try
            {
                // Get the list if Network ID's of the network name
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string[] networkIDs = networkObj.GetNetworksIDList(AUTH_URL, AUTH_TOKEN, TENANT_ID, IN_NETWORK_NAME);

                if (networkIDs.Length == 0)
                {
                    Console.WriteLine($"Empty List of networkid's to delete for {IN_NETWORK_NAME}");
                }

                foreach (string nid in networkIDs)
                {
                    Console.WriteLine(nid);
                    networkObj.DeleteNetwork(AUTH_URL, AUTH_TOKEN, TENANT_ID, nid);
                }
                Console.WriteLine($"Deleted network {IN_NETWORK_NAME} Test is successfully");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

        }

        [TestMethod, TestCategory("smoke"), TestCategory("capture"), TestCategory("Regression"), TestCategory("Release")]
        public void Create_Capture_workFlow()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                //1.Get the workflowname:
                WORKFLOW_NAME = "QA_WF_";
                CommonUtil wfname = new CommonUtil();
                WORKFLOW_NAME = wfname.GetWorkFlowName(WORKFLOW_NAME);
                string source = "MFP";  //"MFP"  or "PHONE" , default value : "PHONE"
                WORKFLOW_ID = captureObj.CreateWorkFlow(WORKFLOW_NAME, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, source);
                Debug.Assert(WORKFLOW_ID != null, $"Capture workflow is failed to create : {WORKFLOW_NAME}");
                Console.WriteLine($"Test executed successfully create- workflow:{WORKFLOW_NAME}  & ID: {WORKFLOW_ID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }


        }

        [TestMethod, TestCategory("smoke"), TestCategory("capture"), TestCategory("Regression"), TestCategory("Release")]
        public void DeleteWorkFlow()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                //1.Get the workflowname:
                WORKFLOW_NAME = "Kofax_WF_";
                CommonUtil wfname = new CommonUtil();
                WORKFLOW_NAME = wfname.GetWorkFlowName(WORKFLOW_NAME);
                //source =  MFP  or PHONE   otherwise it will create a workflow with source type = phone

                WORKFLOW_ID = captureObj.CreateWorkFlow(WORKFLOW_NAME, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "PHONE");

                string[] workflowIDs = captureObj.GetWorkFlowIDList(AUTH_URL, AUTH_TOKEN, TENANT_ID, WORKFLOW_NAME);

                if (workflowIDs.Length == 0)
                {
                    Console.WriteLine($"Empty List workflowid's to delete for {WORKFLOW_NAME}");
                }
                foreach (string wid in workflowIDs)
                {
                    Console.WriteLine(wid);
                    captureObj.DeleteWorkflow(AUTH_URL, AUTH_TOKEN, TENANT_ID, wid);
                }
                Console.WriteLine($"Deleted workflow {WORKFLOW_NAME} successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
            
        }

        [TestMethod, TestCategory("smoke"), TestCategory("capture"), TestCategory("regression"), TestCategory("Release")]
        public void Upload_Black_Page_PDF_to_Capture_workFlow()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                CommonUtil resourceFile = new CommonUtil();
                //1a. get workflow_id 
                string[] workflowIDs = captureObj.GetWorkFlowIDList(AUTH_URL, AUTH_TOKEN, TENANT_ID, IN_WORKFLOW_NAME);
                Debug.Assert(workflowIDs.Length != 0, $"Capture workflow is failed to fetch workflow ID to : {IN_WORKFLOW_NAME}");
                //1b.Create upload URL.
                string[] uploadLink = captureObj.CreateUploadLink(AUTH_URL, AUTH_TOKEN, TENANT_ID, workflowIDs[0]);
                Debug.Assert(uploadLink.Length != 0, $"Capture workflow is failed to create upload Link: {IN_WORKFLOW_NAME}");

                Console.WriteLine($"Upload PDF Link URL is: {uploadLink[0]}");
                Console.WriteLine($"Finish Upload PDF Link URL is: {uploadLink[1]}");

                //2. Upload file to Azure Blob or other sources.
                UPLOAD_LINK_WF = uploadLink[0];
                FÍNISH_UPLOAD_LINK_WF = uploadLink[1];

                string fileName = resourceFile.GetResourceFilePath("blackPDF.pdf");
                string fileType = "PDF";    // "word"    or "excel"   or  "PDF" or "png"
                string upload_status = captureObj.UploadFile(UPLOAD_LINK_WF, fileName, fileType);
                Debug.Assert(upload_status == "Created", $"Failed to upload PDF to Workflow:{IN_WORKFLOW_NAME}");
                //3.FinishUpload url
                if (upload_status == "Created")
                {
                    string finishUpload = captureObj.FinishUploadFile(FÍNISH_UPLOAD_LINK_WF, AUTH_TOKEN, fileType);
                    Debug.Assert(finishUpload == "Accepted", $"Failed to Finish upload black PDF to Workflow:{IN_WORKFLOW_NAME}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
            

        }

        [TestMethod, TestCategory("capture"), TestCategory("regression"), TestCategory("Release")]
        public void Upload_White_Page_PDF_to_Capture_workFlow()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                CommonUtil resourceFile = new CommonUtil();
                //1a. get workflow_id 
                string[] workflowIDs = captureObj.GetWorkFlowIDList(AUTH_URL, AUTH_TOKEN, TENANT_ID, IN_WORKFLOW_NAME);
                Debug.Assert(workflowIDs.Length != 0, $"Capture workflow is failed to fetch workflow ID to : {IN_WORKFLOW_NAME}");
                //1b.Create upload URL.
                string[] uploadLink = captureObj.CreateUploadLink(AUTH_URL, AUTH_TOKEN, TENANT_ID, workflowIDs[0]);
                Debug.Assert(uploadLink.Length != 0, $"Capture workflow is failed to create upload Link: {IN_WORKFLOW_NAME}");

                Console.WriteLine($"Upload PDF Link URL is: {uploadLink[0]}");
                Console.WriteLine($"Finish Upload PDF Link URL is: {uploadLink[1]}");

                //2. Upload file to Azure Blob or other sources.
                UPLOAD_LINK_WF = uploadLink[0];
                FÍNISH_UPLOAD_LINK_WF = uploadLink[1];

                string fileName = resourceFile.GetResourceFilePath("whitePDF.pdf");
                string fileType = "PDF";    // "word"    or "excel"   or  "PDF" or "png"
                string upload_status = captureObj.UploadFile(UPLOAD_LINK_WF, fileName, fileType);
                Debug.Assert(upload_status == "Created", $"Failed to upload PDF to Workflow:{IN_WORKFLOW_NAME}");
                //3.FinishUpload url
                if (upload_status == "Created")
                {
                    string finishUpload = captureObj.FinishUploadFile(FÍNISH_UPLOAD_LINK_WF, AUTH_TOKEN, fileType);
                    Debug.Assert(finishUpload == "Accepted", $"Failed to Finish upload white PDF to Workflow:{IN_WORKFLOW_NAME}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
            

        }

        [TestMethod, TestCategory("capture"), TestCategory("regression"), TestCategory("Release")]
        [TestProperty("ExecutionOrder", "14")]
        public void Upload_Big_File_PNG_to_Capture_workFlow()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                CommonUtil resourceFile = new CommonUtil();
                //1a. get workflow_id 
                string[] workflowIDs = captureObj.GetWorkFlowIDList(AUTH_URL, AUTH_TOKEN, TENANT_ID, IN_WORKFLOW_NAME);
                Debug.Assert(workflowIDs.Length != 0, $"Capture workflow is failed to fetch workflow ID to : {IN_WORKFLOW_NAME}");
                //1b.Create upload URL.
                string[] uploadLink = captureObj.CreateUploadLink(AUTH_URL, AUTH_TOKEN, TENANT_ID, workflowIDs[0]);
                Debug.Assert(uploadLink.Length != 0, $"Capture workflow is failed to create upload Link: {IN_WORKFLOW_NAME}");

                Console.WriteLine($"Upload PNG Link URL is: {uploadLink[0]}");
                Console.WriteLine($"Finish Upload PNG Link URL is: {uploadLink[1]}");

                //2. Upload file to Azure Blob or other sources.
                UPLOAD_LINK_WF = uploadLink[0];
                FÍNISH_UPLOAD_LINK_WF = uploadLink[1];

                string fileName = resourceFile.GetResourceFilePath("too-big.png");
                string fileType = "png";    // "word"    or "excel"   or  "PDF" or "png"
                string upload_status = captureObj.UploadFile(UPLOAD_LINK_WF, fileName, fileType);
                Debug.Assert(upload_status == "Created", $"Failed to upload PNG to Workflow:{IN_WORKFLOW_NAME}");
                //3.FinishUpload url
                if (upload_status == "Created")
                {
                    string finishUpload = captureObj.FinishUploadFile(FÍNISH_UPLOAD_LINK_WF, AUTH_TOKEN, fileType);
                    Debug.Assert(finishUpload == "Accepted", $"Failed to Finish upload too big file to Workflow:{IN_WORKFLOW_NAME}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
            

        }

        [TestMethod, TestCategory("capture"), TestCategory("regression"), TestCategory("Release")]
        [TestProperty("ExecutionOrder", "15")]
        public void Upload_Invalid_File_PNG_to_Capture_workFlow()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                CommonUtil resourceFile = new CommonUtil();
                //1a. get workflow_id 
                string[] workflowIDs = captureObj.GetWorkFlowIDList(AUTH_URL, AUTH_TOKEN, TENANT_ID, IN_WORKFLOW_NAME);
                Debug.Assert(workflowIDs.Length != 0, $"Capture workflow is failed to fetch workflow ID to : {IN_WORKFLOW_NAME}");
                //1b.Create upload URL.
                string[] uploadLink = captureObj.CreateUploadLink(AUTH_URL, AUTH_TOKEN, TENANT_ID, workflowIDs[0]);
                Debug.Assert(uploadLink.Length != 0, $"Capture workflow is failed to create upload Link: {IN_WORKFLOW_NAME}");

                Console.WriteLine($"Upload PNG Link URL is: {uploadLink[0]}");
                Console.WriteLine($"Finish Upload PNG Link URL is: {uploadLink[1]}");

                //2. Upload file to Azure Blob or other sources.
                UPLOAD_LINK_WF = uploadLink[0];
                FÍNISH_UPLOAD_LINK_WF = uploadLink[1];

                string fileName = resourceFile.GetResourceFilePath("invalidPNG.png");
                string fileType = "png";    // "word"    or "excel"   or  "PDF" or "png"
                string upload_status = captureObj.UploadFile(UPLOAD_LINK_WF, fileName, fileType);
                Debug.Assert(upload_status == "Created", $"Failed to upload PNG to Workflow:{IN_WORKFLOW_NAME}");
                //3.FinishUpload url
                if (upload_status == "Created")
                {
                    string finishUpload = captureObj.FinishUploadFile(FÍNISH_UPLOAD_LINK_WF, AUTH_TOKEN, fileType);
                    Debug.Assert(finishUpload == "Accepted", $"Failed to Finish upload invalid file to Workflow:{IN_WORKFLOW_NAME}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
           

        }

        [TestMethod, TestCategory("capture"), TestCategory("regression"), TestCategory("Release")]
        public void Upload_Valid_File_PNG_to_Capture_workFlow()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                CommonUtil resourceFile = new CommonUtil();
                //1a. get workflow_id 
                string[] workflowIDs = captureObj.GetWorkFlowIDList(AUTH_URL, AUTH_TOKEN, TENANT_ID, IN_WORKFLOW_NAME);
                Debug.Assert(workflowIDs.Length != 0, $"Capture workflow is failed to fetch workflow ID to : {IN_WORKFLOW_NAME}");
                //1b.Create upload URL.
                string[] uploadLink = captureObj.CreateUploadLink(AUTH_URL, AUTH_TOKEN, TENANT_ID, workflowIDs[0]);
                Debug.Assert(uploadLink.Length != 0, $"Capture workflow is failed to create upload Link: {IN_WORKFLOW_NAME}");

                Console.WriteLine($"Upload PNG File Link URL is: {uploadLink[0]}");
                Console.WriteLine($"Finish Upload PNG Link URL is: {uploadLink[1]}");

                //2. Upload file to Azure Blob or other sources.
                UPLOAD_LINK_WF = uploadLink[0];
                FÍNISH_UPLOAD_LINK_WF = uploadLink[1];

                string fileName = resourceFile.GetResourceFilePath("invalidPNG.png");
                string fileType = "png";    // "word"    or "excel"   or  "PDF" or "png"
                string upload_status = captureObj.UploadFile(UPLOAD_LINK_WF, fileName, fileType);
                Debug.Assert(upload_status == "Created", $"Failed to upload PNG to Workflow:{IN_WORKFLOW_NAME}");
                //3.FinishUpload url
                if (upload_status == "Created")
                {
                    string finishUpload = captureObj.FinishUploadFile(FÍNISH_UPLOAD_LINK_WF, AUTH_TOKEN, fileType);
                    Debug.Assert(finishUpload == "Accepted", $"Failed to Finish upload valid file to Workflow:{IN_WORKFLOW_NAME}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

        }

        [TestMethod, TestCategory("capture"), TestCategory("Regression"), TestCategory("Release")]
        public void Modify_workFlow_PHONE()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                Console.WriteLine($"1.Create a Work flow");
                WORKFLOW_NAME = "Kofax_WF_";
                CommonUtil wfname = new CommonUtil();
                WORKFLOW_NAME = wfname.GetWorkFlowName(WORKFLOW_NAME);
                //source =  MFP  or PHONE   otherwise it will create a workflow with source type = phone

                WORKFLOW_ID = captureObj.CreateWorkFlow(WORKFLOW_NAME, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "PHONE");
                Debug.Assert(WORKFLOW_ID != null, $"Capture workflow is failed to create : {WORKFLOW_NAME}");

                Console.WriteLine($"2.Modify Workflow :{WORKFLOW_NAME}");
                //Data for Modification order : string workflow_name, string workflow_id, string url, string authtoken, string tenantID, string email_to, string source="PHONE", string active="true", string orientation= "PORTRAIT", string Ftype= "GENERATE_SEARCHABLE_PDF", string pdfversion= "PDF_1_6"
                string status = captureObj.ModifyWorkFlow(WORKFLOW_NAME, WORKFLOW_ID, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "PHONE", "true", "PORTRAIT", "GENERATE_SEARCHABLE_PDF", "PDF_1_6");
                //string status = captureObj.CreateWorkFlow1(WORKFLOW_NAME, WORKFLOW_ID, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "PHONE");
                Debug.Assert(status == "Accepted", $"Capture workflow is failed to Modify : {WORKFLOW_NAME}");

                Console.WriteLine($"3.Delete Workflow {WORKFLOW_NAME}");
                captureObj.DeleteWorkflow(AUTH_URL, AUTH_TOKEN, TENANT_ID, WORKFLOW_ID);

                Console.WriteLine($"Test executed successfully Modify workflow is created, modified & deleted:{WORKFLOW_NAME}  & ID: {WORKFLOW_ID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
            
        }

        [TestMethod, TestCategory("capture"), TestCategory("Regression"), TestCategory("Release")]
        public void Modify_workFlow_MFP()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                Console.WriteLine($"1.Create a Work flow");
                WORKFLOW_NAME = "Kofax_WF_";
                CommonUtil wfname = new CommonUtil();
                WORKFLOW_NAME = wfname.GetWorkFlowName(WORKFLOW_NAME);
                //source =  MFP  or PHONE   otherwise it will create a workflow with source type = phone

                WORKFLOW_ID = captureObj.CreateWorkFlow(WORKFLOW_NAME, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "MFP");
                Debug.Assert(WORKFLOW_ID != null, $"Capture workflow is failed to create : {WORKFLOW_NAME}");

                Console.WriteLine($"2.Modify Workflow :{WORKFLOW_NAME}");
                //Data for Modification order : string workflow_name, string workflow_id, string url, string authtoken, string tenantID, string email_to, string source="PHONE", string active="true", string orientation= "PORTRAIT", string Ftype= "GENERATE_SEARCHABLE_PDF", string pdfversion= "PDF_1_6"
                string status = captureObj.ModifyWorkFlow(WORKFLOW_NAME, WORKFLOW_ID, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "MFP", "true", "PORTRAIT", "GENERATE_SEARCHABLE_PDF", "PDF_2_0");
                //string status = captureObj.CreateWorkFlow1(WORKFLOW_NAME, WORKFLOW_ID, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "PHONE");
                Debug.Assert(status == "Accepted", $"Capture workflow is failed to Modify : {WORKFLOW_NAME}");

                Console.WriteLine($"3.Delete Workflow {WORKFLOW_NAME}");
                captureObj.DeleteWorkflow(AUTH_URL, AUTH_TOKEN, TENANT_ID, WORKFLOW_ID);

                Console.WriteLine($"Test executed successfully Modify workflow is created, modified & deleted:{WORKFLOW_NAME}  & ID: {WORKFLOW_ID}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
           
        }

        [TestMethod, TestCategory("capture"), TestCategory("Regression"), TestCategory("Release")]
        public void Modify_workFlow_PHONE_TO_MFP()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                // Code must be placed here 
                Console.WriteLine($"1.Create a Work flow");
                WORKFLOW_NAME = "Kofax_WF_";
                CommonUtil wfname = new CommonUtil();
                WORKFLOW_NAME = wfname.GetWorkFlowName(WORKFLOW_NAME);
                //source =  MFP  or PHONE   otherwise it will create a workflow with source type = phone

                WORKFLOW_ID = captureObj.CreateWorkFlow(WORKFLOW_NAME, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "PHONE");
                Debug.Assert(WORKFLOW_ID != null, $"Capture workflow is failed to create : {WORKFLOW_NAME}");

                Console.WriteLine($"2.Modify Workflow :{WORKFLOW_NAME}");
                //Data for Modification order : string workflow_name, string workflow_id, string url, string authtoken, string tenantID, string email_to, string source="PHONE", string active="true", string orientation= "PORTRAIT", string Ftype= "GENERATE_SEARCHABLE_PDF", string pdfversion= "PDF_1_6"
                string status = captureObj.ModifyWorkFlow(WORKFLOW_NAME, WORKFLOW_ID, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "MFP", "true", "PORTRAIT", "GENERATE_SEARCHABLE_PDF", "PDF_2_0");
                //string status = captureObj.CreateWorkFlow1(WORKFLOW_NAME, WORKFLOW_ID, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "PHONE");
                Debug.Assert(status == "Accepted", $"Capture workflow is failed to Modify : {WORKFLOW_NAME}");

                Console.WriteLine($"3.Delete Workflow {WORKFLOW_NAME}");
                captureObj.DeleteWorkflow(AUTH_URL, AUTH_TOKEN, TENANT_ID, WORKFLOW_ID);

                Console.WriteLine($"Test executed successfully Modify workflow is created, modified & deleted:{WORKFLOW_NAME}  & ID: {WORKFLOW_ID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
            
        }

        [TestMethod,TestCategory("capture"), TestCategory("Regression"), TestCategory("Release")]
        public void Modify_workFlow_MFP_TO_PHONE()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                // Code must be placed here 
                Console.WriteLine($"1.Create a Work flow");
                WORKFLOW_NAME = "Kofax_WF_";
                CommonUtil wfname = new CommonUtil();
                WORKFLOW_NAME = wfname.GetWorkFlowName(WORKFLOW_NAME);
                //source =  MFP  or PHONE   otherwise it will create a workflow with source type = phone

                WORKFLOW_ID = captureObj.CreateWorkFlow(WORKFLOW_NAME, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "MFP");
                Debug.Assert(WORKFLOW_ID != null, $"Capture workflow is failed to create : {WORKFLOW_NAME}");

                Console.WriteLine($"2.Modify Workflow :{WORKFLOW_NAME}");
                //Data for Modification order : string workflow_name, string workflow_id, string url, string authtoken, string tenantID, string email_to, string source="PHONE", string active="true", string orientation= "PORTRAIT", string Ftype= "GENERATE_SEARCHABLE_PDF", string pdfversion= "PDF_1_6"
                string status = captureObj.ModifyWorkFlow(WORKFLOW_NAME, WORKFLOW_ID, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "PHONE", "true", "PORTRAIT", "GENERATE_SEARCHABLE_PDF", "PDF_2_0");
                //string status = captureObj.CreateWorkFlow1(WORKFLOW_NAME, WORKFLOW_ID, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "PHONE");
                Debug.Assert(status == "Accepted", $"Capture workflow is failed to Modify : {WORKFLOW_NAME}");

                Console.WriteLine($"3.Delete Workflow {WORKFLOW_NAME}");
                captureObj.DeleteWorkflow(AUTH_URL, AUTH_TOKEN, TENANT_ID, WORKFLOW_ID);

                Console.WriteLine($"Test executed successfully Modify workflow is created, modified & deleted:{WORKFLOW_NAME}  & ID: {WORKFLOW_ID}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
            
        }

        [TestMethod, TestCategory("capture"), TestCategory("Regression"), TestCategory("Release")]
        public void Crate_Modify_Delete_Group_WorkFlow()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                // Code must be placed here 
                Console.WriteLine($"1.Create a Work flow");
                WORKFLOW_NAME = "Kofax_WF_";
                CommonUtil wfname = new CommonUtil();
                WORKFLOW_NAME = wfname.GetWorkFlowName(WORKFLOW_NAME);
                //source =  MFP  or PHONE   otherwise it will create a workflow with source type = phone

                WORKFLOW_ID = captureObj.CreateWorkFlow_Group(WORKFLOW_NAME, AUTH_URL, AUTH_TOKEN, TENANT_ID, GROUP_ID, EMAIL_TO, "MFP");
                Debug.Assert(WORKFLOW_ID != null, $"Capture workflow is failed to create : {WORKFLOW_NAME}");

                Console.WriteLine($"2.Modify Workflow :{WORKFLOW_NAME}");
                //Data for Modification order : string workflow_name, string workflow_id, string url, string authtoken, string tenantID, string email_to, string source="PHONE", string active="true", string orientation= "PORTRAIT", string Ftype= "GENERATE_SEARCHABLE_PDF", string pdfversion= "PDF_1_6"
                string status = captureObj.ModifyWorkFlow(WORKFLOW_NAME, WORKFLOW_ID, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "PHONE", "true", "PORTRAIT", "GENERATE_SEARCHABLE_PDF", "PDF_2_0");
                //string status = captureObj.CreateWorkFlow1(WORKFLOW_NAME, WORKFLOW_ID, AUTH_URL, AUTH_TOKEN, TENANT_ID, EMAIL_TO, "PHONE");
                Debug.Assert(status == "Accepted", $"Capture workflow is failed to Modify : {WORKFLOW_NAME}");

                Console.WriteLine($"3.Delete Workflow {WORKFLOW_NAME}");
                captureObj.DeleteWorkflow(AUTH_URL, AUTH_TOKEN, TENANT_ID, WORKFLOW_ID);

                Console.WriteLine($"Test executed successfully Modify workflow is created, modified & deleted:{WORKFLOW_NAME}  & ID: {WORKFLOW_ID}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
            
        }

        [TestMethod, TestCategory("Regression"), TestCategory("printer"), TestCategory("Release")]
        public void GetPrintersList()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string[] printerNames = printerObj.GetPrintersList(AUTH_URL, AUTH_TOKEN, TENANT_ID);
                Debug.Assert(printerNames.Length != 0, "Printers List should not Empty");
                Console.WriteLine($"List of Teant Printers:");
                foreach (string name in printerNames)
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

        [TestMethod, TestCategory("Regression"), TestCategory("queue"), TestCategory("Release")]
        public void GetQueuesList()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string[] queueNames = queueObj.GetQueuesList(AUTH_URL, AUTH_TOKEN, TENANT_ID);
                Debug.Assert(queueNames.Length != 0, "Queues List should not Empty");
                Console.WriteLine($"List of Teant Queues:");
                foreach (string name in queueNames)
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

        [TestMethod, TestCategory("Regression"), TestCategory("Authentication"), TestCategory("Release")]
        public void Get_JWT_Token()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string JWT = AuthObj.GetJWT_Token(TENANT, Session_Environment,AUTH_TOKEN_URL);

                Console.WriteLine($"Test executed successfully : Test09_GetJWT() : Token for Tenant Login JWT: {JWT}");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

          
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Authentication"), TestCategory("Release")]
        public void Verify_Valid_Signature_JWT()
        {
            try
            {
                // pass valid  username, password & JWT to login printix domain.  expect return is 401.
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                //string JWT = AuthObj.GetJWT_Token(TENANT, Session_Environment);
                string JWT = AuthObj.GetJWT_Token(TENANT, Session_Environment,AUTH_TOKEN_URL);
                //environment must be  devenv or testenv or prodenv or devenv.us or testenv.us
                //auto_redirect  = true or false.  default is true.
                string status = AuthObj.tenantLogin(ENV, JWT, ADMIN_USER, ADMIN_PASS, "true");
                Debug.Assert(status == "OK", "Get Request stauts must be OK, status code = 200");
                Console.WriteLine($"Test executed successfully : {testContextInstance.TestName}, Tenant Login is successful to user {ADMIN_USER}");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

        
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Authentication"), TestCategory("Release")]
        public void Verify_Expired_Signature_JWT()
        {
            try
            {
                // pass invalid  username, password & expired  JWT to login printix domain.  expect return is 401.
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string username = "Not checked anyway";
                string password = "Not checked anyway";
                //environment must be  devenv or testenv or prodenv or devenv.us or testenv.us
                //auto_redirect  = true or false.  default is true.
                string status = AuthObj.tenantLogin(Session_Environment, VALID_EXP_B_JWT, username, password, "false");
                Debug.Assert(status == "Expired"|| status == "failed", "Get Request stauts must be failed, status code = 401");
                Console.WriteLine($"Test executed successfully : {testContextInstance.TestName}, Tenant Login is failed due to expired token to user {username}");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

         
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Authentication"), TestCategory("Release")]
        public void Client_redirect_to_local()
        {
            try
            {
                Console.WriteLine($"Test Execution:{testContextInstance.TestName}");
                string redirect_uri = "https://localhost:12345/authorization-response";
                string status = AuthObj.redirect_url(TENANT, redirect_uri, Session_Environment,RESPONSE_TYPE,CLIENT_ID,CLIENT_SECRET);
                Debug.Assert(status == "Found", "Get Request stauts must be Found, status code = 302");
                Console.WriteLine($"Test executed successfully : {testContextInstance.TestName}  to tenant {TENANT}");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

     
        }

        
        [TestMethod, TestCategory("Regression"), TestCategory("Authentication"), TestCategory("Release")]
        public void Client_redirect_to_Printix()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string status = AuthObj.redirect_url(TENANT, REDIRECT_URI, Session_Environment, RESPONSE_TYPE, CLIENT_ID, CLIENT_SECRET);
                Debug.Assert(status == "Found", "Get Request stauts must be Found, status code = 302");
                Console.WriteLine($"Test is passed with status code 302: {testContextInstance.TestName}  to tenant {TENANT}");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }


        }

        [TestMethod, TestCategory("Regression"), TestCategory("Authentication"), TestCategory("Release")]
        public void Client_redirect_to_Missing_Tenant()
        {
            try
            {
                // pass tenant_name, redirect_uri, environment
                //environment must be  devenv or testenv or prod or devenv.us or testenv.us
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string redirect_uri = "https://final-redirect." + Session_Environment + ".printix.net";
                string status = AuthObj.redirect_url_missing_tenant(redirect_uri, Session_Environment);
                Debug.Assert(status == "BAD URI"|| status == "Failed", "Get Request stauts must be Not Found, status code = 404");
                Console.WriteLine($"Test is passed with expected status code 404: {testContextInstance.TestName} - redirect uri {redirect_uri}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }
        
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Authentication"), TestCategory("Release")]
        public void Get_OneTimeCode_ExpiredToken()
        {
            try
            {
                // pass tenant_name, redirect_uri, environment & Expired JWT.
                //environment must be  devenv or testenv or prod or devenv.us or testenv.us
                // true means verification of Header reponse for token
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string redirect_uri = "https://final-redirect.printix.net";
                //string status = AuthObj.redirect_url(TENANT, redirect_uri, Session_Environment, VALID_EXP_B_JWT, "true");
                string status = AuthObj.redirect_url(TENANT, redirect_uri, Session_Environment, RESPONSE_TYPE, CLIENT_ID, CLIENT_SECRET, VALID_EXP_B_JWT, "true");
                Debug.Assert(status == "Failed", "Get Request stauts must be Found, status code = 302");
                Console.WriteLine($"Test is passed  with expected status code 302:: {testContextInstance.TestName} - redirect uri {redirect_uri}");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

           
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Authentication"), TestCategory("Release")]
        public void Get_OneTimeCode_InvalidToken()
        {
            try
            {
                //pass tenant_name, redirect_uri, environment & Invalid JWT.
                //environment must be  devenv or testenv or prod or devenv.us or testenv.us
                // true means verification of Header reponse for token 
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string redirect_uri = "https://final-redirect.printix.net";
                //string status = AuthObj.redirect_url(TENANT, redirect_uri, Session_Environment, INVALID_B_JWT, "true");
                string status = AuthObj.redirect_url(TENANT, redirect_uri, Session_Environment, RESPONSE_TYPE, CLIENT_ID, CLIENT_SECRET, INVALID_B_JWT, "true");
                Debug.Assert(status == "Failed"||status == "Unauthorised", "Get Request stauts must be Found, status code = 401");
                Console.WriteLine($"Test is passed  with expected status code 401:: {testContextInstance.TestName} - redirect uri {redirect_uri}");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

         
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Authentication"), TestCategory("Release")]
        public void Get_Signin_No_Context()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string status = AuthObj.signin_context(Session_Environment);
                Debug.Assert(status == "Unauthorised" || status == "failed", "Get Request stauts must be Unauthorised, status code = 401");
                Console.WriteLine($"Test is passed  with expected status code 401:: {testContextInstance.TestName} - environment {Session_Environment}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

            

        }

      
        // Authentication for Admin page with auth token and without token.

        [TestMethod, TestCategory("Regression"), TestCategory("Authentication"), TestCategory("Release")]
        public void Tenant_Info_With_Auth_Token()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string[] response = AuthObj.Get_Tenant_Info(Session_Environment, TENANT, AUTH_TOKEN);
                Debug.Assert(response[0] != "*", $"Tenant createdByName is hidden, it must be  {response[0]}, status code = 200");
                Debug.Assert(response[1] != "*", $"Tenant email is hidden, it must be{response[1]}, status code = 200");
                Console.WriteLine($"Test is passed  with expected status code 200: {testContextInstance.TestName} - createdByName {response[0]}, Tenant email: {response[1]}");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

        }

        [TestMethod, TestCategory("Regression"), TestCategory("Authentication")]
        public void Tenant_Info_WithOut_Auth_Token()
        {
            try
            {
                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
                string tempTenant = TEMP_TENANT;
                Console.WriteLine($"temp tenant for this case : {tempTenant}");
                Assert.IsNotNull(AuthObj, "AuthObj should not be null.");
                string[] response = AuthObj.Get_Tenant_Info(Session_Environment, tempTenant);
                Debug.Assert(!String.IsNullOrEmpty(response[0]), "Tenant Name field is missing from response");
                Debug.Assert(!String.IsNullOrEmpty(response[1]), "hosting domains  is missing from response");
               
                Console.WriteLine($"Test is passed  with expected status code 200: {testContextInstance.TestName}  createdByName: {response[0]}, Tenant email: {response[1]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

            
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Authentication"), TestCategory("Release")]
        public async Task createDummyPrinterResouceWithOutSerialNumberTest()
        {
            try
            {
                var printerservice = new Printers();
                var networkservice = new Networks();
                string model = "HP";
                dummyPrinterName = CommonUtil.GeneratePrinterName(model);
                string snNumber = "";
                string ip = networkservice.GenerateRandomIpAddress();
                string mac = networkservice.GenerateRandomMacAddress();
                MockResource mock_Printer = new MockResource();
                var responseContent = await mock_Printer.CreateMockPrinter(AUTH_URL, AUTH_TOKEN, TENANT_ID, IN_NETWORK_ID, model, dummyPrinterName,
                       "2024", snNumber, ip, mac);
                var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);
                var printer = responseJson.newPrinters[0];
                string printerId = printer.id;
                // Assert that printerId and signId are not null or empty
                Assert.IsNotNull(printerId, "Printer ID should not be null.");

                // Verify serial number 
                var printerContent = await printerservice.getPrinter(AUTH_URL, AUTH_TOKEN, TENANT_ID, printerId);

                // Deserialize the JSON response
                var printerResp = JsonConvert.DeserializeObject<dynamic>(printerContent);

                // Access the serial number from the deserialized response
                string serialNumber = printerResp.serialNumber.ToString();

                // Assert that the serial number is present and matches the expected value
                Assert.AreEqual("", serialNumber, "Serial number should be an empty string.");

                Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Assert.Fail("Test Failed:", testContextInstance.TestName);

            }

            
        }

        [TestMethod, TestCategory("Regression"), TestCategory("Printer")]
        public async Task createDummyPrinterResouceWithSerialNumberTest()
        {
            var printerservice = new Printers();
            var networkservice = new Networks();
            string model = "HP";
            dummyPrinterName = CommonUtil.GeneratePrinterName(model);
            string  snNumber = model + CommonUtil.randomNumber();
            string ip = networkservice.GenerateRandomIpAddress();
            string mac = networkservice.GenerateRandomMacAddress();
            MockResource mock_Printer = new MockResource();
            var responseContent = await mock_Printer.CreateMockPrinter(AUTH_URL, AUTH_TOKEN, TENANT_ID, IN_NETWORK_ID, model, dummyPrinterName,
                   "2024", snNumber, ip, mac);
            var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);
            var printer = responseJson.newPrinters[0];
            string printerId = printer.id;
            // Assert that printerId and signId are not null or empty
            Assert.IsNotNull(printerId, "Printer ID should not be null.");

            // Verify serial number 
            var printerContent = await printerservice.getPrinter(AUTH_URL, AUTH_TOKEN, TENANT_ID, printerId);

            // Deserialize the JSON response
            var printerResp = JsonConvert.DeserializeObject<dynamic>(printerContent);

            // Access the serial number from the deserialized response
            string serialNumber = printerResp.serialNumber.ToString();

            // Assert that the serial number is present and matches the expected value
            Assert.IsNotNull(serialNumber, "Serial number should not be null.");
            Assert.AreEqual(snNumber, serialNumber, "Serial number does not match the expected value.");

            Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");

        }

        [TestMethod, TestCategory("Regression"), TestCategory("workstation")]
        public async Task testCreateWS()
        {
            // Create workstation  
            Console.WriteLine($"Test Execution Environment {Session_Environment} & Test Case :{testContextInstance.TestName}");
            //Prepate ws body 
            CreateWsRequestDto requestBody = wsObj.createWsBody();
            WsResponseDto newWs = await wsObj.CreateWS(AUTH_URL, AUTH_TOKEN, TENANT_ID, requestBody);
            Debug.Assert(newWs.Name != null, $"workstation name should not Empty");

        }

        [TestCleanup()]
        public void Teardown()
        {
            // End of each test case 
        }

    }
}
