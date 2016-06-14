using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.Sync.Mappings.Abstract
{
    public interface IMapping
    {
        #region Properties

        //Type MappingFor { get; }

        IEnumerable<string> QueryFields { get; }
        
        #endregion

        #region Methods

        IPersistable GetPersistableItem(Template template);

        Template ParseQueryResponse(string rawQueryResponse);

        #endregion
    }
}
