namespace ILSA.Core.Classes {
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    partial class NodesFactory {
        sealed class Reference : Node<AssemblyName> {
            public Reference(INodesFactory factory, AssemblyName assemblyName)
                : base(factory, assemblyName) {
            }
            protected sealed override string GetName() {
                return source.Name;
            }
            public override int TypeCode {
                get { return (int)NodeType.Reference; }
            }
        }
        sealed class References : Node<Assembly> {
            public References(INodesFactory factory, Assembly assembly)
                : base(factory, assembly) {
            }
            protected sealed override string GetName() {
                return "References";
            }
            public override int TypeCode {
                get { return (int)NodeType.References; }
            }
            protected override IReadOnlyCollection<Node> GetNodes() {
                var references = source.GetReferencedAssemblies();
                return references.Select(factory.Create).OrderBy(x => x.Name).ToArray();
            }
        }
    }
}