namespace ILSA.Core.Hierarchy {
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;

    public interface INodesFactory {
        Node Create(Assembly assembly);
        Node Create(AssemblyName assemblyName);
        Node Create(Type type);
        Node Create(MethodBase method);
        Node References(Assembly assembly);
        Node Namespaces<TNode>(IGrouping<string, TNode> types) where TNode : Node;
        Node BaseTypes(Type type);
        Node BaseType(Type type);
    }
    public partial class NodesFactory : INodesFactory {
        public NodesFactory() {
            assembliesCache = new ConcurrentDictionary<Assembly, AssemblyNode>();
            referencesCache = new ConcurrentDictionary<AssemblyName, Reference>();
            typesCache = new ConcurrentDictionary<Type, TypeNode>();
            methodsCache = new ConcurrentDictionary<MethodBase, MethodNode>();
            baseTypesCache = new ConcurrentDictionary<Type, BaseTypes>();
        }
        readonly ConcurrentDictionary<Assembly, AssemblyNode> assembliesCache;
        Node INodesFactory.Create(Assembly assembly) {
            return assembliesCache.GetOrAdd(assembly, x => new AssemblyNode(this, x));
        }
        readonly ConcurrentDictionary<AssemblyName, Reference> referencesCache;
        Node INodesFactory.Create(AssemblyName assemblyName) {
            return referencesCache.GetOrAdd(assemblyName, x => new Reference(this, x));
        }
        readonly ConcurrentDictionary<Type, TypeNode> typesCache;
        Node INodesFactory.Create(Type type) {
            return typesCache.GetOrAdd(type, x => new TypeNode(this, x));
        }
        readonly ConcurrentDictionary<MethodBase, MethodNode> methodsCache;
        Node INodesFactory.Create(MethodBase method) {
            return methodsCache.GetOrAdd(method, x => new MethodNode(this, x));
        }
        readonly ConcurrentDictionary<Type, BaseTypes> baseTypesCache;
        Node INodesFactory.BaseType(Type type) {
            return baseTypesCache.GetOrAdd(type, x => new BaseTypes(this, x, false));
        }
        Node INodesFactory.Namespaces<TNode>(IGrouping<string, TNode> types) {
            return new Namespaces(this, types);
        }
        Node INodesFactory.References(Assembly assembly) {
            return new References(this, assembly);
        }
        Node INodesFactory.BaseTypes(Type type) {
            return new BaseTypes(this, type, true);
        }
    }
}