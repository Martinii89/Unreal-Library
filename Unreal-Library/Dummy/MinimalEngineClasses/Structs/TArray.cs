using System;
using System.Collections.Generic;
using System.IO;
using RLUPKT.Core.UTypes;

namespace UELib.Dummy.Structs
{
    // List wrapper with Unreal array serialization methods
    public class TArray<T> : List<T>, IUESerializable, IDummySerializable where T : new()
    {
        private Func<T> Constructor = null;

        public TArray() : base()
        {
        }

        public TArray(Func<T> InConstructor) : base()
        {
            Constructor = InConstructor;
        }

        public void Deserialize(BinaryReader Reader)
        {
            var Length = Reader.ReadInt32();

            Clear();
            Capacity = Length;

            for (var i = 0; i < Length; i++)
            {
                var Elem = Constructor != null ? Constructor() : new T();
                Elem = (T) (GenericSerializer.Deserialize(Elem, Reader));
                Add(Elem);
            }
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(Count);
            var serializable = typeof(T).GetInterface(nameof(IDummySerializable));
            if (serializable == null)
            {
                throw new InvalidCastException($"Can't serialize this type: {typeof(T).Name}");
            }
            foreach (var unknown in this)
            {
                var element = (IDummySerializable)unknown;
                element.Serialize(writer);
            }
        }
    }
}