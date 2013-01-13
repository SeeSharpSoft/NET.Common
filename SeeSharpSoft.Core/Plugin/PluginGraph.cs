using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeeSharpSoft.Collections;

namespace SeeSharpSoft.Plugin
{
    public class PluginGraph : Graph<Type>
    {
        public PluginGraph(PluginManager pluginManager)
        {
            PluginManager = pluginManager;
        }

        public PluginManager PluginManager { private set; get; }

        public override void AddNode(GraphNode<Type> node)
        {
            if (this.Contains(node.Value)) throw new PluginException("Plugin '" + node.Value.GetPluginName() + "' already added!");

            base.AddNode(node);

            foreach (PluginRequiredAttribute dependency in node.Value.GetPluginRequirements().Where(elem => !elem.IsOptional))
            {
                ((PluginNode)node).MissingDependencies.Add(dependency);
            }

            UpdateMissingDependencies();

            //NotifyPlugins(node.Value, PluginNotification.Registered);
        }

        private void UpdateMissingDependencies()
        {
            PluginNode dependencyNode;
            
            foreach (PluginNode pluginNode in Nodes)
            {
                List<PluginRequiredAttribute> foundDependencies = new List<PluginRequiredAttribute>();

                foreach (PluginRequiredAttribute dependency in pluginNode.MissingDependencies)
                {
                    dependencyNode = Nodes.FirstOrDefault(elem =>
                            elem.Value == dependency.Type) as PluginNode;

                    if (dependencyNode != null)
                    {
                        dependencyNode.AddSuccessor(pluginNode);
                        //pluginNode.AddSuccessor(dependencyNode);
                        foundDependencies.Add(dependency);
                    }
                }

                foundDependencies.ForEach(elem => pluginNode.MissingDependencies.Remove(elem));
            }
        }

        public void NotifyPlugins(IPlugin plugin, PluginNotification notification)
        {
            foreach (PluginNode node in Nodes)
            {
                node.NotifyPlugin(plugin, notification);
            }
        }

        public new PluginNode GetNode(Type value)
        {
            return base.GetNode(value) as PluginNode;
        }

        public override GraphNode<Type> CreateNode()
        {
            return new PluginNode();
        }
    }

    public class PluginNode : GraphNode<Type>
    {
        public PluginNode()
            : base()
        {
            MissingDependencies = new List<PluginRequiredAttribute>();
        }

        public new PluginGraph Graph
        {
            get { return base.Graph as PluginGraph; }
        }

        public List<PluginRequiredAttribute> MissingDependencies { private set; get; }
        public bool IsActive { private set; get; }
        public IPlugin PluginInstance { private set; get; }
        public PluginContext PluginContext { private set; get; }

        public bool Activate()
        {
            return Activate(false);
        }

        public bool Activate(bool activateDependencies)
        {
            if (MissingDependencies.Count > 0) return false;

            if (IsActive) return true;

            //check whether all predececcors are active -> actual plugin depends on
            if (activateDependencies)
            {
                //try to activate depend plugins
                if (Predecessors.Count(elem => !((PluginNode)elem).Activate(true)) > 0) return false;
            }
            else
            {
                //just count inactive plugins
                if (Predecessors.Count(elem => !((PluginNode)elem).IsActive) > 0) return false;
            }

            PluginInstance = Graph.PluginManager.CreatePluginInstance(Value);

            if (PluginInstance == null) return false;

            Graph.NotifyPlugins(PluginInstance, PluginNotification.Created);

            PluginContext = Graph.PluginManager.CreatePluginContext(PluginInstance);

            PluginInstance.ActivatePlugin(PluginContext);

            IsActive = true;

            Graph.NotifyPlugins(PluginInstance, PluginNotification.Activated);

            return true;
        }

        public bool Deactivate()
        {
            return Deactivate(false);
        }

        public bool Deactivate(bool deactivateDependencies)
        {
            if (!IsActive) return true;

            //check all successors -> these are the ones which depends on actual plugin
            if (deactivateDependencies)
            {
                //try to activate depend plugins
                if (Predecessors.Count(elem => !((PluginNode)elem).Deactivate(true)) > 0) return false;
            }
            else
            {
                //just count active plugins
                if (Predecessors.Count(elem => ((PluginNode)elem).IsActive) > 0) return false;
            }

            PluginInstance.DeactivatePlugin(PluginContext);

            IsActive = false;

            Graph.NotifyPlugins(PluginInstance, PluginNotification.Deactivated);

            PluginInstance.Dispose();

            Graph.NotifyPlugins(PluginInstance, PluginNotification.Disposed);

            PluginInstance = null;
            PluginContext = null;

            return true;
        }

        public void NotifyPlugin(IPlugin plugin, PluginNotification notification)
        {
            if(!IsActive) return;

            PluginContext.RaisePluginNotify(plugin, notification);
        }
    }
}