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


        public DummyExportTableItem(UExportTableItem b)
        {
            original = b;
        }
    }
}
