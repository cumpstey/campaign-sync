using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using log4net;
using Zone.Campaign.Sync.Data;

namespace Zone.Campaign.Sync.Services
{
    public class CsvImageDataProvider : IImageDataProvider
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(CsvImageDataProvider));

        private readonly CsvConfiguration Configuration;

        #endregion

        #region Constructor

        public CsvImageDataProvider()
        {
            DataFileName = "imageData.csv";

            Configuration = new CsvConfiguration();
            ////Configuration.RegisterClassMap<ImageDataMap>();
        }

        #endregion

        #region Properties

        public string DataFileName { get; set; }

        #endregion

        #region Methods

        public IEnumerable<ImageData> GetData(string directoryPath)
        {
            var filePath = Path.Combine(directoryPath, DataFileName);

            if (!File.Exists(filePath))
            {
                Log.WarnFormat("File {0} does not exist.", filePath);
                return Enumerable.Empty<ImageData>();
            }

            var data = ReadDataFromFile(filePath);

            // Change the relative path to absolute path for each file.
            var root = Path.GetDirectoryName(filePath);
            foreach (var item in data)
            {
                if (!Path.IsPathRooted(item.FilePath))
                {
                    item.FilePath = Path.Combine(root, item.FilePath);
                }
            }

            return data;
        }

        public void GenerateDataFile(string directoryPath, bool recursive, IEnumerable<string> extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException("extensions");
            }

            if (!Directory.Exists(directoryPath))
            {
                Log.WarnFormat("Directory does not exist: {0}", directoryPath);
                return;
            }

            var filePath = Path.Combine(directoryPath, DataFileName);

            // Read data from any existing file.
            var existingData = Enumerable.Empty<ImageData>();
            if (File.Exists(filePath))
            {
                existingData = ReadDataFromFile(filePath);
            }

            // Combine existing data with the list of files.
            var data = new List<ImageData>();
            var imageFiles = extensions.SelectMany(i => Directory.GetFiles(directoryPath, string.Concat("*", i))).ToArray();
            foreach (var imageFile in imageFiles)
            {
                var fileName = Path.GetFileName(imageFile);
                var existing = existingData.FirstOrDefault(i => i.FilePath == fileName);
                if (existing != null)
                {
                    data.Add(existing);
                }
                else
                {
                    data.Add(new ImageData
                    {
                        FilePath = fileName,
                    });
                }
            }

            // Write data to file, but don't bother if there is no data.
            if (data.Any())
            {
                using (var textWriter = new StreamWriter(filePath))
                {
                    var csv = new CsvWriter(textWriter, Configuration);
                    csv.WriteRecords(data);
                }
            }

            // Loop through all subdirectories recursively.
            if (recursive)
            {
                var subdirectories = Directory.GetDirectories(directoryPath);
                foreach (var subdirectory in subdirectories)
                {
                    GenerateDataFile(subdirectory, true, extensions);
                }
            }
        }

        #endregion

        #region Helpers

        private IEnumerable<ImageData> ReadDataFromFile(string filePath)
        {
            IEnumerable<ImageData> data;
            using (var textReader = new StreamReader(filePath))
            using (var csv = new CsvReader(textReader, Configuration))
            {
                try
                {
                    data = csv.GetRecords<ImageData>().ToArray();
                }
                catch (CsvMissingFieldException ex)
                {
                    Log.ErrorFormat("File {0} does not appear to contain the correct data: {1}", filePath, ex.Message);
                    return Enumerable.Empty<ImageData>();
                }
            }

            return data;
        }

        #endregion
    }
}
