namespace ILSA.Core.Hierarchy {
    using System.Collections.Generic;
    using System.Linq;

    partial class NodesFactory {
        sealed class Namespaces : Node<string> {
            readonly IEnumerable<Node> types;
            public Namespaces(INodesFactory factory, IGrouping<string, Node> types)
                : base(factory, types.Key) {
                this.types = types;
            }
            protected sealed override string GetName() {
                return source;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                var typeNodes = types.OfType<TypeNode>();
                return typeNodes.Where(x => !x.IsNested).ToArray();
            }
            public sealed override NodeType Type {
                get { return NodeType.Namespace; }
            }
        }
    }
}