namespace UELib.Core
{
    [UnrealRegisterClass]
    public partial class UTextBuffer : UObject
    {
        protected uint _Pos;
        protected uint _Top;
        public string ScriptText = string.Empty;

        public UTextBuffer()
        {
            ShouldDeserializeOnDemand = true;
        }

        protected override void Deserialize()
        {
            base.Deserialize();
            _Top = _Buffer.ReadUInt32();
            _Pos = _Buffer.ReadUInt32();

            ScriptText = _Buffer.ReadText();
        }
    }
}