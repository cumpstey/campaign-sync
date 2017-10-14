using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Cwm.AdobeCampaign.WebServices.Model.Abstract
{
    /// <summary>
    /// Interface implemented by classes representing objects which are queryable in Adobe Campaign.
    /// </summary>
    public interface IQueryable
    {
        IEnumerable<string> FieldsToQuery { get; }

        void LoadFromQueryResponse();
    }
}
