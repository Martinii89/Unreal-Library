using System;
using System.Collections.Generic;
using System.IO;

namespace UELib.Dummy.Property
{
    public class PropertyListSerializer
    {
        private List<BaseProperty> Deserialize(IUnrealStream stream)
        {
            var properties = new List<BaseProperty>();
            var propertyName = stream.ReadNameReference();
            if (propertyName.IsNone())
            {
                return null;
            }

            var propertyType = stream.ReadNameReference();
            var size = stream.ReadInt32();
            var arrayIndex = stream.ReadInt32();

            var typeName = propertyType.ToString();
            switch (typeName)
            {
                case "IntProperty":
                    properties.Add(new IntProperty(stream, propertyName, propertyType, size, arrayIndex));
                    break;
                case "FloatProperty":
                case "StrProperty":
                case "ObjectProperty":
                case "NameProperty":
                    stream.Seek(size, SeekOrigin.Current);
                    break;
                case "ArrayProperty":
                    stream.Seek(size, SeekOrigin.Current);
                    break;
                case "BoolProperty":
                    stream.Seek(1, SeekOrigin.Current);
                    break;
                case "ByteProperty":
                    // Two FNames
                    stream.Seek(16, SeekOrigin.Current);
                    break;
                default:
                    throw new NotImplementedException($"offset for {typeName} is now implemented");
            }

            return properties;
        }
    }
}