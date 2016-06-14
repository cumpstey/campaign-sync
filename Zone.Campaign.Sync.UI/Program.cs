﻿using CommandLine;
using log4net;
using log4net.Config;
using StructureMap;
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

            // Logon
            var rootUri = new Uri(options.Server);
            var sessionService = container.With(rootUri).GetInstance<IAuthenticationService>();
            var logonResponse = sessionService.Logon(options.Username, options.Password);
            if (logonResponse.Status != ResponseStatus.Success)
            {
                // Logon failed - take no further action.
                Log.WarnFormat("Logon failed: {0}, {1}", logonResponse.Status, logonResponse.Message);
                return;
            }

            var tokens = logonResponse.Data;

            // Do download or upload as specified.
            switch (options.Mode)
            {
                case "download":
                    var downloader = container.GetInstance<IDownloader>();
                    downloader.DoDownload(rootUri, tokens, new DownloadSettings
                    {
                        Conditions = options.DownloadConditions,
                        DirectoryMode = options.DownloadDirectoryMode,
                        OutputDirectory = options.DownloadOutputDirectory,
                        Schema = options.DownloadSchema,
                    });
                    //DoDownload(options, rootUri, tokens);
                    break;
                case "upload":
                    var uploader = container.GetInstance<IUploader>();
                    uploader.DoUpload(rootUri, tokens, new UploadSettings
                    {
                        FilePaths = options.UploadFilePaths,
                        TestMode = options.UploadTestMode,
                    });
                    //DoUpload(options, rootUri, tokens);
                    break;
            }

            if (options.PromptToExit)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        #region Helpers

        private static bool ValidateArguments(string[] args, Options options)
        {
            if (!Parser.Default.ParseArguments(args, options))
            {
                // This will print out nicely formatted text for those errors picked up by the parser.
                return false;
            }

            var errors = new List<string>();

            switch (options.Mode)
            {
                case SyncMode.Download:
                    // TODO: we do need something like this.
                    //if (!KnownSchemas.ContainsKey(options.Schema))
                    //{
                    //    errors.Add(string.Format("Unrecognised schema: {0}. Known schemas are: {1}", options.Schema, string.Join(", ", KnownSchemas.Keys)));
                    //}

                    if (options.DownloadDirectoryMode != SubdirectoryMode.Default && options.DownloadDirectoryMode != SubdirectoryMode.UnderscoreDelimited)
                    {
                        errors.Add(string.Format("Subdirectory mode must be one of: {0}, {1}.", SubdirectoryMode.Default, SubdirectoryMode.UnderscoreDelimited));
                    }

                    break;
                case SyncMode.Upload:
                    break;
                default:
                    errors.Add(string.Format("Mode must be one of: {0}, {1}.", SyncMode.Download, SyncMode.Upload));
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