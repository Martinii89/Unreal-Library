using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    class TextureCube : MinimalBase
    {

        public static int SerialSize = 28;

        protected override byte[] MinimalByteArray { get; } =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1D, 0x05, 0x00, 0x00
        };

        public override int GetSerialSize() => SerialSize;

        public override void Write(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "None", 4);
            stream.Write(MinimalByteArray, 0, SerialSize);
        }

        public TextureCube(UExportTableItem exportTableItem, UnrealPackage package) : base(exportTableItem, package)
        {
        }
    }
}


