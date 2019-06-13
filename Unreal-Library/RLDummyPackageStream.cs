using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib
{
    class RLPackageWriter : UnrealWriter
    {
        uint _version;

        protected override uint _Version
        {
            get { return _version; }
        }


        public RLPackageWriter(Stream stream, uint version) : base(stream)
        {
            _version = version;
        }
    }

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
        }


        public void Serialize()
        {
            initHeader();

            //Serialize name table
            var nameOffset = UW.BaseStream.Position;
            WriteIntAtPosition(package.Names.Count(), NameCountPosition);
            WriteIntAtPosition((int)nameOffset, NameCountPosition+4);

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

            //var exportsToSerialize = package.Exports.Skip(1);
            var exportsToSerialize = new List<UExportTableItem>() { package.Exports[1], package.Exports[3] };
            //WriteIntAtPosition();
            var exportOffset = UW.BaseStream.Position;
            WriteIntAtPosition(exportsToSerialize.Count(), ExportCountPosition);
            WriteIntAtPosition((int)exportOffset, ExportCountPosition + 4);

            

            int calculatedTotalHeaderSize = (int)(UW.BaseStream.Position + 68 * exportsToSerialize.Count()) + 24; //24 = unknown footer data
            int i = 0;
            foreach (var export in exportsToSerialize)
            {
                int serialOffset = calculatedTotalHeaderSize + 12 * i;
                DummyExportTableSerialize(export, serialOffset);
                i++;
                //export.Serialize( this );
            }
            //unknown footer info 
            WriteIntAtPosition((int)UW.BaseStream.Position, 49);
            UW.Write(0);
            UW.Write(0);
            WriteIntAtPosition((int)UW.BaseStream.Position, 53);
            UW.Write(0);
            UW.Write(0);
            UW.Write(0);
            WriteIntAtPosition((int)UW.BaseStream.Position, 65);
            UW.Write(0);

            WriteIntAtPosition((int)UW.BaseStream.Position, TotalHeaderSizePosition);

            int noneIndex = package.Names.FindIndex((n) => n.Name == "None");
            foreach(var exportObject in exportsToSerialize)
            {
                //We just need the netindex and a None FName
                package.Stream.Position = exportObject.SerialOffset;
                //NetIndex
                UW.Write(package.Stream.ReadInt32());
                //None index and count
                UW.Write(noneIndex);
                UW.Write(0);
            }



            //var stream = package.Stream;
            //// Serialize tables
            //var namesBuffer = new UObjectStream(stream);
            //foreach (var name in package.Names)
            //{
            //    name.Serialize(namesBuffer);
            //}

            //var importsBuffer = new UObjectStream(stream);
            //foreach (var import in package.Imports)
            //{
            //    import.Serialize(importsBuffer);
            //}

            //var exportsBuffer = new UObjectStream(stream);
            //foreach (var export in package.Exports)
            //{
            //    //export.Serialize( exportsBuffer );
            //}

            //stream.Seek(0, SeekOrigin.Begin);

            //stream.Write(Signature);

            //// Serialize header
            //var version = (int)(Version & 0x0000FFFFU) | (LicenseeVersion << 16);
            //stream.Write(version);

            //var headerSizePosition = stream.Position;
            //if (Version >= VHeaderSize)
            //{
            //    stream.Write((int)HeaderSize);
            //    if (Version >= VGroup)
            //    {
            //        stream.WriteString(Group);
            //    }
            //}

            //stream.Write(PackageFlags);

            //_TablesData.NamesCount = (uint)Names.Count;
            //_TablesData.ExportsCount = (uint)Exports.Count;
            //_TablesData.ImportsCount = (uint)Imports.Count;

            //var tablesDataPosition = stream.Position;
            //_TablesData.Serialize(stream);

            //// TODO: Serialize Heritages

            //stream.Write(Guid.NewGuid().ToByteArray(), 0, 16);
            //Generations.Serialize(stream);

            //if (Version >= VEngineVersion)
            //{
            //    stream.Write(EngineVersion);
            //    if (Version >= VCOOKEDPACKAGES)
            //    {
            //        stream.Write(CookerVersion);

            //        if (Version >= VCompression)
            //        {
            //            if (IsCooked())
            //            {
            //                stream.Write(CompressionFlags);
            //                CompressedChunks.Serialize(stream);
            //            }
            //        }
            //    }
            //}

            //// TODO: Unknown data
            //stream.Write((uint)0);

            //// Serialize objects

            //// Write tables

            //// names
            //Log.Info("Writing names at position " + stream.Position);
            //_TablesData.NamesOffset = (uint)stream.Position;
            //var namesBytes = namesBuffer.GetBuffer();
            //stream.Write(namesBytes, 0, (int)namesBuffer.Length);

            //// imports
            //Log.Info("Writing imports at position " + stream.Position);
            //_TablesData.ImportsOffset = (uint)stream.Position;
            //var importsBytes = importsBuffer.GetBuffer();
            //stream.Write(importsBytes, 0, (int)importsBuffer.Length);

            //// exports
            //Log.Info("Writing exports at position " + stream.Position);

            //// Serialize tables data again now that offsets are known.
            //var currentPosition = stream.Position;
            //stream.Seek(tablesDataPosition, SeekOrigin.Begin);
            //_TablesData.Serialize(stream);
            //stream.Seek(currentPosition, SeekOrigin.Begin);
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

        public void DummyExportTableSerialize(UExportTableItem tableItem, int serialOffset)
        {
            this.Write(tableItem?.ClassTable?.Object);
            this.Write(tableItem?.SuperTable?.Object);
            this.Write((int)tableItem?.OuterTable?.Object);
            this.Write(tableItem.ObjectName);
            this.Write(tableItem.ArchetypeIndex);
            //this.Write(tableItem.ObjectFlags);
            this.Write(4222141830530048);

            this.Write(DummySerialSize);

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
