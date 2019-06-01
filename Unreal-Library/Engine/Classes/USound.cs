using System.Collections.Generic;
using UELib.Core;

namespace UELib.Engine
{
    [UnrealRegisterClass]
    public class USound : UObject, IUnrealViewable, IUnrealExportable
    {
        private const string WAVExtension = "wav";

        public IEnumerable<string> ExportableExtensions
        {
            get{ return new[]{WAVExtension}; }
        }

        private string SoundFormat{ get; set; }

        private byte[] _SoundBuffer;

        public USound()
        {
            ShouldDeserializeOnDemand = true;
        }

        public bool CompatableExport()
        {
            return Package.Version >= 61 && Package.Version <= 129
                && SoundFormat != null && SoundFormat.ToLower() == WAVExtension && _SoundBuffer != null;
        }

        public void SerializeExport( string desiredExportExtension, System.IO.Stream exportStream )
        {
            switch( desiredExportExtension )
            {
                case WAVExtension:
                    exportStream.Write( _SoundBuffer, 0, _SoundBuffer.Length );
                    break;
            }
        }

        protected override void Deserialize()
        {
            base.Deserialize();

            // Format
            SoundFormat = Package.GetIndexName( _Buffer.ReadNameIndex() );
            Record( "SoundFormat", SoundFormat );

            if( Package.Version >= 63 )
            {
                // OffsetNext
                _Buffer.Skip( 4 );
                Record( "OffsetNext" );
            }

            var size = _Buffer.ReadIndex();
            Record( "soundSize", size );
            // Resource Interchange File Format
            _Buffer.Read( _SoundBuffer = new byte[size], 0, size );
        }
    }
}