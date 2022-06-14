using UELib.Flags;

namespace UELib.Core
{
    [UnrealRegisterClass]
    public class UScriptStruct : UStruct
    {
        protected override void Deserialize()
        {
            base.Deserialize();
            StructFlags = _Buffer.ReadUInt32();
            Record("StructFlags", (StructFlags) StructFlags);
            DeserializeProperties();
        }
    }
}