using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Core
{
    /// <summary>
    /// QWord Property
    /// </summary>
    [UnrealRegisterClass]
    public class UQWordProperty : UIntProperty
    {
        /// <inheritdoc/>
        public override string GetFriendlyType()
        {
            return "Qword";
        }
    }
}
