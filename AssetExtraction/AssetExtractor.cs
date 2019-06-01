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

        public int Extract(IList<string> types, string outputPath)
        {
            var objects = FindObjectsOfType(types);
            Console.WriteLine($"Extracting {objects.Count} objects of type(s): {String.Join(", ", types)}");
            foreach (var obj in objects)
            {
                var outputFile = Path.Combine(outputPath, obj.Name + ".uc");
                new FileInfo(outputFile).Directory.Create();
                File.WriteAllText(outputFile, obj.Decompile());
            }

            return objects.Count;
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
