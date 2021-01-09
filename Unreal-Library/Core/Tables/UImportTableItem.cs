using System;
using System.Diagnostics.Contracts;
using UELib.Logging;

namespace UELib
{
    /// <summary>
    /// Represents a unreal import table with deserialized data from a unreal package header.
    /// </summary>
    public sealed class UImportTableItem : UObjectTableItem, IUnrealSerializableClass
    {
        #region Serialized Members
        public UName PackageName;
        public UName _ClassName;

        [Pure]public override string ClassName{ get{ return _ClassName; } }
        #endregion

        public void Serialize( IUnrealStream stream )
        {
            Log.Info( $"Writing import {ObjectName} at {stream.Position}");
            stream.Write( PackageName );
            stream.Write( _ClassName );
            stream.Write( OuterTable != null ? (int)OuterTable.Object : 0 ); // Always an ordinary integer
            stream.Write( ObjectName );
        }

        public void Deserialize( IUnrealStream stream )
        {
            Log.Debug($"Reading import {Index} at {stream.Position}");
            PackageName         = stream.ReadNameReference();
            _ClassName          = stream.ReadNameReference();
            ClassIndex         = (int)_ClassName;
            OuterIndex          = stream.ReadInt32(); // ObjectIndex, though always written as 32bits regardless of build.
            ObjectName          = stream.ReadNameReference();
        }

        #region Methods
        public override string ToString()
        {
            return ObjectName + "(" + -(Index + 1) + ")";
        }
        #endregion
    }
}