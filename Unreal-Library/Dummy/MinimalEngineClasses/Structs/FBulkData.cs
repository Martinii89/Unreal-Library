using System;
using System.IO;
using RLUPKT.Core.UTypes;

namespace UELib.Dummy.Structs
{
    public class FBulkData: IUESerializable, IDummySerializable
    {
        public uint BulkDataFlags { get; set; }
        public int ElementCount { get; set; }
        public int BulkDataSizeOnDisk { get; set; }
        public int BulkDataOffsetInFile { get; set; }
        public byte[] BulkData { get; set; }


        public void Deserialize(BinaryReader Reader)
        {
            BulkDataFlags = Reader.ReadUInt32();
            ElementCount = Reader.ReadInt32();
            BulkDataSizeOnDisk = Reader.ReadInt32();
            if ((BulkDataFlags & BulkdataStoreInSeparateFile) != 0)
            {
                BulkDataOffsetInFile = (int) Reader.ReadInt64();
            }
            else
            {
                BulkDataOffsetInFile = (int)Reader.BaseStream.Position;

            }
            if (BulkDataSizeOnDisk > 0)
            {
                BulkData = Reader.ReadBytes(BulkDataSizeOnDisk);
            }
        }

        private const uint BulkdataStoreInSeparateFile = 0x01;

        public void Serialize(IUnrealStream writer)
        {
            writer.Write(BulkDataFlags);
            writer.Write(ElementCount);
            writer.Write(BulkDataSizeOnDisk);
            // TODO: Verify!
            writer.Write((int)(writer.Position+4));
            if (BulkDataSizeOnDisk > 0)
            {
                writer.Write(BulkData, 0, BulkDataSizeOnDisk);
            }
        }
    }
}