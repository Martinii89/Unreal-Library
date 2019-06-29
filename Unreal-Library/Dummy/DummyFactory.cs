using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

            foreach (Type type in currAssembly.GetTypes())
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
                return null;

            return (MinimalBase)Activator.CreateInstance(type, parameters);
        }
    }
}