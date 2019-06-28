using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    class MinimalTextureCube : MinimalBase
    {

        public static int serialSize = 28;

        byte[] MinimalTextureCubeByteArray = {
            0xFF, 0xFF, 0xFF, 0xFF, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1D, 0x05, 0x00, 0x00
        };

        protected override byte[] minimalByteArray => MinimalTextureCubeByteArray;

        public override void Write(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "None", 4);
            stream.Write(minimalByteArray, 0, serialSize);
        }
    }
}


