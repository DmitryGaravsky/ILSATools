namespace ILSA.Core.Classes {
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
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
                await LoadAsync(workload);
                return workload;
            }
            protected override void Advance(Node node) {
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
            public override bool CanNavigate {
                get { return !IsEmpty && branchesCore.Count > 0; }
            }
            readonly ConcurrentDictionary<int, Branch> branchesCore = new ConcurrentDictionary<int, Branch>();
            readonly List<Node> backTraceCore = new List<Node>();
            public List<Node> BackTrace {
                get { return backTraceCore; }
            }
            int? analysisCompletion;
            int methodsProcessed;
            protected override Branch[] BeforeAnalyze(WorkloadBase effects) {
                analysisCompletion = 0;
                methodsProcessed = 0;
                branchesCore.Clear();
                backTraceCore.Clear();
                RaiseAnalysisProgress(0);
                return base.BeforeAnalyze(effects);
            }
            protected override void Advance(Node node, Branch branch) {
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
                        UpdateProgress();
                        break;
                }
            }
            void UpdateProgress() {
                int progress = (Interlocked.Increment(ref methodsProcessed) * 100) / methods;
                if(progress != analysisCompletion.GetValueOrDefault())
                    RaiseAnalysisProgress((analysisCompletion = progress).Value);
            }
            protected override void EndAnalyze(Branch[] branches) {
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
            public override Node Next(Node node, IClassesFactory factory) {
                int? rootId = factory.GetRootNodeID(node, out Node? navigationOrigin);
                if(rootId.HasValue && navigationOrigin != null) {
                    if(branchesCore.TryGetValue(rootId.Value, out var branch))
                        return branch.NextMatch(navigationOrigin) ?? node;
                }
                return node;
            }
            public override Node Previous(Node node, IClassesFactory factory) {
                int? rootId = factory.GetRootNodeID(node, out Node? navigationOrigin);
                if(rootId.HasValue && navigationOrigin != null) {
                    if(branchesCore.TryGetValue(rootId.Value, out var branch))
                        return branch.PrevMatch(navigationOrigin) ?? node;
                }
                return node;
            }
            public override string ToString() {
                return $"Assemblies: {methods} methods and {types} types from {assemblies} assemblies";
            }
        }
    }
}