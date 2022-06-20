namespace UELib.Dummy
{
    internal class DummyExportTableItem
    {
        public int newArchetypeIndex = 0;
        public int newClassIndex = 0;
        public int newOuterIndex = 0;
        public int newSuperIndex = 0;

        public IUExportTableItem original;


        public DummyExportTableItem(IUExportTableItem b)
        {
            original = b;
        }

        public int PackageFlag { get; private set; }

        public long GetExportObjectFlag()
        {
            switch (original.ClassName)
            {
                case "Package":
                    return 0x7000400000000;
                case "AkSoundCue":
                case "AkBank":
                    return 0xF000400000400;
                //case "Material":
                //    return 0xF000400000400; // No thumbnail generation for materials
                default:
                    return 0xF000400000000;
            }
        }
    }
}