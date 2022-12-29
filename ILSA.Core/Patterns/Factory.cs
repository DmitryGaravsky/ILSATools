namespace ILSA.Core.Patterns {
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;

    public interface INodesFactory {
        Node Create(Assembly assembly);
        Node Create(MethodInfo method);
        Node Namespaces<TNode>(IGrouping<string, TNode> types) where TNode : Node;
    }
    //
    public partial class NodesFactory : INodesFactory {
        public NodesFactory() {
            createAssemblyNode = x => new AssemblyNode(this, x);
            assembliesCache = new ConcurrentDictionary<Assembly, AssemblyNode>();
            createMethodNode = x => new MethodNode(this, x);
            methodsCache = new ConcurrentDictionary<MethodInfo, MethodNode>();
        }
        readonly ConcurrentDictionary<Assembly, AssemblyNode> assembliesCache;
        readonly Func<Assembly, AssemblyNode> createAssemblyNode;
        Node INodesFactory.Create(Assembly assembly) {
            return assembliesCache.GetOrAdd(assembly, createAssemblyNode);
        }
        readonly ConcurrentDictionary<MethodInfo, MethodNode> methodsCache;
        readonly Func<MethodInfo, MethodNode> createMethodNode;
        Node INodesFactory.Create(MethodInfo method) {
            return methodsCache.GetOrAdd(method, createMethodNode);
        }
        Node INodesFactory.Namespaces<TNode>(IGrouping<string, TNode> types) {
            return new Namespaces(this, types);
        }
    }
}