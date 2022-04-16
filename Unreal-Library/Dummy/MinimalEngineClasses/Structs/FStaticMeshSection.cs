using System.IO;
using RLUPKT.Core.UTypes;
using UELib.Core;

namespace UELib.Dummy.Structs
{
    public class FStaticMeshSection : IUESerializable, IDummySerializable
    {
        public UObject Mat { get; set; }
        public int MatIndex { get; set; }
        public int F10 { get; private set; }
        public int F14 { get; private set; }
        public int BEnableShadowCasting { get; private set; }
        public int FirstIndex { get; private set; }
        public int NumFaces { get; private set; }
        public int F24 { get; private set; }
        public int F28 { get; private set; }
        public int Index { get; private set; }
        public TArray<TwoInts> F30 { get; private set; } = new TArray<TwoInts>();
        public byte Unk { get; private set; }

        public void Deserialize(BinaryReader Reader)
        {
            if (Reader is UnrealReader r)
            {
                var package = r.GetPackage();
                Mat = package.Stream.ReadObject();
            }
            else
            {
                MatIndex = Reader.ReadInt32();
            }

            F10 = Reader.ReadInt32();
            F14 = Reader.ReadInt32();
            BEnableShadowCasting = Reader.ReadInt32();
            FirstIndex = Reader.ReadInt32();
            NumFaces = Reader.ReadInt32();
            F24 = Reader.ReadInt32();
            F28 = Reader.ReadInt32();
            Index = Reader.ReadInt32();
            F30.Deserialize(Reader);
            Unk = Reader.ReadByte();
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write((int) 0); // Mat
            writer.Write(F10);
            writer.Write(F14);
            writer.Write(BEnableShadowCasting);
            writer.Write(FirstIndex);
            writer.Write(NumFaces);
            writer.Write(F24);
            writer.Write(F28);
            writer.Write(Index);
            F30.Serialize(writer);
            writer.Write(Unk);
        }
    }

    public class TwoInts : IUESerializable, IDummySerializable
    {
        public int A { get; private set; }
        public int B { get; private set; }

        public void Deserialize(BinaryReader Reader)
        {
            A = Reader.ReadInt32();
            B = Reader.ReadInt32();
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(A);
            writer.Write(B);
        }
    }
}