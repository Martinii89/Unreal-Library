using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    internal abstract class MinimalBase
    {
        //protected static int serialSize;
        abstract protected byte[] minimalByteArray { get;}

        abstract public void Write(IUnrealStream stream, UnrealPackage package);
        abstract public int GetSerialSize();

        protected void FixNameIndexAtPosition(UnrealPackage package, string name, int startPosition)
        {
            var test = package.Names.FindIndex((n) => n.Name == name);
            var bytes = BitConverter.GetBytes(test);
            for (int i = 0; i < bytes.Length; i++)
            {
                minimalByteArray[i + startPosition] = bytes[i];
            }
        }



        protected static void AddNamesToNameTable(UnrealPackage package, IList<string> namesToAdd)
        {
            foreach (var name in namesToAdd)
            {
                if (!package.Names.Any((o) => o.Name == name))
                {
                    package.Names.Add(new UNameTableItem() { Name = name, Flags = 1970393556451328 });
                }
            }
        }

    }
}