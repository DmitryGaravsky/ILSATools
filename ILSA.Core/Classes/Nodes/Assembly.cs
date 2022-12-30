namespace ILSA.Core.Classes {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    partial class ClassesFactory {
        public static Assembly? GetAssembly(Node node) {
            var assemblyNode = node as AssemblyNode;
            return (assemblyNode != null) ? assemblyNode.GetSource() : null;
        }
        sealed class AssemblyNode : Node<Assembly> {
            public AssemblyNode(IClassesFactory factory, Assembly assembly)
                : base(factory, assembly) {
            }
            public Assembly GetSource() {
                return source;
            }
            protected sealed override string GetName() {
                return source.GetName().Name;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                var types = GetTypes(source).OfType<Type>().Select(factory.Create);
                var namespaces = types.GroupBy(x => x.Group).Select(factory.Namespaces).ToArray();
                var nodes = new Node[namespaces.Length + 1];
                nodes[0] = factory.References(source);
                for(int i = 0; i < namespaces.Length; i++)
                    nodes[i + 1] = namespaces[i];
                return nodes;
            }
            public sealed override int TypeCode {
                get { return (int)NodeType.Assembly; }
            }
            static Type[] GetTypes(Assembly assembly) {
                try { return assembly.GetTypes(); }
                catch(ReflectionTypeLoadException e) { return e.Types; }
            }
        }
    }
}