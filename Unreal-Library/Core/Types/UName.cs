using System;
using System.Diagnostics;
using UELib.Logging;

namespace UELib
{
    public sealed class UName : IUnrealSerializableClass
    {
        private const string    None = "None";
        public const int        Numeric = 0;
        internal const int      VNameNumbered = 343;

        private UNameTableItem  _NameItem;
        private int             _Number;
        public int              Number{ get; private set; }

        private string          _Text
        {
            get{ return _Number > Numeric ? _NameItem.Name + "_" + _Number : _NameItem.Name; }
        }

        private int             _Index
        {
            get{ return _NameItem.Index; }
        }

        public int              Index{ get; private set; }

        public int              Length
        {
            get{ return _Text.Length; }
        }

        public UName( IUnrealStream stream )
        {
            Deserialize( stream );
        }

        public UName( UNameTableItem nameItem, int num )
        {
            _NameItem = nameItem;
            _Number = num;
        }

        public UName(UnrealPackage package, string name)
        {
            foreach (var tableItem in package.Names)
            {
                if (tableItem.Name != name) continue;
                _NameItem = tableItem;
                _Number = -1;
                return;
            }
        }

        public bool IsNone()
        {
            return _NameItem.Name.Equals( None, StringComparison.OrdinalIgnoreCase );
        }

        public void Deserialize( IUnrealStream stream )
        {
            var index = stream.ReadNameIndex( out _Number );
            _NameItem = stream.Package.Names[index];

            //Debug.Assert( _NameItem != null, "_NameItem cannot be null! " + index );
            //Debug.Assert( _Number >= -1, "Invalid _Number value! " + _Number );
        }

        public void Serialize( IUnrealStream stream )
        {
            stream.WriteIndex( _Index );
            if( stream.Version >= VNameNumbered )
            {
                //Log.Info($"UName.Deserialize: {_Number} {_Text}");
                stream.Write( (uint)_Number + 1 );
            }
        }

        public override string ToString()
        {
            return _Text;
        }

        public static bool operator ==( UName a, object b )
        {
            return Equals( a, b );
        }

        public static bool operator !=( UName a, object b )
        {
            return !Equals( a, b );
        }

        public static bool operator ==( UName a, string b )
        {
            return String.Equals( a, b );
        }

        public static bool operator !=( UName a, string b )
        {
            return !String.Equals( a, b );
        }

        public static implicit operator string( UName a )
        {
            return !Equals( a, null ) ? a._Text : null;
        }

        public static explicit operator int( UName a )
        {
            return a._Index;
        }
    }
}