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


        public void Serialize(IUnrealStream stream)
        {
            stream.Write(original.PackageName);
            stream.Write(original._ClassName);
            stream.Write(newOuterIndex); // Always an ordinary integer
            stream.Write(original.ObjectName);
        }
    }
}
