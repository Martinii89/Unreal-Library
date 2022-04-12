using System.IO;
using RLUPKT.Core.UTypes;

namespace UELib.Dummy.Structs
{
    public class FkDOPBounds: IUESerializable, IDummySerializable
    {
        public FVector V1 { get; set; } = new FVector();
        public FVector V2 { get; set; } = new FVector();

        public void Deserialize(BinaryReader Reader)
        {
            V1.Deserialize(Reader);
            V2.Deserialize(Reader);
        }

        public void Serialize(IUnrealStream writer)
        {
            V1.Serialize(writer);
            V2.Serialize(writer);
        }
    }
}