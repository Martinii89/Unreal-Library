using System;
using System.Collections.Generic;
using UELib.Flags;

namespace UELib.Core
{
    /// <summary>
    ///     Represents a unreal function.
    /// </summary>
    [UnrealRegisterClass]
    public partial class UFunction : UStruct, IUnrealNetObject
    {
        // TODO: Corrigate version. Attested in version 61, but deprecated since at least version 68.
        private const uint VDeprecatedData = 68;
        private const uint VFriendlyName = 189;

        public ushort NativeToken { get; private set; }

        public byte OperPrecedence { get; private set; }

        /// <value>
        ///     32bit in UE2
        ///     64bit in UE3
        /// </value>
        private ulong FunctionFlags { get; set; }

        public List<UProperty> Params { get; private set; }
        public UProperty ReturnProperty { get; private set; }

        public ushort RepOffset { get; private set; }

        public bool RepReliable => HasFunctionFlag(Flags.FunctionFlags.NetReliable);

        public uint RepKey => RepOffset | ((uint) Convert.ToByte(RepReliable) << 16);

        protected override void Deserialize()
        {
            base.Deserialize();

            NativeToken = _Buffer.ReadUShort();
            Record("NativeToken", NativeToken);
            OperPrecedence = _Buffer.ReadByte();
            Record("OperPrecedence", OperPrecedence);
            if (Package.Version < VDeprecatedData)
            {
                // ParmsSize, NumParms, and ReturnValueOffset
                _Buffer.Skip(5);
            }


            FunctionFlags = _Buffer.ReadUInt64();

            Record("FunctionFlags", (FunctionFlags) FunctionFlags);
            if (HasFunctionFlag(Flags.FunctionFlags.Net))
            {
                RepOffset = _Buffer.ReadUShort();
                Record("RepOffset", RepOffset);
            }


            if (Package.Version >= VFriendlyName && !Package.IsConsoleCooked())
            {
                FriendlyName = _Buffer.ReadNameReference();
                Record("FriendlyName", FriendlyName);
            }
        }

        protected override void FindChildren()
        {
            base.FindChildren();
            Params = new List<UProperty>();
            foreach (var property in Variables)
            {
                if (property.HasPropertyFlag(PropertyFlagsLO.ReturnParm))
                {
                    ReturnProperty = property;
                }

                if (property.IsParm())
                {
                    Params.Add(property);
                }
            }
        }

        public bool HasFunctionFlag(FunctionFlags flag)
        {
            return ((uint) FunctionFlags & (uint) flag) != 0;
        }

        public bool IsOperator()
        {
            return HasFunctionFlag(Flags.FunctionFlags.Operator);
        }

        public bool IsPost()
        {
            return IsOperator() && OperPrecedence == 0;
        }

        public bool IsPre()
        {
            return IsOperator() && HasFunctionFlag(Flags.FunctionFlags.PreOperator);
        }
    }
}