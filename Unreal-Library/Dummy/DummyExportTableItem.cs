using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    class DummyExportTableItem: UExportTableItem
    {
        int newClassIndex;
        int newSuperIndex;
        int newPackageIndex;
        int newArchetypeIndex;


        public DummyExportTableItem(UExportTableItem b)
        {
            this.ClassIndex = b.ClassIndex;
            this.SuperIndex = b.SuperIndex;
            this.OuterIndex = b.OuterIndex;
            this.ArchetypeIndex = b.ArchetypeIndex;
        }
    }
}
