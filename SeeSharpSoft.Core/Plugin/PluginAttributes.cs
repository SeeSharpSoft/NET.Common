using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeeSharpSoft.Plugin
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PluginNameAttribute : NameAttribute
    {
        public PluginNameAttribute(String name) : base(name) { }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class PluginRequiredAttribute : TypeAttribute
    {
        public bool IsOptional { private set; get; }

        public PluginRequiredAttribute(Type pluginType, bool isOptional) : this(pluginType)
        {
            IsOptional = isOptional;
        }
        public PluginRequiredAttribute(Type pluginType) : base(pluginType) { }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PluginFactoryAttribute : TypeAttribute
    {
        public PluginFactoryAttribute(Type pluginType) : base(pluginType) { }
    }
}