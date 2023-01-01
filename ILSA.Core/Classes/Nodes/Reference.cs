namespace ILSA.Core.Classes {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    partial class ClassesFactory {
        sealed class Reference : Node<Tuple<AssemblyName, Assembly>> {
            public Reference(IClassesFactory factory, Tuple<AssemblyName, Assembly> reference)
                : base(factory, reference) {
            }
            internal Assembly GetAssembly() {
                return source.Item2;
            }
            protected sealed override string GetName() {
                return source.Item1.Name;
            }
            public override int TypeCode {
                get { return (int)NodeType.Reference; }
            }
        }
        sealed class References : Node<Assembly> {
            public References(IClassesFactory factory, Assembly assembly)
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
                return references.OrderBy(x => x.Name)
                    .Select(x => Tuple.Create(x, source))
                    .Select(factory.Create).ToArray();
            }
        }
    }
}