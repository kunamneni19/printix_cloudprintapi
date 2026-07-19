using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Net;
using System.Xml.Linq;

namespace ExternalAPI
{
    internal class DataConversion
    {
        internal string Create_Workflow_Email_Data_Json(string name, string active, string orientation, string email_to, string filetype, string source)
        {
            string SourceOutData=null;
            try
            {
                //validate data  :   name , active, orientation, FileType, source

                if (source != "MFP" || source != "PHONE")
                {
                    source = "PHONE";
                }

                if (active != "true" || active != "false")
                {
                    active = "true";
                }

                if (orientation != "PORTRAIT" || orientation != "LANDSCAPE")
                {
                    orientation = "PORTRAIT";
                }

                if (filetype != "GENERATE_SEARCHABLE_PDF" || filetype != "GENERATE_DOCX")
                {
                    filetype = "GENERATE_SEARCHABLE_PDF";
                }


                switch (source)
                {
                    case "PHONE":
                        SourceOutData ="{\"active\":true,\"name\":\""+name+ "\",\"groups\":[],\"activities\":[{\"type\":\"UPLOAD_FILES\"},{\"type\":\""+ filetype + "\",\"pdfCompatibility\":\"PDF_1_6\",\"imageQuality\":\"MEDIUM\",\"ocrConfiguration\":{\"languages\":[\"ENGLISH\"]},\"imageProcessing\":{\"crop\":false,\"deleteBlank\":false,\"deskew\":false,\"rotation\":\"0\"}},{\"type\":\"SEND_TO_EMAIL\",\"to\":[\"" + email_to+"\"],\"cc\":[],\"bcc\":[],\"subject\":\""+name+"\",\"from\":\"\",\"scanner\":{\"to\":false,\"cc\":false,\"bcc\":false},\"filenameConfiguration\":{\"template\":\"{WORKFLOW_NAME}-{YYYY_MM_DD}\"}}],\"sources\":[\""+source+"\"]}";
                        break;
                    case "MFP":
                        SourceOutData = "{\"active\":"+active+",\"name\":\"" + name + "\",\"groups\":[],\"activities\":[{\"type\":\"SCAN_FILES\",\"paperSize\":\"AUTO\",\"pageOrientation\":\""+orientation+ "\",\"duplexMode\":\"SIMPLEX\",\"dotsPerInch\":300,\"color\":\"GREY\"},{\"type\":\""+ filetype + "\",\"pdfCompatibility\":\"PDF_1_6\",\"imageQuality\":\"MEDIUM\",\"ocrConfiguration\":{\"languages\":[\"ENGLISH\"]},\"imageProcessing\":{\"crop\":false,\"deleteBlank\":false,\"deskew\":false,\"rotation\":\"AUTO\"}},{\"type\":\"SEND_TO_EMAIL\",\"to\":[\""+email_to+"\"],\"cc\":[],\"bcc\":[],\"subject\":\"hello\",\"from\":\"\",\"scanner\":{\"to\":false,\"cc\":false,\"bcc\":false},\"filenameConfiguration\":{\"template\":\"{WORKFLOW_NAME}-{YYYY_MM_DD}\"}}],\"sources\":[\""+source+"\"]}";
                        break;
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine($"failed to convert jaon data WorkflowCreateJsonData(): {ex.Message}");
                throw;
            }
            return SourceOutData;
        }

        internal string Modify_Workflow_Email_Data_Json(string name, string active, string orientation, string email_to, string filetype, string source, string pdfversion)
        {
            string SourceOutData = null;
            try
            {
                //validate data  :   name , active, orientation, FileType, source

                if (source != "MFP" || source != "PHONE")
                {
                    source = "PHONE";
                }

                if (active != "true" || active != "false")
                {
                    active = "true";
                }

                if (orientation != "PORTRAIT" || orientation != "LANDSCAPE")
                {
                    orientation = "PORTRAIT";
                }

                if (filetype != "GENERATE_SEARCHABLE_PDF" || filetype != "GENERATE_DOCX")
                {
                    filetype = "GENERATE_SEARCHABLE_PDF";
                }


                switch (source)
                {
                    case "PHONE":
                        SourceOutData = "{\"active\":true,\"name\":\"" + name + "\",\"groups\":[],\"activities\":[{\"type\":\"UPLOAD_FILES\"},{\"type\":\"" + filetype + "\",\"pdfCompatibility\":\""+ pdfversion+"\",\"imageQuality\":\"MEDIUM\",\"ocrConfiguration\":{\"languages\":[\"ENGLISH\"]},\"imageProcessing\":{\"crop\":false,\"deleteBlank\":false,\"deskew\":false,\"rotation\":\"0\"}},{\"type\":\"SEND_TO_EMAIL\",\"to\":[\"" + email_to + "\"],\"cc\":[],\"bcc\":[],\"subject\":\"" + name + "\",\"from\":\"\",\"scanner\":{\"to\":false,\"cc\":false,\"bcc\":false},\"filenameConfiguration\":{\"template\":\"{WORKFLOW_NAME}-{YYYY_MM_DD}\"}}],\"sources\":[\"" + source + "\"]}";


                        break;
                    case "MFP":
                        SourceOutData = "{\"active\":" + active + ",\"name\":\"" + name + "\",\"groups\":[],\"activities\":[{\"type\":\"SCAN_FILES\",\"paperSize\":\"AUTO\",\"pageOrientation\":\"" + orientation + "\",\"duplexMode\":\"SIMPLEX\",\"dotsPerInch\":300,\"color\":\"GREY\"},{\"type\":\"" + filetype + "\",\"pdfCompatibility\":\""+ pdfversion+"\",\"imageQuality\":\"MEDIUM\",\"ocrConfiguration\":{\"languages\":[\"ENGLISH\"]},\"imageProcessing\":{\"crop\":false,\"deleteBlank\":false,\"deskew\":false,\"rotation\":\"AUTO\"}},{\"type\":\"SEND_TO_EMAIL\",\"to\":[\"" + email_to + "\"],\"cc\":[],\"bcc\":[],\"subject\":\"hello\",\"from\":\"\",\"scanner\":{\"to\":false,\"cc\":false,\"bcc\":false},\"filenameConfiguration\":{\"template\":\"{WORKFLOW_NAME}-{YYYY_MM_DD}\"}}],\"sources\":[\"" + source + "\"]}";
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to convert jaon data WorkflowCreateJsonData(): {ex.Message}");
                throw;
            }
            return SourceOutData;
        }

        internal string Create_Workflow_Group_Data_Json(string name, string active, string groupid, string orientation, string email_to, string filetype, string source)
        {
            string SourceOutData = null;
            try
            {
                //validate data  :   name , active, orientation, FileType, source

                if (source != "MFP" || source != "PHONE")
                {
                    source = "PHONE";
                }

                if (active != "true" || active != "false")
                {
                    active = "true";
                }

                if (orientation != "PORTRAIT" || orientation != "LANDSCAPE")
                {
                    orientation = "PORTRAIT";
                }

                if (filetype != "GENERATE_SEARCHABLE_PDF" || filetype != "GENERATE_DOCX")
                {
                    filetype = "GENERATE_SEARCHABLE_PDF";
                }


                switch (source)
                {
                    case "PHONE":
                        SourceOutData = "{\"active\":true,\"name\":\"" + name + "\",\"groups\":["+groupid+"],\"activities\":[{\"type\":\"UPLOAD_FILES\"},{\"type\":\"" + filetype + "\",\"pdfCompatibility\":\"PDF_1_6\",\"imageQuality\":\"MEDIUM\",\"ocrConfiguration\":{\"languages\":[\"ENGLISH\"]},\"imageProcessing\":{\"crop\":false,\"deleteBlank\":false,\"deskew\":false,\"rotation\":\"0\"}},{\"type\":\"SEND_TO_EMAIL\",\"to\":[\"" + email_to + "\"],\"cc\":[],\"bcc\":[],\"subject\":\"" + name + "\",\"from\":\"\",\"scanner\":{\"to\":false,\"cc\":false,\"bcc\":false},\"filenameConfiguration\":{\"template\":\"{WORKFLOW_NAME}-{YYYY_MM_DD}\"}}],\"sources\":[\"" + source + "\"]}";


                        break;
                    case "MFP":
                        SourceOutData = "{\"active\":" + active + ",\"name\":\"" + name + "\",\"groups\":["+groupid+"],\"activities\":[{\"type\":\"SCAN_FILES\",\"paperSize\":\"AUTO\",\"pageOrientation\":\"" + orientation + "\",\"duplexMode\":\"SIMPLEX\",\"dotsPerInch\":300,\"color\":\"GREY\"},{\"type\":\"" + filetype + "\",\"pdfCompatibility\":\"PDF_1_6\",\"imageQuality\":\"MEDIUM\",\"ocrConfiguration\":{\"languages\":[\"ENGLISH\"]},\"imageProcessing\":{\"crop\":false,\"deleteBlank\":false,\"deskew\":false,\"rotation\":\"AUTO\"}},{\"type\":\"SEND_TO_EMAIL\",\"to\":[\"" + email_to + "\"],\"cc\":[],\"bcc\":[],\"subject\":\"hello\",\"from\":\"\",\"scanner\":{\"to\":false,\"cc\":false,\"bcc\":false},\"filenameConfiguration\":{\"template\":\"{WORKFLOW_NAME}-{YYYY_MM_DD}\"}}],\"sources\":[\"" + source + "\"]}";
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to convert jaon data WorkflowCreateJsonData(): {ex.Message}");
                throw;
            }
            return SourceOutData;
        }
    }

   
}


