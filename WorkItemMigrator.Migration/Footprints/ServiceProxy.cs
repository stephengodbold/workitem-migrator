using System.Linq;
using System.Net;

namespace WorkItemMigrator.Migration.Footprints
{
    [System.Web.Services.WebServiceBindingAttribute(Name = "GetIssueDetailsService",Namespace = "MRWebServices")]
    public class ServiceProxy : System.Web.Services.Protocols.SoapHttpClientProtocol
    {
        public ServiceProxy()
        {
            Url = "http://helpdesk.studygroup.com/MRcgi/MRWebServices.pl";
            Credentials = new NetworkCredential(@"staff\sgodbold", "");
        }

        [System.Web.Services.Protocols.SoapDocumentMethodAttribute(
            "MRWebServices#MRWebServices__getIssueDetails",
            RequestNamespace = "MRWebServices",
            ResponseNamespace = "MRWebServices",
            Use = System.Web.Services.Description.SoapBindingUse.Encoded,
            ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public string GetIssueDetails(
            string usr,
            string pw,
            string extraInfo,
            string proj,
            string num)
        {
            var results = Invoke("GetIssueDetails",
                new object[] { 
                    usr, 
                    pw, 
                    extraInfo, 
                    proj, 
                    num });

            return results.Any() ? results.First().ToString() : string.Empty;
        }
    }
}
