namespace UELib.Dummy
{
    class Material : MinimalBase
    {
        public static int SerialSize = 96;


        //protected override byte[] MinimalByteArray { get; } =
        //{
        //    0xFF, 0xFF, 0xFF, 0xFF, 0x3B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00
        //};

        //------------------------------------------------------------
        //-----------       Created with 010 Editor        -----------
        //------         www.sweetscape.com/010editor/          ------
        //
        // File    : D:\Projects\Unreal-Library\010-Stuff\MaterialTest.upk
        // Address : 7807 (0x1E7F)
        // Size    : 96 (0x60)
        //------------------------------------------------------------
        protected override byte[] MinimalByteArray { get; } = 
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xF5, 0x04, 0xEC,
            0x14, 0x8F, 0x83, 0x4E, 0xA1, 0x24, 0xA4, 0x09, 0x91, 0x57, 0x82, 0xFC, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };


        public Material(UExportTableItem exportTableItem, UnrealPackage package) : base(exportTableItem, package)
        {
        }

        public override int GetSerialSize() => SerialSize;

        public override void Write(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "None", 4);
            //stream.Write(MinimalByteArray2, 0, SerialSize);
            stream.Write(MinimalByteArray, 0, SerialSize);
        }


    }

    class MaterialInstanceConstant : MinimalBase
    {
        private const int SerialSize = 12;


        protected override byte[] MinimalByteArray { get; } =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 
        };

        public override void Write(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "None", 4);
            stream.Write(MinimalByteArray, 0, SerialSize);
        }

        public override int GetSerialSize()
        {
            return SerialSize;
        }

        public MaterialInstanceConstant(UExportTableItem exportTableItem, UnrealPackage package) : base(exportTableItem, package)
        {
        }
    }
}

