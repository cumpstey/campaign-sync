using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Zone.Campaign.Sync.Mappings.Abstract;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.Sync.Mappings
{
    public abstract class Mapping<T> : IMapping
    {
        #region Properties

        protected virtual string Schema
        {
            get { return typeof(T).GetCustomAttributes(typeof(SchemaAttribute), false).Cast<SchemaAttribute>().First().Name; }
        }

        //public abstract Type MappingFor { get; }

        public abstract IEnumerable<string> QueryFields { get; }

        #endregion

        #region Methods

        public abstract IPersistable GetPersistableItem(Template template);

        public abstract Template ParseQueryResponse(string rawQueryResponse);

        #endregion
    }
}
