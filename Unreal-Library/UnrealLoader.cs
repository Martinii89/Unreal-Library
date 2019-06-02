using System;
using System.Collections.Generic;
using System.IO;
using UELib.Core;
using UELib.Decoding;
using UELib.Logging;

namespace UELib
{
    /// <summary>
    /// Provides static methods for loading unreal packages.
    /// </summary>
    public static class UnrealLoader
    {
        /// <summary>
        /// Stored packages that were imported by certain objects. Kept here that in case re-use is necessary, that it will be loaded faster.
        /// The packages and the list is closed and cleared by the main package that loaded them with ImportObjects().
        /// In any other case the list needs to be cleared manually.
        /// </summary>
        private static readonly List<UnrealPackage> _CachedPackages = new List<UnrealPackage>();
        private static readonly Dictionary<string, UnrealPackage> _LoadedPackages = new Dictionary<string, UnrealPackage>();


        private static UnrealPackage GetPreloadedPackage(string packageName)
        {
            if (_LoadedPackages.ContainsKey(packageName))
            {
                return _LoadedPackages[packageName];
            }
            return null;
        }

        private static Dictionary<Tuple<String, String>, UStruct> FindClassInPackageCache = new Dictionary<Tuple<string, string>, UStruct>();

        /// <summary>
        /// Tries to find the given class in a preloaded package
        /// Caches the results for some better performance
        /// </summary>
        public static UStruct FindClassInPackage(string packageName, string className)
        {
            #region memoization
            var argTuple = Tuple.Create<string, string>(packageName, className);
            UStruct cachedResults;
            FindClassInPackageCache.TryGetValue(argTuple, out cachedResults);
            if (cachedResults != null)
            {
                Log.Info($"Used cached result for {packageName}:{className}");
                return cachedResults;
            }
            #endregion
            var package = GetPreloadedPackage(packageName);
            if (package == null)
            {
                Log.Error($"Package: {packageName} not preloaded. Finding the real class of {className} failed");
                return null;
            }
            var foundClass = package.Objects.Find(o => String.Compare(o.Name, className, StringComparison.OrdinalIgnoreCase) == 0 && o.Class == null);
            var foundStruct = foundClass as UStruct;
            #region memoization
            if (foundStruct != null)
            {
                FindClassInPackageCache.Add(argTuple, foundStruct);
            }else
            {
                Log.Debug($"Did not find {className} in {packageName}");
            }
            #endregion
            return foundStruct;
        } 
        /// <summary>
        /// Loads the given file specified by PackagePath and
        /// returns the serialized UnrealPackage.
        /// </summary>
        public static UnrealPackage LoadPackage( string packagePath, FileAccess fileAccess = FileAccess.Read )
        {
            var packageName = Path.GetFileNameWithoutExtension(packagePath);
            UnrealPackage preloadedPackage;
            _LoadedPackages.TryGetValue(packageName, out preloadedPackage);
            if (preloadedPackage != null)
            {
                return preloadedPackage;
            }

            UPackageStream stream;
            if (packageName.EndsWith("_decrypted"))
            {
                stream = new UPackageStream(packagePath, FileMode.Open, fileAccess);
                Log.Info("Loading decrypted RL package");
            }
            else
            {
                stream = new RLPackageStream(packagePath);
                Log.Info("Loading encrypted RL package");
            }
            var package = new UnrealPackage( stream );
            package.Deserialize( stream );
            _LoadedPackages.Add(packageName, package);
            return package;
        }

        /// <summary>
        /// Loads the given file specified by PackagePath and
        /// returns the serialized UnrealPackage.
        /// </summary>
        public static UnrealPackage LoadPackage( string packagePath, IBufferDecoder decoder, FileAccess fileAccess = FileAccess.Read )
        {
            var stream = new UPackageStream( packagePath, FileMode.Open, fileAccess );
            var package = new UnrealPackage( stream ) {Decoder = decoder};
            package.Deserialize( stream );
            var packageName = Path.GetFileNameWithoutExtension(packagePath);
            _LoadedPackages.Add(packageName, package);
            return package;
        }

        /// <summary>
        /// Looks if the package is already loaded before by looking into the CachedPackages list first.
        /// If it is not found then it loads the given file specified by PackagePath and returns the serialized UnrealPackage.
        /// </summary>
        public static UnrealPackage LoadCachedPackage( string packagePath, FileAccess fileAccess = FileAccess.Read )
        {
            var package = _CachedPackages.Find( pkg => pkg.PackageName == Path.GetFileNameWithoutExtension( packagePath ) );
            if( package != null )
                return package;

            package = LoadPackage( packagePath, fileAccess);
            if( package != null )
            {
                _CachedPackages.Add( package );
            }
            return package;
        }

        /// <summary>
        /// Loads the given file specified by PackagePath and
        /// returns the serialized UnrealPackage with deserialized objects.
        /// </summary>
        public static UnrealPackage LoadFullPackage( string packagePath, FileAccess fileAccess = FileAccess.Read )
        {
            var package = LoadPackage( packagePath, fileAccess );
            if( package != null )
            {
                package.InitializePackage();
            }
            return package;
        }
    }
}