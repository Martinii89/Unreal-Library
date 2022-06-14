using System;
using System.Diagnostics.Contracts;
using System.IO;
using UELib.Core;

namespace UELib
{
    /// <summary>
    ///     Represents a unreal table with general deserialized data from a unreal package header.
    /// </summary>
    public abstract class UObjectTableItem : UTableItem, IBuffered
    {
        /// <summary>
        ///     Reference to the serialized object based on this table.
        ///     Only valid if Owner != null and Owner is fully serialized or on demand.
        /// </summary>
        public UObject Object;

        /// <summary>
        ///     Reference to the UnrealPackage this object resists in
        /// </summary>
        public UnrealPackage Owner;

        /// <summary>
        ///     Name index to the name of this object
        ///     -- Fixed
        /// </summary>
        [Pure] public UNameTableItem ObjectTable => Owner.Names[(int) ObjectName];

        public UName ObjectName { get; set; }

        /// <summary>
        ///     Import:Name index to the class of this object
        ///     Export:Object index to the class of this object
        ///     -- Not Fixed
        /// </summary>
        /// TODO: Fix at a later point. Focus on mesh data now!
        public int ClassIndex { get; protected set; }

        [Pure] public UObjectTableItem ClassTable => Owner.GetIndexTable(ClassIndex);
        [Pure] public virtual string ClassName => ClassIndex != 0 ? Owner.GetIndexTable(ClassIndex).ObjectName : "class";

        /// <summary>
        ///     Object index to the outer of this object
        ///     -- Not Fixed
        /// </summary>
        /// TODO: Fix at a later point. Focus on mesh data now!
        public int OuterIndex { get; protected set; }


        [Pure] public UObjectTableItem OuterTable => Owner.GetIndexTable(OuterIndex);

        [Pure] public string OuterName
        {
            get
            {
                var table = OuterTable;
                return table != null ? table.ObjectName : string.Empty;
            }
        }

        public virtual byte[] CopyBuffer()
        {
            var buff = new byte[Size];
            Owner.Stream.Seek(Offset, SeekOrigin.Begin);
            Owner.Stream.Read(buff, 0, Size);
            if (Owner.Stream.BigEndianCode)
            {
                Array.Reverse(buff);
            }

            return buff;
        }

        [Pure]
        public IUnrealStream GetBuffer()
        {
            return Owner.Stream;
        }

        [Pure]
        public int GetBufferPosition()
        {
            return Offset;
        }

        [Pure]
        public int GetBufferSize()
        {
            return Size;
        }

        [Pure]
        public string GetBufferId(bool fullName = false)
        {
            return fullName ? Owner.PackageName + "." + ObjectName + ".table" : ObjectName + ".table";
        }
    }
}