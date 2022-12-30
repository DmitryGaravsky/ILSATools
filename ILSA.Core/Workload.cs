namespace ILSA.Core {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

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
        protected readonly StringBuilder ErrorsBuilder = new StringBuilder();
        public static async Task AnalyzeAsync(WorkloadBase target, WorkloadBase effects) {
            Action<Node> advance = x => target.Advance(x, effects);
            effects.ErrorsBuilder.Clear();
            Task[] visits = new Task[target.nodesCore.Count];
            for(int i = 0; i < visits.Length; i++) {
                var node = target.nodesCore[i];
                visits[i] = Task.Run(() => node.Visit(advance));
            }
            await Task.WhenAll(visits);
        }
        //
        protected abstract void Advance(Node node);
        protected abstract void Advance(Node node, WorkloadBase effects);
        // Effects
        protected internal virtual void Apply(Node node, Assembly assembly) { }
        protected internal virtual void Apply(Node node, Type type) { }
        protected internal virtual void Apply(Node node, MethodBase method) { }
    }
}