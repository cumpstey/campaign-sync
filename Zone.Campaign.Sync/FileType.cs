namespace Zone.Campaign.Sync
{
    public enum FileType
    {
        [FileExtension(".txt")] Unknown,
        [FileExtension(".html")] Html,
        [FileExtension(".xml")] Xml,
        [FileExtension(".js")] JavaScript,
    }
}
