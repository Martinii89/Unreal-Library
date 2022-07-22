using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UELib;
using UELib.Dummy;
using UELib.Logging;

namespace AssetExtraction
{
    internal class Options
    {
        [Option('d', "debug", Default = false)]
        public bool Debug { get; set; }

        [Option('c', "classes", Default = false, Required = false, HelpText = "Should classes be outputted?")]
        public bool ExtractClasses { get; set; }

        [Option('o', "objects", Default = false, Required = false, HelpText = "Should objects be outputted?")]
        public bool ExtractData { get; set; }

        [Option('m', "meshinfo", Default = false, Required = false, HelpText = "Should data be outputted?")]
        public bool ExtractMeshInfo { get; set; }

        [Option("dummy", Default = false, Required = false, HelpText = "Should dummy package be outputted?")]
        public bool ExtractDummy { get; set; }

        [Option("objectsizelog", Default = false, Required = false, HelpText = "Log out the size contribution for all objects")]
        public bool LogObjectSizes { get; set; }

        [Option("realmesh", Default = true, Required = false, HelpText = "Should dummy package contain real mesh data")]
        public bool RealMeshDataInDummy { get; set; }

        [Option("realtexture", Default = true, Required = false, HelpText = "Should dummy package contain real texture data")]
        public bool RealTextureDataInDummy { get; set; }
       
        [Option("realtexturemaxres", Default = 256, Required = false, HelpText = "Max texture resolution to include in dummy packages")]
        public int RealTextureDataMaxResInDummy { get; set; }

        [Option("dummyFolder", Required = false, HelpText = "Path to DUMMY output package folder. If not specified current working folder  + .dummy will be used")]
        public string dummyPackageFolder { get; set; }

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
            if (package == null)
            {
                Console.WriteLine($"Unable to load: {packageName}");
                return;
            }
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
            if (options.ExtractDummy)
            {
                string outputDummyFolder = Path.Combine("Extracted", ".Dummy");
                if (options.dummyPackageFolder != null)
                {
                    outputDummyFolder = options.dummyPackageFolder;
                }
                assetExtractor.ExportDummyAssets(outputDummyFolder, options);
            }

            string deserializationErrors = $"Total deserialization errors: {Log.DeserializationErrors}";
            Log.Debug(deserializationErrors);
            //Console.WriteLine(deserializationErrors);
        }

        private static List<string> GetFilesToProcess(Options options, string pathToPackages)
        {
            var filesToProcess = new List<string>();
            if (options.packages != null)
            {
                filesToProcess.AddRange(options.packages.Select(package => !package.EndsWith(".upk") ? $"{package}.upk" : package));
            }

            if (options.fileGlob == null) return filesToProcess;
            
            foreach (var file in Directory.EnumerateFiles(pathToPackages, options.fileGlob))
            {
                var fileName = Path.GetFileName(file);
                if (!filesToProcess.Contains(fileName))
                {
                    filesToProcess.Add(fileName);
                }
            }

            return filesToProcess;
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