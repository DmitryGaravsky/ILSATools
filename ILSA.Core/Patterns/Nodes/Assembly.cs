namespace ILSA.Core.Patterns {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;

    partial class NodesFactory {
        public static Assembly? GetAssembly(Node node) {
            var assemblyNode = node as AssemblyNode;
            return (assemblyNode != null) ? assemblyNode.GetAssembly() : null;
        }
        sealed class AssemblyNode : Node<Assembly> {
            public AssemblyNode(INodesFactory factory, Assembly assembly)
                : base(factory, assembly) {
            }
            public Assembly GetAssembly() {
                return source;
            }
            protected sealed override string GetName() {
                return source.GetName().Name;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                var types = GetTypes(source).OfType<Type>()
                    .Where(x => x.IsSealed && x.IsAbstract);
                var matchMethods = types.Select(FindMatchMethod).OfType<MethodInfo>()
                    .Select(factory.Create);
                var namespaces = matchMethods.GroupBy(x => x.Group)
                    .Select(factory.Namespaces).ToArray();
                var nodes = new Node[namespaces.Length];
                for(int i = 0; i < namespaces.Length; i++)
                    nodes[i] = namespaces[i];
                return nodes;
            }
            public sealed override int TypeCode {
                get { return (int)NodeType.Assembly; }
            }
            static MethodInfo? FindMatchMethod(Type type) {
                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                for(int i = 0; i < methods.Length; i++) {
                    var method = methods[i];
                    if(method.Name != "Match")
                        continue;
                    if(method.ReturnType != typeof(bool))
                        continue;
                    var parameters = method.GetParameters();
                    if(parameters == null || parameters.Length != 2)
                        continue;
                    if(parameters[0].ParameterType != typeof(IILReader))
                        continue;
                    if(parameters[1].ParameterType != typeof(StringBuilder))
                        continue;
                    return method;
                }
                return null;
            }
            static Type[] GetTypes(Assembly assembly) {
                try { return assembly.GetExportedTypes(); }
                catch(ReflectionTypeLoadException e) { return e.Types; }
            }
        }
    }
}