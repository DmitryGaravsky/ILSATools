namespace ILSA.Core.Classes {
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
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
                get { return !IsEmpty && navigationBranches.Count > 0; }
            }
            readonly ConcurrentDictionary<int, NavigationBranch> navigationBranches = new ConcurrentDictionary<int, NavigationBranch>();
            protected override AdvanceContext[] BeforeAnalyze(WorkloadBase effects) {
                navigationBranches.Clear();
                return base.BeforeAnalyze(effects);
            }
            protected override void Advance(Node node, AdvanceContext context) {
                node.Reset();
                switch(node) {
                    case AssemblyNode a:
                        context.Apply(a, a.GetSource());
                        break;
                    case TypeNode t:
                        context.Apply(t, t.GetSource());
                        break;
                    case MethodNode m:
                        context.Apply(m, m.GetSource());
                        break;
                }
            }
            protected override void EndAnalyze(AdvanceContext[] contexts) {
                foreach(var context in contexts) {
                    if(context.HasNavigation)
                        navigationBranches.AddOrUpdate(context.GetID(), x => new NavigationBranch(context.GetNavigation()), (x, b) => b);
                }
            }
            public override Node Next(Node node, IClassesFactory factory) {
                Node? assemblyNode;
                int? assemblyNodeId = ((ClassesFactory)factory).GetAssemblyNodeID(node, out assemblyNode);
                if(assemblyNodeId.HasValue && assemblyNode != null) {
                    if(navigationBranches.TryGetValue(assemblyNodeId.Value, out var branch)) {
                        branch.Reset();
                        if(assemblyNode.Visit(branch.Next))
                            return branch.Result ?? branch.First;
                    }
                }
                return node;
            }
            public override string ToString() {
                return $"Assemblies: {methods} methods and {types} types from {assemblies} assemblies";
            }
            sealed class NavigationBranch {
                readonly HashSet<int> matches = new HashSet<int>();
                readonly Dictionary<int, Node> nextNodes = new Dictionary<int, Node>();
                public NavigationBranch(List<Node> navigation) {
                    First = navigation[0];
                    var prev = navigation[navigation.Count - 1];
                    foreach(var node in navigation) {
                        matches.Add(node.NodeID);
                        nextNodes.Add(prev.NodeID, prev = node);
                    }
                }
                public readonly Node First;
                public void Reset() {
                    Result = null;
                }
                public bool Next(Node node) {
                    if(matches.Contains(node.NodeID))
                        Result = nextNodes[node.NodeID];
                    return (Result != null);
                }
                public Node? Result;
            }
        }
    }
}