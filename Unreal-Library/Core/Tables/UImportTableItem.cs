using System.Diagnostics.Contracts;
using UELib.Logging;

namespace UELib
{
    /// <summary>
    ///     Represents a unreal import table with deserialized data from a unreal package header.
    /// </summary>
    public sealed class UImportTableItem : UObjectTableItem, IUnrealSerializableClass
    {
        public UName _ClassName;
        public UName PackageName;

        [Pure] public override string ClassName => _ClassName;

        public void Serialize(IUnrealStream stream)
        {
            Log.Info($"Writing import {ObjectName} at {stream.Position}");
            stream.Write(PackageName);
            stream.Write(_ClassName);
            stream.Write(OuterTable != null ? (int) OuterTable.Object : 0); // Always an ordinary integer
            stream.Write(ObjectName);
        }

        public void Deserialize(IUnrealStream stream)
        {
            Log.Debug($"Reading import {Index} at {stream.Position}");
            PackageName = stream.ReadNameReference();
            _ClassName = stream.ReadNameReference();
            ClassIndex = (int) _ClassName;
            OuterIndex = stream.ReadInt32(); // ObjectIndex, though always written as 32bits regardless of build.
            ObjectName = stream.ReadNameReference();
        }

        public override string ToString()
        {
            return ObjectName + "(" + -(Index + 1) + ")";
        }

        /*
        private UObjectTableItem FindClassInPackage(UnrealPackage package, string className)
        {
            var exportClassMatches = package.Exports.Where(ex => ex.ObjectName == className).ToList();
            if (exportClassMatches.Count > 0)
            {
                return exportClassMatches.First();
            }

            var importClassMatches = package.Imports.Where(im => im.ClassName == "Class" && im.ObjectName == className).ToList();
            if (importClassMatches.Count > 0)
            {
                return importClassMatches.First(); ;
            }

            return null;
        }
        
        private UObject FindExportObjectInPackage(UnrealPackage package, string className, string objectName)
        {
            var exportMatch = package.Exports.Where(ex => ex.ClassName == className && ex.ObjectName == objectName).ToList();
            return exportMatch.Count > 0 ? exportMatch.First().Object : null;
        }

        private UObjectTableItem RootOuter()
        {
            var root = OuterTable;
            while (root.OuterTable != null)
            {
                root = root.OuterTable;
            }
            return root;
        }

        private string OuterGroup()
        {
            var name = OuterName;
            var current = OuterTable;
            while (current.OuterTable != null)
            {
                current = current.OuterTable;
                name = $"{current.ObjectName}.{name}";
            }

            return name;

        }

        public void LoadImportClass()
        {
            return;
            // TODO: fix this at a later point. focus on mesh data now..
            if (Owner.PackageName == PackageName)
            {
                //Native object probably?
                var ownerClassTableItem = FindClassInPackage(Owner, ClassName);
                if (ownerClassTableItem != null)
                {
                    ClassTable = ownerClassTableItem;
                    return;
                }
                Log.Error($"Failed finding the classTableITem for class {ClassName}");

                return;
            }

            Log.Info($"importing object (({ClassName}){OuterGroup()}.{ObjectName}) class from {RootOuter()}");
            var outerPackage = UnrealLoader.LoadCachedPackage($"{Path.GetDirectoryName(Owner.FullPackageName)}\\{RootOuter().ObjectName}.upk");
            if (outerPackage != null)
            {
                var obj = FindExportObjectInPackage(outerPackage, ClassName, ObjectName);
                if (obj != null)
                {
                    Object = obj;
                }
            }

            var srcPackage = UnrealLoader.LoadCachedPackage($"{Path.GetDirectoryName(Owner.FullPackageName)}\\{PackageName}.upk");
            if (srcPackage == null)
            {
                Log.Info($"Failed to find import package {OuterName} for import named {ToString()} in package {Owner.PackageName}{this} ");
                return;
            }

            var classTableItem = FindClassInPackage(srcPackage, ClassName);
            if (classTableItem != null)
            {
                ClassTable = classTableItem;
                return;
            }




            Log.Error($"Failed finding the classTableItem for class {ClassName}");
            return;

        }
        */
    }
}