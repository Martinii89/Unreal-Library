using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using UELib.Annotations;
using UELib.Core;
using UELib.Decoding;
using UELib.Flags;
using UELib.Logging;

namespace UELib
{
    /// <summary>
    ///     Represents the method that will handle the UELib.UnrealPackage.NotifyObjectAdded
    ///     event of a new added UELib.Core.UObject.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A UELib.UnrealPackage.ObjectEventArgs that contains the event data.</param>
    public delegate void NotifyObjectAddedEventHandler(object sender, ObjectEventArgs e);

    /// <summary>
    ///     Represents the method that will handle the UELib.UnrealPackage.NotifyPackageEvent
    ///     event of a triggered event within the UELib.UnrealPackage.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A UELib.UnrealPackage.PackageEventArgs that contains the event data.</param>
    public delegate void PackageEventHandler(object sender, UnrealPackage.PackageEventArgs e);

    /// <summary>
    ///     Represents the method that will handle the UELib.UnrealPackage.NotifyInitializeUpdate
    ///     event of a UELib.Core.UObject update.
    /// </summary>
    [PublicAPI]
    public delegate void NotifyUpdateEvent();

    /// <summary>
    ///     Registers the class as an Unreal class. The class's name is required to begin with the letter "U".
    ///     When an Unreal Package is initializing, all described objects will be initialized as the registered class if its
    ///     name matches as described by its export item.
    ///     Note: Usage restricted to the executing assembly(UELib) only!
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class UnrealRegisterClassAttribute : Attribute
    {
    }

    /// <summary>
    ///     Represents data of a loaded unreal package.
    /// </summary>
    public sealed class UnrealPackage : IDisposable, IBuffered
    {
        /// <summary>
        ///     A Collection of flags describing how a package should be initialized.
        /// </summary>
        [Flags]
        [Obfuscation(Exclude = true)]
        public enum InitFlags : ushort
        {
            Construct = 0x0001,
            Deserialize = 0x0002, //           | Construct,
            Import = 0x0004, //           | Serialize,
            Link = 0x0008, //           | Serialize,
            All = RegisterClasses | Construct | Deserialize | Import | Link,
            RegisterClasses = 0x0010
        }

        /// <summary>
        ///     The signature of a 'Unreal Package'.
        /// </summary>
        public const uint Signature = 0x9E2A83C1;

        public const uint Signature_BigEndian = 0xC1832A9E;

        /// <summary>
        ///     64
        /// </summary>
        public const ushort VSIZEPREFIXDEPRECATED = 64;

        /// <summary>
        ///     178
        /// </summary>
        public const ushort VINDEXDEPRECATED = 178;

        /// <summary>
        ///     277
        /// </summary>
        public const ushort VCOOKEDPACKAGES = 277;

        /// <summary>
        ///     DLLBind(Name)
        ///     655
        /// </summary>
        public const ushort VDLLBIND = 655;

        /// <summary>
        ///     New class modifier "ClassGroup(Name[,Name])"
        ///     789
        /// </summary>
        public const ushort VCLASSGROUP = 789;

        private const int VCompression = 334;
        private const int VEngineVersion = 245;
        private const int VGroup = 269;
        private const int VHeaderSize = 249;

        /// <summary>
        ///     For debugging purposes. Change this to override the present Version deserialized from the package.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
        public static ushort OverrideVersion;

        /// <summary>
        ///     For debugging purposes. Change this to override the present Version deserialized from the package.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
        public static ushort OverrideLicenseeVersion;

        /// <summary>
        ///     Class types that should get added to the ObjectsList.
        /// </summary>
        private readonly List<ClassType> _RegisteredClasses = new List<ClassType>();

        // Reference to the stream used when reading this package
        public readonly UPackageStream Stream;

        /// <summary>
        ///     List of heritages. UE1 way of defining generations.
        /// </summary>
        private IList<ushort> _Heritages;

        private ushort _LicenseeVersion;

        private TablesData _TablesData;

        private uint _Version;

        [PublicAPI]
        public IBufferDecoder Decoder;

        /// <summary>
        ///     The group the package is associated with in the Content Browser.
        /// </summary>
        public string Group;

        [PublicAPI]
        public NativesTablePackage NTLPackage;

        /// <summary>
        ///     The bitflags of this package.
        /// </summary>
        public uint PackageFlags;

        /// <summary>
        ///     Creates a new instance of the UELib.UnrealPackage class with a PackageStream and name.
        /// </summary>
        /// <param name="stream">A loaded UELib.PackageStream.</param>
        public UnrealPackage(UPackageStream stream)
        {
            FullPackageName = stream.Name;
            Stream = stream;
            Stream.PostInit(this);

            // File Type
            // Signature is tested in UPackageStream
            IsBigEndianEncoded = stream.BigEndianCode;
        }

        /// <summary>
        ///     The full name of this package including directory.
        /// </summary>
        [PublicAPI]
        public string FullPackageName { get; } = "UnrealPackage";

        [PublicAPI]
        public string PackageName => Path.GetFileNameWithoutExtension(FullPackageName);

        [PublicAPI]
        public string PackageDirectory => Path.GetDirectoryName(FullPackageName);

        public uint Version
        {
            get => OverrideVersion > 0 ? OverrideVersion : _Version;
            private set => _Version = value;
        }

        public ushort LicenseeVersion
        {
            get => OverrideLicenseeVersion > 0 ? OverrideLicenseeVersion : _LicenseeVersion;
            private set => _LicenseeVersion = value;
        }

        public GameBuild Build { get; private set; }

        /// <summary>
        ///     Whether the package was serialized in BigEndian encoding.
        /// </summary>
        public bool IsBigEndianEncoded { get; }

        /// <summary>
        ///     Size of the Header. Basically points to the first Object in the package.
        /// </summary>
        public long HeaderSize { get; private set; }

        /// <summary>
        ///     The guid of this package. Used to test if the package on a client is equal to the one on a server.
        /// </summary>
        [PublicAPI]
        public string GUID { get; private set; }

        /// <summary>
        ///     List of package generations.
        /// </summary>
        [PublicAPI]
        public UArray<UGenerationTableItem> Generations { get; private set; }

        /// <summary>
        ///     The Engine version the package was created with.
        /// </summary>
        [DefaultValue(-1)]
        [PublicAPI]
        public int EngineVersion { get; private set; }

        /// <summary>
        ///     The Cooker version the package was cooked with.
        /// </summary>
        [PublicAPI]
        public int CookerVersion { get; private set; }

        /// <summary>
        ///     The type of compression the package is compressed with.
        /// </summary>
        [PublicAPI]
        public uint CompressionFlags { get; private set; }

        /// <summary>
        ///     List of compressed chunks throughout the package.
        /// </summary>
        [PublicAPI]
        public UArray<CompressedChunk> CompressedChunks { get; private set; }

        /// <summary>
        ///     List of unique unreal names.
        /// </summary>
        [PublicAPI]
        public List<UNameTableItem> Names { get; private set; }

        /// <summary>
        ///     List of info about exported objects.
        /// </summary>
        [PublicAPI]
        public List<UExportTableItem> Exports { get; private set; }

        /// <summary>
        ///     List of info about imported objects.
        /// </summary>
        [PublicAPI]
        public List<UImportTableItem> Imports { get; private set; }

        /// <summary>
        ///     List of UObjects that were constructed by function ConstructObjects, later deserialized and linked.
        ///     Includes Exports and Imports!.
        /// </summary>
        [PublicAPI]
        public List<UObject> Objects { get; private set; }

        public byte[] CopyBuffer()
        {
            var buff = new byte[HeaderSize];
            Stream.Seek(0, SeekOrigin.Begin);
            Stream.Read(buff, 0, (int) HeaderSize);
            if (Stream.BigEndianCode)
            {
                Array.Reverse(buff);
            }

            return buff;
        }

        [System.Diagnostics.Contracts.Pure]
        public IUnrealStream GetBuffer()
        {
            return Stream;
        }

        [System.Diagnostics.Contracts.Pure]
        public int GetBufferPosition()
        {
            return 0;
        }

        [System.Diagnostics.Contracts.Pure]
        public int GetBufferSize()
        {
            return (int) HeaderSize;
        }

        [System.Diagnostics.Contracts.Pure]
        public string GetBufferId(bool fullName = false)
        {
            return fullName ? FullPackageName : PackageName;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Log.Debug($"Disposing {PackageName}");

            DisposeStream();
            if (Objects != null && Objects.Any())
            {
                foreach (var obj in Objects)
                {
                    obj.Dispose();
                }

                Objects.Clear();
                Objects = null;
            }
        }

        [Obsolete]
        [PublicAPI]
        public static UnrealPackage DeserializePackage(string packagePath, FileAccess fileAccess = FileAccess.Read)
        {
            var stream = new UPackageStream(packagePath, FileMode.Open, fileAccess);
            var pkg = new UnrealPackage(stream);
            pkg.Deserialize(stream);
            return pkg;
        }

        public void Serialize(IUnrealStream stream)
        {
            // Serialize tables
            var namesBuffer = new UObjectStream(stream);
            foreach (var name in Names)
            {
                name.Serialize(namesBuffer);
            }

            var importsBuffer = new UObjectStream(stream);
            foreach (var import in Imports)
            {
                import.Serialize(importsBuffer);
            }

            var exportsBuffer = new UObjectStream(stream);
            foreach (var export in Exports)
            {
                //export.Serialize( exportsBuffer );
            }

            stream.Seek(0, SeekOrigin.Begin);

            stream.Write(Signature);

            // Serialize header
            var version = (int) (Version & 0x0000FFFFU) | (LicenseeVersion << 16);
            stream.Write(version);

            var headerSizePosition = stream.Position;
            if (Version >= VHeaderSize)
            {
                stream.Write((int) HeaderSize);
                if (Version >= VGroup)
                {
                    stream.WriteString(Group);
                }
            }

            stream.Write(PackageFlags);

            _TablesData.NamesCount = (uint) Names.Count;
            _TablesData.ExportsCount = (uint) Exports.Count;
            _TablesData.ImportsCount = (uint) Imports.Count;

            var tablesDataPosition = stream.Position;
            _TablesData.Serialize(stream);

            // TODO: Serialize Heritages

            stream.Write(Guid.NewGuid().ToByteArray(), 0, 16);
            Generations.Serialize(stream);

            if (Version >= VEngineVersion)
            {
                stream.Write(EngineVersion);
                if (Version >= VCOOKEDPACKAGES)
                {
                    stream.Write(CookerVersion);

                    if (Version >= VCompression)
                    {
                        if (IsCooked())
                        {
                            stream.Write(CompressionFlags);
                            CompressedChunks.Serialize(stream);
                        }
                    }
                }
            }

            // TODO: Unknown data
            stream.Write((uint) 0);

            // Serialize objects

            // Write tables

            // names
            Log.Info("Writing names at position " + stream.Position);
            _TablesData.NamesOffset = (uint) stream.Position;
            var namesBytes = namesBuffer.GetBuffer();
            stream.Write(namesBytes, 0, (int) namesBuffer.Length);

            // imports
            Log.Info("Writing imports at position " + stream.Position);
            _TablesData.ImportsOffset = (uint) stream.Position;
            var importsBytes = importsBuffer.GetBuffer();
            stream.Write(importsBytes, 0, (int) importsBuffer.Length);

            // exports
            Log.Info("Writing exports at position " + stream.Position);

            // Serialize tables data again now that offsets are known.
            var currentPosition = stream.Position;
            stream.Seek(tablesDataPosition, SeekOrigin.Begin);
            _TablesData.Serialize(stream);
            stream.Seek(currentPosition, SeekOrigin.Begin);
        }

        public void Deserialize(UPackageStream stream)
        {
            // Read as one variable due Big Endian Encoding.
            Version = stream.ReadUInt32();
            LicenseeVersion = (ushort) (Version >> 16);
            Version = Version & 0xFFFFU;
            Log.Info("\tPackage Version:" + Version + "/" + LicenseeVersion);

            Build = new GameBuild(this);
            Log.Info("\tBuild:" + Build.Name);

            stream.BuildDetected(Build);

            if (Version >= VHeaderSize)
            {
                // Offset to the first class(not object) in the package.
                HeaderSize = stream.ReadUInt32();
                Log.Info("\tHeader Size: " + HeaderSize);
                if (Version >= VGroup)
                {
                    // UPK content category e.g. Weapons, Sounds or Meshes.
                    Group = stream.ReadText();
                }
            }

            // Bitflags such as AllowDownload.
            PackageFlags = stream.ReadUInt32();
            Log.Info("\tPackage Flags:" + PackageFlags);

            // Summary data such as ObjectCount.
            _TablesData = new TablesData();
            _TablesData.Deserialize(stream);
            if (Version < 68)
            {
                var heritageCount = stream.ReadInt32();
                var heritageOffset = stream.ReadInt32();

                stream.Seek(heritageOffset, SeekOrigin.Begin);
                _Heritages = new List<ushort>(heritageCount);
                for (var i = 0; i < heritageCount; ++i)
                {
                    _Heritages.Add(stream.ReadUShort());
                }
            }
            else
            {
                GUID = stream.ReadGuid();
                Log.Info("\r\n\tGUID:" + GUID + "\r\n");


                var generationCount = stream.ReadInt32();
                Generations = new UArray<UGenerationTableItem>(stream, generationCount);
                Log.Debug($"Deserialized {Generations.Count} generations");


                if (Version >= VEngineVersion)
                {
                    // The Engine Version this package was created with
                    EngineVersion = stream.ReadInt32();
                    Log.Info("\tEngineVersion:" + EngineVersion);
                    if (Version >= VCOOKEDPACKAGES)
                    {
                        // The Cooker Version this package was cooked with
                        CookerVersion = stream.ReadInt32();
                        Log.Info("\tCookerVersion:" + CookerVersion);

                        // Read compressed info?
                        if (Version >= VCompression)
                        {
                            if (IsCooked())
                            {
                                CompressionFlags = stream.ReadUInt32();
                                Log.Info("\tCompressionFlags:" + CompressionFlags);
                                CompressedChunks = new UArray<CompressedChunk> { Capacity = stream.ReadInt32() };
                                //long uncookedSize = stream.Position;
                                if (CompressedChunks.Capacity > 0)
                                {
                                    CompressedChunks.Deserialize(stream, CompressedChunks.Capacity);
                                    return;

                                    //try
                                    //{
                                    //    UPackageStream outStream = new UPackageStream( packagePath + ".dec", System.IO.FileMode.Create, FileAccess.ReadWrite );
                                    //    //File.SetAttributes( packagePath + ".dec", FileAttributes.Temporary );
                                    //    outStream.Package = pkg;
                                    //    outStream._BigEndianCode = stream._BigEndianCode;

                                    //    var headerBytes = new byte[uncookedSize];
                                    //    stream.Seek( 0, SeekOrigin.Begin );
                                    //    stream.Read( headerBytes, 0, (int)uncookedSize );
                                    //    outStream.Write( headerBytes, 0, (int)uncookedSize );
                                    //    foreach( var chunk in pkg.CompressedChunks )
                                    //    {
                                    //        chunk.Decompress( stream, outStream );
                                    //    }
                                    //    outStream.Flush();
                                    //    pkg.Stream = outStream;
                                    //    stream = outStream;
                                    //    return pkg;
                                    //}
                                    //catch( Exception e )
                                    //{
                                    //    throw new DecompressPackageException();
                                    //}
                                }
                            }
                        }
                    }
                }
            }

            // Read the name table
            if (_TablesData.NamesCount > 0)
            {
                Log.Debug($"stream position: {stream.Position} Names offset: {_TablesData.NamesOffset}");

                stream.Seek(_TablesData.NamesOffset, SeekOrigin.Begin);
                Names = new List<UNameTableItem>((int) _TablesData.NamesCount);
                for (var i = 0; i < _TablesData.NamesCount; ++i)
                {
                    var nameEntry = new UNameTableItem { Offset = (int) stream.Position, Index = i };
                    nameEntry.Deserialize(stream);
                    nameEntry.Size = (int) (stream.Position - nameEntry.Offset);
                    Names.Add(nameEntry);
                }

                Log.Debug($"Deserialized {Names.Count} names");
            }

            // Read Import Table
            if (_TablesData.ImportsCount > 0)
            {
                Log.Debug($"stream position: {stream.Position} Imports offset: {_TablesData.ImportsOffset}");

                stream.Seek(_TablesData.ImportsOffset, SeekOrigin.Begin);
                Imports = new List<UImportTableItem>((int) _TablesData.ImportsCount);
                for (var i = 0; i < _TablesData.ImportsCount; ++i)
                {
                    var imp = new UImportTableItem { Offset = (int) stream.Position, Index = i, Owner = this };
                    imp.Deserialize(stream);
                    imp.Size = (int) (stream.Position - imp.Offset);
                    Imports.Add(imp);
                }

                Log.Debug($"Deserialized {Imports.Count} imports");
            }

            // Read Export Table
            if (_TablesData.ExportsCount > 0)
            {
                Log.Debug($"stream position: {stream.Position} Exports offset: {_TablesData.ExportsOffset}");

                stream.Seek(_TablesData.ExportsOffset, SeekOrigin.Begin);
                Exports = new List<UExportTableItem>((int) _TablesData.ExportsCount);
                for (var i = 0; i < _TablesData.ExportsCount; ++i)
                {
                    var exp = new UExportTableItem { Offset = (int) stream.Position, Index = i, Owner = this };
                    // For the GetObjectName like functions
                    try
                    {
                        exp.Deserialize(stream);
                    }
                    catch
                    {
                        Log.Error("Failed to deserialize export object at index:" + i);
                        break;
                    }
                    finally
                    {
                        exp.Size = (int) (stream.Position - exp.Offset);
                        Exports.Add(exp);
                    }
                }

                Log.Debug($"Deserialized {Exports.Count} exports");
            }

            /*if( pkg.Data.DependsOffset > 0 )
            {
                stream.Seek( pkg.Data.DependsOffset, SeekOrigin.Begin );
                pkg._DependsTableList = new List<UnrealDependsTable>( (int)pkg.Data.DependsCount );
                for( var i = 0; i < pkg.Data.DependsCount; ++ i )
                {
                    var dep = new UnrealDependsTable{TableOffset = stream.Position, TableIndex = i, Owner = pkg};
                    dep.Deserialize( stream );
                    dep.TableSize = (int)(stream.Position - dep.TableOffset);
                    pkg.DependsTableList.Add( dep );
                }
                Log.WriteLine( "Deserialized {0} dependencies", pkg.DependsTableList.Count );
            }*/

            HeaderSize = stream.Position;
        }

        /// <summary>
        ///     Constructs all export objects.
        /// </summary>
        /// <param name="initFlags">Initializing rules such as deserializing and/or linking.</param>
        [PublicAPI]
        public void InitializeExportObjects(InitFlags initFlags = InitFlags.All)
        {
            Objects = new List<UObject>(Exports.Count);
            foreach (var exp in Exports)
            {
                CreateObjectForTable(exp);
            }

            if ((initFlags & InitFlags.Deserialize) == 0)
            {
                return;
            }

            DeserializeObjects();
            if ((initFlags & InitFlags.Link) != 0)
            {
                LinkObjects();
            }
        }

        /// <summary>
        ///     Constructs all import objects.
        /// </summary>
        /// <param name="initialize">If TRUE initialize all constructed objects.</param>
        [PublicAPI]
        public void InitializeImportObjects(bool initialize = true)
        {
            Objects = new List<UObject>(Imports.Count);
            foreach (var imp in Imports)
            {
                CreateObjectForTable(imp);
            }

            if (!initialize)
            {
                return;
            }

            foreach (var obj in Objects)
            {
                obj.PostInitialize();
            }
        }

        /// <summary>
        ///     Initializes all the objects that resist in this package as well tries to import deserialized data from imported
        ///     objects.
        /// </summary>
        /// <param name="initFlags">A collection of initializing flags to notify what should be initialized.</param>
        /// <example>InitializePackage( UnrealPackage.InitFlags.All )</example>
        [PublicAPI]
        public void InitializePackage(InitFlags initFlags = InitFlags.All)
        {
            if ((initFlags & InitFlags.RegisterClasses) != 0)
            {
                RegisterAllClasses();
            }

            if ((initFlags & InitFlags.Construct) == 0)
            {
                return;
            }

            ConstructObjects();
            if ((initFlags & InitFlags.Deserialize) == 0)
            {
                return;
            }

            try
            {
                DeserializeObjects();
            }
            catch
            {
                throw new DeserializingObjectsException();
            }

            try
            {
                if ((initFlags & InitFlags.Import) != 0)
                {
                    ImportObjects();
                }
            }
            catch (Exception e)
            {
                //can be treat with as a warning!
                throw new Exception("An exception occurred while importing objects", e);
            }

            try
            {
                if ((initFlags & InitFlags.Link) != 0)
                {
                    LinkObjects();
                }
            }
            catch
            {
                throw new LinkingObjectsException();
            }
            //DisposeStream();
        }

        /// <summary>
        /// </summary>
        [PublicAPI]
        public event PackageEventHandler NotifyPackageEvent;

        private void OnNotifyPackageEvent(PackageEventArgs e)
        {
            if (NotifyPackageEvent != null)
            {
                NotifyPackageEvent.Invoke(this, e);
            }
        }

        /// <summary>
        ///     Called when an object is added to the ObjectsList via the AddObject function.
        /// </summary>
        [PublicAPI]
        public event NotifyObjectAddedEventHandler NotifyObjectAdded;

        /// <summary>
        ///     Constructs all the objects based on data from _ExportTableList and _ImportTableList, and
        ///     all constructed objects are added to the _ObjectsList.
        /// </summary>
        private void ConstructObjects()
        {
            Objects = new List<UObject>();
            OnNotifyPackageEvent(new PackageEventArgs(PackageEventArgs.Id.Construct));
            foreach (var exp in Exports)
            {
                try
                {
                    CreateObjectForTable(exp);
                }
                catch (Exception exc)
                {
                    throw new UnrealException("couldn't create export object for " + exp, exc);
                }
            }

            //TODO: Fix later. focus on mesh data now
            // Ugly hack, but idk how else to make sure every object has a known class.
            //if (PackageName == "Core")
            //{
            //    UName className = new UName(this, "Class");
            //    UName objectName = new UName(this, "Package");
            //    UName classPackage = new UName(this, "Core");
            //    int outerIndex = -Imports.First(i => i.ObjectName == "Core").Index - 1;
            //    var fakePackageImport = new FakeImportTableItem(this, classPackage, className, outerIndex, objectName);
            //    Imports.Add(fakePackageImport);
            //}

            foreach (var imp in Imports)
            {
                try
                {
                    CreateObjectForTable(imp);
                }
                catch (Exception exc)
                {
                    throw new UnrealException("couldn't create import object for " + imp, exc);
                }
            }
        }

        /// <summary>
        ///     Deserializes all exported objects.
        /// </summary>
        private void DeserializeObjects()
        {
            // Only exports should be deserialized and PostInitialized!
            OnNotifyPackageEvent(new PackageEventArgs(PackageEventArgs.Id.Deserialize));
            foreach (var exp in Exports)
            {
                if (!(exp.Object is UnknownObject || exp.Object.ShouldDeserializeOnDemand))
                {
                    //Log.WriteLine( "Deserializing object:" + exp.ObjectName );
                    exp.Object.BeginDeserializing();
                }

                OnNotifyPackageEvent(new PackageEventArgs(PackageEventArgs.Id.Object));
            }
        }

        /// <summary>
        ///     Tries to import necessary deserialized data from imported objects.
        /// </summary>
        private void ImportObjects()
        {
            // TODO:Figure out why this freezes.
            /*OnNotifyPackageEvent( new PackageEventArgs( PackageEventArgs.Id.Import ) );
            foreach( UnrealImportTable Imp in _ImportTableList )
            {
                if( !(Imp.Object.GetType() == typeof(UnknownObject)) )
                {
                    Imp.Object.InitializeImports();
                }
                OnNotifyPackageEvent( new PackageEventArgs( PackageEventArgs.Id.Object ) );
            }
            UnrealLoader.CachedPackages.Clear();*/
        }

        /// <summary>
        ///     Initializes all exported objects.
        /// </summary>
        private void LinkObjects()
        {
            // Notify that deserializing is done on all objects, now objects can read properties that were dependent on deserializing
            OnNotifyPackageEvent(new PackageEventArgs(PackageEventArgs.Id.Link));
            foreach (var exp in Exports)
            {
                try
                {
                    if (!(exp.Object is UnknownObject))
                    {
                        exp.Object.PostInitialize();
                    }

                    OnNotifyPackageEvent(new PackageEventArgs(PackageEventArgs.Id.Object));
                }
                catch (InvalidCastException)
                {
                    Log.Error("InvalidCastException occurred on object: " + exp.Object);
                }
            }
        }

        private void RegisterAllClasses()
        {
            var exportedTypes = Assembly.GetExecutingAssembly().GetExportedTypes();
            foreach (var exportedType in exportedTypes)
            {
                var attributes = exportedType.GetCustomAttributes(typeof(UnrealRegisterClassAttribute), false);
                if (attributes.Length == 1)
                {
                    RegisterClass(exportedType.Name.Substring(1), exportedType);
                }
            }
        }

        [System.Diagnostics.Contracts.Pure] private Type GetClassTypeByClassName(string className)
        {
            return _RegisteredClasses.FirstOrDefault(registered => string.Compare(registered.Name, className, StringComparison.OrdinalIgnoreCase) == 0).Class;
        }


        private void CreateObjectForTable(UObjectTableItem table)
        {
            var objectType = GetClassTypeByClassName(table.ClassName);
            //TODO: Fix later. focus on mesh data for now.
            /*
            if (table is UExportTableItem export)
            {
                if (export.ClassIndex == UCLASS_INDEX)
                {
                    var corePackage = PackageName == "Core" ? this : UnrealLoader.LoadCachedPackage($"{Path.GetDirectoryName(FullPackageName)}\\Core.upk");
                    if (corePackage != null)
                    {
                        var classImport = corePackage.Imports.First(im => im.ClassName == "Class" && im.ObjectName == "Class");
                        export.ClassTable = classImport;
                    }
                }
                else
                {
                    export.ClassTable = export.Owner.GetIndexTable(export.ClassIndex);
                }
            }else if (table is UImportTableItem import)
            {
                import.LoadImportClass();
            }
            */
            table.Object = objectType == null ? new UnknownObject() : (UObject) Activator.CreateInstance(objectType);
            AddObject(table.Object, table);
            OnNotifyPackageEvent(new PackageEventArgs(PackageEventArgs.Id.Object));
        }

        private void AddObject(UObject obj, UObjectTableItem T)
        {
            T.Object = obj;
            obj.Package = this;
            obj.Table = T;

            Objects.Add(obj);
            if (NotifyObjectAdded != null)
            {
                NotifyObjectAdded.Invoke(this, new ObjectEventArgs(obj));
            }
        }

        /*

        private UObject CreateImportExport(int index)
        {
            return index >= 0 ? CreateExport(index) : CreateImport(index);
        }

        const int UCLASS_INDEX = 0;

        private UObjectTableItem GetClassTableItem(int classIndex)
        {
            if (classIndex == UCLASS_INDEX)
            {
                // UClass
                return null;
            }

            if (classIndex > 0)
            {
                //ExportItem
                return CreateExport(classIndex - 1)?.Table;
            }

            return CreateImport(classIndex)?.Table;
        }

        private UObject CreateExport(int exportIndex)
        {
            Debug.Assert(exportIndex >= 0, "Invalid export");
            var objectExport = Exports[exportIndex];
            if (objectExport.Object != null)
            {
                return objectExport.Object;
            }


            var classTableItem = GetClassTableItem(objectExport.ClassIndex);
            if (classTableItem != null)
            {
                objectExport.ClassTable = classTableItem;
            }

            var objectType = GetClassTypeByClassName(objectExport.ClassName);
            objectExport.Object = objectType == null ? new UnknownObject() : (UObject)Activator.CreateInstance(objectType);
            AddObject(objectExport.Object, objectExport);
            OnNotifyPackageEvent(new PackageEventArgs(PackageEventArgs.Id.Object));

            return objectExport.Object;
        }

        public UObject CreateImport(int importIndex)
        {
            Debug.Assert(importIndex < 0, "Invalid import");
            var objectImport = Imports[-importIndex-1];
            if (objectImport.Object != null)
            {
                return objectImport.Object;
            }

            //var classTableItem = GetClassTableItem(objectImport.ClassIndex);
            //if (classTableItem != null)
            //{
            //    objectImport.ClassTable = classTableItem;
            //}

            var objectType = GetClassTypeByClassName(objectImport.ClassName);
            objectImport.Object = objectType == null ? new UnknownObject() : (UObject)Activator.CreateInstance(objectType);
            AddObject(objectImport.Object, objectImport);
            OnNotifyPackageEvent(new PackageEventArgs(PackageEventArgs.Id.Object));

            return objectImport.Object;
        }
        */


        /// <summary>
        ///     Writes the present PackageFlags to disk. HardCoded!
        ///     Only supports UT2004.
        /// </summary>
        public void WritePackageFlags()
        {
            Stream.Position = 8;
            Stream.UW.Write(PackageFlags);
        }

        [PublicAPI]
        public void RegisterClass(string className, Type classObject)
        {
            var obj = new ClassType { Name = className, Class = classObject };
            _RegisteredClasses.Add(obj);
        }

        /// <summary>
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public bool IsRegisteredClass(string className)
        {
            return _RegisteredClasses.Exists(o => o.Name.ToLower() == className.ToLower());
        }

        /// <summary>
        ///     Returns an Object that resides at the specified ObjectIndex.
        ///     if index is positive an exported Object will be returned.
        ///     if index is negative an imported Object will be returned.
        ///     if index is zero null will be returned.
        /// </summary>
        /// <param name="objectIndex">The index of the Object in a tablelist.</param>
        /// <returns>The found UELib.Core.UObject if any.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public UObject GetIndexObject(int objectIndex)
        {
            return objectIndex < 0 ? Imports[-objectIndex - 1].Object
                : objectIndex > 0 ? Exports[objectIndex - 1].Object
                : null;
        }

        /// <summary>
        ///     Returns an Object name that resides at the specified ObjectIndex.
        /// </summary>
        /// <param name="objectIndex">The index of the object in a tablelist.</param>
        /// <returns>The found UELib.Core.UObject name if any.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public string GetIndexObjectName(int objectIndex)
        {
            return GetIndexTable(objectIndex).ObjectName;
        }

        /// <summary>
        ///     Returns a name that resides at the specified NameIndex.
        /// </summary>
        /// <param name="nameIndex">A NameIndex into the NameTableList.</param>
        /// <returns>The name at specified NameIndex.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public string GetIndexName(int nameIndex)
        {
            return Names[nameIndex].Name;
        }

        /// <summary>
        ///     Returns an UnrealTable that resides at the specified TableIndex.
        ///     if index is positive an ExportTable will be returned.
        ///     if index is negative an ImportTable will be returned.
        ///     if index is zero null will be returned.
        /// </summary>
        /// <param name="tableIndex">The index of the Table.</param>
        /// <returns>The found UELib.Core.UnrealTable if any.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public UObjectTableItem GetIndexTable(int tableIndex)
        {
            return tableIndex < 0 ? Imports[-tableIndex - 1]
                : tableIndex > 0 ? (UObjectTableItem) Exports[tableIndex - 1]
                : null;
        }

        /// <summary>
        ///     Tries to find an UELib.Core.UObject with a specified name and type.
        /// </summary>
        /// <param name="objectName">The name of the object to find.</param>
        /// <param name="type">The type of the object to find.</param>
        /// <param name="checkForSubclass">Whether to test for subclasses of type as well.</param>
        /// <returns>The found UELib.Core.UObject if any.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public UObject FindObject(string objectName, Type type, bool checkForSubclass = false)
        {
            if (Objects == null)
            {
                return null;
            }

            var obj = Objects.Find(o => string.Compare(o.Name, objectName, StringComparison.OrdinalIgnoreCase) == 0 &&
                                        (checkForSubclass ? o.GetType().IsSubclassOf(type) : o.GetType() == type));
            return obj;
        }

        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public UObject FindObjectByGroup(string objectGroup)
        {
            var groups = objectGroup.Split('.');
            UObject lastObj = null;
            for (var i = 0; i < groups.Length; ++i)
            {
                var obj = Objects.Find(o => string.Compare(o.Name, groups[i], StringComparison.OrdinalIgnoreCase) == 0 && o.Outer == lastObj);
                if (obj != null)
                {
                    lastObj = obj;
                }
                else
                {
                    lastObj = Objects.Find(o => string.Compare(o.Name, groups[i], StringComparison.OrdinalIgnoreCase) == 0);
                    break;
                }
            }

            return lastObj;
        }

        /// <summary>
        ///     Checks whether this package is marked with @flag.
        /// </summary>
        /// <param name="flag">The enum @flag to test.</param>
        /// <returns>Whether this package is marked with @flag.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public bool HasPackageFlag(PackageFlags flag)
        {
            return (PackageFlags & (uint) flag) != 0;
        }

        /// <summary>
        ///     Checks whether this package is marked with @flag.
        /// </summary>
        /// <param name="flag">The uint @flag to test</param>
        /// <returns>Whether this package is marked with @flag.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public bool HasPackageFlag(uint flag)
        {
            return (PackageFlags & flag) != 0;
        }

        /// <summary>
        ///     Tests the packageflags of this UELib.UnrealPackage instance whether it is cooked.
        /// </summary>
        /// <returns>True if cooked or False if not.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public bool IsCooked()
        {
            return HasPackageFlag(Flags.PackageFlags.Cooked) && Version >= VCOOKEDPACKAGES;
        }

        /// <summary>
        ///     Tests the package for console build indications.
        /// </summary>
        /// <returns>Whether package is cooked for consoles.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public bool IsConsoleCooked()
        {
            return IsCooked() && (IsBigEndianEncoded || Build.IsConsoleCompressed) && !Build.IsXenonCompressed;
        }

        /// <summary>
        ///     Checks for the Map flag in PackageFlags.
        /// </summary>
        /// <returns>Whether if this package is a map.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public bool IsMap()
        {
            return HasPackageFlag(Flags.PackageFlags.Map);
        }

        /// <summary>
        ///     Checks if this package contains code classes.
        /// </summary>
        /// <returns>Whether if this package contains code classes.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public bool IsScript()
        {
            return HasPackageFlag(Flags.PackageFlags.Script);
        }

        /// <summary>
        ///     Checks if this package was built using the debug configuration.
        /// </summary>
        /// <returns>Whether if this package was built in debug configuration.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public bool IsDebug()
        {
            return HasPackageFlag(Flags.PackageFlags.Debug);
        }

        /// <summary>
        ///     Checks for the Stripped flag in PackageFlags.
        /// </summary>
        /// <returns>Whether if this package is stripped.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public bool IsStripped()
        {
            return HasPackageFlag(Flags.PackageFlags.Stripped);
        }

        /// <summary>
        ///     Tests the packageflags of this UELib.UnrealPackage instance whether it is encrypted.
        /// </summary>
        /// <returns>True if encrypted or False if not.</returns>
        [PublicAPI]
        [System.Diagnostics.Contracts.Pure] public bool IsEncrypted()
        {
            return HasPackageFlag(Flags.PackageFlags.Encrypted);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return PackageName;
        }

        private void DisposeStream()
        {
            if (Stream == null)
            {
                return;
            }

            Log.Debug("Disposing package stream");
            Stream.Dispose();
        }

        public sealed class GameBuild
        {
            public enum BuildName
            {
                Unset,
                Default,
                Unknown,

                /// <summary>
                ///     61/000
                /// </summary>
                [Build(61, 0)]
                Unreal1,

                /// <summary>
                ///     68:69/000
                /// </summary>
                [Build(68, 69, 0u, 0u)]
                UT,

                /// <summary>
                ///     95/69
                /// </summary>
                [Build(95, 69)]
                DeusEx_IW,

                /// <summary>
                ///     95/133
                /// </summary>
                [Build(95, 133)]
                Thief_DS,

                /// <summary>
                ///     99:117/005:008
                /// </summary>
                [Build(99, 117, 5u, 8u)]
                UT2003,

                /// <summary>
                ///     100/058
                /// </summary>
                [Build(100, 58)]
                XIII,

                /// <summary>
                ///     110/2609
                /// </summary>
                [Build(110, 2609)]
                Unreal2,

                /// <summary>
                ///     118/025:029
                /// </summary>
                [Build(118, 128, 25u, 29u)]
                UT2004,

                /// <summary>
                ///     129/027
                /// </summary>
                [Build(129, 27)]
                Swat4,

                /// <summary>
                ///     129/035
                /// </summary>
                [Build(129, 35)]
                Vanguard,

                /// <summary>
                ///     130:143/056:059
                /// </summary>
                [Build(130, 143, 56u, 59u, 0, 0)]
                Bioshock,

                // IrrationalGames - 129:143/027:059

                /// <summary>
                ///     369/006
                /// </summary>
                [Build(369, 6)]
                RoboBlitz,

                /// <summary>
                ///     421/011
                /// </summary>
                [Build(421, 11)]
                MOHA,

                /// <summary>
                ///     472/046
                /// </summary>
                [Build(472, 46, 1)]
                MKKE,

                /// <summary>
                ///     490/009
                /// </summary>
                [Build(490, 9)]
                GoW1,

                /// <summary>
                ///     512/000
                /// </summary>
                [Build(512, 0)]
                UT3,

                /// <summary>
                ///     536/043
                /// </summary>
                [Build(536, 43)]
                MirrorsEdge,

                /// <summary>
                ///     539/091
                /// </summary>
                [Build(539, 91)]
                AlphaProtcol,

                /// <summary>
                ///     547/028:032
                /// </summary>
                [Build(547, 547, 28u, 32u)]
                APB,

                /// <summary>
                ///     575/000
                /// </summary>
                [Build(575, 0, 0, 1)]
                GoW2,

                /// <summary>
                ///     576/005
                /// </summary>
                [Build(576, 5)]
                CrimeCraft,

                /// <summary>
                ///     576/100
                /// </summary>
                [Build(576, 100)]
                Homefront,

                /// <summary>
                ///     584/058
                /// </summary>
                [Build(584, 58)]
                Borderlands,

                /// <summary>
                ///     584/126
                /// </summary>
                [Build(584, 126)]
                Singularity,

                /// <summary>
                ///     590/001
                /// </summary>
                [Build(590, 1, 0, 1)]
                ShadowComplex,

                /// <summary>
                ///     610/014
                /// </summary>
                [Build(610, 14)]
                Tera,

                /// <summary>
                ///     727/075
                /// </summary>
                [Build(727, 75)]
                Bioshock_Infinite,

                /// <summary>
                ///     742/029
                /// </summary>
                [Build(742, 29)]
                BulletStorm,

                /// <summary>
                ///     801/030
                /// </summary>
                [Build(801, 30)]
                Dishonored,

                /// <summary>
                ///     828/000
                /// </summary>
                [Build(828, 0)]
                InfinityBlade,

                /// <summary>
                ///     828/000
                /// </summary>
                [Build(828, 0)]
                GoW3,

                /// <summary>
                ///     832/021
                /// </summary>
                [Build(832, 21)]
                RememberMe,

                /// <summary>
                ///     832/046
                /// </summary>
                [Build(832, 46)]
                Borderlands2,

                /// <summary>
                ///     842/001
                /// </summary>
                [Build(842, 1, 1)]
                InfinityBlade2,

                /// <summary>
                ///     845/059
                /// </summary>
                [Build(845, 59)]
                XCOM_EU,

                /// <summary>
                ///     846/181
                /// </summary>
                [Build(511, 039)] // The Bourne Conspiracy
                [Build(511, 145)] // Transformers: War for Cybertron (PC version)
                [Build(511, 144)] // Transformers: War for Cybertron (PS3 and XBox 360 version)
                [Build(537, 174)] // Transformers: Dark of the Moon
                [Build(846, 181, 2, 1)] // Transformers: Fall of Cybertron
                Transformers,

                /// <summary>
                ///     860/004
                /// </summary>
                [Build(860, 4)]
                Hawken,

                /// <summary>
                ///     904/009
                /// </summary>
                [Build(904, 904, 09u, 014u, 0, 0)]
                SpecialForce2,

                /// <summary>
                ///     845/120
                /// </summary>
                [Build(845, 120)]
                XCOM2WotC
            }

            public GameBuild(UnrealPackage package)
            {
                if (UnrealConfig.Platform == UnrealConfig.CookedPlatform.Console)
                {
                    IsConsoleCompressed = true;
                }

                var gameBuilds = Enum.GetValues(typeof(BuildName)) as BuildName[];
                foreach (var gameBuild in gameBuilds)
                {
                    var gameBuildMember = typeof(BuildName).GetMember(gameBuild.ToString());
                    if (gameBuildMember.Length == 0)
                    {
                        continue;
                    }

                    var attribs = gameBuildMember[0].GetCustomAttributes(false);
                    var game = attribs.OfType<BuildAttribute>().SingleOrDefault(attr => attr.Verify(this, package));
                    if (game == null)
                    {
                        continue;
                    }

                    Name = (BuildName) Enum.Parse(typeof(BuildName), Enum.GetName(typeof(BuildName), gameBuild));
                    if (package.Decoder != null)
                    {
                        break;
                    }

                    var buildDecoderAttr = attribs.SingleOrDefault(attr => attr is BuildDecoderAttribute) as BuildDecoderAttribute;
                    if (buildDecoderAttr == null)
                    {
                        break;
                    }

                    package.Decoder = buildDecoderAttr.CreateDecoder();
                    break;
                }

                if (Name == BuildName.Unset)
                {
                    Name = package.LicenseeVersion == 0 ? BuildName.Default : BuildName.Unknown;
                }
            }

            public BuildName Name { get; }

            public uint Version { get; private set; }
            public uint LicenseeVersion { get; private set; }

            /// <summary>
            ///     Is cooked for consoles.
            /// </summary>
            public bool IsConsoleCompressed { get; private set; }

            /// <summary>
            ///     Is cooked for Xenon(Xbox 360). Could be true on PC games.
            /// </summary>
            public bool IsXenonCompressed { get; private set; }

            public static bool operator ==(GameBuild b, BuildName i)
            {
                return b != null && b.Name == i;
            }

            public static bool operator !=(GameBuild b, BuildName i)
            {
                return b != null && b.Name != i;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return Name == (BuildName) obj;
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return (int) Name;
            }

            [UsedImplicitly]
            [AttributeUsage(AttributeTargets.Field)]
            private sealed class BuildDecoderAttribute : Attribute
            {
                private readonly Type _BuildDecoder;

                public BuildDecoderAttribute(Type buildDecoder)
                {
                    _BuildDecoder = buildDecoder;
                }

                public IBufferDecoder CreateDecoder()
                {
                    return (IBufferDecoder) Activator.CreateInstance(_BuildDecoder);
                }
            }

            [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
            private sealed class BuildAttribute : Attribute
            {
                private readonly byte _IsConsoleCompressed;
                private readonly byte _IsXenonCompressed;
                private readonly uint _MaxLicensee;
                private readonly int _MaxVersion;
                private readonly uint _MinLicensee;
                private readonly int _MinVersion;

                private readonly bool _VerifyEqual;

                public BuildAttribute(int minVersion, uint minLicensee,
                    byte isConsoleCompressed = 2, byte isXenonCompressed = 2)
                {
                    _MinVersion = minVersion;
                    _MinLicensee = minLicensee;
                    _IsConsoleCompressed = isConsoleCompressed;
                    _IsXenonCompressed = isXenonCompressed;
                    _VerifyEqual = true;
                }

                public BuildAttribute(int minVersion, int maxVersion, uint minLicensee, uint maxLicensee,
                    byte isConsoleCompressed = 2, byte isXenonCompressed = 2)
                {
                    _MinVersion = minVersion;
                    _MaxVersion = maxVersion;
                    _MinLicensee = minLicensee;
                    _MaxLicensee = maxLicensee;
                    _IsConsoleCompressed = isConsoleCompressed;
                    _IsXenonCompressed = isXenonCompressed;
                }

                public bool Verify(GameBuild gb, UnrealPackage package)
                {
                    if (_VerifyEqual
                            ? package.Version != _MinVersion || package.LicenseeVersion != _MinLicensee
                            : package.Version < _MinVersion || package.Version > _MaxVersion ||
                              package.LicenseeVersion < _MinLicensee || package.LicenseeVersion > _MaxLicensee)
                    {
                        return false;
                    }

                    gb.Version = package.Version;
                    gb.LicenseeVersion = package.LicenseeVersion;

                    if (_IsConsoleCompressed < 2)
                    {
                        gb.IsConsoleCompressed = _IsConsoleCompressed == 1;
                    }

                    if (_IsXenonCompressed < 2)
                    {
                        gb.IsXenonCompressed = _IsXenonCompressed == 1;
                    }

                    return true;
                }
            }
        }

        private struct TablesData : IUnrealSerializableClass
        {
            public uint NamesCount { get; internal set; }
            public uint NamesOffset { get; internal set; }

            public uint ExportsCount { get; internal set; }
            public uint ExportsOffset { get; internal set; }

            public uint ImportsCount { get; internal set; }
            public uint ImportsOffset { get; internal set; }

            public uint DependsOffset { get; internal set; }
            public uint DependsCount => ExportsCount;

            public void Serialize(IUnrealStream stream)
            {
                stream.Write(NamesCount);
                stream.Write(NamesOffset);

                stream.Write(ExportsCount);
                stream.Write(ExportsOffset);

                stream.Write(ImportsCount);
                stream.Write(ImportsOffset);

                if (stream.Version < 415)
                {
                    return;
                }

                // DependsOffset
                stream.Write(DependsOffset);
            }

            public void Deserialize(IUnrealStream stream)
            {
                NamesCount = stream.ReadUInt32();
                NamesOffset = stream.ReadUInt32();
                ExportsCount = stream.ReadUInt32();
                ExportsOffset = stream.ReadUInt32();

                ImportsCount = stream.ReadUInt32();
                ImportsOffset = stream.ReadUInt32();

                Log.Info("\tNames Count:" + NamesCount + "\tNames Offset:" + NamesOffset
                         + "\r\n\tExports Count:" + ExportsCount + "\tExports Offset:" + ExportsOffset
                         + "\r\n\tImports Count:" + ImportsCount + "\tImports Offset:" + ImportsOffset);

                if (stream.Version < 415)
                {
                    return;
                }

                DependsOffset = stream.ReadUInt32();
                if (stream.Version >= 584)
                {
                    // Additional tables, like thumbnail, and guid data.
                    if (stream.Version >= 623)
                    {
                        stream.Skip(12);
                    }

                    stream.Skip(4);
                }
            }
        }

        /// <summary>
        ///     List of info about dependency objects.
        /// </summary>
        //public List<UDependencyTableItem> Dependencies{ get; private set; }
        private struct ClassType
        {
            public string Name;
            public Type Class;
        }

        /// <summary>
        /// </summary>
        public class PackageEventArgs : EventArgs
        {
            /// <summary>
            ///     Event identification.
            /// </summary>
            public enum Id : byte
            {
                /// <summary>
                ///     Constructing Export/Import objects.
                /// </summary>
                Construct = 0,

                /// <summary>
                ///     Deserializing objects.
                /// </summary>
                Deserialize = 1,

                /// <summary>
                ///     Importing objects from linked packages.
                /// </summary>
                Import = 2,

                /// <summary>
                ///     Connecting deserialized object indexes.
                /// </summary>
                Link = 3,

                /// <summary>
                ///     Deserialized a Export/Import object.
                /// </summary>
                Object = 0xFF
            }

            /// <summary>
            ///     The event identification.
            /// </summary>
            [PublicAPI]
            public readonly Id EventId;

            /// <summary>
            ///     Constructs a new event with @eventId.
            /// </summary>
            /// <param name="eventId">Event identification.</param>
            public PackageEventArgs(Id eventId)
            {
                EventId = eventId;
            }
        }
    }
}