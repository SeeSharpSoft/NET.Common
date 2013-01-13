using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SeeSharpSoft.Collections
{
    public class Node<T>
    {
        private T _value;
        private ICollection<Node<T>> _successors = null;

        public Node() : this(default(T)) { }
        public Node(T value) : this(value, null) { }
        public Node(T value, ICollection<Node<T>> successors)
        {
            this._value = value;
            if (successors == null)
                this._successors = CreateSuccessorCollection();
            else
                this._successors = successors;
        }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        protected ICollection<Node<T>> SuccessorCollection
        {
            get
            {
                return _successors;
            }
            set
            {
                _successors = value;
            }
        }

        protected virtual ICollection<Node<T>> CreateSuccessorCollection()
        {
            return new List<Node<T>>();
        }

        public IEnumerable<Node<T>> Successors
        {
            get
            {
                return SuccessorCollection.AsEnumerable();
            }
        }
    }

    public class GraphNode<T> : Node<T>
    {
        public Graph<T> Graph { internal set; get; }
        private IDictionary<GraphNode<T>, double> costs;

        public GraphNode() : this(default(T)) { }
        public GraphNode(T value) : this(value, null) { }
        public GraphNode(T value, ICollection<Node<T>> successors) : base(value, successors) { }

        public IDictionary<GraphNode<T>, double> Costs
        {
            get
            {
                if (costs == null)
                    costs = CreateCostDictionary();

                return costs;
            }
        }

        public void AddSuccessor(GraphNode<T> successor)
        {
            AddSuccessor(successor, 0f);
        }

        public virtual void AddSuccessor(GraphNode<T> neighbor, double weight)
        {
            SuccessorCollection.Add(neighbor);
            Costs.Add(neighbor, weight);
        }

        protected virtual IDictionary<GraphNode<T>, double> CreateCostDictionary()
        {
            return new HashDictionary<GraphNode<T>, double>();
        }

        public IEnumerable<GraphNode<T>> Predecessors
        {
            get
            {
                return Graph.GetPredecessors(this);
            }
        }
    }

    public class Graph<T>
    {
        private ICollection<GraphNode<T>> _nodeSet;
        /// <summary>
        /// Constructor.
        /// </summary>
        public Graph() : this(null) { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nodeSet">Nodeset used to define graph.</param>
        public Graph(ICollection<GraphNode<T>> nodeSet)
        {
            if (nodeSet == null)
                this._nodeSet = CreateNodes();
            else
                this._nodeSet = nodeSet;
        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node">Node to add.</param>
        public virtual void AddNode(GraphNode<T> node)
        {
            if (node.Graph != null && node.Graph != this) throw new InvalidOperationException("Graph@AddNode(node): Cannot add a node of another graph!");
            node.Graph = this;
            _nodeSet.Add(node);
        }

        /// <summary>
        /// Adds a node with given predefined value.
        /// </summary>
        /// <param name="value">Value to set for created node.</param>
        public void AddNode(T value)
        {
            AddNode(CreateNode(value));
        }

        /// <summary>
        /// Adds a directed edge to the graph.
        /// </summary>
        /// <param name="from">Source node.</param>
        /// <param name="to">Destination node.</param>
        /// <param name="cost">Weight of the edge.</param>
        public void AddDirectedEdge(GraphNode<T> from, GraphNode<T> to, double cost)
        {
            from.AddSuccessor(to, cost);
        }
        /// <summary>
        /// Adds an undirected edge to graph by adding two directed edges.
        /// </summary>
        /// <param name="from">Source/destination node.</param>
        /// <param name="to">Destination/source node.</param>
        /// <param name="cost">Weight of the edge.</param>
        public void AddUndirectedEdge(GraphNode<T> from, GraphNode<T> to, double cost)
        {
            from.AddSuccessor(to, cost);
            to.AddSuccessor(from, cost);
        }
        /// <summary>
        /// Creates and returns a node with given value.
        /// </summary>
        /// <param name="value">Predefined value of created node.</param>
        /// <returns>A node that can be used in this graph.</returns>
        public GraphNode<T> CreateNode(T value)
        {
            GraphNode<T> result = CreateNode();
            result.Value = value;
            return result;
        }
        /// <summary>
        /// Creates and returns a node that can be used in this graph.
        /// </summary>
        /// <returns>A node that can be used in this graph.</returns>
        public virtual GraphNode<T> CreateNode()
        {
            return new GraphNode<T>();
        }

        protected virtual ICollection<GraphNode<T>> CreateNodes()
        {
            return new List<GraphNode<T>>();
        }

        /// <summary>
        /// Check whether given value exists in graph.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if a node contains given value, false else.</returns>
        public bool Contains(T value)
        {
            return Nodes.FirstOrDefault(elem => elem.Value.Equals(value)) != null;
        }

        /// <summary>
        /// Get the node containing given value.
        /// </summary>
        /// <param name="value">Value to get node for.</param>
        /// <returns>First node containing given value, or null if not existant.</returns>
        public GraphNode<T> GetNode(T value)
        {
            return Nodes.FirstOrDefault(
                elem =>
                    (elem.Value == null && value == null) ||
                    (elem.Value != null && elem.Value.Equals(value))
                );
        }

        /// <summary>
        /// Get all nodes of the graph.
        /// </summary>
        public IEnumerable<GraphNode<T>> Nodes
        {
            get
            {
                return _nodeSet;
            }
        }
        /// <summary>
        /// Get all nodevalues of the graph.
        /// </summary>
        public IEnumerable<T> Values
        {
            get
            {
                return _nodeSet.Select(elem => elem.Value);
            }
        }

        public IEnumerable<GraphNode<T>> GetPredecessors(GraphNode<T> neighbor)
        {
            return Nodes.Where(elem => elem.Successors.Contains(neighbor));
        }
    }

    public class BinaryTree<T>
    {
        public BinaryTreeNode<T> RootNode { private set; get; }
    }

    public class BinaryTreeNode<T> : Node<T>
    {
        public BinaryTree<T> Tree { private set; get; }
        protected BinaryTreeNode(BinaryTree<T> tree)
        {
            Tree = tree;
        }

        protected new List<BinaryTreeNode<T>> SuccessorCollection
        {
            get
            {
                return base.SuccessorCollection as List<BinaryTreeNode<T>>;
            }
        }

        public BinaryTreeNode<T> LeftNode
        {
            set
            {
                SuccessorCollection[0] = value;
            }
            get
            {
                return SuccessorCollection[0];
            }
        }

        public BinaryTreeNode<T> RightNode
        {
            set
            {
                SuccessorCollection[1] = value;
            }
            get
            {
                return SuccessorCollection[1];
            }
        }

        protected override ICollection<Node<T>> CreateSuccessorCollection()
        {
            List<Node<T>> result = new List<Node<T>>(2);
            result.Add(null);
            result.Add(null);
            return result;
        }
    }
}