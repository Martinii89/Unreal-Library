using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    class DummyImportTableItem
    {

        public int newOuterIndex = 0;

        public UImportTableItem original;

        public DummyImportTableItem(UImportTableItem original)
        {
            this.original = original;
            newOuterIndex = original.OuterIndex;
        }
    }
}
