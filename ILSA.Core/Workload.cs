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
            var contexts = target.BeforeAnalyze(effects);
            for(int i = 0; i < visits.Length; i++) {
                var node = target.nodesCore[i];
                var context = contexts[i];
                Action<Node> advance = x => target.Advance(x, context);
                visits[i] = Task.Run(() => node.Visit(advance));
            }
            await Task.WhenAll(visits);
            target.EndAnalyze(contexts);
        }
        protected virtual AdvanceContext[] BeforeAnalyze(WorkloadBase effects) {
            var contexts = new AdvanceContext[nodesCore.Count];
            for(int i = 0; i < contexts.Length; i++)
                contexts[i] = new AdvanceContext(nodesCore[i], effects);
            return contexts;
        }
        protected virtual void EndAnalyze(AdvanceContext[] contexts) { }
        protected abstract void Advance(Node node);
        protected abstract void Advance(Node node, AdvanceContext context);
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
        public sealed class AdvanceContext {
            readonly Node root;
            readonly WorkloadBase effects;
            readonly List<Node> navigation = new List<Node>();
            public AdvanceContext(Node root, WorkloadBase effects) {
                this.root = root;
                this.effects = effects;
            }
            public void Apply(Node node, Assembly assembly) {
                if(effects.Apply(node, assembly))
                    navigation.Add(node);
            }
            public void Apply(Node node, Type type) {
                if(effects.Apply(node, type))
                    navigation.Add(node);
            }
            public void Apply(Node node, MethodBase method) {
                if(effects.Apply(node, method))
                    navigation.Add(node);
            }
            public int GetID() {
                return root.NodeID;
            }
            public bool HasNavigation {
                get { return navigation.Count > 0; }
            }
            public List<Node> GetNavigation() {
                return navigation;
            }
        }
    }
}