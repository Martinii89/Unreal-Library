using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using RLUPKT.Core.UTypes;

namespace UELib.Dummy.Property
{
    public class BaseProperty : IUESerializable
    {
        public UName Name { get; set; }
        public UName Type { get; set; }
        public int Size { get; set; }
        public int ArrayIndex { get; set; }

        public bool IsObjectProperty() => Type.ToString() == "ObjectProperty";

        public void Deserialize(BinaryReader Reader)
        {
            var r = Reader as UnrealReader;
            if (r == null)
            {
                throw new InvalidCastException("Reader was not a unrealreader");
            }

            var stream = r.GetStream();
            Name = stream.ReadNameReference();
            if (Name.IsNone())
            {
                return;
            }

            Type = stream.ReadNameReference();
            Size = Reader.ReadInt32();
            ArrayIndex = Reader.ReadInt32();
            // TODO actually read the different values
            var typeName = Type.ToString();
            switch (typeName)
            {
                case "IntProperty":
                case "StrProperty":
                case "ObjectProperty":
                case "NameProperty":
                    Reader.BaseStream.Seek(Size, SeekOrigin.Current);
                    break;
                case "BoolProperty":
                    Reader.BaseStream.Seek(1, SeekOrigin.Current);
                    break;
                case "ByteProperty":
                    // Two FNames
                    Reader.BaseStream.Seek(16, SeekOrigin.Current);
                    break;
                default:
                    throw new NotImplementedException($"offset for {typeName} is now implemented");
            }
        }

        public bool IsValid()
        {
            return !Name.IsNone();
        }
    }
}