using System.IO;
using RLUPKT.Core.UTypes;

namespace UELib.Dummy.Structs
{
    public class FVector: IUESerializable, IDummySerializable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public void Deserialize(BinaryReader Reader)
        {
            X = Reader.ReadSingle();
            Y = Reader.ReadSingle();
            Z = Reader.ReadSingle();
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }
    }
}