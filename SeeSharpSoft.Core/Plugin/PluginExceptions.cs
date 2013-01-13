using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeeSharpSoft.Plugin
{
    [Serializable]
    public class PluginException : Exception
    {
        public PluginException(String message)
            : base(message)
        {
        }
    }
}