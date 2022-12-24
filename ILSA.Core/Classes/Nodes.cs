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
        MethodPrivate
    }
    //
    public abstract class Node {
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
    }
    abstract class Node<TSource> : Node {
        protected readonly TSource source;
        readonly Lazy<string> name;
        readonly Lazy<string> group;
        readonly Lazy<IReadOnlyCollection<Node>> nodes;
        protected Node(INodesFactory factory, TSource source)
            : base(factory) {
            this.source = source;
            this.name = new Lazy<string>(GetName);
            this.group = new Lazy<string>(GetGroup);
            this.nodes = new Lazy<IReadOnlyCollection<Node>>(GetNodes);
        }
        public sealed override string Name {
            get { return name.Value; }
        }
        public sealed override string Group {
            get { return group.Value; }
        }
        public sealed override IReadOnlyCollection<Node> Nodes {
            get { return nodes.Value; }
        }
        protected virtual string GetGroup() {
            return string.Empty;
        }
        public readonly static Node[] EmptyNodes = new Node[] { };
        protected virtual IReadOnlyCollection<Node> GetNodes() {
            return EmptyNodes;
        }
        protected abstract string GetName();
    }
}