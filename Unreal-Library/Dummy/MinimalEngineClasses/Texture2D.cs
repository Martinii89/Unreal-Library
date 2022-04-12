using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLUPKT.Core.UTypes;
using UELib.Dummy.Property;
using UELib.Dummy.Structs;

namespace UELib.Dummy
{
    internal class Mip: IUESerializable, IDummySerializable
    {
        public FBulkData Data { get; set; } = new FBulkData();
        public int SizeX { get; set; }
        public int SizeY { get; set; }

        public void Deserialize(BinaryReader Reader)
        {
            Data.Deserialize(Reader);
            SizeX = Reader.ReadInt32();
            SizeY = Reader.ReadInt32();
        }

        public void Serialize(IUnrealStream writer)
        {
            Data.Serialize(writer);
            writer.Write(SizeX);
            writer.Write(SizeY);
        }
    }

    internal class GUID : IUESerializable, IDummySerializable
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }

        public void Deserialize(BinaryReader Reader)
        {
            A = Reader.ReadInt32();
            B = Reader.ReadInt32();
            C = Reader.ReadInt32();
            D = Reader.ReadInt32();
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(A);
            writer.Write(B);
            writer.Write(C);
            writer.Write(D);
        }
    }

    internal class Texture2D : MinimalBase
    {
        public static int SerialSize = 402;

        protected override byte[] MinimalByteArray { get; } = {
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

        //public override int GetSerialSize() => SerialSize;
        public override int GetSerialSize() => ExportTableItem.SerialSize;

        public FBulkData SourceArt { get; set; } = new FBulkData();
        public Structs.TArray<Mip> Mips { get; set; } = new Structs.TArray<Mip>();
        public GUID TextureFileCacheGuid { get; set; } = new GUID();
        public Structs.TArray<Mip> CachedPVRTCMips { get; set; } = new Structs.TArray<Mip>();
        public int CachedFlashMipsMaxResolution { get; set; }
        public Structs.TArray<Mip> CachedATITCMips { get; set; } = new Structs.TArray<Mip>();
        public FBulkData CachedFlashMips { get; set; } = new FBulkData();
        public Structs.TArray<Mip> CachedETCMips { get; set; } = new Structs.TArray<Mip>();

        public Texture2D(UExportTableItem exportTableItem, UnrealPackage package) : base(exportTableItem, package)
        {
            var reader = package.Stream.UR;
            var writer = package.Stream.UW;
            reader.BaseStream.Position = exportTableItem.SerialOffset;
            var netIndex = reader.ReadInt32();
            var property = new BaseProperty();
            property.Deserialize(reader);
            while (property.IsValid())
            {
                if (property.IsObjectProperty())
                {
                    // Null out any object references instead of trying to fix up the index.
                    Debug.Assert(property.Size == 4, "Object property was not 4. Freak out!");
                    package.Stream.Skip(-property.Size);
                    package.Stream.Write(0);
                }
                property.Deserialize(reader);
            }

            PropertyEnd = reader.BaseStream.Position;

            SourceArt.Deserialize(reader);
            Mips.Deserialize(reader);
            TextureFileCacheGuid.Deserialize(reader);
            CachedPVRTCMips.Deserialize(reader);
            CachedFlashMipsMaxResolution = reader.ReadInt32();
            CachedATITCMips.Deserialize(reader);
            CachedFlashMips.Deserialize(reader);
            CachedETCMips.Deserialize(reader);
        }

        public long PropertyEnd { get; set; }

        public override void Write(IUnrealStream stream, UnrealPackage package)
        {
            var startPos = stream.Position;
            package.Stream.UR.BaseStream.Seek(ExportTableItem.SerialOffset, SeekOrigin.Begin);
            var propertyBuffer = package.Stream.UR.ReadBytes((int)(PropertyEnd - ExportTableItem.SerialOffset));
            stream.Write(propertyBuffer, 0, propertyBuffer.Length);

            // Write the mesh data

            SourceArt.Serialize(stream);
            Mips.Serialize(stream);
            TextureFileCacheGuid.Serialize(stream);
            CachedPVRTCMips.Serialize(stream);
            stream.Write(CachedFlashMipsMaxResolution);
            CachedATITCMips.Serialize(stream);
            CachedFlashMips.Serialize(stream);
            CachedETCMips.Serialize(stream);

            var endPos = stream.Position;
            Console.WriteLine($"Written {endPos-startPos} to TextureData");

            return;
            package.Stream.UR.BaseStream.Seek(ExportTableItem.SerialOffset, SeekOrigin.Begin);
            var buffer = package.Stream.UR.ReadBytes(ExportTableItem.SerialSize);
            stream.Write(buffer, 0, buffer.Length);
            return;
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

            stream.Write(MinimalByteArray, 0, SerialSize);
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
            AddNamesToNameTable(package, namesToAdd);
        }


    }
}