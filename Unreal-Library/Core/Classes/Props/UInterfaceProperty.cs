using UELib.Types;

namespace UELib.Core
{
    /// <summary>
    ///     Interface Property
    ///     UE3 Only
    /// </summary>
    [UnrealRegisterClass]
    public class UInterfaceProperty : UProperty
    {
        public UClass InterfaceObject;
        //public UInterfaceProperty InterfaceType = null;

        /// <summary>
        ///     Creates a new instance of the UELib.Core.UInterfaceProperty class.
        /// </summary>
        public UInterfaceProperty()
        {
            Type = PropertyType.InterfaceProperty;
        }

        protected override void Deserialize()
        {
            base.Deserialize();

            var index = _Buffer.ReadObjectIndex();
            InterfaceObject = (UClass) GetIndexObject(index);

            //Index = _Buffer.ReadObjectIndex();
            //_InterfaceType = (UInterfaceProperty)GetIndexObject( Index );
        }

        /// <inheritdoc />
        public override string GetFriendlyType()
        {
            return InterfaceObject != null ? InterfaceObject.GetFriendlyType() : "@NULL";
        }
    }
}