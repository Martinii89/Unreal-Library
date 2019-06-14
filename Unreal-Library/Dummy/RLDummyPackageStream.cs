using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{

    public class RLDummyPackageStream : UPackageStream, IUnrealStream
    {
        private const int DummyPackageFlag = 1;
        private const int DummyFileVersion = 868;
        private const int DummyLicenseeVersion = 0;
        private const int DummyEngineVersion = 12791;
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

        private UnrealPackage package;

        //public uint Version { get; set; }

        public RLDummyPackageStream(UnrealPackage package, string filePath)
        {
            this.package = package;
            var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write);
            _stream = fileStream;
            Package = package;

            UW = new RLPackageWriter(_stream, DummyEngineVersion);
            MinimalTexture2D.AddNamesToNameTable(package);
        }

        private const int ExportTableItemSize = 68;

        public void Serialize()
        {
            initHeader();

            //Serialize name table
            var nameOffset = UW.BaseStream.Position;
            WriteIntAtPosition(package.Names.Count(), NameCountPosition);
            WriteIntAtPosition((int)nameOffset, NameCountPosition + 4);

            foreach (var name in package.Names)
            {
                name.Serialize(this);
            }

            //Serialize imports table
            var importOffset = UW.BaseStream.Position;
            WriteIntAtPosition(package.Imports.Count(), ImportCountPosition);
            WriteIntAtPosition((int)importOffset, ImportCountPosition + 4);
            foreach (var import in package.Imports)
            {
                import.Serialize(this);
            }

            //var exportsToSerialize = package.Exports;
            var exportsToSerialize = package.Exports.Where((e) => e.SerialSize != 0).Select((e) => e as DummyExportTableItem);
            foreach(var export in exportsToSerialize)
            {
                //FixObjectReferencesInFilteredExports(export, exportsToSerialize, package.Exports);
            }

            //var exportsToSerialize = new List<UExportTableItem>() { package.Exports[1], package.Exports[3] };
            //WriteIntAtPosition();
            var exportOffset = UW.BaseStream.Position;
            WriteIntAtPosition(exportsToSerialize.Count(), ExportCountPosition);
            WriteIntAtPosition((int)exportOffset, ExportCountPosition + 4);

            int footerNumbers = 8;
            int calculatedTotalHeaderSize = (int)(UW.BaseStream.Position + ExportTableItemSize * exportsToSerialize.Count()) + footerNumbers * 4; //24 = unknown footer data
            int serialOffset = calculatedTotalHeaderSize;
            foreach (var export in exportsToSerialize)
            {
                switch (export.ClassName)
                {
                    case "Texture2D":
                        DummyExportTableSerialize(export, serialOffset, MinimalTexture2D.serialSize);
                        serialOffset += MinimalTexture2D.serialSize;
                        break;
                    case "StaticMesh":
                        DummyExportTableSerialize(export, serialOffset, MinimalStaticMesh.serialSize);
                        serialOffset += MinimalStaticMesh.serialSize;
                        break;
                    case "class":
                        if (export.SerialSize == 0)
                        {
                            DummyExportTableSerialize(export, 0, 0);
                        }
                        break;
                    default:
                        DummyExportTableSerialize(export, serialOffset, DummySerialSize);
                        serialOffset += DummySerialSize;
                        break;
                }

                //export.Serialize( this );
            }
            //unknown footer info
            WriteIntAtPosition((int)UW.BaseStream.Position, 49);
            WriteIntAtPosition((int)UW.BaseStream.Position, 53);
            WriteIntAtPosition((int)UW.BaseStream.Position, 65);
            for (int ii = 0; ii < footerNumbers; ii++)
            {
                UW.Write(0);
            }

            WriteIntAtPosition((int)UW.BaseStream.Position, TotalHeaderSizePosition);

            int noneIndex = package.Names.FindIndex((n) => n.Name == "None");
            foreach (var exportObject in exportsToSerialize)
            {
                switch (exportObject.ClassName)
                {
                    case "Texture2D":
                        new MinimalTexture2D().Write(this, package);
                        break;

                    case "StaticMesh":
                        new MinimalStaticMesh().Write(this, package);
                        break;
                    case "class":
                        if (exportObject.SerialSize == 0)
                        {
                            break;
                        }
                        break;
                    default:
                        //We just need the netindex and a None FName
                        package.Stream.Position = exportObject.SerialOffset;
                        //NetIndex
                        UW.Write(package.Stream.ReadInt32());
                        //None index and count
                        UW.Write(noneIndex);
                        UW.Write(0);
                        break;
                }
            }
        }

        private void FixObjectReferencesInFilteredExports(DummyExportTableItem export, IList<UExportTableItem> exportsToSerialize, List<UExportTableItem> exports)
        {
            if (export.ClassIndex > 0)
            {
                var reference = package.Exports[export.ClassIndex - 1];
                var newReferenceIndex = exportsToSerialize.IndexOf(reference);
                //export.ClassIndex = newReferenceIndex;
            }
            throw new NotImplementedException();
        }

        private void WriteIntAtPosition(int value, int writePosition)
        {
            var oldPosition = UW.BaseStream.Position;
            UW.BaseStream.Position = writePosition;
            UW.Write(value);
            UW.BaseStream.Position = oldPosition;
        }

        private void initHeader()
        {
            UW.BaseStream.Position = 0;
            //Tag
            UW.Write(UnrealPackage.Signature);
            //File Version
            var version = (int)(DummyFileVersion & 0x0000FFFFU) | DummyLicenseeVersion << 16;
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
            UW.Write(Guid.Parse(package.GUID).ToByteArray(), 0, 16);

            //Generations
            package.Generations.Serialize(this);

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

        private const int DummySerialSize = 12;

        public void DummyExportTableSerialize(UExportTableItem tableItem, int serialOffset, int serialSize)
        {
            this.Write(tableItem?.ClassTable?.Object);
            this.Write(tableItem?.SuperTable?.Object);
            this.Write((int)tableItem?.OuterTable?.Object);
            this.Write(tableItem.ObjectName);
            this.Write(tableItem.ArchetypeIndex);
            //this.Write(tableItem.ObjectFlags);
            this.Write(4222141830530048);

            this.Write(serialSize);

            //this.Write(tableItem.SerialOffset);
            this.Write(serialOffset);
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
            this.Write(0);
        }
    }
}