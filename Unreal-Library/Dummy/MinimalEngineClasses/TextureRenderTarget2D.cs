using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    class TextureRenderTarget2D : MinimalBase
    {
        public static int serialSize = 84;

        byte[] MinimalTextureRenderTarget2DByteArray = {
            0xFF, 0xFF, 0xFF, 0xFF, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x55, 0x05, 0x00, 0x00
        };

        protected override byte[] minimalByteArray => MinimalTextureRenderTarget2DByteArray;
        public override int GetSerialSize() => serialSize;

        public override void Write(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "SizeX", 4);
            FixNameIndexAtPosition(package, "IntProperty", 12);

            FixNameIndexAtPosition(package, "SizeY", 32);
            FixNameIndexAtPosition(package, "IntProperty", 40);

            FixNameIndexAtPosition(package, "None", 60);
            stream.Write(minimalByteArray, 0, serialSize-4);
            stream.Write((int)stream.Position + sizeof(int));
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
