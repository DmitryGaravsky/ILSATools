namespace ILSA.Core.Patterns {
    public partial class NodesFactory {
        public class Workload : WorkloadBase {
            public override void Advance(Node node) {
                switch(node) {
                    case AssemblyNode a:
                        OnAssembly(a.GetAssembly());
                        break;
                    case MethodNode:
                        OnMethod();
                        break;
                }
                base.Advance(node);
            }
            public override string ToString() {
                return $"Patterns: {methods} patterns from {assemblies} assemblies";
            }
        }
    }
}