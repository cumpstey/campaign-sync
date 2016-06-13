using System;

namespace Zone.Campaign.WebServices.Model
{
    public class SchemaAttribute : Attribute
    {
        #region Constructor

        public SchemaAttribute(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        #endregion
    }
}
