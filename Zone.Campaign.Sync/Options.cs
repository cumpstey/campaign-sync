using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Zone.Campaign.Sync
{
    public class Options
    {
        [ParserState]
        public IParserState LastParserState { get; set; }

        [Option('d', "dir", MutuallyExclusiveSet = "UploadOrDownload", HelpText = "Download root directory.")]
        public string OutputDirectory { get; set; }

        [Option("schemadir", DefaultValue = true, HelpText = "Create subdirectory for schema.")]
        public bool CreateSchemaSubdirectory { get; set; }

        [Option("schema", HelpText = "Schema of items to download, eg. xtk:jst.")]
        public string Schema { get; set; }

        [OptionList("conditions", HelpText = @"Filter conditions to be applied, eg. ""@namespace = 'zne'"".")]
        public IList<string> Conditions { get; set; }

        [Option('l', "list", MutuallyExclusiveSet = "UploadOrDownload", HelpText = "Path to file containing list of items to upload.")]
        public string UploadListFilePath { get; set; }

        [Option("uploadtest", HelpText = "Upload test mode: don't upload, but print list of files.")]
        public bool UploadTestMode { get; set; }
        
        [Option('s', "server", Required = true, HelpText = "Server root url, eg. https://neolane.com/.")]
        public string Server { get; set; }

        [Option('u', "username", Required = true, HelpText = "Server username.")]
        public string Username { get; set; }

        [Option('p', "password", Required = true, HelpText = "Server password.")]
        public string Password { get; set; }

        [Option("prompt", HelpText = "Prompt to exit.")]
        public bool PromptToExit { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
