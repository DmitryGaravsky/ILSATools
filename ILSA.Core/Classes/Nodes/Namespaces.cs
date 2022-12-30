namespace ILSA.Core.Classes {
    using System.Collections.Generic;
    using System.Linq;

    partial class ClassesFactory {
        sealed class Namespaces : Node<string> {
            readonly IEnumerable<Node> types;
            public Namespaces(IClassesFactory factory, IGrouping<string, Node> types)
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
            public sealed override int TypeCode {
                get { return (int)NodeType.Namespace; }
            }
        }
    }
}