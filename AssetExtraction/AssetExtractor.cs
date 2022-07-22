using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UELib;
using UELib.Core;
using UELib.Dummy;
using UELib.Flags;

namespace AssetExtraction
{
    class AssetExtractor
    {
        private readonly UnrealPackage package;
        private readonly bool onlyExports;

        private static string CreateFolderAndGetFullPath(string outputFile)
        {
            var fi = new FileInfo(outputFile);
            fi.Directory.Create();
            //https://stackoverflow.com/questions/5188527/how-to-deal-with-files-with-a-name-longer-than-259-characters
            var fullPath = @"\\?\" + fi.FullName;
            return fullPath;
        }

        public AssetExtractor(UnrealPackage package, bool onlyExports = true)
        {
            this.package = package;
            this.onlyExports = onlyExports;
        }

        private string GetFullObjectName(UObject obj)
        {
            string name = obj.Name;
            while (obj.Outer != null)
            {
                name = $"{obj.Outer.Name}\\{name}";
                obj = obj.Outer;
            }

            return name;
        }

        public int Export(IList<string> types, string outputPath)
        {
            var objects = FindObjectsOfType(types);
            Console.WriteLine($"Extracting {objects.Count} objects of type(s): {String.Join(", ", types)}");
            foreach (var obj in objects)
            {
                var outputFile = Path.Combine(outputPath, GetFullObjectName(obj) + ".uc");
                var filePath = CreateFolderAndGetFullPath(outputFile);
                File.WriteAllText(filePath, obj.Decompile());
            }

            return objects.Count;
        }

        public int ExportClasses(string outputPath)
        {
            string outputFolder = Path.Combine(outputPath, ".Classes");
            var objects = FindObjectsOfType(new List<string>() {"Class"});
            Console.WriteLine($"\tExtracting {objects.Count} classes");
            foreach (var o in objects)
            {
                var obj = (UClass) o;
                //if (obj.HasClassFlag(ClassFlags.ParseConfig))
                //{
                //    Console.WriteLine($"Skipping: {obj.Name}");
                //    continue;
                //}
                var outputFile = Path.Combine(outputFolder, GetFullObjectName(obj) + ".uc");
                var filePath = CreateFolderAndGetFullPath(outputFile);
                File.WriteAllText(filePath, obj.Decompile());
            }

            return objects.Count;
        }

        private string DefaultsFolder(UObject obj)
        {
            var outerGroup = obj.GetOuterGroup();
            if (outerGroup != null && outerGroup.StartsWith("Default__"))
            {
                return ".Classes\\Defaults\\";
            }
            else
            {
                return "";
            }
        }

        public int ExportData(string outputPath)
        {
            //var objects = package.Objects.Where((u) => u.ExportTable != null && u.Name != "None" && u.ExportTable.ExportFlags == 1 && u.Table.ClassName != "Package");
            var extractableObjects = package.Objects.Where((o) => o.ExportTable != null && o is IExtract && o.Name != "None");
            var dataObjects = extractableObjects.Where(
                (o) => !o.IsClassType("Class"));
            //Theworld stuff does not set exportflag = 1
            Console.WriteLine($"\tExtracting {dataObjects.Count()} objects");
            foreach (var obj in dataObjects)
            {
                var outputFile = Path.Combine(outputPath, $"{DefaultsFolder(obj)}{GetFullObjectName(obj)}.uc");
                var filePath = CreateFolderAndGetFullPath(outputFile);
                File.WriteAllText(filePath, obj.Decompile());
            }

            return dataObjects.Count();
        }

        public int ExportMeshObjects(string outputPath)
        {
            var outputFile = Path.Combine(outputPath, ".json", $"{package.PackageName}_MeshObjects.json");
            var dataObjects = FindObjectsOfType(new List<string>() {"StaticMeshComponent"});
            var nodeCache = new Dictionary<UObject, DataNode>();
            Console.WriteLine($"\tExtracting {dataObjects.Count()} instances of meshInfo");
            foreach (var obj in dataObjects)
            {
                obj.BeginDeserializing();
                var outer = obj.Outer;
                if (outer == null) continue;
                nodeCache.TryGetValue(outer, out var outerNode);
                if (outerNode == null)
                {
                    outer.BeginDeserializing();
                    outerNode = new DataNode(outer);
                    nodeCache.Add(outer, outerNode);
                }

                outerNode.AddChild(new DataNode(obj));
                while (outer.Outer != null)
                {
                    var nextOuter = outer.Outer;
                    nodeCache.TryGetValue(nextOuter, out var nextOuterNode);
                    if (nextOuterNode == null)
                    {
                        //If it's not in the cache. 
                        //We need to add it, add the previous as a child. And continue up the chain
                        nextOuter.BeginDeserializing();
                        nextOuterNode = new DataNode(nextOuter);
                        nodeCache.Add(nextOuter, nextOuterNode);
                        nextOuterNode.AddChild(outerNode);
                        outer = nextOuter;
                        outerNode = nextOuterNode;
                    }
                    else
                    {
                        //If it's already in the cache. We can add the previous as a child and exit out of the loop
                        nextOuterNode.AddChild(outerNode);
                        break;
                    }
                }
            }

            var rootNodes = nodeCache.Where((kv) => kv.Value.parent == null).Select((kv) => kv.Value);
            new FileInfo(outputFile).Directory.Create();
            using (var fileWriter = new StreamWriter(outputFile))
            {
                var json_data = JsonConvert.SerializeObject(rootNodes,
                    Newtonsoft.Json.Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                fileWriter.Write(json_data);
                //foreach (var rootNode in rootNodes)
                //{
                //    fileWriter.WriteLine(rootNode.print());
                //}
            }

            return 0;
        }

        public void ExportDummyAssets(string outputPath, Options options)
        {
            var outputFile = Path.Combine(outputPath, package.PackageName + ".upk");
            var filePath = CreateFolderAndGetFullPath(outputFile);
            var dummyOptions = new DummyOptions
            {
                LogObjectSizes = options.LogObjectSizes, 
                RealMeshDataInDummy = options.RealMeshDataInDummy,
                RealTextureDataInDummy = options.RealTextureDataInDummy, 
                RealTextureDataMaxResInDummy = options.RealTextureDataMaxResInDummy
            };
            RlDummyPackageStream packageSerializer = new RlDummyPackageStream(package, filePath, dummyOptions);
            packageSerializer.Serialize();
        }

        private bool FilteredDeserialization(UObject obj, ISet<string> properties, out string output, int tabs)
        {
            if ((obj.DeserializationState & UObject.ObjectState.Deserialied) == 0)
            {
                obj.BeginDeserializing();
            }

            output = "";
            string indentation = new StringBuilder().Insert(0, "\t", tabs).ToString();
            if (obj.Properties == null) return false;

            var propsToDeserialize = obj?.Properties.Where((p) => properties.Contains(p.Name));
            foreach (var prop in propsToDeserialize)
            {
                output += ($"{indentation}{prop.Decompile()}\r\n");
            }

            return output.Length > 0;
        }

        private List<UObject> FindObjectsOfType(IList<string> types)
        {
            var objects = new List<UObject>();
            foreach (var obj in package.Objects)
            {
                if (onlyExports && obj.ExportTable == null || obj.Name == "None")
                {
                    //Skip classes that are not defined in this package. And None classes(Wtf even is that..)
                    continue;
                }

                foreach (var type in types)
                {
                    if (obj.IsClassType(type))
                    {
                        objects.Add(obj);
                        break;
                    }
                }
            }

            return objects;
        }
    }
}