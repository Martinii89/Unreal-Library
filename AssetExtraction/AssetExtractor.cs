using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UELib;
using UELib.Core;

namespace AssetExtraction
{
    class AssetExtractor
    {
        private readonly UnrealPackage package;
        private readonly bool onlyExports;

        public AssetExtractor(UnrealPackage package, bool onlyExports = true)
        {
            this.package = package;
            this.onlyExports = onlyExports;
        }

        private string GetFullObjectName(UObject obj)
        {
            string name = obj.Name;
            while(obj.Outer != null)
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
                new FileInfo(outputFile).Directory.Create();
                File.WriteAllText(outputFile, obj.Decompile());
            }

            return objects.Count;
        }

        public int ExportClasses(string outputPath)
        {
            string outputFolder = Path.Combine(outputPath, ".Classes");
            var objects = FindObjectsOfType(new List<string>() { "Class" });
            Console.WriteLine($"Extracting {objects.Count} classes");
            foreach (var obj in objects)
            {
                var outputFile = Path.Combine(outputFolder, GetFullObjectName(obj) + ".uc");
                new FileInfo(outputFile).Directory.Create();
                File.WriteAllText(outputFile, obj.Decompile());
            }
            return objects.Count;

        }

        public int ExportData(string outputPath)
        {
            //var objects = package.Objects.Where((u) => u.ExportTable != null && u.Name != "None" && u.ExportTable.ExportFlags == 1 && u.Table.ClassName != "Package");
            var extractableObjects = package.Objects.Where((o) => o.ExportTable != null && o is IExtract && o.Name != "None");
            var dataObjects = extractableObjects.Where(
                (o) => !o.IsClassType("Class")
                && !o.GetOuterGroup().StartsWith("Default__"));
            //Theworld stuff does not set exportflag = 1
            Console.WriteLine($"Extracting {dataObjects.Count()} objects");
            foreach (var obj in dataObjects)
            {
                var outputFile = Path.Combine(outputPath, GetFullObjectName(obj) + ".uc");
                new FileInfo(outputFile).Directory.Create();
                File.WriteAllText(outputFile, obj.Decompile());
            }

            return dataObjects.Count();
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
                foreach(var type in types)
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
