using System;
using System.Collections.Generic;
using UELib;
using UELib.Core;

namespace AssetExtraction
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 2)
            {
                Console.WriteLine("Usage: -p \"Path to package\"");
            }
            var pathToPackage = args[1];
            var package = UnrealLoader.LoadFullPackage(pathToPackage, System.IO.FileAccess.Read);

            var staticMeshActors = new List<UObject>();

            foreach (UObject obj in package.Objects)
            {
                if (obj.IsClassType("StaticMeshActor") )
                {
                    staticMeshActors.Add(obj);
                }
            }

            foreach (var actor in staticMeshActors)
            {
                Console.WriteLine(actor.Decompile());
            }
        }
    }
}
