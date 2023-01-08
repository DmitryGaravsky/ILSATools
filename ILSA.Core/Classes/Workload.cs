namespace ILSA.Core.Classes {
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using ILSA.Core.Sources;

    public partial class ClassesFactory {
        public class Workload : WorkloadBase {
            Workload(List<Node> nodes)
                : base(nodes) {
            }
            public static async Task<Workload> LoadAsync(IAssembliesSource assembliesSource, IClassesFactory factory) {
                var nodes = assembliesSource.Assemblies
                    .OrderBy(x => x.GetName().Name)
                    .Select(factory.Create)
                    .ToList();
                var workload = new Workload(nodes);
                await LoadAsync(workload, workload.Nodes);
                return workload;
            }
            public static async Task<Workload> LoadBackTraceAsync(Workload workload) {
                await LoadAsync(workload, workload.BackTrace);
                return workload;
            }
            protected sealed override void Advance(Node node) {
                node.Reset();
                switch(node) {
                    case AssemblyNode a:
                        OnAssembly(a.GetSource());
                        break;
                    case TypeNode t:
                        OnType(t.GetSource());
                        break;
                    case MethodNode m:
                        OnMethod(m.GetSource());
                        break;
                }
            }
            public sealed override bool CanNavigate {
                get { return !IsEmpty && branchesCore.Count > 0; }
            }
            readonly List<Node> backTraceCore = new List<Node>();
            public List<Node> BackTrace {
                get { return backTraceCore; }
            }
            readonly ConcurrentDictionary<int, Branch> branchesCore = new ConcurrentDictionary<int, Branch>();
            int? analysisCompletion;
            int methodsProcessed;
            readonly ConcurrentDictionary<MethodBase, HashSet<MethodBase>> callers = new ConcurrentDictionary<MethodBase, HashSet<MethodBase>>();
            protected sealed override Branch[] BeforeAnalyze(WorkloadBase effects) {
                analysisCompletion = 0;
                methodsProcessed = 0;
                branchesCore.Clear();
                backTraceCore.Clear();
                callers.Clear();
                var scope = new HashSet<Assembly>();
                foreach(AssemblyNode a in Nodes)
                    scope.Add(a.GetSource());
                effects.SetScope(scope, callers);
                RaiseAnalysisProgress(0);
                var branches = new Branch[Nodes.Count];
                for(int i = 0; i < branches.Length; i++)
                    branches[i] = new Branch(Nodes[i], effects, callers);
                return branches;
            }
            protected internal sealed override void SetScope(HashSet<Assembly> scope, ConcurrentDictionary<MethodBase, HashSet<MethodBase>> callers) {
                throw new System.NotImplementedException();
            }
            protected sealed override void Advance(Node node, Branch branch) {
                node.Reset();
                switch(node) {
                    case AssemblyNode a:
                        branch.Apply(a, a.GetSource());
                        break;
                    case TypeNode t:
                        branch.Apply(t, t.GetSource());
                        break;
                    case MethodNode m:
                        branch.Apply(m, m.GetSource());
                        int progress = (Interlocked.Increment(ref methodsProcessed) * 100) / methods;
                        if(progress != analysisCompletion.GetValueOrDefault())
                            RaiseAnalysisProgress((analysisCompletion = progress).Value);
                        break;
                }
            }
            protected sealed override void EndAnalyze(Branch[] branches) {
                branchesCore.Clear();
                backTraceCore.Clear();
                var effectiveBranches = branches.Where(x => x.HasMatches);
                foreach(var branch in effectiveBranches) {
                    branchesCore.GetOrAdd(branch.GetID(), x => branch);
                    backTraceCore.Add(CreateBackTrace(branch));
                }
                methodsProcessed = 0;
                analysisCompletion = null;
                RaiseAnalysisProgress(100);
            }
            public sealed override Node Next(Node node, IClassesFactory factory) {
                int? rootId = factory.GetRootNodeID(node, out Node? navigationOrigin);
                if(rootId.HasValue && navigationOrigin != null) {
                    if(branchesCore.TryGetValue(rootId.Value, out var branch))
                        return branch.NextMatch(navigationOrigin) ?? node;
                }
                return node;
            }
            public sealed override Node Previous(Node node, IClassesFactory factory) {
                int? rootId = factory.GetRootNodeID(node, out Node? navigationOrigin);
                if(rootId.HasValue && navigationOrigin != null) {
                    if(branchesCore.TryGetValue(rootId.Value, out var branch))
                        return branch.PrevMatch(navigationOrigin) ?? node;
                }
                return node;
            }
            public sealed override string ToString() {
                return $"Assemblies: {methods} methods and {types} types from {assemblies} assemblies";
            }
        }
    }
}