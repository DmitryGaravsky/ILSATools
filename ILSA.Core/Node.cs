namespace ILSA.Core {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Threading;

    public abstract class Node {
        public readonly static Node[] EmptyNodes = new Node[0];
        //
        string? name;
        public string Name {
            get { return name ?? (name = GetName()); }
        }
        protected abstract string GetName();
        //
        IReadOnlyCollection<Node>? nodes;
        public IReadOnlyCollection<Node> Nodes {
            get { return nodes ?? (nodes = GetNodes()); }
        }
        protected virtual IReadOnlyCollection<Node> GetNodes() {
            return EmptyNodes;
        }
        string? group;
        [Display(AutoGenerateField = false)]
        public string Group {
            get { return group ?? (group = GetGroup()); }
        }
        protected virtual string GetGroup() {
            return string.Empty;
        }
        [Display(AutoGenerateField = false)]
        public virtual int TypeCode {
            get { return 0; }
        }
        public void Visit(Action<Node> action) {
            action(this);
            foreach(Node child in Nodes)
                child.Visit(action);
        }
    }
}