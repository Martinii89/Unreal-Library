using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    class TextureRenderTargetCube : MinimalBase
    {
        public static int SerialSize = 56;

        protected override byte[] MinimalByteArray { get; } =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xD6, 0x04, 0x00, 0x00
        };

        public override int GetSerialSize() => SerialSize;

        public override void Write(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "SizeX", 4);
            FixNameIndexAtPosition(package, "IntProperty", 12);


            FixNameIndexAtPosition(package, "None", 32);
            stream.Write(MinimalByteArray, 0, SerialSize-4);
            stream.Write((int)stream.Position + sizeof(int));
        }

        public static void AddNamesToNameTable(UnrealPackage package)
        {
            var namesToAdd = new List<string>()
            {
                "SizeX", "IntProperty", "None"
            };
            AddNamesToNameTable(package, namesToAdd);
        }

        public TextureRenderTargetCube(UExportTableItem exportTableItem, UnrealPackage package) : base(exportTableItem, package)
        {
        }
    }
}


