using System;
using System.Collections.Generic;
using System.IO;
using RLUPKT.Core.UTypes;

namespace UELib.Dummy.Structs
{
    public class TArrayWithElemtSize<T>: List<T>, IUESerializable, IDummySerializable where T : new ()
    {
        private readonly Func<T> _constructor;

        public TArrayWithElemtSize()
        { }

        public TArrayWithElemtSize(Func<T> inConstructor)
        {
            _constructor = inConstructor;
        }

        public int ElementSize { get; set; }
        public int ElementCount { get; set; }
        public void Deserialize(BinaryReader reader)
        {
            ElementSize = reader.ReadInt32();
            ElementCount = reader.ReadInt32();

            Clear();
            Capacity = ElementCount;

            for (var i = 0; i < ElementCount; i++)
            {
                var elem = _constructor != null ? _constructor() : new T();
                elem = (T)(GenericSerializer.Deserialize(elem, reader));
                Add(elem);
            }
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(ElementSize);
            writer.Write(ElementCount);

            foreach (var unknown in this)
            {
                switch (unknown)
                {
                    case IDummySerializable i:
                        i.Serialize(writer);
                        break;
                    case int i :
                        writer.Write(i);
                        break;
                    case ushort i:
                        writer.Write(i);
                        break;
                    default:
                        throw new InvalidCastException(nameof(T));
                }

            }
        }
    }
}