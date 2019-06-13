using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib
{
    internal class MinimalTexture2D
    {
        public static int serialSize = 402;

        private byte[] minimalTex2D = new byte[] {
            0xFF, 0xFF, 0xFF, 0xFF, 0x17, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x01, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x0E, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1D, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x19, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xCA, 0x1C, 0x18, 0x15, 0x18, 0x50, 0x24, 0x40, 0xAD, 0xF7,
            0xE2, 0x10, 0xF0, 0x7C, 0xA9, 0x4E, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0xC8, 0x06, 0x00, 0x00, 0xFF, 0xFF,
            0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x04, 0x00,
            0x00, 0x00, 0xE0, 0x06, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00,
            0x00, 0x00, 0x3E, 0x1F, 0x64, 0xDA, 0xE1, 0x27, 0x27, 0x4F, 0xA7, 0xAA, 0x20, 0x7E, 0x43, 0xA9,
            0xCE, 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x18, 0x07, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00
        };

        public void Write(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "SizeX", 4);
            FixNameIndexAtPosition(package, "IntProperty", 12);

            FixNameIndexAtPosition(package, "SizeY", 32);
            FixNameIndexAtPosition(package, "IntProperty", 40);

            FixNameIndexAtPosition(package, "OriginalSizeX", 60);
            FixNameIndexAtPosition(package, "IntProperty", 68);

            FixNameIndexAtPosition(package, "OriginalSizeY", 88);
            FixNameIndexAtPosition(package, "IntProperty", 96);

            FixNameIndexAtPosition(package, "OriginalSizeX", 60);
            FixNameIndexAtPosition(package, "IntProperty", 68);

            FixNameIndexAtPosition(package, "Format", 116);
            FixNameIndexAtPosition(package, "ByteProperty", 124);

            FixNameIndexAtPosition(package, "EPixelFormat", 140);
            FixNameIndexAtPosition(package, "PF_A8R8G8B8", 148);

            FixNameIndexAtPosition(package, "bIsSourceArtUncompressed", 156);
            FixNameIndexAtPosition(package, "BoolProperty", 164);

            FixNameIndexAtPosition(package, "CompressionNone", 181);
            FixNameIndexAtPosition(package, "BoolProperty", 189);

            FixNameIndexAtPosition(package, "MipGenSettings", 206);
            FixNameIndexAtPosition(package, "ByteProperty", 214);

            FixNameIndexAtPosition(package, "TextureMipGenSettings", 230);
            FixNameIndexAtPosition(package, "TMGS_NoMipmaps", 238);

            FixNameIndexAtPosition(package, "LightingGuid", 246);
            FixNameIndexAtPosition(package, "StructProperty", 254);

            FixNameIndexAtPosition(package, "Guid", 270);
            FixNameIndexAtPosition(package, "None", 294);

            stream.Write(minimalTex2D, 0, serialSize);
        }

        private void FixNameIndexAtPosition(UnrealPackage package, string name, int startPosition)
        {
            var test = package.Names.FindIndex((n) => n.Name == name);
            var bytes = BitConverter.GetBytes(test);
            for (int i = 0; i < bytes.Length; i++)
            {
                minimalTex2D[i + startPosition] = bytes[i];
            }
        }

        public static void AddNamesToNameTable(UnrealPackage package)
        {
            var namesToAdd = new List<string>()
            {
                "SizeX", "IntProperty", "SizeY", "OriginalSizeX",
                "OriginalSizeY","Format","ByteProperty","EPixelFormat",
                "PF_A8R8G8B8","bIsSourceArtUncompressed","BoolProperty","CompressionNone",
                "MipGenSettings","TextureMipGenSettings","TMGS_NoMipmaps",
                "LightingGuid", "StructProperty", "Guid", "None"
            };
            foreach (var name in namesToAdd)
            {
                if (!package.Names.Any((o) => o.Name == name))
                {
                    package.Names.Add(new UNameTableItem() { Name = name, Flags = 1970393556451328 });
                }
            }
        }
    }

    internal class RLPackageWriter : UnrealWriter
    {
        private uint _version;

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

            var exportsToSerialize = package.Exports;

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
                        //MinimalTexture2D.Write(this, package);
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