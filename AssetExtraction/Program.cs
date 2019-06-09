using System;
using System.Collections.Generic;
using System.IO;
using UELib;
using UELib.Types;
using UELib.Core;
using UELib.Logging;
using System.Reflection;

namespace AssetExtraction
{
    public class MyFileLogger : ILogger, IDisposable
    {
        private readonly string logFile;
        readonly StreamWriter writer;

        public MyFileLogger(string fileName = "log", bool overwriteFile = true)
        {
            logFile = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\{fileName}.txt";
            if (overwriteFile)
            {
                DeleteOldLogfile();
            }
            writer = new StreamWriter(logFile, append: true);
        }

        private void DeleteOldLogfile()
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }


        public void WriteLine(string message)
        {
            writer.WriteLine(message);
        }

        public void Dispose()
        {
            ((IDisposable)writer).Dispose();
        }
    }

    internal class Program
    {

        static AssetExtractor assetExtractor;

        private static void Main(string[] args)
        {
            using (var logger = new MyFileLogger())
            {
                Log.SetLogger(logger);
                Log.IsDebugEnabled = true;
                //Preloading packages works better and more reliable
                //ConfigArrayTypes();
                string pathToPackage;
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: -p \"Path to package\"");
                    return;
                }
                else
                {
                    pathToPackage = args[1];
                }
                PreloadBasicPackages();

                var package = UnrealLoader.LoadFullPackage(pathToPackage, System.IO.FileAccess.Read);
                var packageName = Path.GetFileNameWithoutExtension(pathToPackage);
                var outputMainFolder = Path.Combine("Extracted", packageName);
                //Init the asset extractor
                Log.DeserializationErrors = 0;
                assetExtractor = new AssetExtractor(package);
                assetExtractor.ExportClasses(outputMainFolder);
                assetExtractor.ExportData(outputMainFolder);
                assetExtractor.ExportMeshObjects(outputMainFolder);
                string deserializationErrors = $"Total deserialization errors: {Log.DeserializationErrors}";
                Log.Debug(deserializationErrors);
                Console.WriteLine(deserializationErrors);

            }
        }

        private static void PreloadBasicPackages()
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
                    UnrealLoader.LoadFullPackage(packageName, System.IO.FileAccess.Read);
                }catch (FileNotFoundException e)
                {
                    Console.WriteLine(
                        $"Did not find {packageName} in the current directory."
                        +"This is optional, but recommended. "
                        +"Preloading this package improves type recognition.");
                }
            }
        }

        //private static void ConfigArrayTypes()
        //{
        //    if (UnrealConfig.VariableTypes == null)
        //    {
        //        UnrealConfig.VariableTypes = new Dictionary<string, Tuple<string, PropertyType>>();
        //    }
        //    var tupleList = new List<(string propName, PropertyType propType)>
        //      {
        //        ("Skins",  PropertyType.ObjectProperty),
        //        ("Components",  PropertyType.ObjectProperty),
        //        ("AnimSets",  PropertyType.ObjectProperty),
        //        ("InputLinks",  PropertyType.StructProperty),
        //        ("OutputLinks",  PropertyType.StructProperty),
        //        ("VariableLinks",  PropertyType.StructProperty),
        //        ("Targets",  PropertyType.ObjectProperty),
        //        ("Controls",  PropertyType.ObjectProperty),
        //        ("Expressions",  PropertyType.ObjectProperty),
        //        ("Emitters",  PropertyType.ObjectProperty),
        //        //("Attachments", PropertyType.StructProperty)
        //      };
        //    foreach(var (propName, propType) in tupleList)
        //    {
        //        UnrealConfig.VariableTypes.Add(propName, new Tuple<string, PropertyType>(propName, propType));
        //    }
        //}

        private static void ExtractStaticMeshes(string packageName)
        {
            string outputFolder = Path.Combine(packageName, "StaticMesh");
            var assetTypes = new List<string>() { "StaticMeshComponent", "StaticMeshActor" };
            assetExtractor.Export(assetTypes, outputFolder);
        }

        private static void ExtractFXActors(string packageName)
        {
            string outputFolder = Path.Combine(packageName, "FXActor_TA");
            var assetTypes = new List<string>() { "FXActor_TA"};
            assetExtractor.Export(assetTypes, outputFolder);
        }
    }
}