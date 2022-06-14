using UELib.Types;

namespace UELib.Core
{
    /// <summary>
    ///     Fixed Array Property
    /// </summary>
    [UnrealRegisterClass]
    public class UFixedArrayProperty : UProperty
    {
        public UProperty InnerObject;

        /// <summary>
        ///     Creates a new instance of the UELib.Core.UFixedArrayProperty class.
        /// </summary>
        public UFixedArrayProperty()
        {
            Count = 0;
            Type = PropertyType.FixedArrayProperty;
        }

        public int Count { get; private set; }

        protected override void Deserialize()
        {
            base.Deserialize();

            var innerIndex = _Buffer.ReadObjectIndex();
            InnerObject = (UProperty) GetIndexObject(innerIndex);
            Count = _Buffer.ReadIndex();
        }

        /// <inheritdoc />
        public override string GetFriendlyType()
        {
            // Just move to decompiling?
            return base.GetFriendlyType() + "[" + Count + "]";
        }
    }
}