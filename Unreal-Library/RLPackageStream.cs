using RLUPKT.Core;
using System.IO;

namespace UELib
{
    class RLPackageStream : UPackageStream
    {
        UPKFile upkFile;
        MemoryStream decryptedStream;
        public DecryptionState decryptionState;
        public RLPackageStream(string path)
        {
            upkFile = new UPKFile(path);
            Name = path;
            decryptedStream = new MemoryStream();
            decryptionState = upkFile.Decrypt(decryptedStream);
            _stream = decryptedStream;
            _stream.Position = 0;
            UR = new UnrealReader(this, _stream);
            UW = new UnrealWriter(_stream);
        }

        public override void Dispose()
        {
        }
    }
}
