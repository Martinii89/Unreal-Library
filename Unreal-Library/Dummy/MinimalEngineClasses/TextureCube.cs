namespace UELib.Dummy
{
    class TextureCube : MinimalBase
    {
        protected override byte[] MinimalByteArray { get; } =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1D, 0x05, 0x00, 0x00
        };

        public TextureCube(UExportTableItem exportTableItem, UnrealPackage package) : base(exportTableItem, package)
        {
        }


        protected override void WriteSerialData(IUnrealStream stream, UnrealPackage package)
        {
            FixNameIndexAtPosition(package, "None", 4);
            stream.Write(MinimalByteArray, 0, MinimalByteArray.Length);
        }
    }
}