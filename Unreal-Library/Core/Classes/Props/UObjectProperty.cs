using UELib.Types;

namespace UELib.Core
{
    /// <summary>
    ///     Object Reference Property
    /// </summary>
    [UnrealRegisterClass]
    public class UObjectProperty : UProperty
    {
        /// <summary>
        ///     Creates a new instance of the UELib.Core.UObjectProperty class.
        /// </summary>
        public UObjectProperty()
        {
            Type = PropertyType.ObjectProperty;
        }

        public UObject Object { get; private set; }

        protected override void Deserialize()
        {
            base.Deserialize();

            var objectIndex = _Buffer.ReadObjectIndex();
            Object = GetIndexObject(objectIndex);
        }

        /// <inheritdoc />
        public override string GetFriendlyType()
        {
            return Object != null ? Object.GetFriendlyType() : "@NULL";
        }
    }
}