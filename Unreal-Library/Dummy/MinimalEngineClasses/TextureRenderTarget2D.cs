using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    class TextureRenderTarget2D : MinimalBase
    {
        protected override byte[] MinimalByteArray { get; } =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x55, 0x05, 0x00, 0x00
        };

        public TextureRenderTarget2D(UExportTableItem exportTableItem, UnrealPackage package) : base(exportTableItem, package)
        {
        }


        protected override void WriteSerialData(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "SizeX", 4);
            FixNameIndexAtPosition(package, "IntProperty", 12);

            FixNameIndexAtPosition(package, "SizeY", 32);
            FixNameIndexAtPosition(package, "IntProperty", 40);

            FixNameIndexAtPosition(package, "None", 60);
            stream.Write(MinimalByteArray, 0, MinimalByteArray.Length - 4);
            stream.Write((int) stream.Position + sizeof(int));
        }

        public static void AddNamesToNameTable(UnrealPackage package)
        {
            var namesToAdd = new List<string>()
            {
                "SizeX", "IntProperty", "SizeY", "None"
            };
            AddNamesToNameTable(package, namesToAdd);
        }
    }
}