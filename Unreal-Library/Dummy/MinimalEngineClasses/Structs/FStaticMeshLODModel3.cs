using System;
using System.IO;
using RLUPKT.Core.UTypes;

namespace UELib.Dummy.Structs
{
    public class FStaticMeshLODModel3 : IUESerializable, IDummySerializable
    {
        public FBulkData FBulkData { get; set; } = new FBulkData();
        public TArray<FStaticMeshSection> FStaticMeshSections { get; set; } = new TArray<FStaticMeshSection>();
        public VertexStream VertexStream { get; set; } = new VertexStream();
        public UvStream UvStream { get; set; } = new UvStream();
        public ColorStream ColorStream { get; set; } = new ColorStream();
        public int NumVerts { get; set; }
        public TArrayWithElemtSize<ushort> Indicies { get; set; } = new TArrayWithElemtSize<ushort>();
        public TArrayWithElemtSize<ushort> Indicies2 { get; set; } = new TArrayWithElemtSize<ushort>();
        public TArrayWithElemtSize<ushort> Indicies3 { get; set; } = new TArrayWithElemtSize<ushort>();

        public void Deserialize(BinaryReader Reader)
        {
            FBulkData.Deserialize(Reader);
            FStaticMeshSections.Deserialize(Reader);
            VertexStream.Deserialize(Reader);
            UvStream.Deserialize(Reader);
            ColorStream.Deserialize(Reader);
            NumVerts = Reader.ReadInt32();
            Indicies.Deserialize(Reader);
            Indicies2.Deserialize(Reader);
            Indicies3.Deserialize(Reader);
        }

        public void Serialize(IUnrealStream writer)
        {
            FBulkData.Serialize(writer);
            FStaticMeshSections.Serialize(writer);
            VertexStream.Serialize(writer);
            UvStream.Serialize(writer);
            ColorStream.Serialize(writer);
            writer.Write(NumVerts);
            Indicies.Serialize(writer);
            Indicies2.Serialize(writer);
            Indicies3.Serialize(writer);
        }
    }

    public class ColorStream : IUESerializable, IDummySerializable
    {
        public int ItemSize { get; set; }
        public int NumVerts { get; set; }
        public TArrayWithElemtSize<FColor> Colors { get; set; } = new TArrayWithElemtSize<FColor>();


        public void Deserialize(BinaryReader Reader)
        {
            ItemSize = Reader.ReadInt32();
            NumVerts = Reader.ReadInt32();

            if (NumVerts > 0)
            {
                Colors.Deserialize(Reader);
            }
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(ItemSize);
            writer.Write(NumVerts);
            if (NumVerts > 0)
            {
                Colors.Serialize(writer);
            }
        }
    }

    public class FColor : IUESerializable, IDummySerializable
    {
        public byte R { get; private set; }
        public byte G { get; private set; }
        public byte B { get; private set; }
        public byte A { get; private set; }

        public void Deserialize(BinaryReader Reader)
        {
            R = Reader.ReadByte();
            G = Reader.ReadByte();
            B = Reader.ReadByte();
            A = Reader.ReadByte();
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(R);
            writer.Write(G);
            writer.Write(B);
            writer.Write(A);
        }
    }

    public class VertexStream : IUESerializable, IDummySerializable
    {
        public int VertexSize { get; set; }
        public int VertexCount { get; set; }
        private TArrayWithElemtSize<FVector> VertexStreamArray { get; set; } = new TArrayWithElemtSize<FVector>();

        public void Deserialize(BinaryReader Reader)
        {
            VertexSize = Reader.ReadInt32();
            VertexCount = Reader.ReadInt32();
            VertexStreamArray.Deserialize(Reader);
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(VertexSize);
            writer.Write(VertexCount);
            VertexStreamArray.Serialize(writer);
        }
    }

    public class UvStream : IUESerializable, IDummySerializable
    {
        public int NumTexCords { get; set; }
        public int ItemSize { get; set; }
        public int NumVerts { get; set; }
        public int BUseFullPrecisionUVs { get; set; }
        public TArrayWithElemtSize<UvItem> UvStreamItems { get; set; }

        public void Deserialize(BinaryReader Reader)
        {
            NumTexCords = Reader.ReadInt32();
            ItemSize = Reader.ReadInt32();
            NumVerts = Reader.ReadInt32();
            BUseFullPrecisionUVs = Reader.ReadInt32();
            UvStreamItems = new TArrayWithElemtSize<UvItem>(() => new UvItem(new UvHalf[NumTexCords]));
            UvStreamItems.Deserialize(Reader);
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(NumTexCords);
            writer.Write(ItemSize);
            writer.Write(NumVerts);
            writer.Write(BUseFullPrecisionUVs);
            UvStreamItems.Serialize(writer);
        }
    }

    public class UvItem : IUESerializable, IDummySerializable
    {
        public FPackedNormal N0 { get; } = new FPackedNormal();
        public FPackedNormal N1 { get; } = new FPackedNormal();
        public UvHalf[] Uv { get; }

        public UvItem()
        {
        }

        public UvItem(UvHalf[] uv)
        {
            Uv = uv;
        }

        public void Deserialize(BinaryReader Reader)
        {
            N0.Deserialize(Reader);
            N1.Deserialize(Reader);
            for (var i = 0; i < Uv.Length; i++)
            {
                Uv[i] = new UvHalf();
                Uv[i].Deserialize(Reader);
            }
        }

        public void Serialize(IUnrealStream writer)
        {
            N0.Serialize(writer);
            N1.Serialize(writer);
            foreach (var uvHalf in Uv)
            {
                uvHalf.Serialize(writer);
            }
        }
    }

    public class FPackedNormal : IUESerializable, IDummySerializable
    {
        public uint Data { get; private set; }

        public void Deserialize(BinaryReader Reader)
        {
            Data = Reader.ReadUInt32();
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(Data);
        }
    }

    public class UvHalf : IUESerializable, IDummySerializable
    {
        public ushort A { get; private set; }
        public ushort B { get; private set; }

        public void Deserialize(BinaryReader Reader)
        {
            A = Reader.ReadUInt16();
            B = Reader.ReadUInt16();
        }

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(A);
            writer.Write(B);
        }
    }
}