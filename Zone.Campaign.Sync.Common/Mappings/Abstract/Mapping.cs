using System.Collections.Generic;
using System.Linq;
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
        
        /// <summary>
        /// List of field names which should be requested when querying Campaign.
        /// </summary>
        public abstract IEnumerable<string> QueryFields { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Map the information parsed from a file into a class which can be sent to Campaign to be saved.
        /// </summary>
        /// <param name="template">Class containing file content and metadata</param>
        /// <returns>Class containing information which can be sent to Campaign</returns>
        public abstract IPersistable GetPersistableItem(Template template);

        /// <summary>
        /// Map the information sent back by Campaign into a format which can be saved as a file to disk.
        /// </summary>
        /// <param name="rawQueryResponse">Raw response from Campaign</param>
        /// <returns>Class containing file content and metadata</returns>
        public abstract Template ParseQueryResponse(string rawQueryResponse);

        #endregion
    }
}
