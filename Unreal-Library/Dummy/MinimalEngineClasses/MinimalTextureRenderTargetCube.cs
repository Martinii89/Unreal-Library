using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    class MinimalTextureRenderTargetCube : MinimalBase
    {
        public static int serialSize = 56;

        byte[] MinimalTextureRenderTargetCubeByteArray = {
            0xFF, 0xFF, 0xFF, 0xFF, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xD6, 0x04, 0x00, 0x00
        };

        protected override byte[] minimalByteArray => MinimalTextureRenderTargetCubeByteArray;

        public override void Write(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "SizeX", 4);
            FixNameIndexAtPosition(package, "IntProperty", 12);


            FixNameIndexAtPosition(package, "None", 32);
            stream.Write(minimalByteArray, 0, serialSize-4);
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
    }
}


