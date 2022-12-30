namespace ILSA.Core.Classes {
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
                    .Select(factory.Create).ToList();
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
            protected override void Advance(Node node, WorkloadBase effects) {
                node.Reset();
                switch(node) {
                    case AssemblyNode a:
                        effects.Apply(a, a.GetSource());
                        break;
                    case TypeNode t:
                        effects.Apply(t, t.GetSource());
                        break;
                    case MethodNode m:
                        effects.Apply(m, m.GetSource());
                        break;
                }
            }
            public override string ToString() {
                return $"Assemblies: {methods} methods and {types} types from {assemblies} assemblies";
            }
        }
    }
}