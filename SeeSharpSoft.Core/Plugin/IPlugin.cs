using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SeeSharpSoft.Plugin
{
    public interface IPlugin : IDisposable
    {
        PluginSettings PluginSettings { get; }

        void ActivatePlugin(PluginContext manager);
        void DeactivatePlugin(PluginContext manager);

        void OnPluginNotify(object sender, PluginNotification notification);
    }
}