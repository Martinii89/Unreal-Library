using System;
using UELib.Flags;
using UELib.Types;

namespace UELib.Core
{
    /// <summary>
    ///     Represents a unreal property.
    /// </summary>
    public partial class UProperty : UField, IUnrealNetObject
    {
        /// <summary>
        ///     Creates a new instance of the UELib.Core.UProperty class.
        /// </summary>
        public UProperty()
        {
            Type = PropertyType.None;
        }

        public PropertyType Type { get; protected set; }

        public ushort ArrayDim { get; private set; }

        public ushort ElementSize { get; private set; }

        public ulong PropertyFlags { get; private set; }

        public int CategoryIndex { get; private set; }

        public UEnum ArrayEnum { get; private set; }

        private bool _IsArray => ArrayDim > 1;

        public string CategoryName => CategoryIndex != -1 ? Package.Names[CategoryIndex].Name : "@Null";

        public ushort RepOffset { get; private set; }

        public bool RepReliable => HasPropertyFlag(PropertyFlagsLO.Net);

        public uint RepKey => RepOffset | ((uint) Convert.ToByte(RepReliable) << 16);

        protected override void Deserialize()
        {
            base.Deserialize();

            var info = _Buffer.ReadUInt32();
            ArrayDim = (ushort) (info & 0x0000FFFFU);
            Record("ArrayDim", ArrayDim);
            ElementSize = (ushort) (info >> 16);
            Record("ElementSize", ElementSize);
            skipInfo:

            PropertyFlags = Package.Version >= 220 ? _Buffer.ReadUInt64() : _Buffer.ReadUInt32();
            Record("PropertyFlags", PropertyFlags);

            if (!Package.IsConsoleCooked())
            {
                CategoryIndex = _Buffer.ReadNameIndex();
                Record("CategoryIndex", CategoryIndex);

                ArrayEnum = GetIndexObject(_Buffer.ReadObjectIndex()) as UEnum;
                Record("ArrayEnum", ArrayEnum);
            }
            else
            {
                CategoryIndex = -1;
            }

            if (HasPropertyFlag(PropertyFlagsLO.Net))
            {
                RepOffset = _Buffer.ReadUShort();
                Record("RepOffset", RepOffset);
            }

            //Kinda pointless that the name is here as a Fstring, we already have the name from UObject?
            var _name = _Buffer.ReadText();
        }

        protected override bool CanDisposeBuffer()
        {
            return true;
        }

        public bool HasPropertyFlag(PropertyFlagsLO flag)
        {
            return ((uint) (PropertyFlags & 0x00000000FFFFFFFFU) & (uint) flag) != 0;
        }

        public bool HasPropertyFlag(PropertyFlagsHO flag)
        {
            return ((PropertyFlags >> 32) & (uint) flag) != 0;
        }

        public bool IsParm()
        {
            return HasPropertyFlag(PropertyFlagsLO.Parm);
        }

        public virtual string GetFriendlyInnerType()
        {
            return string.Empty;
        }
    }
}