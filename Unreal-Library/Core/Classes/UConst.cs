namespace UELib.Core
{
    /// <summary>
    ///     Represents a unreal const.
    /// </summary>
    [UnrealRegisterClass]
    public partial class UConst : UField
    {
        /// <summary>
        ///     Constant Value
        /// </summary>
        public string Value { get; private set; }

        protected override void Deserialize()
        {
            base.Deserialize();
            Value = _Buffer.ReadText();
        }
    }
}