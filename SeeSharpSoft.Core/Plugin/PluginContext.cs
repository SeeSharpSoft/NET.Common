using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeeSharpSoft.Plugin
{
    public class PluginContext
    {
        public PluginContext(PluginManager pluginManager)
        {
            PluginManager = pluginManager;
        }

        public event EventHandler<PluginNotificationEventArgs> PluginNotify;

        protected PluginManager PluginManager { private set; get; }

        public T GetPlugin<T>() where T : IPlugin
        {
            object result = GetPlugin(typeof(T));
            if (result == null) return default(T);
            return (T)result;
        }

        public virtual object GetPlugin(Type pluginType)
        {
            return PluginManager.GetPlugin(pluginType);
        }

        internal void RaisePluginNotify(IPlugin plugin, PluginNotification notification)
        {
            if (PluginNotify != null)
            {
                PluginNotify.Invoke(this, new PluginNotificationEventArgs(plugin, notification));
            }
        }
    }
}