using System;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Zone.Campaign.Sync.Data
{
    public sealed class ImageDataMap : CsvClassMap<ImageData>
    {
        public ImageDataMap()
        {
            Map(m => m.FilePath).Name("FilePath");
            Map(m => m.FolderName).Name("FolderName");
            Map(m => m.InternalName).Name("InternalName");
            Map(m => m.Label).Name("Label");
            Map(m => m.Alt).Name("Alt");
        }
    }
}
