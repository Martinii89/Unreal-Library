using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UELib.Dummy
{

    public class DummyOptions
    {
        public bool RealTextureDataInDummy { get; set; } = true;
        public bool RealMeshDataInDummy { get; set; } = true;
        public int RealTextureDataMaxResInDummy { get; set; } = 256;
        public bool LogObjectSizes { get; set; } = false;
    }

    public class RlDummyPackageStream : UPackageStream
    {
        private const int DummyPackageFlag = 1;
        private const int DummyFileVersion = 867;
        private const int DummyLicenseeVersion = 0;
        private const int DummyEngineVersion = 10897;
        private const int DummyCookerVersion = 0;
        private const int DummyCompression = 0;
        private const int DummyCompressedChunksData = 0;
        private const int DummyUnknown5 = 0;
        private const int DummyUnknownStringArray = 0;
        private const int DummyUnknownTypeArray = 0;

        private const int NameCountPosition = 25;
        private const int ExportCountPosition = 33;
        private const int ImportCountPosition = 41;

        private const int TotalHeaderSizePosition = 8;

        private const int DependsOffsetPosition = 49;
        private const int ThumbnailDataOffsetPosition = 53;
        private const int ThumbnailTableOffsetPosition = 65;
        private readonly DummyFactory _dummyFactory;

        private readonly UnrealPackage _package;
        private readonly bool _logObjectSizes;

        //public uint Version { get; set; }

        public RlDummyPackageStream(UnrealPackage package, string filePath, DummyOptions options)
        {
            Console.WriteLine($"Using real mesh data: {options.RealMeshDataInDummy}");
            Console.WriteLine($"Using real texture data: {options.RealTextureDataInDummy}");
            Console.WriteLine($"Using texture max res: {options.RealTextureDataMaxResInDummy}");
            StaticMesh.UseRealMeshData = options.RealMeshDataInDummy;
            Texture2D.UseRealTextureData = options.RealTextureDataInDummy;
            Texture2D.RealTextureDataMaxRes = options.RealTextureDataMaxResInDummy;
            _logObjectSizes = options.LogObjectSizes;
            _package = package;
            var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write);
            _stream = fileStream;
            Package = package;

            UW = new RLPackageWriter(_stream, DummyEngineVersion);
            //Could this be done by the factory?
            Texture2D.AddNamesToNameTable(package);
            StaticMesh.AddNamesToNameTable(package);
            TextureRenderTarget2D.AddNamesToNameTable(package);
            TextureRenderTargetCube.AddNamesToNameTable(package);
            SkeletalMesh.AddNamesToNameTable(package);
            var nameList = new List<string> { "tagame", "DummyMic" };
            MinimalBase.AddNamesToNameTable(package, nameList);

            //Init the factory
            _dummyFactory = DummyFactory.Instance;
        }

        public void Serialize()
        {
            InitHeader();
            // TODO: Trim these tables down to what the dummy assets actually use.
            // This would require pre-parsing the export items. Should not be impossible to do.
            SerializeNameTable();

            var exportsToSerialize = GetExportsToSerialize();
            var micClass = Package.Imports.FirstOrDefault(x => x.ClassName == "Class" && x.ObjectName == "MaterialInstanceConstant");
            if (micClass != null)
            {
                var micClassIndex = micClass.Index;
                var dummyMic = new FakeExportItem("MaterialInstanceConstant", -micClassIndex - 1, 0, new UName(Package, "DummyMic"), 0, 0, 0);
                var dummyExportTableItem = new DummyExportTableItem(dummyMic);
                exportsToSerialize.Add(dummyExportTableItem);
                FixObjectReferencesInFilteredExports(dummyExportTableItem, exportsToSerialize, _package.Exports);
            }

            var newImportTable = new List<DummyImportTableItem>();

            //Let's start by trimming down the import table
            TrimImportTable(exportsToSerialize, newImportTable);

            SerializeImportTable(newImportTable);

            //Write info about exports in header
            var exportOffset = UW.BaseStream.Position;
            WriteIntAtPosition(exportsToSerialize.Count, ExportCountPosition);
            WriteIntAtPosition((int) exportOffset, ExportCountPosition + 4);

            // Build up the list of dummy exports.
            var dummyExports = exportsToSerialize.Select(x => _dummyFactory.Create(x.original.ClassName, x.original, _package)).ToList();

            //Serialize the export table. We will have to redo this once we know the real serial offset and sizes. 
            SerializeExportTable(exportsToSerialize, dummyExports);

            //Serialize depends table.
            SerializeDependsTable(exportsToSerialize.Count);

            var thumbnails = new ThumbnailTable();
            thumbnails.Init(exportsToSerialize);
            thumbnails.Serialize(this);

            WriteIntAtPosition(thumbnails.thumbnailDataOffset, ThumbnailDataOffsetPosition);
            WriteIntAtPosition(thumbnails.thumbnailTableOffset, ThumbnailTableOffsetPosition);
            WriteIntAtPosition((int) UW.BaseStream.Position, TotalHeaderSizePosition);

            SerializeExportSerialData(dummyExports);
            if (_logObjectSizes)
            {
                var pairs = exportsToSerialize.Select((t, i) => new Tuple<DummyExportTableItem, MinimalBase>(t, dummyExports[i])).ToList();
                pairs.Sort((a,b) => a.Item2.SerialSize.CompareTo(b.Item2.SerialSize));
                foreach (var pair in pairs)
                {
                    var export = pair.Item1.original;
                    var dummyBase = pair.Item2;
                    var name = $"{export.ClassName}.{export.ObjectName}";
                    var right = $"{dummyBase.SerialSize} bytes";
                    Console.WriteLine($"{name, -50}{right, 25}");
                }
            }

            // Serialize export table again now that serial offsets and sizes are known
            UW.BaseStream.Seek(exportOffset, SeekOrigin.Begin);
            SerializeExportTable(exportsToSerialize, dummyExports);
        }

        private void SerializeImportTable(List<DummyImportTableItem> newImportTable)
        {
            var importOffset = UW.BaseStream.Position;
            WriteIntAtPosition(newImportTable.Count, ImportCountPosition);
            WriteIntAtPosition((int) importOffset, ImportCountPosition + 4);

            //Serialize the import table
            foreach (var import in newImportTable)
            {
                import.Serialize(this);
            }
        }

        private void SerializeExportSerialData(List<MinimalBase> exportsToSerialize)
        {
            foreach (var dummyExport in exportsToSerialize)
            {
                dummyExport.Write(this, _package);
            }
        }

        private void TrimImportTable(List<DummyExportTableItem> exportsToSerialize, List<DummyImportTableItem> newImportTable)
        {
            foreach (var export in exportsToSerialize)
            {
                export.newClassIndex = AddToImportTable(newImportTable, export.newClassIndex);
                export.newSuperIndex = AddToImportTable(newImportTable, export.newSuperIndex);
                export.newOuterIndex = AddToImportTable(newImportTable, export.newOuterIndex);
                export.newArchetypeIndex = AddToImportTable(newImportTable, export.newArchetypeIndex);
            }
        }

        private int FromListIndexToImportTableIndex(int index)
        {
            return -index - 1;
        }

        private int AddClassToImportTable(List<DummyImportTableItem> newImportTable, DummyExportTableItem export, int importIndex)
        {
            if (importIndex < 0) //import
            {
                var import = _package.Imports[-importIndex - 1];
                var newIndex = newImportTable.FindIndex(i => i.original == import);
                if (newIndex == -1)
                {
                    newImportTable.Add(new DummyImportTableItem(import));
                    newIndex = newImportTable.Count - 1;
                }

                //Process the outer\parent object(s) of the import
                var dummyImport = newImportTable[newIndex];
                while (dummyImport.newOuterIndex < 0)
                {
                    var parentImport = _package.Imports[-dummyImport.original.OuterIndex - 1];
                    var newParentIndex = newImportTable.FindIndex(i => i.original == parentImport);
                    if (newParentIndex == -1)
                    {
                        newImportTable.Add(new DummyImportTableItem(parentImport));
                        newParentIndex = newImportTable.Count - 1;
                        dummyImport.newOuterIndex = FromListIndexToImportTableIndex(newParentIndex);
                        dummyImport = newImportTable[newParentIndex];
                    }
                    else
                    {
                        dummyImport.newOuterIndex = FromListIndexToImportTableIndex(newParentIndex);
                        //We've already added this to the table. And we assume we can break the "recursion" here..
                        break;
                    }
                }

                return FromListIndexToImportTableIndex(newIndex);
            }

            //if export, just return the old one.
            var exportClass = _package.Exports[export.original.ClassIndex];

            // Need to add the class to the import table
            // Also needs to add the tagame package as a import for the class?

            var nameList = new List<string> { "tagame" };
            MinimalBase.AddNamesToNameTable(_package, nameList);


            //UName uclass_name = package.Names.FindIndex();
            var thisClassUName = new UName(_package, exportClass.ClassName);
            var tagameUName = new UName(_package, "tagame");
            var coreUName = new UName(_package, "Core");
            var classUname = new UName(_package, "Class");
            var packageUName = new UName(_package, "Package");

            var thisClassImport = new UImportTableItem
            {
                PackageName = coreUName, _ClassName = classUname, ObjectName = thisClassUName
            };

            newImportTable.Add(new DummyImportTableItem(thisClassImport));
            var thisClassDummyImportIndex = newImportTable.Count - 1;
            var thisClassDummyImport = newImportTable[thisClassDummyImportIndex];
            thisClassDummyImport.newOuterIndex = FromListIndexToImportTableIndex(thisClassDummyImportIndex + 1);

            var tagamePackageImport = new UImportTableItem
            {
                PackageName = coreUName, _ClassName = packageUName, ObjectName = tagameUName
            };

            newImportTable.Add(new DummyImportTableItem(tagamePackageImport));
            var taGamePackageIndex = newImportTable.Count - 1;
            var taGameDummyPackage = newImportTable[taGamePackageIndex];
            taGameDummyPackage.newOuterIndex = 0;


            //var dummyClassImport = new UImportTableItem();
            //dummyClassImport._ClassName = export.original.ClassTable.ObjectName;


            return FromListIndexToImportTableIndex(thisClassDummyImportIndex);
        }

        private int AddToImportTable(List<DummyImportTableItem> newImportTable, int importIndex)
        {
            if (importIndex < 0) //import
            {
                var import = _package.Imports[-importIndex - 1];
                var newIndex = newImportTable.FindIndex(i => i.original == import);
                if (newIndex == -1)
                {
                    newImportTable.Add(new DummyImportTableItem(import));
                    newIndex = newImportTable.Count - 1;
                }

                //Process the outer\parent object(s) of the import
                var dummyImport = newImportTable[newIndex];
                while (dummyImport.newOuterIndex < 0)
                {
                    var parentImport = _package.Imports[-dummyImport.original.OuterIndex - 1];
                    var newParentIndex = newImportTable.FindIndex(i => i.original == parentImport);
                    if (newParentIndex == -1)
                    {
                        newImportTable.Add(new DummyImportTableItem(parentImport));
                        newParentIndex = newImportTable.Count - 1;
                        dummyImport.newOuterIndex = FromListIndexToImportTableIndex(newParentIndex);
                        dummyImport = newImportTable[newParentIndex];
                    }
                    else
                    {
                        dummyImport.newOuterIndex = FromListIndexToImportTableIndex(newParentIndex);
                        //We've already added this to the table. And we assume we can break the "recursion" here..
                        break;
                    }
                }

                return FromListIndexToImportTableIndex(newIndex);
            }

            //if export, just return the old one.
            return importIndex;
        }

        private void SerializeExportTable(IReadOnlyList<DummyExportTableItem> exportsToSerialize, IReadOnlyList<MinimalBase> dummyExports)
        {
            for (var i = 0; i < exportsToSerialize.Count; i++)
            {
                DummyExportTableSerialize(exportsToSerialize[i], dummyExports[i]);
            }
        }

        private void SerializeDependsTable(int exportCount)
        {
            WriteIntAtPosition((int) UW.BaseStream.Position, DependsOffsetPosition);
            for (var i = 0; i < exportCount; i++)
            {
                UW.Write(0);
            }
        }

        private List<DummyExportTableItem> GetExportsToSerialize()
        {
            var packagesNotInUse = _package.Exports.Where(e => e.ClassName == "Package").ToList();
            var exportsToSerialize = new List<DummyExportTableItem>();
            var classesToSkip = new List<string>
            {
                "ObjectReferencer", "World", "ObjectRedirector", "ShadowMapTexture2D", "SoundClass", "TeamColorScriptedTexture_TA", "DecalMaterial"
            };
            var i = 1;
            foreach (var export in _package.Exports)
            {
                //None 
                if (export.SerialSize == 0)
                {
                    Console.WriteLine($"Skipping {export.ObjectName} 0 serial size");
                    continue;
                }

                if (export.Object.IsClassType("Class") || export.ObjectName.ToString().StartsWith("Default__"))
                {
                    Console.WriteLine($"Skipping {export.ObjectName} 0 class or default object");


                    Console.WriteLine($"Skipping {export.SerialSize:F2} null class or default object");


                    continue;
                }

                // any object not a child of package. Don't need it
                if (export.OuterTable != null && export.OuterTable.ClassName != "Package")
                {
                    //Console.WriteLine($"Skipping {export.ObjectName} not a child of a package");
                    continue;
                }

                if (classesToSkip.Contains(export.ClassName))
                {
                    Console.WriteLine($"Skipping {export.ObjectName} blacklisted class");
                    continue;
                }

                if (export.OuterTable?.ClassName == "Package")
                {
                    var outerPackage = export.OuterTable as UExportTableItem;
                    packagesNotInUse.Remove(outerPackage);
                }

                Console.WriteLine($"{i}: Adding ({export.ClassName}) {export.ObjectName} to dummy export");
                exportsToSerialize.Add(new DummyExportTableItem(export));


                if (i >= 200)
                {
                    //break;
                }

                i++;
            }

            foreach (var packageToRemove in packagesNotInUse)
            {
                exportsToSerialize.RemoveAll(e => e.original == packageToRemove);
            }


            foreach (var export in exportsToSerialize)
            {
                FixObjectReferencesInFilteredExports(export, exportsToSerialize, _package.Exports);
            }

            return exportsToSerialize;
        }

        private void FixObjectReferencesInFilteredExports(DummyExportTableItem export, List<DummyExportTableItem> exportsToSerialize,
            List<UExportTableItem> exports)
        {
            export.newClassIndex = FindNewExportReference(export.original.ClassIndex, exportsToSerialize, exports);
            export.newSuperIndex = FindNewExportReference(export.original.SuperIndex, exportsToSerialize, exports);
            export.newOuterIndex = FindNewExportReference(export.original.OuterIndex, exportsToSerialize, exports);
            export.newArchetypeIndex = FindNewExportReference(export.original.ArchetypeIndex, exportsToSerialize, exports);
        }

        private int FindNewExportReference(int originalIndex, List<DummyExportTableItem> exportsToSerialize, List<UExportTableItem> exports)
        {
            if (originalIndex <= 0)
            {
                return originalIndex;
            }

            var reference = exports[originalIndex - 1];
            var newIndex = exportsToSerialize.FindIndex(e => e.original == reference);
            //if (new_index == -1 && !reference.Object.IsClassType("Class"))
            //{
            //    exportsToSerialize.Add(new DummyExportTableItem(reference));
            //    new_index = exportsToSerialize.FindIndex((e) => e.original == reference);
            //}
            var newReferenceIndex = newIndex + 1;
            return newReferenceIndex;
        }

        private void SerializeNameTable()
        {
            var nameOffset = UW.BaseStream.Position;
            WriteIntAtPosition(_package.Names.Count, NameCountPosition);
            WriteIntAtPosition((int) nameOffset, NameCountPosition + 4);

            foreach (var name in _package.Names)
            {
                name.Serialize(this);
            }
        }

        private void WriteIntAtPosition(int value, int writePosition)
        {
            var oldPosition = UW.BaseStream.Position;
            UW.BaseStream.Position = writePosition;
            UW.Write(value);
            UW.BaseStream.Position = oldPosition;
        }

        private void InitHeader()
        {
            UW.BaseStream.Position = 0;
            //Tag
            UW.Write(UnrealPackage.Signature);
            //File Version
            var version = (int) (DummyFileVersion & 0x0000FFFFU) | (DummyLicenseeVersion << 16);
            UW.Write(version);
            //Total HeaderSize will be calculated later
            UW.Write(0);
            //Group name
            UW.WriteString("None");
            //Package flags
            UW.Write(DummyPackageFlag);

            //Name / Import / Export count and offsets
            UW.Write(0);
            UW.Write(1);
            UW.Write(2);
            UW.Write(3);
            UW.Write(4);
            UW.Write(5);

            //DependsOffset / ImportExportGuidsOffset / ImportGuidsCount / ExportGuidsCount / ThumbnailTableOffset
            UW.Write(0);
            UW.Write(0);
            UW.Write(0);
            UW.Write(0);
            UW.Write(0);

            //guid (this might work?..)
            UW.Write(Guid.Parse(_package.GUID).ToByteArray(), 0, 16);

            //Generations
            _package.Generations.Serialize(this);

            //EngineVersion
            UW.Write(DummyEngineVersion);

            //CookerVersion
            UW.Write(DummyCookerVersion);
            //CompressionFlags
            UW.Write(DummyCompression);
            //compressedChunks
            UW.Write(DummyCompressedChunksData);
            //Unknown5
            UW.Write(DummyUnknown5);
            //UnknownStringArray
            UW.Write(DummyUnknownStringArray);
            //UnknownTypeArray
            UW.Write(DummyUnknownTypeArray);
        }

        private void DummyExportTableSerialize(DummyExportTableItem tableItem, MinimalBase exportObject)
        {
            this.Write(tableItem.newClassIndex);
            this.Write(tableItem.newSuperIndex);
            this.Write(tableItem.newOuterIndex);
            this.Write(tableItem.original.ObjectName);
            this.Write(tableItem.newArchetypeIndex);
            this.Write(tableItem.GetExportObjectFlag());
            this.Write(exportObject.SerialSize);

            this.Write(exportObject.SerialOffset);
            //this.Write(tableItem.ExportFlags);
            this.Write(0);
            //Net objects count
            this.Write(0);

            //PackageGUID
            this.Write(0);
            this.Write(0);
            this.Write(0);
            this.Write(0);

            //Package flags
            if (tableItem.original.ClassName == "Package")
            {
                this.Write(1);
            }
            else
            {
                this.Write(0);
            }
        }
    }
}