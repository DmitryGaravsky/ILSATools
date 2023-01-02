namespace ILSA.Core {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using ILSA.Core.Classes;

    public abstract class WorkloadBase {
        readonly List<Node> nodesCore;
        protected WorkloadBase(List<Node> nodes) {
            this.nodesCore = nodes;
        }
        public List<Node> Nodes {
            get { return nodesCore; }
        }
        protected int assemblies, types, methods;
        readonly List<string> assemblyLocations = new List<string>();
        public virtual bool IsEmpty {
            get { return assemblyLocations.Count == 0; }
        }
        public virtual bool CanNavigate {
            get { return !IsEmpty; }
        }
        protected void OnAssembly(Assembly assembly) {
            if(assembly != null && !assembly.IsDynamic)
                assemblyLocations.Add(assembly.Location);
            Interlocked.Increment(ref assemblies);
        }
        protected void OnType(Type type) {
            Interlocked.Increment(ref types);
        }
        protected void OnMethod(MethodBase method) {
            Interlocked.Increment(ref methods);
        }
        protected static async Task LoadAsync<TWorkload>(TWorkload workload)
            where TWorkload : WorkloadBase {
            Action<Node> advance = workload.Advance;
            Task[] visits = new Task[workload.nodesCore.Count];
            for(int i = 0; i < visits.Length; i++) {
                var node = workload.nodesCore[i];
                visits[i] = Task.Run(() => node.Visit(advance));
            }
            await Task.WhenAll(visits);
        }
        public static async Task AnalyzeAsync(WorkloadBase target, WorkloadBase effects) {
            Task[] visits = new Task[target.nodesCore.Count];
            var branches = target.BeforeAnalyze(effects);
            for(int i = 0; i < visits.Length; i++) {
                var node = target.nodesCore[i];
                var branch = branches[i];
                Action<Node> advance = x => target.Advance(x, branch);
                visits[i] = Task.Run(() => node.Visit(advance));
            }
            await Task.WhenAll(visits);
            target.EndAnalyze(branches);
        }
        protected virtual Branch[] BeforeAnalyze(WorkloadBase effects) {
            var branches = new Branch[nodesCore.Count];
            for(int i = 0; i < branches.Length; i++)
                branches[i] = new Branch(nodesCore[i], effects);
            return branches;
        }
        protected virtual void EndAnalyze(Branch[] brunches) { }
        protected abstract void Advance(Node node);
        protected abstract void Advance(Node node, Branch branch);
        // Effects
        protected internal virtual bool Apply(Node node, Assembly assembly) {
            return false;
        }
        protected internal virtual bool Apply(Node node, Type type) {
            return false;
        }
        protected internal virtual bool Apply(Node node, MethodBase method) {
            return false;
        }
        public virtual Node Next(Node node, IClassesFactory factory) {
            return node;
        }
        public virtual Node Previous(Node node, IClassesFactory factory) {
            return node;
        }
        //
        public sealed class Branch {
            readonly Node root;
            readonly WorkloadBase effects;
            readonly List<Node> nodesInNavigationOrder = new List<Node>();
            readonly Dictionary<int, int> nodeIdToIndex = new Dictionary<int, int>();
            readonly HashSet<int> matchedNodeIndices = new HashSet<int>();
            public Branch(Node root, WorkloadBase effects) {
                this.root = root;
                this.effects = effects;
            }
            public void Apply(Node node, Assembly assembly) {
                int index = EnsureNodeIndexAndNavigationOrder(node);
                if(effects.Apply(node, assembly))
                    matchedNodeIndices.Add(index);
            }
            public void Apply(Node node, Type type) {
                int index = EnsureNodeIndexAndNavigationOrder(node);
                if(effects.Apply(node, type))
                    matchedNodeIndices.Add(index);
            }
            public void Apply(Node node, MethodBase method) {
                int index = EnsureNodeIndexAndNavigationOrder(node);
                if(effects.Apply(node, method))
                    matchedNodeIndices.Add(index);
            }
            int EnsureNodeIndexAndNavigationOrder(Node node) {
                int index = nodesInNavigationOrder.Count;
                nodeIdToIndex.Add(node.NodeID, index);
                nodesInNavigationOrder.Add(node);
                return index;
            }
            public int GetID() {
                return root.NodeID;
            }
            public bool HasMatches {
                get { return matchedNodeIndices.Count > 0; }
            }
            public Node? NextMatch(Node node) {
                var currentIndex = nodeIdToIndex[node.NodeID];
                for(int i = currentIndex + 1; i < nodesInNavigationOrder.Count; i++) {
                    if(matchedNodeIndices.Contains(i))
                        return nodesInNavigationOrder[i];
                }
                for(int i = 0; i < currentIndex; i++) {
                    if(matchedNodeIndices.Contains(i))
                        return nodesInNavigationOrder[i];
                }
                return null;
            }
            public Node? PrevMatch(Node node) {
                var currentIndex = nodeIdToIndex[node.NodeID];
                for(int i = currentIndex - 1; i > 0; i--) {
                    if(matchedNodeIndices.Contains(i))
                        return nodesInNavigationOrder[i];
                }
                for(int i = nodesInNavigationOrder.Count - 1; i > currentIndex; i--) {
                    if(matchedNodeIndices.Contains(i))
                        return nodesInNavigationOrder[i];
                }
                return null;
            }
        }
    }
}