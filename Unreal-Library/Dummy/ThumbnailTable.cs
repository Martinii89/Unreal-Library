using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    class ThumbnailTable
    {
        List<ThumbnailTableItem> thumbnailTable;
        List<ThumbnailDataItem> thumbnailDataTable;

        public int thumbnailDataOffset;
        public int thumbnailTableOffset;

        public void Init(List<DummyExportTableItem> dummyExports)
        {
            thumbnailTable = new List<ThumbnailTableItem>();
            thumbnailDataTable = new List<ThumbnailDataItem>();
            var exportsWithThumbnail = dummyExports.Where((e) => e.PackageFlag == 0).ToList();
            foreach (var export in exportsWithThumbnail)
            {
                thumbnailTable.Add(new ThumbnailTableItem(export.original.ClassName, export.original.ObjectName, 0));
                // Can everything be a 0 pixel large thumbnail? 
                thumbnailDataTable.Add(new ThumbnailDataItem(0, 0, null));
            }
        }

        public void Serialize(IUnrealStream stream)
        {
            var offsetList = new List<int>();
            thumbnailDataOffset = (int) stream.Position;
            foreach (var thumbnailData in thumbnailDataTable)
            {
                offsetList.Add((int) stream.Position);
                thumbnailData.Serialize(stream);
            }

            thumbnailTableOffset = (int) stream.Position;
            stream.Write(thumbnailTable.Count());
            for (int i = 0; i < thumbnailTable.Count(); i++)
            {
                thumbnailTable[i].dataOffset = offsetList[i];
                thumbnailTable[i].Serialize(stream);
            }
        }
    }

    class ThumbnailTableItem
    {
        string name;
        string group;
        public int dataOffset;

        public ThumbnailTableItem(string name, string group, int dataOffset)
        {
            this.name = name;
            this.group = group;
            this.dataOffset = dataOffset;
        }

        internal void Serialize(IUnrealStream stream)
        {
            stream.WriteString(name);
            stream.WriteString(group);
            stream.Write(dataOffset);
        }
    }

    class ThumbnailDataItem
    {
        public int sizeX;
        public int sizeY;
        public int dataSize;
        public byte[] data;

        public ThumbnailDataItem(int sizeX, int sizeY, byte[] data)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.data = data;
            dataSize = data?.Length ?? 0;
        }

        internal void Serialize(IUnrealStream stream)
        {
            stream.Write(sizeX);
            stream.Write(sizeY);
            stream.Write(dataSize);
            if (dataSize > 0)
            {
                stream.Write(data, 0, data.Length);
            }
        }
    }
}