using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeeSharpSoft.Plugin
{
    public static class PluginExtensions
    {
        public static bool IsPlugin(this Type pluginType)
        {
            return pluginType.GetInterface("SeeSharpSoft.Plugin.IPlugin", true) != null;
        }
        //public static String GetPluginVersion(this Type pluginType)
        //{
        //    object[] attributes = pluginType.GetCustomAttributes(typeof(PluginVersionAttribute), true);
        //    if (attributes.Length == 0) throw new PluginException("No version defined for plugin '" + pluginType + "'");
        //    return (attributes[0] as PluginVersionAttribute).Version;
        //}
        public static IEnumerable<PluginRequiredAttribute> GetPluginRequirements(this Type pluginType)
        {
            List<PluginRequiredAttribute> result = new List<PluginRequiredAttribute>();
            object[] attributes = pluginType.GetCustomAttributes(typeof(PluginRequiredAttribute), true);
            foreach (PluginRequiredAttribute dependency in attributes)
            {
                result.Add(dependency);
            }
            return result;
        }
        public static String GetPluginName(this Type pluginType)
        {
            object[] attributes = pluginType.GetCustomAttributes(typeof(PluginNameAttribute), true);
            if (attributes.Length == 0) throw new PluginException("No name defined for plugin '" + pluginType + "'");
            return (attributes[0] as PluginNameAttribute).Name;
        }
        public static String GetName(this IPlugin plugin)
        {
            return plugin.GetType().GetPluginName();
        }
        //public static String GetVersion(this IPlugin plugin)
        //{
        //    return plugin.GetType().GetPluginVersion();
        //}
        public static bool PluginRequires(this Type pluginType, Type targetPluginType)
        {
            if (!pluginType.IsPlugin()) return false;
            return pluginType.GetPluginRequirements().FirstOrDefault(elem => elem.Type == targetPluginType) != null;
        }
        public static bool Requires(this IPlugin plugin, Type targetPluginType)
        {
            return plugin.GetType().PluginRequires(targetPluginType);
        }
        public static Type GetPluginFactoryType(this Type pluginType)
        {
            object[] attributes = pluginType.GetCustomAttributes(typeof(PluginFactoryAttribute), true);
            if (attributes.Length == 0) return typeof(PluginFactory);
            return (attributes[0] as PluginFactoryAttribute).Type;
        }
    }
}