using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeeSharpSoft.Plugin
{
    public class PluginEventArgs : EventArgs
    {
        public IPlugin Plugin { private set; get; }

        public PluginEventArgs(IPlugin plugin)
            : base()
        {
            Plugin = plugin;
        }
    }

    public class PluginNotificationEventArgs : PluginEventArgs
    {
        public PluginNotification Notification { private set; get; }

        public PluginNotificationEventArgs(IPlugin plugin, PluginNotification notification)
            : base(plugin)
        {
            Notification = notification;
        }
    }
}