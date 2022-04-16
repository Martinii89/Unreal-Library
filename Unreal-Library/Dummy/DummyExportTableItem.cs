using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    class DummyExportTableItem
    {
        public int newClassIndex = 0;
        public int newSuperIndex = 0;
        public int newOuterIndex = 0;
        public int newArchetypeIndex = 0;

        public UExportTableItem original;

        public int PackageFlag { get; private set; }

        public long GetExportObjectFlag()
        {
            switch (original.ClassName)
            {
                case "Package":
                    return 0x7000400000000;
                //case "Material":
                //    return 0xF000400000400; // No thumbnail generation for materials
                default:
                    return 0xF000400000000;
            }
        }


        public DummyExportTableItem(UExportTableItem b)
        {
            original = b;
        }
    }
}
