using RLUPKT.Core;
using RLUPKT.Core.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UELib
{
    class RLPackageStream : UPackageStream
    {
        UPKFile upkFile;
        MemoryStream decryptedStream;
        public RLPackageStream(string path)
        {
            upkFile = new UPKFile(path);
            Name = Path.GetFileNameWithoutExtension(path);
            decryptedStream = new MemoryStream();
            upkFile.Decrypt(new RLDecryptor().GetCryptoTransform(), decryptedStream);
            _stream = decryptedStream;
            _stream.Position = 0;
            UR = new UnrealReader(this, _stream);
            UW = new UnrealWriter(_stream);
        }

        public override void Dispose()
        {
            return;
        }
    }
}
