using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib.Dummy
{
    internal class RLPackageWriter : UnrealWriter
    {
        protected override uint _Version { get; }

        public RLPackageWriter(Stream stream, uint version) : base(stream)
        {
            _Version = version;
        }
    }
}