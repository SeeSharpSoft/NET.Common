using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SeeSharpSoft.Plugin
{
    public static class PluginHelper
    {
        private static IDictionary<String, Assembly> _registeredAssemblies = new Dictionary<String, Assembly>();

        private static IDictionary<String, Assembly> RegisteredAssemblies
        {
            get { return PluginHelper._registeredAssemblies; }
            set { PluginHelper._registeredAssemblies = value; }
        }

        public static void RegisterPluginAssembly(String filename)
        {
            if (RegisteredAssemblies.ContainsKey(filename)) return;
            Assembly assembly = Assembly.LoadFile(filename);
            RegisteredAssemblies.Add(filename, assembly);
        }

        public static void UnregisterAssembly(String filename)
        {
            Assembly assembly;
            if (!RegisteredAssemblies.TryGetValue(filename, out assembly)) return;
            RegisteredAssemblies.Remove(filename);
        }

        public static IEnumerable<Type> GetRegisteredPlugins()
        {
            List<Type> result = new List<Type>();
            foreach (Assembly assembly in RegisteredAssemblies.Values)
            {
                foreach (Type type in assembly.GetExportedTypes())
                {
                    if (type.IsPlugin()) result.Add(type);
                }
            }
            return result;
        }
    }
}