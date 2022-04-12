using System;
using System.Collections.Generic;
using System.Linq;

namespace UELib.Dummy
{
    internal abstract class MinimalBase
    {
        protected MinimalBase(UExportTableItem exportTableItem, UnrealPackage package)
        {
            ExportTableItem = exportTableItem;
            Package = package;
        }

        protected abstract byte[] MinimalByteArray { get; }

        protected UExportTableItem ExportTableItem { get; }
        public UnrealPackage Package { get; }

        public abstract void Write(IUnrealStream stream, UnrealPackage package);
        public abstract int GetSerialSize();

        protected void FixNameIndexAtPosition(UnrealPackage package, string name, int startPosition)
        {
            var test = package.Names.FindIndex(n => n.Name == name);
            var bytes = BitConverter.GetBytes(test);
            for (var i = 0; i < bytes.Length; i++) MinimalByteArray[i + startPosition] = bytes[i];
        }


        public static void AddNamesToNameTable(UnrealPackage package, IList<string> namesToAdd)
        {
            foreach (var name in namesToAdd)
                if (package.Names.All(o => o.Name != name))
                    package.Names.Add(new UNameTableItem {Name = name, Flags = 1970393556451328, Index = package.Names.Count});
        }
    }
}