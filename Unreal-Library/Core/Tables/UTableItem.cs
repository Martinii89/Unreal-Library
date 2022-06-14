namespace UELib
{
    /// <summary>
    ///     Represents a basic file table.
    /// </summary>
    public abstract class UTableItem
    {
        /// <summary>
        ///     Index into the table's enumerable.
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        ///     Table offset in bytes.
        /// </summary>
        public int Offset { get; internal set; }

        /// <summary>
        ///     Table size in bytes.
        /// </summary>
        public int Size { get; internal set; }

        public string ToString(bool shouldPrintMembers)
        {
            return shouldPrintMembers
                ? string.Format("\r\nTable Index:{0}\r\nTable Offset:0x{1:X8}\r\nTable Size:0x{2:X8}\r\n", Index, Offset, Size)
                : base.ToString();
        }
    }
}