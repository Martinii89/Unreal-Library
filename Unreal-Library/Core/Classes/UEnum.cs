using System.Collections.Generic;

namespace UELib.Core
{
    /// <summary>
    ///     Represents a unreal enum.
    /// </summary>
    [UnrealRegisterClass]
    public partial class UEnum : UField
    {
        /// <summary>
        ///     Names of each element in the UEnum.
        /// </summary>
        public IList<UName> Names;

        protected override void Deserialize()
        {
            base.Deserialize();

            var count = ReadCount();
            Names = new List<UName>(count);
            for (var i = 0; i < count; ++i)
            {
                Names.Add(_Buffer.ReadNameReference());
            }
        }
    }
}