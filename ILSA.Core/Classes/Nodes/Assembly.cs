namespace ILSA.Core.Hierarchy {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    partial class NodesFactory {
        public static Assembly? GetAssembly(Node node) {
            var assemblyNode = node as AssemblyNode;
            return (assemblyNode != null) ? assemblyNode.GetAssembly() : null;
        }
        sealed class AssemblyNode : Node<Assembly> {
            public AssemblyNode(INodesFactory factory, Assembly assembly)
                : base(factory, assembly) {
            }
            public Assembly? GetAssembly() {
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
            public sealed override NodeType Type {
                get { return NodeType.Assembly; }
            }
            static Type[] GetTypes(Assembly assembly) {
                try {
                    return assembly.GetTypes();
                }
                catch(ReflectionTypeLoadException e) { return e.Types; }
            }
        }
    }
}