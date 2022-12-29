namespace ILSA.Core.Classes {
    public partial class NodesFactory {
        public class Workload : WorkloadBase {
            public override void Advance(Node node) {
                switch(node) {
                    case AssemblyNode a:
                        OnAssembly(a.GetAssembly());
                        break;
                    case TypeNode:
                        OnType();
                        break;
                    case MethodNode:
                        OnMethod();
                        break;
                }
                base.Advance(node);
            }
            public override string ToString() {
                return $"Assemblies: {methods} methods and {types} types from {assemblies} assemblies";
            }
        }
    }
}