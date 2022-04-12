using System.IO;
using RLUPKT.Core.UTypes;

namespace UELib.Dummy.Structs
{
    public class FBoxSphereBounds: IUESerializable, IDummySerializable
    {
        public FVector Origin { get; set; } = new FVector();
        public FVector BoxExtent { get; set; } = new FVector();
        public float SphereRadius { get; set; }

        public void Deserialize(BinaryReader Reader)
        {
            Origin.Deserialize(Reader);
            BoxExtent.Deserialize(Reader);
            SphereRadius = Reader.ReadSingle();
        }

        public void Serialize(IUnrealStream writer)
        {
            Origin.Serialize(writer);
            BoxExtent.Serialize(writer);
            writer.Write(SphereRadius);
        }
    }
}