using System.Collections.Generic;

namespace UELib.Dummy
{
    class TextureRenderTargetCube : MinimalBase
    {
        protected override byte[] MinimalByteArray { get; } =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xD6, 0x04, 0x00, 0x00
        };

        public TextureRenderTargetCube(UExportTableItem exportTableItem, UnrealPackage package) : base(exportTableItem, package)
        {
        }

        protected override void WriteSerialData(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "SizeX", 4);
            FixNameIndexAtPosition(package, "IntProperty", 12);


            FixNameIndexAtPosition(package, "None", 32);
            stream.Write(MinimalByteArray, 0, MinimalByteArray.Length - 4);
            stream.Write((int) stream.Position + sizeof(int));
        }

        public static void AddNamesToNameTable(UnrealPackage package)
        {
            var namesToAdd = new List<string>()
            {
                "SizeX", "IntProperty", "None"
            };
            AddNamesToNameTable(package, namesToAdd);
        }
    }
}