using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    public interface ISoapRequestHandler
    {
        Response<XmlNode> ExecuteRequest(Uri uri, IEnumerable<string> customHeaders, Tokens tokens, string serviceName, string serviceNamespace, XmlDocument requestDoc);

    }
}
