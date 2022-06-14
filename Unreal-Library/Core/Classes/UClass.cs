using System;
using System.Collections.Generic;
using UELib.Flags;

namespace UELib.Core
{
    /// <summary>
    ///     Represents a unreal class.
    /// </summary>
    [UnrealRegisterClass]
    public partial class UClass : UState, IExtract
    {
        /// <summary>
        ///     Index of auto collapsed categories names into the NameTableList.
        ///     UE3
        /// </summary>
        public IList<int> AutoCollapseCategories;

        /// <summary>
        ///     Index of auto expanded categories names into the NameTableList.
        ///     UE3
        /// </summary>
        public IList<int> AutoExpandCategories;

        /// <summary>
        ///     A list of class dependencies that this class depends on. Includes Imports and Exports.
        ///     Deprecated @ PackageVersion:186
        /// </summary>
        public UArray<Dependency> ClassDependencies;

        /// <summary>
        ///     A list of class group.
        /// </summary>
        public IList<int> ClassGroups;

        /// <summary>
        ///     Index of component names into the NameTableList.
        ///     UE3
        /// </summary>
        public IList<int> Components = null;

        /// <summary>
        ///     Index of unsorted categories names into the NameTableList.
        ///     UE3
        /// </summary>
        public IList<int> DontSortCategories;

        public bool ForceScriptOrder;

        /// <summary>
        ///     Index of hidden categories names into the NameTableList.
        /// </summary>
        public IList<int> HideCategories;

        /// <summary>
        ///     Index of (Object/Name?)
        ///     UE3
        /// </summary>
        public IList<int> ImplementedInterfaces;

        public string NativeClassName = string.Empty;

        /// <summary>
        ///     A list of objects imported from a package.
        /// </summary>
        public IList<int> PackageImports;

        public ulong ClassFlags { get; set; }

        public string ClassGuid { get; private set; }
        public UClass Within { get; private set; }
        public UName ConfigName { get; private set; }
        public UName DLLBindName { get; private set; }

        public IList<UState> States { get; protected set; }

        protected override void Deserialize()
        {
            base.Deserialize();

            if (Package.Version <= 61)
            {
                var oldClassRecordSize = _Buffer.ReadInt32();
                Record("oldClassRecordSize", oldClassRecordSize);
            }

            ClassFlags = _Buffer.ReadUInt64();
            Record("ClassFlags", (ClassFlags) ClassFlags);


            if (Package.Version >= 62)
            {
                // TODO: Corrigate Version
                // At least since Bioshock(140) - 547(APB)
                if (Package.Version >= 140 && Package.Version < 547)
                {
                    var unknown = _Buffer.ReadByte();
                    Record("???", unknown);
                }

                // Class Name Extends Super.Name Within _WithinIndex
                //      Config(_ConfigIndex);
                Within = _Buffer.ReadObject() as UClass;
                Record("Within", Within);
                ConfigName = _Buffer.ReadNameReference();
                Record("ConfigName", ConfigName);

                const int vHideCategoriesOldOrder = 539;
                var isHideCategoriesOldOrder = Package.Version <= vHideCategoriesOldOrder;

                // TODO: Corrigate Version
                if (Package.Version >= 100)
                {
                    // TODO: Corrigate Version
                    if (Package.Version >= 220)
                    {
                        // TODO: Corrigate Version
                        if (isHideCategoriesOldOrder && !Package.IsConsoleCooked() && !Package.Build.IsXenonCompressed)
                        {
                            DeserializeHideCategories();
                        }

                        DeserializeComponentsMap();

                        // RoboBlitz(369)
                        // TODO: Corrigate Version
                        if (Package.Version >= 369)
                        {
                            DeserializeInterfaces();
                        }
                    }

                    if (!Package.IsConsoleCooked() && !Package.Build.IsXenonCompressed)
                    {
                        if (Package.Version >= 603)
                        {
                            DontSortCategories = DeserializeGroup("DontSortCategories");
                        }

                        // TODO: Corrigate Version
                        if (Package.Version < 220 || !isHideCategoriesOldOrder)
                        {
                            DeserializeHideCategories();
                        }

                        // TODO: Corrigate Version
                        if (Package.Version >= 185)
                        {
                            // 490:GoW1, 576:CrimeCraft
                            if (!HasClassFlag(Flags.ClassFlags.CollapseCategories)
                                || Package.Version <= vHideCategoriesOldOrder || Package.Version >= 576)
                            {
                                AutoExpandCategories = DeserializeGroup("AutoExpandCategories");
                            }

                            if (Package.Version > 670)
                            {
                                AutoCollapseCategories = DeserializeGroup("AutoCollapseCategories");

                                if (Package.Version >= 749)
                                {
                                    // bForceScriptOrder
                                    ForceScriptOrder = _Buffer.ReadInt32() > 0;
                                    Record("ForceScriptOrder", ForceScriptOrder);


                                    if (Package.Version >= UnrealPackage.VCLASSGROUP)
                                    {
                                        ClassGroups = DeserializeGroup("ClassGroups");
                                        if (Package.Version >= 813)
                                        {
                                            NativeClassName = _Buffer.ReadText();
                                            Record("NativeClassName", NativeClassName);
                                        }
                                    }
                                }
                            }

                            // FIXME: Found first in(V:655), Definitely not in APB and GoW 2
                            // TODO: Corrigate Version
                            if (Package.Version > 575 && Package.Version < 674)
                            {
                                var unk2 = _Buffer.ReadInt32();
                                Record("??Int32", unk2);
                            }
                        }
                    }

                    if (Package.Version >= UnrealPackage.VDLLBIND)
                    {
                        DLLBindName = _Buffer.ReadNameReference();
                        Record("DLLBindName", DLLBindName);
                    }
                }
            }

            // In later UE3 builds, defaultproperties are stored in separated objects named DEFAULT_namehere,
            // TODO: Corrigate Version
            if (Package.Version >= 322)
            {
                Default = _Buffer.ParseObject(ExportTable.Index + 2);
                if (Default.Name != "Default__" + Name)
                {
                    Default = null;
                }

                Record("Default", Default);
            }
            else
            {
                DeserializeProperties();
            }
        }

        private void DeserializeInterfaces()
        {
            // See http://udn.epicgames.com/Three/UnrealScriptInterfaces.html
            var interfacesCount = _Buffer.ReadInt32();
            Record("Implements.Count", interfacesCount);
            if (interfacesCount <= 0)
            {
                return;
            }

            AssertEOS(interfacesCount * 8, "Implemented");
            ImplementedInterfaces = new List<int>(interfacesCount);
            for (var i = 0; i < interfacesCount; ++i)
            {
                var interfaceIndex = _Buffer.ReadInt32();
                Record("Implemented.InterfaceIndex", interfaceIndex);
                var typeIndex = _Buffer.ReadInt32();
                Record("Implemented.TypeIndex", typeIndex);
                ImplementedInterfaces.Add(interfaceIndex);
            }
        }

        private void DeserializeHideCategories()
        {
            HideCategories = DeserializeGroup("HideCategories");
        }

        private void DeserializeComponentsMap()
        {
            var componentsCount = _Buffer.ReadInt32();
            Record("Components.Count", componentsCount);
            if (componentsCount <= 0)
            {
                return;
            }

            // NameIndex/ObjectIndex
            var numBytes = componentsCount * 12;
            AssertEOS(numBytes, "Components");
            _Buffer.Skip(numBytes);
        }

        protected override void FindChildren()
        {
            base.FindChildren();
            States = new List<UState>();
            for (var child = Children; child != null; child = child.NextField)
            {
                if (child.IsClassType("State"))
                {
                    States.Insert(0, (UState) child);
                }
            }
        }

        private IList<int> DeserializeGroup(string groupName = "List", int count = -1)
        {
            if (count == -1)
            {
                count = ReadCount();
            }

            Record(string.Format("{0}.Count", groupName), count);
            if (count > 0)
            {
                var groupList = new List<int>(count);
                for (var i = 0; i < count; ++i)
                {
                    var index = _Buffer.ReadNameIndex();
                    groupList.Add(index);

                    Record(string.Format("{0}({1})", groupName, Package.GetIndexName(index)), index);
                }

                return groupList;
            }

            return null;
        }

        public bool HasClassFlag(ClassFlags flag)
        {
            return (ClassFlags & (uint) flag) != 0;
        }

        public bool HasClassFlag(uint flag)
        {
            return (ClassFlags & flag) != 0;
        }

        public bool IsClassInterface()
        {
            return (Super != null && string.Compare(Super.Name, "Interface", StringComparison.OrdinalIgnoreCase) == 0)
                   || string.Compare(Name, "Interface", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public bool IsClassWithin()
        {
            return Within != null && !string.Equals(Within.Name, "Object", StringComparison.OrdinalIgnoreCase);
        }

        public struct Dependency : IUnrealSerializableClass
        {
            public int Class { get; private set; }

            public void Serialize(IUnrealStream stream)
            {
                // TODO: Implement code
            }

            public void Deserialize(IUnrealStream stream)
            {
                Class = stream.ReadObjectIndex();

                // Deep
                stream.ReadInt32();

                // ScriptTextCRC
                stream.ReadUInt32();
            }
        }
    }

    public interface IExtract
    {
    }
}