namespace UELib.Dummy.Property
{
    public class IntProperty : BaseProperty
    {
        public IntProperty(IUnrealStream stream, string propertyName, UName propertyType, int propertySize, int arrayIndex)
        {
            Value = stream.ReadInt32();
        }

        private int Value { get; }
    }
}