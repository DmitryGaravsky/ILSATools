namespace ILSA.Core.Patterns {
    using System.Collections.Generic;
    using System.Linq;

    partial class PatternsFactory {
        sealed class Namespaces : Node<string> {
            readonly IEnumerable<Node> methods;
            public Namespaces(IPatternsFactory factory, IGrouping<string, Node> methodNodes)
                : base(factory, methodNodes.Key) {
                this.methods = methodNodes;
            }
            protected sealed override string GetName() {
                return source;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                return methods.ToArray();
            }
            public sealed override int TypeCode {
                get { return (int)NodeType.Namespace; }
            }
        }
    }
}