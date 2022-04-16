using System.IO;
using RLUPKT.Core.UTypes;

namespace UELib.Dummy.Structs
{
    public class FkDOPNode3New: IUESerializable, IDummySerializable
    {
        public byte[] Mins { get; } = new byte[3];
        public byte[] Maxs { get; } = new byte[3];

        public void Deserialize(BinaryReader Reader)
        {
            Reader.Read(Mins, 0, 3);
            Reader.Read(Maxs, 0, 3);
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(Mins, 0, 3);
            writer.Write(Maxs, 0, 3);
        }
    }
}