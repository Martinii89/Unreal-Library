using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UELib.Dummy.Property;

namespace UELib.Dummy
{
    internal abstract class MinimalBase
    {
        protected MinimalBase(IUExportTableItem exportTableItem, UnrealPackage package)
        {
            ExportTableItem = exportTableItem;
            Package = package;
        }

        protected abstract byte[] MinimalByteArray { get; }
        protected IUExportTableItem ExportTableItem { get; }
        protected UnrealPackage Package { get; }

        public long ScriptPropertiesEnd { get; private set; }
        public int SerialSize { get; private set; }
        public int SerialOffset { get; private set; }

        protected abstract void WriteSerialData(IUnrealStream stream, UnrealPackage package);

        protected void ReadScriptProperties()
        {
            var reader = Package.Stream.UR;
            var property = new BaseProperty();
            property.Deserialize(reader);
            var shouldNullObjectReferences = ShouldNullObjectReferences();
            while (property.IsValid())
            {
                if (property.IsObjectProperty() && shouldNullObjectReferences)
                {
                    // Null out any object references instead of trying to fix up the index.
                    Debug.Assert(property.Size == 4, "Object property was not 4. Freak out!");
                    Package.Stream.Skip(-property.Size);
                    Package.Stream.Write(0);
                }

                property.Deserialize(reader);
            }

            ScriptPropertiesEnd = reader.BaseStream.Position;
        }

        public void Write(IUnrealStream stream, UnrealPackage package)
        {
            SerialOffset = (int) stream.Position;
            WriteSerialData(stream, package);
            SerialSize = (int) (stream.Position - SerialOffset);
        }

        protected void WriteScriptProperties(IUnrealStream stream)
        {
            Package.Stream.UR.BaseStream.Seek(ExportTableItem.SerialOffset, SeekOrigin.Begin);
            var propertyBuffer = Package.Stream.UR.ReadBytes((int) (ScriptPropertiesEnd - ExportTableItem.SerialOffset));
            stream.Write(propertyBuffer, 0, propertyBuffer.Length);
        }

        protected virtual bool ShouldNullObjectReferences()
        {
            return true;
        }

        protected void FixNameIndexAtPosition(UnrealPackage package, string name, int startPosition)
        {
            var test = package.Names.FindIndex(n => n.Name == name);
            var bytes = BitConverter.GetBytes(test);
            for (var i = 0; i < bytes.Length; i++)
            {
                MinimalByteArray[i + startPosition] = bytes[i];
            }
        }


        public static void AddNamesToNameTable(UnrealPackage package, IList<string> namesToAdd)
        {
            foreach (var name in namesToAdd)
            {
                if (package.Names.All(o => o.Name != name))
                {
                    package.Names.Add(new UNameTableItem { Name = name, Flags = 1970393556451328, Index = package.Names.Count });
                }
            }
        }
    }
}