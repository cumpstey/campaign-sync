using System.Collections.Generic;
using Zone.Campaign.WebServices.Model.Abstract;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Contains data create/update functions.
    /// </summary>
    public interface IWriteService
    {
        #region Methods

        /// <summary>
        /// Create/update an item.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <param name="item">Item to create/update</param>
        /// <returns>Response</returns>
        Response Write<T>(IRequestHandler requestHandler, T item)
            where T : IPersistable;

        /// <summary>
        /// Create/update a collection of items.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <param name="items">Items to create/update</param>
        /// <returns>Response</returns>
        Response WriteCollection<T>(IRequestHandler requestHandler, IEnumerable<T> items)
            where T : IPersistable;
        
        #endregion
    }
}
