using CloudprintAPI;
using CloudPrintAPI.Contracts;  
using CloudPrintAPI.Utils;      
using CommonUtils;              

namespace CloudPrintAPI.Tests
{
    [TestClass]
    public class SnmpTest
    {
        // Static fields for resources initialized once per test class
        private static SnmpV1 _snmpClient;
        private static AuthenticationClient _authClient;
        private static string _accessToken;

        private static string _baseApiUrl;
        private static string _authUrl;
        private static string _testTenantId;
        private static string _testClientId;
        private static string _testClientSecret;
        public static string _networkID = String.Empty;
        public static string _invalidNetworkID = "b23700bb-fb04-4bd0-bcbc-5c29d4c97564";


        private static string _manualSnmpGetCommunityName = "Green community - get";
        private static string _manualSnmpSetCommunityName = "Green community - set";
        private static SnmpV1SecurityLevel _manualSnmpSecurityLevel = SnmpV1SecurityLevel.AUTH_PRIVACY; 
        private static SnmpVersion _manualSnmpVersion = SnmpVersion.V1;
        private static SnmpV1Authentication _manualSnmpAuthentication = SnmpV1Authentication.SHA;   
        private static SnmpV1Privacy _manualSnmpPrivacy = SnmpV1Privacy.AES;
        private static string _manualSnmpUsername = "testuser_snmp";
        private static string _manualSnmpAuthenticationKey = "testAuthKey123";
        private static string _manualSnmpPrivacyKey = "testPrivKey123";
        private static string? _manualSnmpContextName = null;
        private static List<string> _manualSnmpNetworkIds;
        private static List<string> _manualSnmpInvalidNetworkIds;

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [ClassInitialize] 
        public static async Task ClassInitialize(TestContext context)
        {
            context.WriteLine("--- SnmpTest ClassInitialize: Setting up test environment ---");
            string binPath = AppDomain.CurrentDomain.BaseDirectory;
            string repoRoot = Path.GetFullPath(Path.Combine(binPath, @"..\..\..\"));
            string dataPath = Path.Combine(repoRoot, "APITest.Utils", "Data");
            //string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string environment = Environment.GetEnvironmentVariable("environment");
            CommonUtil.ValidateEnvironmentVariable(environment); 
            string configPath = CommonUtil.env_setup(environment, dataPath); 

            var testdata = CommonUtil.ReadAndValidateJsonData<TestData>($"{configPath}");

            _baseApiUrl = testdata.admin.cp_url;
            _authUrl = testdata.admin.cp_auth_token_url;
            _testTenantId = testdata.admin.tenant_id;
            _testClientId = testdata.admin.cp_client_id;
            _testClientSecret = testdata.admin.cp_client_secret;
            _networkID = testdata.admin.network_id;
            _authClient = new AuthenticationClient();
            _snmpClient = new SnmpV1();
            _manualSnmpNetworkIds = new List<string> { _networkID, _networkID };
            _manualSnmpInvalidNetworkIds = new List<string> { _invalidNetworkID, _invalidNetworkID };
            var authResponse = await _authClient.GetAuthenticationTokenAsync(
                _authUrl,
                _testClientId,
                _testClientSecret
            );
            _accessToken = authResponse.AccessToken;
            Assert.IsFalse(string.IsNullOrEmpty(_accessToken), "ClassInitialize failed: Access token is null or empty.");
        }

        [TestMethod]
        [TestCategory("SNMP_V1_API")]  [TestCategory("Contract")]
        public async Task testCreateSnmpV1Configuration_ValidRequest()
        {
            TestContext.WriteLine($"Test Execution: {TestContext.TestName}");
            var requestBody = GetValidSnmpV1Request();

            // Construct the full URL for the SNMP V1 endpoint
            var fullUrl = $"{_baseApiUrl}"+ "tenants/" + $"{_testTenantId}/snmp";

            SnmpV1ConfigurationResponse response = null;
            try
            {
                response = await _snmpClient.CreateSnmpV1ConfigurationAsync(fullUrl, _accessToken, requestBody);
            }
            catch (HttpRequestException httpEx)
            {
                TestContext.WriteLine($"HttpRequestException caught: {httpEx.StatusCode} - {httpEx.Message}");
                Assert.Fail($"API call failed unexpectedly with HTTP status code {httpEx.StatusCode}. Details: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"An unexpected exception occurred: {ex.GetType().Name} - {ex.Message}");
                Assert.Fail($"An unexpected error occurred during API call or response validation: {ex.Message}");
            }

            // Assert
            Assert.IsNotNull(response, "The response object should not be null after a successful API call.");

            // Assert on values returned to ensure they match the request and contract
            Assert.AreEqual(requestBody.Name, response.Name, $"Response Name '{response.Name}' should match request '{requestBody.Name}'.");
            Assert.AreEqual(requestBody.GetCommunityName, response.GetCommunityName, "GetCommunityName should match the request.");
            Assert.AreEqual(requestBody.SetCommunityName, response.SetCommunityName, "SetCommunityName should match the request.");
            Assert.AreEqual(requestBody.TenantDefault, response.TenantDefault, "TenantDefault should match the request.");
            Assert.AreEqual(requestBody.SecurityLevel, response.SecurityLevel, "SecurityLevel should match the request.");
            Assert.AreEqual(requestBody.Version, response.Version, "Version should match the request.");
            Assert.AreEqual(requestBody.Authentication, response.Authentication, "Authentication should match the request.");
            Assert.AreEqual(requestBody.Privacy, response.Privacy, "Privacy should match the request.");

            // Assert on _links object properties
            Assert.IsNotNull(response.Links, "_links object should not be null.");
            Assert.IsNotNull(response.Links.Self, "_links.self should not be null.");
            Assert.IsFalse(string.IsNullOrEmpty(response.Links.Self.Href), "_links.self.href should not be null or empty.");
            Assert.IsTrue(response.Links.Self.Href.Contains(_testTenantId), $"_links.self.href should contain the tenant ID: {response.Links.Self.Href}");
            Assert.IsTrue(response.Links.Self.Href.Contains("snmp"), $"_links.self.href should contain 'snmp' path: {response.Links.Self.Href}");
            Assert.IsNotNull(response.Links.Networks, "_links.networks should not be null.");

            TestContext.WriteLine($"Successfully created SNMP V1 config '{response.Name}'. Self Link: {response.Links.Self?.Href}");
            TestContext.WriteLine($"Test '{TestContext.TestName}' completed successfully.");
        }

        [TestMethod]
        [TestCategory("SNMP_V1_API")]
        [TestCategory("Contract")]
        public async Task CreateSnmpV1Configuration_InvalidNetworkId_ShouldReturnError()
        {
            TestContext.WriteLine($"Test Execution: {TestContext.TestName}");

            // Use the dedicated invalid request helper
            var requestBody = GetInValidSnmpV1Request();

            var fullUrl = $"{_baseApiUrl}tenants/{_testTenantId}/snmp";

            TestContext.WriteLine($"Attempting to create SNMP V1 config with invalid network ID: '{_invalidNetworkID}' at URL: '{fullUrl}'");
            TestContext.WriteLine($"Request Body: {System.Text.Json.JsonSerializer.Serialize(requestBody, JsonOptions.GetDefaultApiOptions())}");

            try
            {
                // This call is expected to throw an HttpRequestException for a non-success status code
                await _snmpClient.CreateSnmpV1ConfigurationAsync(fullUrl, _accessToken, requestBody);

                // If the above line *doesn't* throw, it's a failure (expected to fail)
                Assert.Fail("API call with invalid network ID did not return an expected error (HttpRequestException was not thrown).");
               
            }
            catch (HttpRequestException httpEx)
            {
                TestContext.WriteLine($"HttpRequestException caught: {httpEx.StatusCode} - {httpEx.Message}");
                // Extract the full JSON response content from the exception message
                string fullResponseMessage = httpEx.Message;
                const string jsonPrefix = "Full response JSON: ";
                int jsonStartIndex = fullResponseMessage.IndexOf(jsonPrefix);

                if (jsonStartIndex == -1)
                {
                    Assert.Fail($"HttpRequestException message did not contain expected JSON prefix: '{jsonPrefix}'. Message: {fullResponseMessage}");
                }

                string errorJsonContent = fullResponseMessage.Substring(jsonStartIndex + jsonPrefix.Length);
                TestContext.WriteLine($"Extracted Error JSON Content: {errorJsonContent}");

                // Assert on the expected HTTP status code
                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, httpEx.StatusCode,
                                $"Expected HTTP status code {System.Net.HttpStatusCode.NotFound} for invalid network ID, but got {httpEx.StatusCode}.");

                // Deserialize the error response content
                SnmpApiErrorResponse errorResponse = null;
                try
                {
                    errorResponse = System.Text.Json.JsonSerializer.Deserialize<SnmpApiErrorResponse>(errorJsonContent, JsonOptions.GetDefaultApiOptions());
                    Assert.IsNotNull(errorResponse, "Deserialized error response object should not be null.");
                }
                catch (System.Text.Json.JsonException ex)
                {
                    Assert.Fail($"Failed to deserialize error response JSON: {ex.Message}. Raw JSON: {errorJsonContent}");
                }

                // Assert on the properties of the deserialized errorResponse
                Assert.IsFalse(errorResponse.Success, "Error response 'success' field should be false.");
                Assert.AreEqual("NETWORKS_NOT_FOUND", errorResponse.ErrorCode, "Expected 'NETWORKS_NOT_FOUND' error code from API response.");
                Assert.IsTrue(errorResponse.ErrorText?.Contains(_invalidNetworkID) ?? false, $"Error text should contain the invalid network ID: {_invalidNetworkID}");
                Assert.IsNotNull(errorResponse.Parameters, "Error response 'parameters' list should not be null.");
                Assert.IsTrue(errorResponse.Parameters.Contains($"[{_invalidNetworkID}]"), $"Error parameters list should contain the invalid network ID string representation: [{_invalidNetworkID}]");

            }

            catch (Exception ex)
            {
                TestContext.WriteLine($"An unexpected exception occurred: {ex.GetType().Name} - {ex.Message}");
                Assert.Fail($"An unexpected error occurred during API call: {ex.Message}");
            }
        }

        private static SnmpV1ConfigurationRequest GetValidSnmpV1Request()
        {
            return new SnmpV1ConfigurationRequest
            {
                Name = $"ManualSnmpConfig_{Guid.NewGuid().ToString().Substring(0, 8)}", // Generate a unique name
                GetCommunityName = _manualSnmpGetCommunityName,
                SetCommunityName = _manualSnmpSetCommunityName,
                TenantDefault = false,
                SecurityLevel = _manualSnmpSecurityLevel,
                Version = _manualSnmpVersion,
                Username = _manualSnmpUsername, 
                ContextName = _manualSnmpContextName,
                Authentication = _manualSnmpAuthentication,
                AuthenticationKey = _manualSnmpAuthenticationKey, 
                Privacy = _manualSnmpPrivacy,
                PrivacyKey = _manualSnmpPrivacyKey, 
                NetworkIds = _manualSnmpNetworkIds 
            };
        }


        private static SnmpV1ConfigurationRequest GetInValidSnmpV1Request()
        {
            return new SnmpV1ConfigurationRequest
            {
                Name = $"ManualSnmpConfig_{Guid.NewGuid().ToString().Substring(0, 8)}", 
                GetCommunityName = _manualSnmpGetCommunityName,
                SetCommunityName = _manualSnmpSetCommunityName,
                TenantDefault = false,
                SecurityLevel = _manualSnmpSecurityLevel,
                Version = _manualSnmpVersion,
                Username = _manualSnmpUsername,
                ContextName = _manualSnmpContextName,
                Authentication = _manualSnmpAuthentication,
                AuthenticationKey = _manualSnmpAuthenticationKey,
                Privacy = _manualSnmpPrivacy,
                PrivacyKey = _manualSnmpPrivacyKey,
                NetworkIds = _manualSnmpInvalidNetworkIds
            };
        }



        // --- Class Cleanup ---
        [ClassCleanup] // Runs once after all tests in the class have completed
        public static void ClassCleanup()
        {            
        }
    }

}