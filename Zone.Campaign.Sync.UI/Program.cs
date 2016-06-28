using CommandLine;
using log4net;
using log4net.Config;
using StructureMap;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using Zone.Campaign.Sync.Services;
using Zone.Campaign.WebServices.Services;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.Sync.UI
{
    internal class Program
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        #endregion

        private static void Main(string[] args)
        {
            // Read and validate options
            var options = new Options();
            if (!ValidateArguments(args, options))
            {
                // Parser will print out help about the failure
                return;
            }

            // Set up log4net
            XmlConfigurator.Configure();

            // Set up IoC container
            var container = Container.For<Initialization.Registry>();

            if (RequiresServerAuthentication(options.RunMode))
            {
                // Logon
                var rootUri = new Uri(options.Server);
                var sessionService = container.GetInstance<IAuthenticationService>();
                var logonResponse = sessionService.Logon(rootUri, options.Username, options.Password);
                if (logonResponse.Status != ResponseStatus.Success)
                {
                    // Logon failed - take no further action.
                    Log.WarnFormat("Logon failed: {0}, {1}", logonResponse.Status, logonResponse.Message);
                    return;
                }

                var tokens = logonResponse.Data;

                // Do download or upload as specified.
                switch (options.RunMode)
                {
                    case RunMode.Download:
                        {
                            var downloader = container.GetInstance<IDownloader>(new ExplicitArguments(new Dictionary<string, object> { { "queryService", container.GetInstance<IQueryService>(options.RequestMode.ToString()) } }));
                            downloader.DoDownload(rootUri, tokens, new DownloadSettings
                            {
                                Conditions = options.DownloadConditions,
                                SubdirectoryMode = options.DownloadSubdirectoryMode,
                                OutputDirectory = options.DownloadOutputDirectory,
                                Schema = options.DownloadSchema,
                            });
                        }

                        break;
                    case RunMode.ImageUpload:
                        {
                            var uploader = container.GetInstance<IUploader>(new ExplicitArguments(new Dictionary<string, object> { { "writeService", container.GetInstance<IWriteService>(options.RequestMode.ToString()) } }));
                            uploader.DoImageUpload(rootUri, tokens, new UploadSettings
                            {
                                FilePaths = options.UploadFilePaths,
                                TestMode = options.UploadTestMode,
                            });
                        }

                        break;
                    case RunMode.Upload:
                        {
                            var uploader = container.GetInstance<IUploader>(new ExplicitArguments(new Dictionary<string, object> { { "writeService", container.GetInstance<IWriteService>(options.RequestMode.ToString()) } }));
                            uploader.DoUpload(rootUri, tokens, new UploadSettings
                            {
                                FilePaths = options.UploadFilePaths,
                                TestMode = options.UploadTestMode,
                            });
                        }

                        break;
                }
            }
            else
            {
                switch (options.RunMode)
                {
                    case RunMode.GenerateImageData:
                        {
                            var imageDataProvider = container.GetInstance<IImageDataProvider>();
                            foreach (var directory in options.GenerateDirectoryPaths)
                            {
                                imageDataProvider.GenerateDataFile(directory, options.GenerateRecursive, ImageHelper.PermittedExtensions);
                            }
                        }

                        break;
                }
            }

            if (options.PromptToExit)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        #region Helpers

        private static bool RequiresServerAuthentication(RunMode runMode)
        {
            return runMode == RunMode.Download
                || runMode == RunMode.ImageUpload
                || runMode == RunMode.Upload;
        }

        private static bool ValidateArguments(string[] args, Options options)
        {
            if (!Parser.Default.ParseArguments(args, options))
            {
                // This will print out nicely formatted text for those errors picked up by the parser.
                return false;
            }

            var errors = new List<string>();

            if (RequiresServerAuthentication(options.RunMode))
            {
                if (string.IsNullOrEmpty(options.Server))
                {
                    errors.Add("Server is required.");
                }

                if (string.IsNullOrEmpty(options.Username))
                {
                    errors.Add("Username is required.");
                }

                if (string.IsNullOrEmpty(options.Password))
                {
                    errors.Add("Password is required.");
                }
            }

            switch (options.RunMode)
            {
                case RunMode.Download:
                    // TODO: we do need something like this.
                    //if (!KnownSchemas.ContainsKey(options.Schema))
                    //{
                    //    errors.Add(string.Format("Unrecognised schema: {0}. Known schemas are: {1}", options.Schema, string.Join(", ", KnownSchemas.Keys)));
                    //}

                    if (options.DownloadSubdirectoryMode != SubdirectoryMode.Default && options.DownloadSubdirectoryMode != SubdirectoryMode.UnderscoreDelimited)
                    {
                        errors.Add(string.Format("Subdirectory mode must be one of: {0}, {1}.", SubdirectoryMode.Default, SubdirectoryMode.UnderscoreDelimited));
                    }

                    break;
            }

            // Print out error messages.
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }

            return !errors.Any();
        }

        #endregion
    }
}
