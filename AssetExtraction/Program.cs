using System;
using System.Collections.Generic;
using UELib;
using UELib.Core;

namespace AssetExtraction
{
    internal class Program
    {
        private static void Main(string[] args)
        {
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
            var package = UnrealLoader.LoadFullPackage(pathToPackage, System.IO.FileAccess.Read);

            var staticMeshComponents = new List<UObject>();

            foreach (UObject obj in package.Objects)
            {
                if (obj.IsClassType("StaticMeshComponent") || obj.IsClassType("StaticMeshActor"))
                {
                    staticMeshComponents.Add(obj);
                }
            }

            foreach (var actor in staticMeshComponents)
            {
                Console.WriteLine(actor.Decompile());
            }
        }
    }
}