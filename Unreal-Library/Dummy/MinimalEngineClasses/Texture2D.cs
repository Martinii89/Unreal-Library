using System.Collections.Generic;
using System.IO;
using RLUPKT.Core.UTypes;
using UELib.Dummy.Structs;

namespace UELib.Dummy
{
    internal class Mip : IUESerializable, IDummySerializable
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

        protected override byte[] MinimalByteArray { get; } =
        {
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


        public FBulkData SourceArt { get; set; } = new FBulkData();
        public Structs.TArray<Mip> Mips { get; set; } = new Structs.TArray<Mip>();
        public GUID TextureFileCacheGuid { get; set; } = new GUID();
        public Structs.TArray<Mip> CachedPVRTCMips { get; set; } = new Structs.TArray<Mip>();
        public int CachedFlashMipsMaxResolution { get; set; }
        public Structs.TArray<Mip> CachedATITCMips { get; set; } = new Structs.TArray<Mip>();
        public FBulkData CachedFlashMips { get; set; } = new FBulkData();
        public Structs.TArray<Mip> CachedETCMips { get; set; } = new Structs.TArray<Mip>();

        public static bool UseRealTextureData { get; set; } = true;
        public static float RealTextureDataMaxRes { get; set; } = 256;

        public Texture2D(UExportTableItem exportTableItem, UnrealPackage package) : base(exportTableItem, package)
        {
            // TODO: Fix this later. focus on mesh data for now.
            var reader = package.Stream.UR;
            reader.BaseStream.Position = exportTableItem.SerialOffset;
            var netIndex = reader.ReadInt32();

            ReadScriptProperties();

            SourceArt.Deserialize(reader);
            Mips.Deserialize(reader);
            TextureFileCacheGuid.Deserialize(reader);
            CachedPVRTCMips.Deserialize(reader);
            CachedFlashMipsMaxResolution = reader.ReadInt32();
            CachedATITCMips.Deserialize(reader);
            CachedFlashMips.Deserialize(reader);
            CachedETCMips.Deserialize(reader);
            if (reader.BaseStream.Position > (exportTableItem.SerialOffset + exportTableItem.SerialSize))
            {
                var readLength = reader.BaseStream.Position - exportTableItem.SerialOffset + exportTableItem.SerialSize;
                //var too_far 
                throw new InvalidDataException("Read too far");
            }
        }


        protected override void WriteSerialData(IUnrealStream stream, UnrealPackage package)
        {
            if (!UseRealTextureData)
            {
                WriteMinimalBytes(stream, package);
                return;
            }

            package.Stream.UR.BaseStream.Seek(ExportTableItem.SerialOffset, SeekOrigin.Begin);
            var propertyBuffer = package.Stream.UR.ReadBytes((int)(ScriptPropertiesEnd - ExportTableItem.SerialOffset));
            stream.Write(propertyBuffer, 0, propertyBuffer.Length);

            SourceArt.Serialize(stream);
            //CBA to read and decompress stuff form the TFC file
            Mips.RemoveAll(m => m.Data.StoredInSeparateFile || m.SizeX > RealTextureDataMaxRes || m.SizeY > RealTextureDataMaxRes);
            Mips.Serialize(stream);
            TextureFileCacheGuid.Serialize(stream);
            CachedPVRTCMips.Serialize(stream);
            stream.Write(CachedFlashMipsMaxResolution);
            CachedATITCMips.Serialize(stream);
            CachedFlashMips.Serialize(stream);
            CachedETCMips.Serialize(stream);

        }

        private void WriteMinimalBytes(IUnrealStream stream, UnrealPackage package)
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

            stream.Write(MinimalByteArray, 0, MinimalByteArray.Length);
        }


        public static void AddNamesToNameTable(UnrealPackage package)
        {
            var namesToAdd = new List<string>()
            {
                "SizeX", "IntProperty", "SizeY", "OriginalSizeX",
                "OriginalSizeY", "Format", "ByteProperty", "EPixelFormat",
                "PF_A8R8G8B8", "bIsSourceArtUncompressed", "BoolProperty", "CompressionNone",
                "MipGenSettings", "TextureMipGenSettings", "TMGS_NoMipmaps",
                "LightingGuid", "StructProperty", "Guid", "None"
            };
            AddNamesToNameTable(package, namesToAdd);
        }
    }
}