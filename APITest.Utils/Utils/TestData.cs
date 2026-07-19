namespace CommonUtils
{
    public class TestData
    {

        public AdminData admin { get; set; }
        public PartnerData partner { get; set; }
        public ReleaseData release { get; set; }
        public SiteData site_dk { get; set; }
        public SiteData site_hyd { get; set; }
        public PrinterData HP_dk { get; set; }
        public PrinterData HP_hyd { get; set; }
        public PrinterData RICOH_dk { get; set; }
        public PrinterData RICOH_hyd { get; set; }
        public PrinterData dummy { get; set; }
        public string browser_env { get; set; }
        public CloudData Cloud { get; set; }
        public GmailData Gmail { get; set; }

        public SnmpV1Config snmp { get; set; } = new SnmpV1Config();

    }
    public class AdminData
    {
        public string auth_url { get; set; }
        public string tenant { get; set; }
        public string tenant_id { get; set; }
        public string network_name { get; set; }
        public string ip { get; set; }
        public string mac { get; set; }
        public string network_id { get; set; }
        public string admin_username { get; set; }
        public string admin_pass { get; set; }
        public string token_url { get; set; }
        public string response_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string redirect_uri { get; set; }
        public string grant_type { get; set; }
        public string auth_token_url { get; set; }
        public string email_to { get; set; }
        public string email_cc { get; set; }
        public string workflow_name { get; set; }
        public string workflow_id { get; set; }
        public string group_id { get; set; }
        public string cp_auth_token_url { get; set; }
        public string cp_grant_type { get; set; }
        public string cp_client_id { get; set; }
        public string cp_client_secret { get; set; }
        public string InvalidSessionBeforeJWT { get; set; }
        public string validButExpiredSessionBeforeCredentials { get; set; }
        public string user_username { get; set; }
        public string user_pass { get; set; }
        public string cp_url { get; set; }
        public string temp_tenant { get; set; }

    }

    public class ReleaseData
    {

    }

    public class PartnerData
    {

    }

    public class SiteData
    {


    }

    public class PrinterData
    {

    }
    public class CloudData
    {
        public string name { get; set; }
        public string subscription { get; set; }
    }

    public class GmailData
    {
        public string user { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }

    public class SnmpV1Config
    {
        // Default values for SNMP V1 properties
        public string default_name_prefix { get; set; } = "TestSnmpV1"; 
        public string get_community_name { get; set; } = "public_community";
        public string set_community_name { get; set; } = "private_community";
        public string security_level { get; set; } = "NoAuthNoPrivacy"; 
        public string version { get; set; } = "V1";                     
        public string authentication { get; set; } = "None";            
        public string privacy { get; set; } = "None";                  
        public List<string> network_ids { get; set; } = new List<string>();
    }


}
