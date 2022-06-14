using System.Collections.Generic;
using UELib.Dummy.Property;

namespace UELib.Dummy
{
    public class FakeExportItem : IUExportTableItem
    {
        public List<BaseProperty> FakeProperties = new List<BaseProperty>();

        public FakeExportItem(string className, int classIndex, int superIndex, UName objectName, int outerIndex, int archetypeIndex, ulong objectFlags)
        {
            ObjectFlags = objectFlags;
            SuperIndex = superIndex;
            ArchetypeIndex = archetypeIndex;
            ObjectName = objectName;
            ClassName = className;
            ClassIndex = classIndex;
            OuterIndex = outerIndex;
            SerialOffset = 0;
            SerialSize = 0;
        }


        public ulong ObjectFlags { get; set; }
        public int SuperIndex { get; }
        public int ArchetypeIndex { get; }
        public UName ObjectName { get; set; }
        public string ClassName { get; }
        public int ClassIndex { get; }
        public int OuterIndex { get; }
        public int SerialOffset { get; set; }
        public int SerialSize { get; set; }
    }
}