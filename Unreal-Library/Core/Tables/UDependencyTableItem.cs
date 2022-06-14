using System.Collections.Generic;
using UELib.Core;

namespace UELib
{
    public sealed class UDependencyTableItem : UTableItem, IUnrealDeserializableClass
    {
        public List<int> Dependencies;

        public void Deserialize(IUnrealStream stream)
        {
            Dependencies.Deserialize(stream);
        }
    }
}