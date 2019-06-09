using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using UELib;
using UELib.Logging;

namespace AssetExtraction
{
    internal class Options
    {
        [Option('d', "debug", Default = false)]
        public bool Debug { get; set; }

        [Option("preload", Default = true, HelpText = "Should you preload default packages?")]
        public bool preload { get; set; }

        [Option('c', "classes", Default = false, Required = false, HelpText = "Should classes be outputted?")]
        public bool ExtractClasses { get; set; }

        [Option('o', "objects", Default = false, Required = false, HelpText = "Should objects be outputted?")]
        public bool ExtractData { get; set; }

        [Option('m', "meshinfo", Default = false, Required = false, HelpText = "Should data be outputted?")]
        public bool ExtractMeshInfo { get; set; }

        [Option('f', "folder", Required = false, HelpText = "Path to package folder. If not specified current working folder will be used")]
        public string packageFolder { get; set; }

        [Option('g', "glob", HelpText = "glob pattern for files to process")]
        public string fileGlob { get; set; }

        [Option('p', "packages", Separator = ':', HelpText = "packages to process. Separated by :")]
        public IEnumerable<string> packages { get; set; }

        [Usage(ApplicationAlias = "AssetExtractor.exe")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Extract all Underwater files", 
                    new UnParserSettings() { PreferShortName = true},
                    new Options
                    {
                        ExtractClasses = true,
                        ExtractData = true,
                        ExtractMeshInfo = true,
                        fileGlob = "Underwater*.upk"
                    });
                yield return new Example("Extract classes from a list of package", new Options
                {
                    ExtractClasses = true,
                    packages = new List<string>() { "Core.upk", "Engine.upk", "ProjectX.upk", "TAGame.upk" }
                });
            }
        }
    }

    internal class Program
    {
        private static AssetExtractor assetExtractor;

        private static void Main(string[] args)
        {
            var argParse = Parser.Default.ParseArguments<Options>(args);
            Options options = null;
            argParse.WithParsed(_options =>
            {
                options = _options;
            });
            if (options == null)
            {
                return;
            }
            using (var logger = new MyFileLogger())
            {
                Log.SetLogger(logger);
                if (options.Debug)
                {
                    Log.IsDebugEnabled = true;
                }

                string pathToPackages = options.packageFolder ?? ".";
                if (options.preload)
                {
                    PreloadBasicPackages(pathToPackages);
                }

                var filesToProcess = GetFilesToProcess(options, pathToPackages);
                foreach (var file in filesToProcess)
                {
                    ProcessPackage(pathToPackages, file, options);
                }
            }
        }

        private static void ProcessPackage(string pathToPackages, string file, Options options)
        {
            var packagePath = Path.Combine(pathToPackages, file);
            var package = UnrealLoader.LoadFullPackage(packagePath, System.IO.FileAccess.Read);
            var packageName = Path.GetFileNameWithoutExtension(packagePath);
            var outputMainFolder = Path.Combine("Extracted", packageName);
            //Init the asset extractor
            Log.DeserializationErrors = 0;
            assetExtractor = new AssetExtractor(package);
            Console.WriteLine($"Processing: {file}");
            if (options.ExtractClasses)
            {
                assetExtractor.ExportClasses(outputMainFolder);
            }
            if (options.ExtractData)
            {
                assetExtractor.ExportData(outputMainFolder);
            }
            if (options.ExtractMeshInfo)
            {
                assetExtractor.ExportMeshObjects(outputMainFolder);
            }

            string deserializationErrors = $"Total deserialization errors: {Log.DeserializationErrors}";
            Log.Debug(deserializationErrors);
            Console.WriteLine(deserializationErrors);
        }

        private static List<string> GetFilesToProcess(Options options, string pathToPackages)
        {
            List<string> filesToProcess = new List<string>();
            if (options.packages != null)
            {
                filesToProcess.AddRange(options.packages);
            }
            if (options.fileGlob != null)
            {
                foreach (var file in Directory.EnumerateFiles(pathToPackages, options.fileGlob))
                {
                    var fileName = Path.GetFileName(file);
                    if (!filesToProcess.Contains(fileName))
                    {
                        filesToProcess.Add(fileName);
                    }
                }
            }

            return filesToProcess;
        }

        private static void PreloadBasicPackages(string baseFolder)
        {
            var basicPackageNames = new List<string>()
            {
                "Core.upk",
                "Engine.upk",
                "ProjectX.upk",
                "AkAudio.upk",
                "TAGame.upk"
            };
            foreach (var packageName in basicPackageNames)
            {
                try
                {
                    Console.WriteLine($"Preloading {packageName}");
                    string packagePath = Path.Combine(baseFolder, packageName);
                    UnrealLoader.LoadFullPackage(packagePath, System.IO.FileAccess.Read);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(
                        $"Did not find {packageName} in the current directory."
                        + "This is optional, but recommended. "
                        + "Preloading this package improves type recognition.");
                }
            }
        }

        private static void ExtractStaticMeshes(string packageName)
        {
            string outputFolder = Path.Combine(packageName, "StaticMesh");
            var assetTypes = new List<string>() { "StaticMeshComponent", "StaticMeshActor" };
            assetExtractor.Export(assetTypes, outputFolder);
        }

        private static void ExtractFXActors(string packageName)
        {
            string outputFolder = Path.Combine(packageName, "FXActor_TA");
            var assetTypes = new List<string>() { "FXActor_TA" };
            assetExtractor.Export(assetTypes, outputFolder);
        }
    }
}