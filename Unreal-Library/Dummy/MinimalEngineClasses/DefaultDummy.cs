namespace UELib.Dummy
{
    class DefaultDummy : MinimalBase
    {
        protected override byte[] MinimalByteArray { get; } =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };


        public DefaultDummy(UExportTableItem exportTableItem, UnrealPackage package) : base(exportTableItem, package)
        {
            FixNameIndexAtPosition(package, "None", 4);
        }


        protected override void WriteSerialData(IUnrealStream stream, UnrealPackage package)
        {
            stream.Write(MinimalByteArray, 0, MinimalByteArray.Length);
        }
    }
}