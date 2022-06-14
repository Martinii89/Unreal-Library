using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UELib.Types;

namespace UELib.Core
{
    /// <summary>
    ///     [Default]Properties values deserializer.
    /// </summary>
    public sealed class UDefaultProperty : IUnrealDecompilable
    {
        [Flags]
        public enum DeserializeFlags : byte
        {
            None = 0x00,
            WithinStruct = 0x01,
            WithinArray = 0x02,
            Complex = WithinStruct | WithinArray
        }

        private const byte DoNotAppendName = 0x01;
        private const byte ReplaceNameMarker = 0x02;

        private const int V3 = 220;
        private const int VEnumName = 633;
        private const int VBoolSizeToOne = 673;

        private const byte ArrayIndexMask = 0x80;
        private readonly UObject _Container;

        /// <summary>
        ///     Value of the UBoolProperty. If Type equals BoolProperty.
        /// </summary>
        private bool _BoolValue;

        private UStruct _Outer;

        /// <summary>
        ///     Whether this property is part of an static array, and the index into it
        /// </summary>
        public int ArrayIndex = -1;

        public UDefaultProperty(UObject owner, UStruct outer = null)
        {
            _Container = owner;
            _Outer = (outer ?? _Container as UStruct) ?? _Container.Outer as UStruct;
        }

        private IUnrealStream _Buffer => _Container.Buffer;

        internal long _BeginOffset { get; set; }
        private long _ValueOffset { get; set; }
        private long _EndOffset => _ValueOffset + Size;
        private byte _TempFlags { get; set; }

        /// <summary>
        ///     Name of the UProperty.
        /// </summary>
        public UName Name { get; private set; }

        //If type equals Object/component/interface or array of these
        public List<UObject> uObjects { get; } = new List<UObject>();

        /// <summary>
        ///     Name of the UStruct. If type equals StructProperty.
        /// </summary>
        private UName ItemName { get; set; }

        /// <summary>
        ///     Name of the UEnum. If Type equals ByteProperty.
        /// </summary>
        private UName EnumName { get; set; }

        /// <summary>
        ///     See PropertysType enum in UnrealFlags.cs
        /// </summary>
        private PropertyType Type { get; set; }

        /// <summary>
        ///     The stream size of this DefaultProperty.
        /// </summary>
        private int Size { get; set; }

        public string Decompile()
        {
            _TempFlags = 0x00;
            string value;
            _Container.EnsureBuffer();
            try
            {
                value = DeserializeValue();
            }
            catch (Exception e)
            {
                value = "//" + e;
            }
            finally
            {
                _Container.MaybeDisposeBuffer();
            }

            // Array or Inlined object
            if ((_TempFlags & DoNotAppendName) != 0)
            {
                // The tag handles the name etc on its own.
                return value;
            }

            var arrayindex = string.Empty;
            if (ArrayIndex > 0 && Type != PropertyType.BoolProperty)
            {
                arrayindex += "[" + ArrayIndex + "]";
            }

            if (Type == PropertyType.ArrayProperty && value == "none")
            {
                return $"{Name}.Empty";
            }

            return Name + arrayindex + "=" + value;
        }

        private int DeserializeSize(byte sizePack)
        {
            switch (sizePack)
            {
                case 0x00:
                    return 1;

                case 0x10:
                    return 2;

                case 0x20:
                    return 4;

                case 0x30:
                    return 12;

                case 0x40:
                    return 16;

                case 0x50:
                    return _Buffer.ReadByte();

                case 0x60:
                    return _Buffer.ReadInt16();

                case 0x70:
                    return _Buffer.ReadInt32();

                default:
                    throw new NotImplementedException(string.Format("Unknown sizePack {0}", sizePack));
            }
        }

        private int DeserializeArrayIndex()
        {
            int arrayIndex;
#if DEBUG || BINARYMETADATA
            var startPos = _Buffer.Position;
#endif
            var b = _Buffer.ReadByte();
            if ((b & ArrayIndexMask) == 0)
            {
                arrayIndex = b;
            }
            else if ((b & 0xC0) == ArrayIndexMask)
            {
                arrayIndex = ((b & 0x7F) << 8) + _Buffer.ReadByte();
            }
            else
            {
                arrayIndex = ((b & 0x3F) << 24)
                             + (_Buffer.ReadByte() << 16)
                             + (_Buffer.ReadByte() << 8)
                             + _Buffer.ReadByte();
            }
#if DEBUG || BINARYMETADATA
            _Buffer.LastPosition = startPos;
#endif
            return arrayIndex;
        }

        public bool Deserialize()
        {
            _BeginOffset = _Buffer.Position;

            Name = _Buffer.ReadNameReference();
            _Container.Record("Name", Name);
            if (Name.IsNone())
            {
                return false;
            }

            // Unreal Engine 1 and 2
            if (_Buffer.Version < V3)
            {
                const byte typeMask = 0x0F;
                const byte sizeMask = 0x70;

                // Packed byte
                var info = _Buffer.ReadByte();
                _Container.Record(string.Format("Info(Type={0},SizeMask=0x{1:X2},ArrayIndexMask=0x{2:X2})",
                    (PropertyType) (byte) (info & typeMask),
                    (byte) (info & sizeMask),
                    info & ArrayIndexMask), info);

                Type = (PropertyType) (byte) (info & typeMask);
                if (Type == PropertyType.StructProperty)
                {
                    ItemName = _Buffer.ReadNameReference();
                    _Container.Record("ItemName", ItemName);
                }

                Size = DeserializeSize((byte) (info & sizeMask));
                if (Size >= 0x50)
                {
                    _Container.Record("Size", Size);
                }

                switch (Type)
                {
                    case PropertyType.BoolProperty:
                        _BoolValue = (info & ArrayIndexMask) != 0;
                        break;

                    default:
                        if ((info & ArrayIndexMask) != 0)
                        {
                            ArrayIndex = DeserializeArrayIndex();
                            _Container.Record("ArrayIndex", ArrayIndex);
                        }

                        break;
                }
            }
            // Unreal Engine 3
            else
            {
                var typeName = _Buffer.ReadName();
                _Container.Record("typeName", typeName);
                Type = (PropertyType) Enum.Parse(typeof(PropertyType), typeName);

                Size = _Buffer.ReadInt32();
                _Container.Record("Size", Size);
                ArrayIndex = _Buffer.ReadInt32();
                _Container.Record("ArrayIndex", ArrayIndex);

                switch (Type)
                {
                    case PropertyType.StructProperty:
                        ItemName = _Buffer.ReadNameReference();
                        _Container.Record("ItemName", ItemName);
                        break;

                    case PropertyType.ByteProperty:
                        if (_Buffer.Version >= VEnumName)
                        {
                            EnumName = _Buffer.ReadNameReference();
                            _Container.Record("EnumName", EnumName);
                        }

                        break;

                    case PropertyType.BoolProperty:
                        _BoolValue = _Buffer.Version >= VBoolSizeToOne ? _Buffer.ReadByte() > 0 : _Buffer.ReadInt32() > 0;
                        _Container.Record("_BoolValue", _BoolValue);
                        break;
                }
            }

            _ValueOffset = _Buffer.Position;
            try
            {
                DeserializeValue();
            }
            finally
            {
                // Even if something goes wrong, we can still skip everything and safely deserialize the next property if any!
                _Buffer.Position = _EndOffset;
            }

            return true;
        }

        /// <summary>
        ///     Deserialize the value of this UPropertyTag instance.
        ///     Note:
        ///     Only call after the whole package has been deserialized!
        /// </summary>
        /// <returns>The deserialized value if any.</returns>
        private string DeserializeValue(DeserializeFlags deserializeFlags = DeserializeFlags.None)
        {
            if (_Buffer == null)
            {
                return "_Buffer is not initialized!";
            }

            _Buffer.Seek(_ValueOffset, SeekOrigin.Begin);
            try
            {
                return DeserializeDefaultPropertyValue(Type, ref deserializeFlags);
            }
            catch (DeserializationException e)
            {
                return e.ToString();
            }
        }

        /// <summary>
        ///     Deserialize a default property value of a specified type.
        /// </summary>
        /// <param name="type">Kind of type to try deserialize.</param>
        /// <returns>The deserialized value if any.</returns>
        private string DeserializeDefaultPropertyValue(PropertyType type, ref DeserializeFlags deserializeFlags)
        {
            if (_Buffer.Position - _ValueOffset > Size)
            {
                throw new DeserializationException("End of defaultproperties stream reached...");
            }

            var orgOuter = _Outer;
            var propertyValue = string.Empty;
            try
            {
                // Deserialize Value
                switch (type)
                {
                    case PropertyType.BoolProperty:
                    {
                        var value = _BoolValue;
                        if (Size == 1 && _Buffer.Version < V3)
                        {
                            value = _Buffer.ReadByte() > 0;
                        }

                        propertyValue = value ? "true" : "false";
                        break;
                    }

                    case PropertyType.StrProperty:
                        propertyValue = "\"" + _Buffer.ReadText().Escape() + "\"";
                        break;

                    case PropertyType.NameProperty:
                        propertyValue = _Buffer.ReadName();
                        break;

                    case PropertyType.IntProperty:
                        propertyValue = _Buffer.ReadInt32().ToString(CultureInfo.InvariantCulture);
                        break;

                    case PropertyType.QWordProperty:
                        propertyValue = _Buffer.ReadInt64().ToString(CultureInfo.InvariantCulture);
                        break;

                    case PropertyType.FloatProperty:
                        propertyValue = _Buffer.ReadFloat().ToUFloat();
                        break;

                    case PropertyType.ByteProperty:
                        if (_Buffer.Version >= V3 && Size == 8)
                        {
                            var enumValue = _Buffer.ReadName();
                            propertyValue = enumValue;
                            if (_Buffer.Version >= VEnumName)
                            {
                                propertyValue = EnumName + "." + propertyValue;
                            }
                        }
                        else
                        {
                            propertyValue = _Buffer.ReadByte().ToString(CultureInfo.InvariantCulture);
                        }

                        break;

                    case PropertyType.InterfaceProperty:
                    case PropertyType.ComponentProperty:
                    case PropertyType.ObjectProperty:
                    {
                        var uObject = _Buffer.ReadObject();
                        uObjects.Add(uObject);
                        _Container.Record("object", uObject);
                        if (uObject != null)
                        {
                            var inline = false;
                            // If true, object is an archetype or subobject.
                            if (uObject.Outer == _Container && (deserializeFlags & DeserializeFlags.WithinStruct) == 0)
                            {
                                // Unknown objects are only deserialized on demand.
                                uObject.BeginDeserializing();
                                if (uObject.Properties != null && uObject.Properties.Count > 0)
                                {
                                    inline = true;
                                    propertyValue = uObject.Decompile() + "\r\n" + UDecompilingState.Tabs;

                                    _TempFlags |= DoNotAppendName;
                                    if ((deserializeFlags & DeserializeFlags.WithinArray) != 0)
                                    {
                                        _TempFlags |= ReplaceNameMarker;
                                        propertyValue += "%ARRAYNAME%=" + uObject.Name;
                                    }
                                    else
                                    {
                                        propertyValue += Name + "=" + uObject.Name;
                                    }
                                }
                            }

                            if (!inline)
                            {
                                // =CLASS'Package.Group(s)+.Name'
                                propertyValue = string.Format("{0}\'{1}\'", uObject.GetClassName(), uObject.GetOuterGroup());
                            }
                        }
                        else
                        {
                            // =none
                            propertyValue = "none";
                        }

                        break;
                    }

                    case PropertyType.ClassProperty:
                    {
                        var uObject = _Buffer.ReadObject();
                        uObjects.Add(uObject);
                        _Container.Record("object", uObject);
                        propertyValue = uObject != null ? "class\'" + uObject.Name + "\'" : "none";
                        break;
                    }

                    case PropertyType.DelegateProperty:
                    {
                        _TempFlags |= DoNotAppendName;
                        var outerIndex = _Buffer.ReadObjectIndex(); // Where the assigned delegate property exists.
                        var delegateValue = _Buffer.ReadName();
                        var delegateName = ((string) Name).Substring(2, Name.Length - 12);
                        propertyValue = delegateName + "=" + delegateValue;
                        break;
                    }

                    case PropertyType.Color:
                    {
                        var b = DeserializeDefaultPropertyValue(PropertyType.ByteProperty, ref deserializeFlags);
                        var g = DeserializeDefaultPropertyValue(PropertyType.ByteProperty, ref deserializeFlags);
                        var r = DeserializeDefaultPropertyValue(PropertyType.ByteProperty, ref deserializeFlags);
                        var a = DeserializeDefaultPropertyValue(PropertyType.ByteProperty, ref deserializeFlags);

                        propertyValue += "R=" + r +
                                         ",G=" + g +
                                         ",B=" + b +
                                         ",A=" + a;
                        break;
                    }

                    case PropertyType.LinearColor:
                    {
                        var b = DeserializeDefaultPropertyValue(PropertyType.FloatProperty, ref deserializeFlags);
                        var g = DeserializeDefaultPropertyValue(PropertyType.FloatProperty, ref deserializeFlags);
                        var r = DeserializeDefaultPropertyValue(PropertyType.FloatProperty, ref deserializeFlags);
                        var a = DeserializeDefaultPropertyValue(PropertyType.FloatProperty, ref deserializeFlags);

                        propertyValue += "R=" + r +
                                         ",G=" + g +
                                         ",B=" + b +
                                         ",A=" + a;
                        break;
                    }

                    case PropertyType.Vector:
                    {
                        var x = DeserializeDefaultPropertyValue(PropertyType.FloatProperty, ref deserializeFlags);
                        var y = DeserializeDefaultPropertyValue(PropertyType.FloatProperty, ref deserializeFlags);
                        var z = DeserializeDefaultPropertyValue(PropertyType.FloatProperty, ref deserializeFlags);

                        propertyValue += "X=" + x +
                                         ",Y=" + y +
                                         ",Z=" + z;
                        break;
                    }

                    case PropertyType.TwoVectors:
                    {
                        var v1 = DeserializeDefaultPropertyValue(PropertyType.Vector, ref deserializeFlags);
                        var v2 = DeserializeDefaultPropertyValue(PropertyType.Vector, ref deserializeFlags);

                        propertyValue += "v1=(" + v1 + "),v2=(" + v2 + ")";
                        break;
                    }

                    case PropertyType.Vector4:
                    {
                        var plane = DeserializeDefaultPropertyValue(PropertyType.Plane, ref deserializeFlags);

                        propertyValue += plane;
                        break;
                    }

                    case PropertyType.Vector2D:
                    {
                        var x = DeserializeDefaultPropertyValue(PropertyType.FloatProperty, ref deserializeFlags);
                        var y = DeserializeDefaultPropertyValue(PropertyType.FloatProperty, ref deserializeFlags);

                        propertyValue += "X=" + x +
                                         ",Y=" + y;
                        break;
                    }

                    case PropertyType.Rotator:
                    {
                        var pitch = DeserializeDefaultPropertyValue(PropertyType.IntProperty, ref deserializeFlags);
                        var yaw = DeserializeDefaultPropertyValue(PropertyType.IntProperty, ref deserializeFlags);
                        var roll = DeserializeDefaultPropertyValue(PropertyType.IntProperty, ref deserializeFlags);

                        propertyValue += "Pitch=" + pitch +
                                         ",Yaw=" + yaw +
                                         ",Roll=" + roll;
                        break;
                    }

                    case PropertyType.Guid:
                    {
                        var a = DeserializeDefaultPropertyValue(PropertyType.IntProperty, ref deserializeFlags);
                        var b = DeserializeDefaultPropertyValue(PropertyType.IntProperty, ref deserializeFlags);
                        var c = DeserializeDefaultPropertyValue(PropertyType.IntProperty, ref deserializeFlags);
                        var d = DeserializeDefaultPropertyValue(PropertyType.IntProperty, ref deserializeFlags);

                        propertyValue += "A=" + a +
                                         ",B=" + b +
                                         ",C=" + c +
                                         ",D=" + d;
                        break;
                    }

                    case PropertyType.Sphere:
                    case PropertyType.Plane:
                    {
                        var v = DeserializeDefaultPropertyValue(PropertyType.Vector, ref deserializeFlags);
                        var w = DeserializeDefaultPropertyValue(PropertyType.FloatProperty, ref deserializeFlags);

                        propertyValue += v + ",W=" + w;
                        break;
                    }

                    case PropertyType.Scale:
                    {
                        var scale = DeserializeDefaultPropertyValue(PropertyType.Vector, ref deserializeFlags);
                        var sheerRate = DeserializeDefaultPropertyValue(PropertyType.FloatProperty, ref deserializeFlags);
                        var sheerAxis = DeserializeDefaultPropertyValue(PropertyType.ByteProperty, ref deserializeFlags);

                        propertyValue += "Scale=(" + scale + ")" +
                                         ",SheerRate=" + sheerRate +
                                         ",SheerAxis=" + sheerAxis;
                        break;
                    }

                    case PropertyType.Box:
                    {
                        var min = DeserializeDefaultPropertyValue(PropertyType.Vector, ref deserializeFlags);
                        var max = DeserializeDefaultPropertyValue(PropertyType.Vector, ref deserializeFlags);
                        var isValid = DeserializeDefaultPropertyValue(PropertyType.ByteProperty, ref deserializeFlags);

                        propertyValue += "Min=(" + min + ")" +
                                         ",Max=(" + max + ")" +
                                         ",IsValid=" + isValid;
                        break;
                    }

                    /*case PropertyType.InterpCurve:
                    {
                        // HACK:
                        UPropertyTag tag = new UPropertyTag( _Owner );
                        tag.Serialize();
                        buffer.Seek( tag.ValueOffset, System.IO.SeekOrigin.Begin );

                        int curvescount = buffer.ReadIndex();
                        if( curvescount <= 0 )
                        {
                            break;
                        }
                        propertyvalue += tag.Name + "=(";
                        for( int i = 0; i < curvescount; ++ i )
                        {
                            propertyvalue += "(" + SerializeDefaultPropertyValue( PropertyType.InterpCurvePoint, buffer, ref serializeFlags ) + ")";
                            if( i != curvescount - 1 )
                            {
                                propertyvalue += ",";
                            }
                        }
                        propertyvalue += ")";
                        break;
                    }*/

                    /*case PropertyType.InterpCurvePoint:
                    {
                        string InVal = SerializeDefaultPropertyValue( PropertyType.Float, buffer, ref serializeFlags );
                        string OutVal = SerializeDefaultPropertyValue( PropertyType.Float, buffer, ref serializeFlags );

                        propertyvalue += "InVal=" + InVal +
                            ",OutVal=" + OutVal;
                        break;
                    }*/

                    case PropertyType.Quat:
                    {
                        propertyValue += DeserializeDefaultPropertyValue(PropertyType.Plane, ref deserializeFlags);
                        break;
                    }

                    case PropertyType.Matrix:
                    {
                        var xPlane = DeserializeDefaultPropertyValue(PropertyType.Plane, ref deserializeFlags);
                        var yPlane = DeserializeDefaultPropertyValue(PropertyType.Plane, ref deserializeFlags);
                        var zPlane = DeserializeDefaultPropertyValue(PropertyType.Plane, ref deserializeFlags);
                        var wPlane = DeserializeDefaultPropertyValue(PropertyType.Plane, ref deserializeFlags);
                        propertyValue += "XPlane=(" + xPlane + ")" +
                                         ",YPlane=(" + yPlane + ")" +
                                         ",ZPlane=(" + zPlane + ")" +
                                         ",WPlane=(" + wPlane + ")";
                        break;
                    }

                    case PropertyType.IntPoint:
                    {
                        var x = DeserializeDefaultPropertyValue(PropertyType.IntProperty, ref deserializeFlags);
                        var y = DeserializeDefaultPropertyValue(PropertyType.IntProperty, ref deserializeFlags);

                        propertyValue += "X=" + x + ",Y=" + y;
                        break;
                    }

                    case PropertyType.PointerProperty:
                    case PropertyType.StructProperty:
                    {
                        deserializeFlags |= DeserializeFlags.WithinStruct;
                        var isHardCoded = false;
                        var hardcodedStructs = (PropertyType[]) Enum.GetValues(typeof(PropertyType));
                        for (var i = (byte) PropertyType.StructOffset; i < hardcodedStructs.Length; ++i)
                        {
                            var structType = Enum.GetName(typeof(PropertyType), (byte) hardcodedStructs[i]);
                            if (string.Compare(ItemName, structType, StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                continue;
                            }

                            isHardCoded = true;
                            propertyValue += DeserializeDefaultPropertyValue(hardcodedStructs[i], ref deserializeFlags);
                            break;
                        }

                        if (!isHardCoded)
                        {
                            // We have to modify the outer so that dynamic arrays within this struct
                            // will be able to find its variables to determine the array type.
                            FindProperty(out _Outer);
                            while (true)
                            {
                                var tag = new UDefaultProperty(_Container, _Outer);
                                if (tag.Deserialize())
                                {
                                    propertyValue += tag.Name +
                                                     (tag.ArrayIndex > 0 && tag.Type != PropertyType.BoolProperty
                                                         ? "[" + tag.ArrayIndex + "]"
                                                         : string.Empty) +
                                                     "=" + tag.DeserializeValue(deserializeFlags) + ",";
                                }
                                else
                                {
                                    if (propertyValue.EndsWith(","))
                                    {
                                        propertyValue = propertyValue.Remove(propertyValue.Length - 1, 1);
                                    }

                                    break;
                                }
                            }
                        }

                        propertyValue = propertyValue.Length != 0 ? "(" + propertyValue + ")" : "none";
                        break;
                    }

                    case PropertyType.ArrayProperty:
                    {
                        var arraySize = _Buffer.ReadIndex();
                        _Container.Record("arraySize", arraySize);
                        if (arraySize == 0)
                        {
                            propertyValue = "none";
                            break;
                        }

                        // Find the property within the outer/owner or its inheritances.
                        // If found it has to modify the outer so structs within this array can find their array variables.
                        // Additionally we need to know the property to determine the array's type.
                        var arrayType = PropertyType.None;
                        var property = FindProperty(out _Outer) as UArrayProperty;
                        if (property != null && property.InnerProperty != null)
                        {
                            arrayType = property.InnerProperty.Type;
                        }
                        // If we did not find a reference to the associated property(because of imports)
                        // then try to determine the array's type by scanning the definined array types.
                        else if (UnrealConfig.VariableTypes != null && UnrealConfig.VariableTypes.ContainsKey(Name))
                        {
                            var varTuple = UnrealConfig.VariableTypes[Name];
                            if (varTuple != null)
                            {
                                arrayType = varTuple.Item2;
                            }
                        }

                        if (arrayType == PropertyType.None)
                        {
                            propertyValue = "/* Array type was not detected. */";
                            break;
                        }

                        deserializeFlags |= DeserializeFlags.WithinArray;
                        if ((deserializeFlags & DeserializeFlags.WithinStruct) != 0)
                        {
                            // Hardcoded fix for InterpCurve and InterpCurvePoint.
                            if (string.Compare(Name, "Points", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                arrayType = PropertyType.StructProperty;
                            }

                            var sb = new StringBuilder();
                            sb.Append("(");
                            for (var i = 0; i < arraySize; ++i)
                            {
                                sb.Append(DeserializeDefaultPropertyValue(arrayType, ref deserializeFlags)
                                          + (i != arraySize - 1 ? "," : string.Empty));
                                //propertyValue += DeserializeDefaultPropertyValue( arrayType, ref deserializeFlags )
                                //    + (i != arraySize - 1 ? "," : String.Empty);
                            }

                            sb.Append(")");
                            propertyValue = sb.ToString();
                            //propertyValue = "(" + propertyValue + ")";
                        }
                        else
                        {
                            var sb = new StringBuilder();
                            for (var i = 0; i < arraySize; ++i)
                            {
                                var elementValue = DeserializeDefaultPropertyValue(arrayType, ref deserializeFlags);
                                if ((_TempFlags & ReplaceNameMarker) != 0)
                                {
                                    sb.Append(elementValue.Replace("%ARRAYNAME%", Name + "(" + i + ")"));

                                    //propertyValue += elementValue.Replace( "%ARRAYNAME%", Name + "(" + i + ")" );
                                    _TempFlags = 0x00;
                                }
                                else
                                {
                                    sb.Append($"{Name}({i})={elementValue}");
                                    //propertyValue += Name + "(" + i + ")=" + elementValue;
                                }

                                if (i != arraySize - 1)
                                {
                                    sb.Append($"\r\n{UDecompilingState.Tabs}");
                                    //propertyValue += "\r\n" + UDecompilingState.Tabs;
                                }
                            }

                            propertyValue += sb.ToString();
                        }

                        _TempFlags |= DoNotAppendName;
                        break;
                    }

                    default:
                        propertyValue = "/* Unknown default property type! */";
                        break;
                }
            }
            catch (Exception e)
            {
                return propertyValue + "\r\n/* Exception thrown while deserializing " + Name + "\r\n" + e + " */";
            }
            finally
            {
                _Outer = orgOuter;
            }

            return propertyValue;
        }

        private UProperty FindProperty(out UStruct outer)
        {
            //If the object is from the import table. Try to find the export object in the package it originates from.
            UStruct GetRealObject(UObject _object)
            {
                if (_object.ExportTable == null)
                {
                    var realClass = UnrealLoader.FindClassInCache(_object.Name);
                    //var realClass = UnrealLoader.FindClassInPackage(_object.Outer.Name, _object.Name);
                    return realClass;
                }

                return _object as UStruct;
            }

            UProperty property = null;

            outer = _Outer ?? _Container.Class as UStruct;
            //outer = GetRealObject(outer);
            for (var structField = outer; structField != null; structField = structField.Super as UStruct)
            {
                var temp = structField;
                structField = GetRealObject(structField);
                if (structField == null)
                {
                    structField = temp;
                }

                if (structField.Variables == null || !structField.Variables.Any())
                {
                    continue;
                }

                property = structField.Variables.Find(i => i.Name == Name);
                if (property == null)
                {
                    continue;
                }

                switch (property.Type)
                {
                    case PropertyType.StructProperty:
                        outer = ((UStructProperty) property).StructObject;
                        break;

                    case PropertyType.ArrayProperty:
                        var arrayField = property as UArrayProperty;
                        Debug.Assert(arrayField != null, "arrayField != null");
                        var arrayInnerField = arrayField.InnerProperty;
                        if (arrayInnerField.Type == PropertyType.StructProperty)
                        {
                            _Outer = ((UStructProperty) arrayInnerField).StructObject;
                        }

                        break;

                    default:
                        outer = structField;
                        break;
                }

                break;
            }

            return property;
        }
    }

    [ComVisible(false)]
    public sealed class DefaultPropertiesCollection : List<UDefaultProperty>
    {
        public UDefaultProperty Find(string name)
        {
            return Find(prop => prop.Name == name);
        }

        public bool Contains(string name)
        {
            return Find(name) != null;
        }
    }
}