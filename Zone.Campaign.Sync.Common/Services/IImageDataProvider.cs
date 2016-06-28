using System;
using System.Collections.Generic;
using Zone.Campaign.Sync.Data;
using Zone.Campaign.WebServices.Security;

namespace Zone.Campaign.Sync.Services
{
    public interface IImageDataProvider
    {
        #region Methods

        IEnumerable<ImageData> GetData(string filePath);

        void GenerateDataFile(string directoryPath, bool recursive, IEnumerable<string> extensions);

        #endregion
    }
}
