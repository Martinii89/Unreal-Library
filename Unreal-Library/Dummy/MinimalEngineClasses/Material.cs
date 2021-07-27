using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    class Material : MinimalBase
    {
        public static int serialSize = 16;

        byte[] minimalMaterialByteArray = {
            0xFF, 0xFF, 0xFF, 0xFF, 0x3B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00
        };


        protected override byte[] minimalByteArray => minimalMaterialByteArray;

        public override int GetSerialSize() => serialSize;

        public override void Write(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "None", 4);
            stream.Write(minimalByteArray, 0, serialSize);
        }

    }

    class MaterialInstanceConstant : MinimalBase
    {
        private const int SerialSize = 12;


        protected override byte[] minimalByteArray { get; } =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 
        };

        public override void Write(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "None", 4);
            stream.Write(minimalByteArray, 0, SerialSize);
        }

        public override int GetSerialSize()
        {
            return SerialSize;
        }
    }
}

