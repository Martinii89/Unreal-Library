using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RLUPKT.Core;
using UELib.Core;
using UELib.Decoding;
using UELib.Logging;

namespace UELib
{
    /// <summary>
    ///     Provides static methods for loading unreal packages.
    /// </summary>
    public static class UnrealLoader
    {
        /// <summary>
        ///     Stored packages that were imported by certain objects. Kept here that in case re-use is necessary, that it will be
        ///     loaded faster.
        ///     The packages and the list is closed and cleared by the main package that loaded them with ImportObjects().
        ///     In any other case the list needs to be cleared manually.
        /// </summary>
        private static readonly List<UnrealPackage> _CachedPackages = new List<UnrealPackage>();

        private static readonly Dictionary<string, UnrealPackage> _LoadedPackages =
            new Dictionary<string, UnrealPackage>();

        private static readonly Dictionary<string, UStruct> _LoadedClasses = new Dictionary<string, UStruct>();

        private static readonly Dictionary<Tuple<string, string>, UStruct> FindClassInPackageCache =
            new Dictionary<Tuple<string, string>, UStruct>();


        private static UnrealPackage GetPreloadedPackage(string packageName)
        {
            if (_LoadedPackages.ContainsKey(packageName))
            {
                return _LoadedPackages[packageName];
            }

            return null;
        }

        /// <summary>
        ///     Tries to find the given class in a preloaded package
        ///     Caches the results for some better performance
        /// </summary>
        public static UStruct FindClassInPackage(string packageName, string className)
        {
            var argTuple = Tuple.Create(packageName, className);
            UStruct cachedResults;
            FindClassInPackageCache.TryGetValue(argTuple, out cachedResults);
            if (cachedResults != null)
            {
                Log.Info($"Used cached result for {packageName}:{className}");
                return cachedResults;
            }

            var package = GetPreloadedPackage(packageName);
            if (package == null)
            {
                Log.Error($"Package: {packageName} not preloaded. Finding the real class of {className} failed");
                return null;
            }

            var foundClass = package.Objects.Find(o =>
                string.Compare(o.Name, className, StringComparison.OrdinalIgnoreCase) == 0 && o.Class == null);
            var foundStruct = foundClass as UStruct;

            if (foundStruct != null)
            {
                FindClassInPackageCache.Add(argTuple, foundStruct);
            }
            else
            {
                Log.Debug($"Did not find {className} in {packageName}");
            }

            return foundStruct;
        }

        public static UStruct FindClassInCache(string className)
        {
            _LoadedClasses.TryGetValue(className, out var classObject);
            if (classObject == null)
            {
                Log.Debug($"Did not find {className} in cache");
                return null;
            }

            return classObject;
        }

        private static void CacheExportClasses(UnrealPackage package)
        {
            foreach (var obj in package.Objects)
            {
                if (obj.ExportTable == null || obj.Name == "None")
                {
                    //Skip classes that are not defined in this package. And None classes(Wtf even is that..)
                    continue;
                }

                if (obj.IsClassType("Class") || obj.IsClassType("ScriptStruct"))
                {
                    if (_LoadedClasses.ContainsKey(obj.Name))
                    {
                        _LoadedClasses[obj.Name] = (UStruct) obj;
                        Log.Debug($"{obj.Name} already in class cache. Overwriting with class from {package.FullPackageName} (What could go wrong right?");
                    }
                    else
                    {
                        _LoadedClasses.Add(obj.Name, (UStruct) obj);
                    }
                }
            }
        }

        /// <summary>
        ///     Loads the given file specified by PackagePath and
        ///     returns the serialized UnrealPackage.
        /// </summary>
        public static UnrealPackage LoadPackage(string packagePath, FileAccess fileAccess = FileAccess.Read)
        {
            var packageName = Path.GetFileNameWithoutExtension(packagePath);

            UPackageStream stream;
            if (packageName.EndsWith("_decrypted"))
            {
                stream = new UPackageStream(packagePath, FileMode.Open, fileAccess);
                Log.Info("Loading decrypted RL package");
            }
            else
            {
                try
                {
                    var rlStream = new RLPackageStream(packagePath);
                    if (rlStream.decryptionState == DecryptionState.Success)
                    {
                        stream = rlStream;
                    }
                    else
                    {
                        return null;
                    }

                    Log.Info("Loading encrypted RL package");
                }
                catch (InvalidDataException e)
                {
                    stream = new UPackageStream(packagePath, FileMode.Open, fileAccess) { Position = 0 };
                }
            }

            var package = new UnrealPackage(stream);
            package.Deserialize(stream);

            FullyLoadImportPackages(package, Path.GetDirectoryName(packagePath));
            Log.Info($"[LoadPackage] done Loading {packageName}");
            return package;
        }

        /// <summary>
        ///     Loads the given file specified by PackagePath and
        ///     returns the serialized UnrealPackage.
        /// </summary>
        public static UnrealPackage LoadPackage(string packagePath, IBufferDecoder decoder,
            FileAccess fileAccess = FileAccess.Read)
        {
            var stream = new UPackageStream(packagePath, FileMode.Open, fileAccess);
            var package = new UnrealPackage(stream) { Decoder = decoder };
            package.Deserialize(stream);

            return package;
        }

        /// <summary>
        ///     Looks if the package is already loaded before by looking into the CachedPackages list first.
        ///     If it is not found then it loads the given file specified by PackagePath and returns the serialized UnrealPackage.
        /// </summary>
        public static UnrealPackage LoadCachedPackage(string packagePath, FileAccess fileAccess = FileAccess.Read)
        {
            var package = _CachedPackages.Find(pkg => pkg.PackageName == Path.GetFileNameWithoutExtension(packagePath));
            if (package != null)
            {
                return package;
            }

            package = LoadPackage(packagePath, fileAccess);
            if (package != null)
            {
                _CachedPackages.Add(package);
            }

            return package;
        }

        /// <summary>
        ///     Loads the given file specified by PackagePath and
        ///     returns the serialized UnrealPackage with deserialized objects.
        /// </summary>
        public static UnrealPackage LoadFullPackage(string packagePath, FileAccess fileAccess = FileAccess.Read)
        {
            var packageName = Path.GetFileNameWithoutExtension(packagePath);
            _LoadedPackages.TryGetValue(packageName, out var preloadedPackage);
            if (preloadedPackage != null)
            {
                return preloadedPackage;
            }

            var package = LoadPackage(packagePath, fileAccess);
            if (package == null)
            {
                return null;
            }

            Log.Info($"[LoadFullPackage] Loading {packageName}");
            FullyLoadImportPackages(package, Path.GetDirectoryName(packagePath));
            package.InitializePackage();
            _LoadedPackages.Add(packageName, package);
            CacheExportClasses(package);
            Log.Info($"[LoadFullPackage] Done loading {packageName}");

            return package;
        }

        private static void FullyLoadImportPackages(UnrealPackage package, string packageFolder)
        {
            var packagesToLoad =
                package.Imports.Where(i => i.ClassName == "Package" && i.ObjectName != package.PackageName)
                    .Select(i => i.ObjectName.ToString()).ToList();
            foreach (var depPackage in packagesToLoad)
            {
                if (_LoadedPackages.ContainsKey(depPackage))
                {
                    continue;
                }

                var packagePath = Path.Combine(packageFolder, depPackage + ".upk");
                if (!File.Exists(packagePath))
                {
                    continue;
                }

                Log.Info($"FullyLoadImportPackages: {package.PackageName} depends on {depPackage} loading it now!");
                LoadFullPackage(packagePath);
            }
        }
    }
}