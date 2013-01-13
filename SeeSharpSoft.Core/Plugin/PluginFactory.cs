using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeeSharpSoft.Plugin
{
    /// <summary>
    /// Factory to create and define a plugin and its context.
    /// </summary>
    public class PluginFactory
    {
        public PluginFactory()
        {
            Contexts = new Dictionary<PluginManager, PluginContext>();
        }

        private Dictionary<PluginManager, PluginContext> _contexts;
        protected Dictionary<PluginManager, PluginContext> Contexts
        {
            get { return _contexts; }
            set { _contexts = value; }
        }

        /// <summary>
        /// Creates a plugincontext.
        /// </summary>
        /// <param name="pluginManager">Manager plugin is started from.</param>
        /// <param name="plugin">Plugin to create context for.</param>
        /// <returns>A PluginContext for given plugin.</returns>
        public virtual PluginContext CreatePluginContext(PluginManager pluginManager, IPlugin plugin)
        {
            if (!_contexts.ContainsKey(pluginManager)) _contexts.Add(pluginManager, new PluginContext(pluginManager));
            return _contexts[pluginManager];
        }

        /// <summary>
        /// Creates a new instance of given plugin type.
        /// </summary>
        /// <param name="pluginType">Type of plugin to create.</param>
        /// <returns>A new instance of a plugin.</returns>
        public virtual IPlugin CreatePluginInstance(Type pluginType)
        {
            return Activator.CreateInstance(pluginType) as IPlugin;
        }
    }
}