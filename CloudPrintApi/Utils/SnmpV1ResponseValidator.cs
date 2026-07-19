using System;
using System.Collections.Generic;
using System.Text.Json;
using CloudPrintAPI.Contracts; 
using System.Text.Json.Serialization;

namespace CloudPrintAPI.Utils
{
    /// <summary>
    /// Provides methods for validating the contract of SnmpV1ConfigurationResponse JSON.
    /// This class is static as its methods operate on input parameters and do not require instance state.
    /// </summary>
    internal static class SnmpV1ResponseValidator
    {
        /// <summary>
        /// Validates a raw JSON string against the SnmpV1ConfigurationResponse contract.
        /// Throws JsonException or a more generic Exception if validation fails.
        /// </summary>
        /// <param name="responseContent">The raw JSON string received from the API response.</param>
        /// <returns>The deserialized SnmpV1ConfigurationResponse object if validation succeeds.</returns>
        /// <exception cref="JsonException">Thrown if deserialization fails or if contract violations are found
        /// <exception cref="Exception">Thrown for other unexpected errors during validation.</exception>
        public static SnmpV1ConfigurationResponse ValidateAndDeserialize(string responseContent)
        {
            // Use the consistent JsonSerializerOptions from JsonOptionsHelper
            var jsonSerializerOptions = JsonOptions.GetDefaultApiOptions();

            try
            {
                // Step 1: Check for unexpected fields at the root level using JsonDocument
                // This ensures the response doesn't contain fields not defined in our contract.
                var expectedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "name", "getCommunityName", "setCommunityName", "tenantDefault",
                    "securityLevel", "version", "authentication", "privacy", "_links",
                    // Optional fields that might appear if present in the request
                    "username", "contextName", "authenticationKey", "privacyKey"
                };

                using JsonDocument doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;

                foreach (JsonProperty property in root.EnumerateObject())
                {
                    if (!expectedFields.Contains(property.Name))
                    {
                        throw new JsonException($"Contract Violation: Unexpected field '{property.Name}' found at the root level of the SNMP V1 configuration response.");
                    }
                }

                // Step 2: Deserialize the JSON into the response contract
                // This will map values and convert enums using JsonStringEnumConverter
                var snmpResponse = JsonSerializer.Deserialize<SnmpV1ConfigurationResponse>(responseContent, jsonSerializerOptions);

                // Step 3: Perform null/empty checks and specific value assertions for required fields
                // These checks go beyond simple deserialization to ensure data integrity.
                if (snmpResponse == null)
                {
                    throw new JsonException("Contract Violation: Deserialized SNMP V1 response object is null.");
                }

                if (string.IsNullOrEmpty(snmpResponse.Name)) throw new JsonException("Contract Violation: 'name' is missing or empty.");
                if (string.IsNullOrEmpty(snmpResponse.GetCommunityName)) throw new JsonException("Contract Violation: 'getCommunityName' is missing or empty.");
                if (string.IsNullOrEmpty(snmpResponse.SetCommunityName)) throw new JsonException("Contract Violation: 'setCommunityName' is missing or empty.");
                // tenantDefault is a bool, so it will always have a default value after deserialization.

                // Enum checks (ensuring they were successfully mapped)
                if (snmpResponse.SecurityLevel == null) throw new JsonException("Contract Violation: 'securityLevel' is missing or null.");
                if (snmpResponse.Version == null) throw new JsonException("Contract Violation: 'version' is missing or null.");
                if (snmpResponse.Authentication == null) throw new JsonException("Contract Violation: 'authentication' is missing or null.");
                if (snmpResponse.Privacy == null) throw new JsonException("Contract Violation: 'privacy' is missing or null.");

                // _links object and its contents
                if (snmpResponse.Links == null) throw new JsonException("Contract Violation: '_links' object is missing.");
                if (snmpResponse.Links.Self == null || string.IsNullOrEmpty(snmpResponse.Links.Self.Href)) throw new JsonException("Contract Violation: '_links.self.href' is missing or empty.");

                // Networks array: should be present even if empty, but usually expected to have at least one link.
                if (snmpResponse.Links.Networks == null) throw new JsonException("Contract Violation: '_links.networks' array is missing.");
               
                return snmpResponse;
            }
            catch (JsonException ex)
            {
                // Re-throw JsonException with more context for clarity in test failures
                throw new Exception(
                    $"{Environment.NewLine}SNMP V1 configuration response contract validation failed (JSON issue).{Environment.NewLine}Details: {ex.Message}. " +
                    $"{Environment.NewLine}Full response JSON: {responseContent}", ex);
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors during the validation process
                throw new Exception(
                    $"{Environment.NewLine}An unexpected error occurred during SNMP V1 configuration response validation: {ex.Message}. {Environment.NewLine}Raw response: {responseContent}", ex);
            }
        }

        /// <summary>
        /// Validates a raw JSON string against the SnmpApiErrorResponse contract (for error cases).
        /// Throws JsonException or a more generic Exception if validation fails.
        /// </summary>
        /// <param name="errorResponseContent">The raw JSON string received from the API error response.</param>
        /// <returns>The deserialized SnmpApiErrorResponse object if validation succeeds.</returns>
        /// <exception cref="JsonException">Thrown if deserialization fails or if contract violations are found.</exception>
        /// <exception cref="Exception">Thrown for other unexpected errors during validation.</exception>
        public static SnmpApiErrorResponse ValidateAndDeserializeError(string errorResponseContent)
        {
            var jsonSerializerOptions = JsonOptions.GetDefaultApiOptions();

            try
            {
                // Deserialize directly into the error response contract
                var errorResponse = JsonSerializer.Deserialize<SnmpApiErrorResponse>(errorResponseContent, jsonSerializerOptions);

                if (errorResponse == null)
                {
                    throw new JsonException("Contract Violation: Deserialized SNMP API error response object is null.");
                }

                // Perform checks for required error fields
                if (string.IsNullOrEmpty(errorResponse.ErrorCode)) throw new JsonException("Contract Violation: 'errorCode' is missing or empty in error response.");
                if (string.IsNullOrEmpty(errorResponse.ErrorText)) throw new JsonException("Contract Violation: 'errorText' is missing or empty in error response.");
                // 'success' should be false for an error
                if (errorResponse.Success) throw new JsonException("Contract Violation: 'success' field in error response should be false.");
                if (string.IsNullOrEmpty(errorResponse.Message)) throw new JsonException("Contract Violation: 'message' is missing or empty in error response.");
                if (string.IsNullOrEmpty(errorResponse.Timestamp)) throw new JsonException("Contract Violation: 'timestamp' is missing or empty in error response.");

                // 'parameters' and 'parameterizedErrorText' are often optional or conditional,
                // so you might not need strict checks unless always expected.
                // Example: if (errorResponse.Parameters == null) throw new JsonException("Contract Violation: 'parameters' array is missing.");

                return errorResponse;
            }
            catch (JsonException ex)
            {
                throw new Exception(
                    $"{Environment.NewLine}SNMP API error response contract validation failed (JSON issue).{Environment.NewLine}Details: {ex.Message}. " +
                    $"{Environment.NewLine}Full error response JSON: {errorResponseContent}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{Environment.NewLine}An unexpected error occurred during SNMP API error response validation: {ex.Message}. {Environment.NewLine}Raw error response: {errorResponseContent}", ex);
            }
        }
    }
}