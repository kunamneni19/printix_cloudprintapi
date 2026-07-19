using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CloudPrintAPI.Contracts
{
    public enum SnmpVersion
    {
        V1,
        V3
    }

    public enum SnmpV1SecurityLevel
    {
        NO_AUTH_NO_PRIVACY,
        AUTH_NO_PRIVACY,
        AUTH_PRIVACY
    }

    public enum SnmpV1Authentication
    {
        NONE,
        MD5,
        SHA
    }

    public enum SnmpV1Privacy
    {
        NONE,
        DES,
        AES
    }

    public enum SnmpV3AuthenticationLevel
    {
        NOAUTHNOPRIV,
        AUTHNOPRIV,
        AUTHPRIV
    }

    public enum SnmpV3AuthenticationAlgorithm
    {
        MD5,
        SHA,
        SHA256,
        SHA384,
        SHA512
    }
}