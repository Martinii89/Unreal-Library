using System;
using System.Collections.Generic;
using System.IO;
using UELib;
using UELib.Types;
using UELib.Core;
using UELib.Logging;

namespace AssetExtraction
{
    internal class Program
    {

        static AssetExtractor assetExtractor;

        private static void Main(string[] args)
        {
            Log.SetLogger(new FileLogger());
            ConfigArrayTypes();
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
            UnrealLoader.LoadFullPackage("Engine.upk", System.IO.FileAccess.Read);
            var package = UnrealLoader.LoadFullPackage(pathToPackage, System.IO.FileAccess.Read);
            assetExtractor = new AssetExtractor(package);
            var packageName = Path.GetFileNameWithoutExtension(pathToPackage);
            ExtractClasses(packageName);
            ExtractStaticMeshes(packageName);
            ExtractFXActors(packageName);
        }

        private static void ConfigArrayTypes()
        {
            if (UnrealConfig.VariableTypes == null)
            {
                UnrealConfig.VariableTypes = new Dictionary<string, Tuple<string, PropertyType>>();
            }
            var tupleList = new List<(string propName, PropertyType propType)>
              {
                ("Skins",  PropertyType.ObjectProperty),
                ("Components",  PropertyType.ObjectProperty),
                ("AnimSets",  PropertyType.ObjectProperty),
                ("InputLinks",  PropertyType.StructProperty),
                ("OutputLinks",  PropertyType.StructProperty),
                ("VariableLinks",  PropertyType.StructProperty),
                ("Targets",  PropertyType.ObjectProperty),
                ("Controls",  PropertyType.ObjectProperty),
                ("Expressions",  PropertyType.ObjectProperty),
                ("Emitters",  PropertyType.ObjectProperty),
                //("Attachments", PropertyType.StructProperty)
              };
            foreach(var (propName, propType) in tupleList)
            {
                UnrealConfig.VariableTypes.Add(propName, new Tuple<string, PropertyType>(propName, propType));
            }
        }

        private static void ExtractClasses(string packageName)
        {
            string outputFolder = Path.Combine(packageName, "Classes");
            var assetTypes = new List<string>() { "Class" };
            assetExtractor.Extract(assetTypes, outputFolder);
        }

        private static void ExtractStaticMeshes(string packageName)
        {
            string outputFolder = Path.Combine(packageName, "StaticMesh");
            var assetTypes = new List<string>() { "StaticMeshComponent", "StaticMeshActor" };
            assetExtractor.Extract(assetTypes, outputFolder);
        }

        private static void ExtractFXActors(string packageName)
        {
            string outputFolder = Path.Combine(packageName, "FXActor_TA");
            var assetTypes = new List<string>() { "FXActor_TA"};
            assetExtractor.Extract(assetTypes, outputFolder);
        }
    }
}