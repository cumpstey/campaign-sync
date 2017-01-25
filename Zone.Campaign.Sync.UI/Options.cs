using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Zone.Campaign.Sync
{
    public class Options
    {
        [ParserState]
        public IParserState LastParserState { get; set; }

        [Option('m', "mode", Required = true,  HelpText = "Mode. [Download, ImageUpload, Upload]")]
        public RunMode RunMode { get; set; }

        #region Shared parameters

        [Option('s', "server", HelpText = "Server SOAP url, eg. https://neolane.com/nl/jsp/soaprouter.jsp.")]
        public string Server { get; set; }

        [Option('u', "username", HelpText = "Server username.")]
        public string Username { get; set; }

        [Option('p', "password", HelpText = "Server password.")]
        public string Password { get; set; }

        [Option("requestmode", DefaultValue = RequestMode.Default, HelpText = "Request mode - whether to use plain xml or zipped requests. [Default, Zip]")]
        public RequestMode RequestMode { get; set; }

        [Option("prompt", HelpText = "Prompt to exit.")]
        public bool PromptToExit { get; set; }

        [OptionArray("headers", HelpText = @"Additional headers to be included in any http request, eg. ""X-Custom: my-value"".")]
        public string[] CustomHeaders { get; set; }

        #endregion

        #region Download parameters

        [Option("dir", HelpText = "Download: Root directory.")]
        public string DownloadOutputDirectory { get; set; }

        [Option("dirmode", DefaultValue = SubdirectoryMode.Default, HelpText = "Download: Directory mode - option to split on underscore. [Default, UnderscoreDelimited]")]
        public SubdirectoryMode DownloadSubdirectoryMode { get; set; }

        [Option("schema", HelpText = "Download: Schema of items to download eg. xtk:jst.")]
        public string DownloadSchema { get; set; }

        [OptionArray("conditions", HelpText = @"Download: Filter conditions to be applied, eg. ""@namespace = 'zne'"".")]
        public string[] DownloadConditions { get; set; }

        #endregion

        #region Upload parameters

        [OptionArray("files", HelpText = "Upload: List of paths of files to upload. Can include individual files or directories, or search patters within directories. If a directory is specified, the search is recursive.")]
        public string[] UploadFilePaths { get; set; }

        [Option("uploadtest", HelpText = "Upload: Test mode - don't upload, but print list of files which have been found.")]
        public bool UploadTestMode { get; set; }

        [OptionArray("replacements", HelpText = @"Upload: String replacements to be applied, eg. ""DEVDB=>LIVDB"".")]
        public string[] Replacements { get; set; }

        #endregion

        #region Generate image data parameters

        [OptionArray("dirs", HelpText = "GenerateImageData: List of paths of directories in which to generate/update data file.")]
        public string[] GenerateDirectoryPaths { get; set; }

        [Option("recursive", HelpText = "GenerateImageData: Generate/update data file recursively in all subdirectories.")]
        public bool GenerateRecursive { get; set; }

        #endregion

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
