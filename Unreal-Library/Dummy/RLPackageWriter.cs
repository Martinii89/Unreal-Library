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
        private uint _version;

        protected override uint _Version
        {
            get { return _version; }
        }

        public RLPackageWriter(Stream stream, uint version) : base(stream)
        {
            _version = version;
        }
    }
}