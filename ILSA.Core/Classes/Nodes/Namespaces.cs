namespace ILSA.Core.Classes {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    partial class ClassesFactory {
        sealed class Namespaces : Node<string> {
            readonly IEnumerable<Node> types;
            readonly Assembly assembly;
            public Namespaces(IClassesFactory factory, Tuple<string, Assembly, IEnumerable<Node>> types)
                : base(factory, types.Item1) {
                this.assembly = types.Item2;
                this.types = types.Item3;
            }
            internal Assembly GetAssembly() {
                return assembly;
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