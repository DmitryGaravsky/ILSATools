namespace ILSA.Core.Hierarchy {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public enum NodeType {
        None,
        Assembly,
        References,
        Reference,
        Namespace,
        Type,
        BaseTypes,
        Interface,
        Class,
        ValueType,
        Enumeration,
        Method,
        MethodPrivate,
        MethodAbstract,
    }
    //
    public abstract class Node {
        public readonly static Node[] EmptyNodes = new Node[0];
        //
        protected readonly INodesFactory factory;
        protected Node(INodesFactory nodesFactory) {
            this.factory = nodesFactory;
        }
        public abstract string Name {
            get;
        }
        [Display(AutoGenerateField = false)]
        public abstract string Group {
            get;
        }
        public abstract IReadOnlyCollection<Node> Nodes {
            get;
        }
        [Display(AutoGenerateField = false)]
        public virtual NodeType Type {
            get { return NodeType.None; }
        }
        public void Visit(Action<Node> action) {
            action(this);
            foreach(Node child in Nodes) 
                child.Visit(action);
        }
    }
    abstract class Node<TSource> : Node {
        protected readonly TSource source;
        readonly string name;
        string? group;
        IReadOnlyCollection<Node>? nodes;
        protected Node(INodesFactory factory, TSource source)
            : base(factory) {
            this.source = source;
            this.name = GetName();
        }
        public sealed override string Name {
            get { return name; }
        }
        public sealed override string Group {
            get { return group ?? (group = GetGroup()); }
        }
        protected virtual string GetGroup() {
            return string.Empty;
        }
        public sealed override IReadOnlyCollection<Node> Nodes {
            get { return nodes ?? (nodes = GetNodes()); }
        }
        protected virtual IReadOnlyCollection<Node> GetNodes() {
            return EmptyNodes;
        }
        protected abstract string GetName();
    }
}