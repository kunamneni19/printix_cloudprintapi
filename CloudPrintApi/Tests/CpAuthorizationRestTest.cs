using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudPrintAPI.DTO;
using CloudPrintAPI.Utils;
using CommonUtils;

namespace CloudPrintAPI
{
    /// <summary>
    /// Contains integration tests for the CloudPrint API authentication functionalities.
    /// This class uses the updated RestSharp-based utility wrappers for authentication.
    /// </summary>
    [TestClass]
    public class CpAuthorizationRestTest
    {
        private static string? authUrl;
        private static string? authTokenUrl;
        private static string cpAuthTokenUrl = string.Empty;
        private static string cpClientId = string.Empty;
        private static string cpClientSecret = string.Empty;
        private static string? adminUser;
        private static string? adminPass;
        private static string? grantType;
        private static string? responseType;
        private static string? redirectUri;
        private static string? tenant;
        private static string environment = string.Empty;
        private static string? accessToken;
        private static string? refreshToken;

        // Using your newly provided RestSharp authorization utility class
        private static CPAuthorizationRest authObj = null!;

        private TestContext? testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance!; }
            set { testContextInstance = value; }
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            string binPath = AppDomain.CurrentDomain.BaseDirectory;
            string repoRoot = Path.GetFullPath(Path.Combine(binPath, @"..\..\..\"));
            string dataPath = Path.Combine(repoRoot, "APITest.Utils", "Data");

            environment = Environment.GetEnvironmentVariable("environment") ?? "dev01";
            CommonUtil.ValidateEnvironmentVariable(environment);
            string configPath = CommonUtil.env_setup(environment, dataPath);
            var testdata = CommonUtil.ReadAndValidateJsonData<TestData>($"{configPath}");

            authUrl = testdata.admin.auth_url;
            authTokenUrl = testdata.admin.auth_token_url;
            cpAuthTokenUrl = testdata.admin.cp_auth_token_url;
            cpClientId = testdata.admin.cp_client_id;
            cpClientSecret = testdata.admin.cp_client_secret;
            adminUser = testdata.admin.admin_username;
            adminPass = testdata.admin.admin_pass;
            grantType = testdata.admin.grant_type;
            responseType = testdata.admin.response_type;
            redirectUri = testdata.admin.redirect_uri;
            tenant = testdata.admin.tenant;

            // Instantiate the RestSharp utility instance
            authObj = new CPAuthorizationRest();

            // Resolve the async initialization call synchronously for ClassInitialize
            var initialAuthResponse = GetAccessTokenWithStatusAsync().GetAwaiter().GetResult();
            if (initialAuthResponse.IsSuccessStatusCode)
            {
                accessToken = initialAuthResponse.AccessToken;
                refreshToken = initialAuthResponse.RefreshToken;
            }
            else
            {
                context.WriteLine($"ClassInitialize failed to get initial access token: {initialAuthResponse.StatusCode} - {initialAuthResponse.ResponseContent}");
                throw new Exception("Failed to initialize access and refresh tokens for test class.");
            }
        }

        [TestMethod, TestCategory("Authentication"), TestCategory("Regression"), TestCategory("Release"), TestCategory("Contract")]
        public async Task TestGetAccessToken_WithContractValidation_ShouldSucceed()
        {
            TestContext.WriteLine($"Test Execution: {TestContext.TestName}");
            CloudPrintAuthTokenResponse? authResponse = null;

            try
            {
                authResponse = await authObj.GetCloudPrintAuthTokenFullResponseAsync(cpAuthTokenUrl, cpClientId, cpClientSecret);
            }
            catch (Exception ex)
            {
                Assert.Fail($"API call failed unexpectedly: {ex.Message}");
            }

            Assert.IsNotNull(authResponse, "Authentication response DTO should not be null after successful call.");
            Assert.AreEqual(HttpStatusCode.OK, authResponse.StatusCode,
                $"Expected status code OK, but got {authResponse.StatusCode}. Response: {authResponse.ResponseContent}");

            TestContext.WriteLine($"Full Response Content (for debug): {authResponse.ResponseContent}");
        }

        [TestMethod, TestCategory("Authentication"), TestCategory("Regression"), TestCategory("Release"), TestCategory("Contract")]
        public async Task GetAccessToken_InvalidClient_ErrorResponseConformsToContract()
        {
            TestContext.WriteLine($"Test Execution: {TestContext.TestName}");
            CloudPrintAuthTokenResponse? authResponse = null;

            try
            {
                authResponse = await authObj.GetCloudPrintAuthTokenFullResponseAsync(cpAuthTokenUrl, "invalid_client", "invalid_secret");
            }
            catch (Exception ex)
            {
                Assert.Fail($"API call failed unexpectedly: {ex.Message}");
            }

            Assert.IsNotNull(authResponse, "Authentication response DTO should not be null for an error scenario.");
            Assert.AreEqual(HttpStatusCode.Unauthorized, authResponse.StatusCode,
                $"Expected status code Unauthorized (401) for invalid credentials, but got {authResponse.StatusCode}. Response: {authResponse.ResponseContent}");
            Assert.IsFalse(authResponse.IsSuccessStatusCode, "Expected non-successful HTTP status code.");
            Assert.IsNull(authResponse.AccessToken, "Access token should be null for an error response.");
            Assert.IsNull(authResponse.RefreshToken, "Refresh token should be null for an error response.");
            Assert.IsNull(authResponse.ExpiresIn, "Expires in should be null for an error response.");
            Assert.IsNull(authResponse.TokenType, "Token type should be null for an error response.");

            TestContext.WriteLine($"Test completed successfully. Status Code: {authResponse.StatusCode}, Response Content: {authResponse.ResponseContent}");
        }

        [TestMethod, TestCategory("Authentication"), TestCategory("Regression"), TestCategory("Release"), TestCategory("Contract")]
        [Ignore("Ignored because the test returns invalid client instead of secret")]
        public async Task GetAccessToken_InvalidSecret_ErrorResponseConformsToContract()
        {
            TestContext.WriteLine($"Test Execution: {TestContext.TestName}");
            CloudPrintAuthTokenResponse? authResponse = null;

            try
            {
                authResponse = await authObj.GetCloudPrintAuthTokenFullResponseAsync(cpAuthTokenUrl, cpClientId, "invalid_secret");
            }
            catch (Exception ex)
            {
                Assert.Fail($"API call failed unexpectedly: {ex.Message}");
            }

            Assert.IsNotNull(authResponse, "Authentication response DTO should not be null for an error scenario.");
            Assert.AreEqual(HttpStatusCode.Unauthorized, authResponse.StatusCode,
                $"Expected status code Unauthorized (401) for invalid credentials, but got {authResponse.StatusCode}. Response: {authResponse.ResponseContent}");
            Assert.IsFalse(authResponse.IsSuccessStatusCode, "Expected non-successful HTTP status code.");
            Assert.IsTrue(authResponse.ResponseContent.Contains("\"error\":\"invalid_secret\""),
                $"invalid error response:{authResponse.ResponseContent}, expected: invalid_secret");

            Assert.IsNull(authResponse.AccessToken, "Access token should be null for an error response.");
            Assert.IsNull(authResponse.RefreshToken, "Refresh token should be null for an error response.");
            Assert.IsNull(authResponse.ExpiresIn, "Expires in should be null for an error response.");
            Assert.IsNull(authResponse.TokenType, "Token type should be null for an error response.");

            TestContext.WriteLine($"Test completed successfully. Status Code: {authResponse.StatusCode}, Response Content: {authResponse.ResponseContent}");
        }

        [TestMethod, TestCategory("Authentication"), TestCategory("Regression"), TestCategory("Release")]
        public async Task AccessTokenAndRefreshTokenTest()
        {
            TestContext.WriteLine($"Test Execution: {TestContext.TestName}");

            CloudPrintAuthTokenResponse authResponse = await authObj.GetCloudPrintAuthTokenFullResponseAsync(cpAuthTokenUrl, cpClientId, cpClientSecret);

            Assert.AreEqual(HttpStatusCode.OK, authResponse.StatusCode, $"Expected status code OK for access token, but got {authResponse.StatusCode}. Response: {authResponse.ResponseContent}");
            Assert.IsTrue(authResponse.IsSuccessStatusCode, "Expected successful HTTP status code (2xx).");
            Assert.IsFalse(string.IsNullOrEmpty(authResponse.AccessToken), "Access token should not be null or empty.");
            Assert.IsFalse(string.IsNullOrEmpty(authResponse.RefreshToken), "Refresh token should not be null or empty.");
            Assert.IsTrue(authResponse.ExpiresIn.HasValue && authResponse.ExpiresIn.Value <= 3659, $"Expires in should be a positive integer less than or equal to 3659, but got {authResponse.ExpiresIn}.");

            TestContext.WriteLine($"Access Token: {authResponse.AccessToken?.Substring(0, 10)}..., Refresh Token: {authResponse.RefreshToken?.Substring(0, 10)}..., Expires In: {authResponse.ExpiresIn}, Status Code: {authResponse.StatusCode}");
            TestContext.WriteLine($"Full Response Content (if needed for debug): {authResponse.ResponseContent}");
        }

        [TestMethod, TestCategory("Authentication"), TestCategory("Regression"), TestCategory("Release")]
        public async Task AccessTokenWithInvalidCredentialsTest()
        {
            TestContext.WriteLine($"Test Execution: {TestContext.TestName}");

            CloudPrintAuthTokenResponse authResponse = await authObj.GetCloudPrintAuthTokenFullResponseAsync(cpAuthTokenUrl, "invalid_client", "invalid_secret");

            Assert.AreEqual(HttpStatusCode.Unauthorized, authResponse.StatusCode, $"Expected status code Unauthorized (401) for invalid credentials, but got {authResponse.StatusCode}. Response: {authResponse.ResponseContent}");
            Assert.IsFalse(authResponse.IsSuccessStatusCode, "Expected non-successful HTTP status code.");
            Assert.IsNull(authResponse.AccessToken, "Access token should be null for invalid credentials.");
            Assert.IsNull(authResponse.RefreshToken, "Refresh token should be null for invalid credentials.");
            Assert.IsNull(authResponse.ExpiresIn, "Expires in should be null for invalid credentials.");
            Assert.IsFalse(string.IsNullOrEmpty(authResponse.ResponseContent), "Error response content should not be empty.");

            TestContext.WriteLine($"Status Code: {authResponse.StatusCode}, Response Content: {authResponse.ResponseContent}");
        }

        [TestMethod, TestCategory("Authentication"), TestCategory("Regression"), TestCategory("Release")]
        public async Task RefreshAccessTokenSuccessfullyTest()
        {
            TestContext.WriteLine($"Test Execution: {TestContext.TestName}");

            Assert.IsFalse(string.IsNullOrEmpty(refreshToken), "Initial refresh token must be obtained before testing refresh.");

            RefreshTokenResponse refreshResponse = await authObj.RefreshTokenAsync(authTokenUrl, environment, cpClientId, refreshToken);

            Assert.AreEqual(HttpStatusCode.OK, refreshResponse.StatusCode, $"Expected status code OK for token refresh, but got {refreshResponse.StatusCode}. Response: {refreshResponse.ResponseBody}");
            Assert.IsTrue(refreshResponse.IsSuccessStatusCode, "Expected successful HTTP status code (200) for token refresh.");
            Assert.IsFalse(string.IsNullOrEmpty(refreshResponse.AccessToken), "New access token should not be null or empty after refresh.");
            Assert.IsFalse(string.IsNullOrEmpty(refreshResponse.NewRefreshToken), "New refresh token should not be null or empty after refresh.");
            Assert.AreNotEqual(accessToken, refreshResponse.AccessToken, "Access token should be different after refresh.");
            Assert.IsTrue(refreshResponse.ExpiresIn.HasValue && refreshResponse.ExpiresIn.Value <= 3659, $"Expires in should be a positive integer less than or equal to 3659, but got {refreshResponse.ExpiresIn}.");

            TestContext.WriteLine($"New Access Token: {refreshResponse.AccessToken?.Substring(0, 10)}..., New Refresh Token: {refreshResponse.NewRefreshToken?.Substring(0, 10)}..., Expires In: {refreshResponse.ExpiresIn}, Status Code: {refreshResponse.StatusCode}");
            TestContext.WriteLine($"Full Response Body (if needed for debug): {refreshResponse.ResponseBody}");

            accessToken = refreshResponse.AccessToken;
            refreshToken = refreshResponse.NewRefreshToken;
        }

        [TestMethod, TestCategory("Authentication"), TestCategory("Regression"), TestCategory("Release")]
        public async Task RefreshAccessTokenWithInvalidRefreshTokenTest()
        {
            TestContext.WriteLine($"Test Execution: {TestContext.TestName}");
            string invalidRefreshToken = "invalid_refresh_token_12345";

            RefreshTokenResponse refreshResponse = await authObj.RefreshTokenAsync(authTokenUrl!, environment, cpClientId, invalidRefreshToken);

            Assert.AreEqual(HttpStatusCode.BadRequest, refreshResponse.StatusCode, $"Expected status code BAD request (400) for invalid refresh token, but got {refreshResponse.StatusCode}. Response: {refreshResponse.ResponseBody}");
            Assert.IsFalse(refreshResponse.IsSuccessStatusCode, "Expected non-successful HTTP status code for invalid refresh token.");
            Assert.IsNull(refreshResponse.AccessToken, "Access token should be null for an invalid refresh token.");
            Assert.IsNull(refreshResponse.NewRefreshToken, "New refresh token should be null for an invalid refresh token.");
            Assert.IsNull(refreshResponse.ExpiresIn, "Expires in should be null for an invalid refresh token.");
            Assert.IsFalse(string.IsNullOrEmpty(refreshResponse.ResponseBody), "Error response body should not be empty.");
            StringAssert.Contains(refreshResponse.ResponseBody, "HTTP error", "HTTP error: Request failed with status code BadRequest.");

            TestContext.WriteLine($"Status Code: {refreshResponse.StatusCode}, Response Body: {refreshResponse.ResponseBody}");
        }

        [TestMethod, TestCategory("Authentication"), TestCategory("Negative"), TestCategory("Release")]
        [DataRow("non_existent_env", HttpStatusCode.BadRequest, "Wrong environment specified.", "Expected 'Wrong environment specified.' error for bad environment.")]
        [DataRow("", HttpStatusCode.BadRequest, "Wrong environment specified.", "Expected 'Wrong environment specified.' error for empty environment.")]
        [DataRow(null, HttpStatusCode.BadRequest, "Wrong environment specified.", "Expected 'Wrong environment specified.' error for null environment.")]
        public async Task RefreshAccessTokenWithBadEnvironmentParameterizedTest(string? badEnv, HttpStatusCode expectedStatusCode, string expectedErrorContent, string message)
        {
            TestContext.WriteLine($"Test Execution: {TestContext.TestName} - Bad Environment: '{badEnv}'");

            RefreshTokenResponse refreshResponse = await authObj.RefreshTokenAsync(authTokenUrl!, badEnv!, cpClientId, refreshToken!);

            Assert.AreEqual(expectedStatusCode, refreshResponse.StatusCode, $"{message}, but got {refreshResponse.StatusCode}. Response: {refreshResponse.ResponseBody}");
            Assert.IsFalse(refreshResponse.IsSuccessStatusCode, "Expected non-successful HTTP status code for bad environment.");
            Assert.IsNull(refreshResponse.AccessToken, "Access token should be null for bad environment.");
            Assert.IsNull(refreshResponse.NewRefreshToken, "New refresh token should be null for bad environment.");
            Assert.IsNull(refreshResponse.ExpiresIn, "Expires in should be null for bad environment.");
            Assert.IsFalse(string.IsNullOrEmpty(refreshResponse.ResponseBody), "Response body should not be empty for bad environment.");
            StringAssert.Contains(refreshResponse.ResponseBody, expectedErrorContent, $"Response body should indicate '{expectedErrorContent}'.");

            TestContext.WriteLine($"Status Code: {refreshResponse.StatusCode}, Response Body: {refreshResponse.ResponseBody}");
        }

        [TestCleanup]
        public void Teardown()
        {
            // Per-test teardown logic
        }

        private static async Task<CloudPrintAuthTokenResponse> GetAccessTokenWithStatusAsync()
        {
            return await authObj.GetCloudPrintAuthTokenFullResponseAsync(cpAuthTokenUrl, cpClientId, cpClientSecret);
        }
    }
}