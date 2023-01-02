namespace ILSA.Core {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using ILSA.Core.Patterns;

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
        [Display(AutoGenerateField = false)]
        public int NodeID {
            get;
            protected set;
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
            OnVisited();
        }
        public bool Visit(Predicate<Node> action) {
            if(action(this))
                return true;
            foreach(Node child in Nodes) {
                if(child.Visit(action))
                    return true;
            }
            return false;
        }
        protected virtual void OnVisited() { }
        protected internal virtual void Reset() { }
        protected internal virtual void OnPatternMatch(Pattern pattern) { }
        protected internal virtual void OnPatternMatch(Pattern pattern, int[] captures) { }
        protected internal virtual StringBuilder GetErrorsBuilder() {
            return new StringBuilder();
        }
        protected internal virtual bool HasErrors {
            get { return false; }
        }
        public override int GetHashCode() {
            return NodeID;
        }
    }
}