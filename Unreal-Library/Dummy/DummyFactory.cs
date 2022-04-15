using System;
using System.Collections.Generic;
using System.Reflection;

namespace UELib.Dummy
{
    internal class DummyFactory
    {
        public static DummyFactory Instance { get; } = new DummyFactory();
        private static Dictionary<string, Type> _registeredTypes;

        private DummyFactory()
        {
            BuildTypeMap();
        }

        static DummyFactory()
        {
        }

        private static void BuildTypeMap()
        {
            var currAssembly = Assembly.GetExecutingAssembly();
            var baseType = typeof(MinimalBase);
            _registeredTypes = new Dictionary<string, Type>();

            foreach (var type in currAssembly.GetTypes())
            {
                if (!type.IsClass || type.IsAbstract ||
                    !type.IsSubclassOf(baseType))
                {
                    continue;
                }

                _registeredTypes.Add(type.Name, type);
            }
        }

        public MinimalBase Create(string id, params object[] parameters)
        {
            if (!_registeredTypes.TryGetValue(id, out var type))
            {
                return new DefaultDummy(parameters[0] as UExportTableItem, parameters[1] as UnrealPackage);
            }

            return (MinimalBase) Activator.CreateInstance(type, parameters);
        }
    }
}