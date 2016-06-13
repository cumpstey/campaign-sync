namespace Zone.Campaign.Sync
{
    public struct SchemaInfo
    {
        public FileType FileType;

        public string DataElement;

        public SchemaInfo(FileType fileType, string dataElement)
        {
            FileType = fileType;
            DataElement = dataElement;
        }
    }
}
