using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SeeSharpSoft.Plugin
{
    public class PluginManager : IDisposable
    {
        public PluginManager()
        {
            PluginGraph = CreatePluginGraph();
            PluginFactories = CreatePluginFactories();
        }

        internal PluginGraph PluginGraph { private set; get; }
        private ICollection<PluginFactory> PluginFactories { set; get; }
        [DefaultValue(false)]
        public bool AcceptUnregisteredPluginFactory { set; get; }

        public IEnumerable<Type> RegisteredPlugins
        {
            get
            {
                return PluginGraph.Values;
            }
        }

        protected virtual PluginGraph CreatePluginGraph()
        {
            return new PluginGraph(this);
        }

        protected virtual ICollection<PluginFactory> CreatePluginFactories()
        {
            return new List<PluginFactory> { new PluginFactory() };
        }

        public virtual IPlugin CreatePluginInstance(Type pluginType)
        {
            PluginFactory factory = GetPluginFactory(pluginType);
            if (factory == null) return null;
            return factory.CreatePluginInstance(pluginType);
        }

        public virtual PluginContext CreatePluginContext(IPlugin plugin)
        {
            PluginFactory factory = GetPluginFactory(plugin.GetType());
            if (factory == null) return null;
            return factory.CreatePluginContext(this, plugin);
        }

        protected virtual PluginFactory GetPluginFactory(Type pluginType)
        {
            PluginFactory factory = PluginFactories.FirstOrDefault(elem => elem.GetType() == pluginType.GetPluginFactoryType());

            if (factory == null)
            {
                factory = CreateMissingPluginFactory(pluginType);
                if (factory != null) PluginFactories.Add(factory);
            }

            return factory;
        }

        protected PluginFactory CreateMissingPluginFactory(Type pluginType)
        {
            if (!AcceptUnregisteredPluginFactory) return null;
            return Activator.CreateInstance(pluginType.GetPluginFactoryType()) as PluginFactory;
        }

        public void RegisterPlugin(Type pluginType)
        {
            PluginGraph.AddNode(pluginType);
        }

        public bool ActivatePlugin(Type pluginType)
        {
            return ActivatePlugin(pluginType, false);
        }

        public virtual bool ActivatePlugin(Type pluginType, bool activateDependencies)
        {
            return PluginGraph.GetNode(pluginType).Activate(activateDependencies);
        }

        public bool DeactivatePlugin(Type pluginType)
        {
            return DeactivatePlugin(pluginType, false);
        }

        public virtual bool DeactivatePlugin(Type pluginType, bool deactivateDependencies)
        {
            return PluginGraph.GetNode(pluginType).Deactivate(deactivateDependencies);
        }

        public IPlugin GetPlugin(Type pluginType)
        {
            return PluginGraph.GetNode(pluginType).PluginInstance;
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (Type pluginType in RegisteredPlugins)
            {
                DeactivatePlugin(pluginType, true);
            }
        }

        #endregion
    }
}