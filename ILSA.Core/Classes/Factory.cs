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
        Node BaseTypes(Type type);
        Node BaseType(Type type);
        Node References(Assembly assembly);
        Node Namespaces<TNode>(IGrouping<string, TNode> types) where TNode : Node;
    }
    //
    public partial class NodesFactory : INodesFactory {
        public NodesFactory() {
            createAssemblyNode = x => new AssemblyNode(this, x);
            assembliesCache = new ConcurrentDictionary<Assembly, AssemblyNode>();
            createReference = x => new Reference(this, x);
            referencesCache = new ConcurrentDictionary<AssemblyName, Reference>();
            createTypeNode = x => new TypeNode(this, x);
            typesCache = new ConcurrentDictionary<Type, TypeNode>();
            createMethodNode = x => new MethodNode(this, x);
            methodsCache = new ConcurrentDictionary<MethodBase, MethodNode>();
            createBaseTypes = x => new BaseTypes(this, x, false);
            baseTypesCache = new ConcurrentDictionary<Type, BaseTypes>();
            createBaseTypesRoot = x => new BaseTypes(this, x, true);
            baseTypesRootsCache = new ConcurrentDictionary<Type, BaseTypes>();
        }
        readonly ConcurrentDictionary<Assembly, AssemblyNode> assembliesCache;
        readonly Func<Assembly, AssemblyNode> createAssemblyNode;
        Node INodesFactory.Create(Assembly assembly) {
            return assembliesCache.GetOrAdd(assembly, createAssemblyNode);
        }
        readonly ConcurrentDictionary<AssemblyName, Reference> referencesCache;
        readonly Func<AssemblyName, Reference> createReference;
        Node INodesFactory.Create(AssemblyName assemblyName) {
            return referencesCache.GetOrAdd(assemblyName, createReference);
        }
        readonly ConcurrentDictionary<Type, TypeNode> typesCache;
        readonly Func<Type, TypeNode> createTypeNode;
        Node INodesFactory.Create(Type type) {
            return typesCache.GetOrAdd(type, createTypeNode);
        }
        readonly ConcurrentDictionary<MethodBase, MethodNode> methodsCache;
        readonly Func<MethodBase, MethodNode> createMethodNode;
        Node INodesFactory.Create(MethodBase method) {
            return methodsCache.GetOrAdd(method, createMethodNode);
        }
        readonly ConcurrentDictionary<Type, BaseTypes> baseTypesCache;
        readonly Func<Type, BaseTypes> createBaseTypes;
        Node INodesFactory.BaseType(Type type) {
            return baseTypesCache.GetOrAdd(type, createBaseTypes);
        }
        readonly ConcurrentDictionary<Type, BaseTypes> baseTypesRootsCache;
        readonly Func<Type, BaseTypes> createBaseTypesRoot;
        Node INodesFactory.BaseTypes(Type type) {
            return baseTypesRootsCache.GetOrAdd(type, createBaseTypesRoot);
        }
        Node INodesFactory.Namespaces<TNode>(IGrouping<string, TNode> types) {
            return new Namespaces(this, types);
        }
        Node INodesFactory.References(Assembly assembly) {
            return new References(this, assembly);
        }
    }
}