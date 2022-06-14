using System;
using System.IO;
using RLUPKT.Core.UTypes;

namespace UELib.Dummy.Property
{
    public class BaseProperty : IUESerializable
    {
        public UName Name { get; set; }
        public UName Type { get; set; }
        public int Size { get; set; }
        public int ArrayIndex { get; set; }

        public void Deserialize(BinaryReader reader)
        {
            var r = reader as UnrealReader;
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
            Size = reader.ReadInt32();
            ArrayIndex = reader.ReadInt32();
            // TODO actually read the different values
            var typeName = Type.ToString();
            switch (typeName)
            {
                case "IntProperty":
                case "FloatProperty":
                case "StrProperty":
                case "ObjectProperty":
                case "NameProperty":
                    reader.BaseStream.Seek(Size, SeekOrigin.Current);
                    break;
                case "ArrayProperty":
                    reader.BaseStream.Seek(Size, SeekOrigin.Current);
                    break;
                case "BoolProperty":
                    reader.BaseStream.Seek(1, SeekOrigin.Current);
                    break;
                case "ByteProperty":
                    // Two FNames
                    reader.BaseStream.Seek(16, SeekOrigin.Current);
                    break;
                default:
                    throw new NotImplementedException($"offset for {typeName} is now implemented");
            }
        }

        public bool IsObjectProperty()
        {
            return Type.ToString() == "ObjectProperty";
        }

        public bool IsValid()
        {
            return !Name.IsNone();
        }
    }
}