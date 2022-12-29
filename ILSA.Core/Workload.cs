namespace ILSA.Core {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    public class WorkloadBase {
        public static async Task<TWorkload> LoadAsync<TWorkload>(IList<Node> nodes, TWorkload workload)
            where TWorkload : WorkloadBase {
            Action<Node> advance = workload.Advance;
            Task[] visits = new Task[nodes.Count];
            for(int i = 0; i < visits.Length; i++) {
                var node = nodes[i];
                visits[i] = Task.Run(() => node.Visit(advance));
            }
            await Task.WhenAll(visits);
            return workload;
        }
        protected int assemblies, types, methods, nodes;
        readonly List<string> assemblyLocations = new List<string>();
        public bool IsEmpty {
            get { return assemblyLocations.Count > 0; }
        }
        public virtual void Advance(Node node) {
            Interlocked.Increment(ref nodes);
        }
        protected void OnAssembly(Assembly assembly) {
            if(assembly != null && !assembly.IsDynamic)
                assemblyLocations.Add(assembly.Location);
            Interlocked.Increment(ref assemblies);
        }
        protected void OnType() {
            Interlocked.Increment(ref types);
        }
        protected void OnMethod() {
            Interlocked.Increment(ref methods);
        }
    }
}