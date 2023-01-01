namespace ILSA.Core {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
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
        //
        protected static class Murmur<TSource> {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int Calc(TSource source) {
                int start = Compress(448839895, typeof(TSource).GetHashCode());
                return Finalization(Compress(start, source?.GetHashCode() ?? 62043647));
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int Compress(int prev, int next) {
                uint num = (uint)prev;
                uint num2 = (uint)next;
                num2 *= 1540483477;
                num2 ^= num2 >> 24;
                num2 *= 1540483477;
                num *= 1540483477;
                return (int)(num ^ num2);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int Finalization(int hashState) {
                uint num = (uint)hashState;
                num ^= num >> 13;
                num *= 1540483477;
                return (int)(num ^ (num >> 15));
            }
        }
    }
}