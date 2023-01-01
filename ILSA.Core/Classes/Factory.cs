﻿namespace ILSA.Core.Classes {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    public interface IClassesFactory {
        Node Create(Assembly assembly);
        Node Create(Tuple<AssemblyName, Assembly> reference);
        Node Create(Type type);
        Node Create(MethodBase method);
        Node BaseTypes(Type type);
        Node BaseType(Type type);
        Node References(Assembly assembly);
        Node Namespaces(Tuple<string, Assembly, IEnumerable<Node>> types);
    }
    //
    public partial class ClassesFactory : IClassesFactory {
        public ClassesFactory() {
            createAssemblyNode = x => new AssemblyNode(this, x);
            assembliesCache = new ConcurrentDictionary<Assembly, AssemblyNode>();
            createReference = x => new Reference(this, x);
            referencesCache = new ConcurrentDictionary<Tuple<AssemblyName, Assembly>, Reference>();
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
        Node IClassesFactory.Create(Assembly assembly) {
            return assembliesCache.GetOrAdd(assembly, createAssemblyNode);
        }
        readonly ConcurrentDictionary<Tuple<AssemblyName, Assembly>, Reference> referencesCache;
        readonly Func<Tuple<AssemblyName, Assembly>, Reference> createReference;
        Node IClassesFactory.Create(Tuple<AssemblyName, Assembly> reference) {
            return referencesCache.GetOrAdd(reference, createReference);
        }
        readonly ConcurrentDictionary<Type, TypeNode> typesCache;
        readonly Func<Type, TypeNode> createTypeNode;
        Node IClassesFactory.Create(Type type) {
            return typesCache.GetOrAdd(type, createTypeNode);
        }
        readonly ConcurrentDictionary<MethodBase, MethodNode> methodsCache;
        readonly Func<MethodBase, MethodNode> createMethodNode;
        Node IClassesFactory.Create(MethodBase method) {
            return methodsCache.GetOrAdd(method, createMethodNode);
        }
        readonly ConcurrentDictionary<Type, BaseTypes> baseTypesCache;
        readonly Func<Type, BaseTypes> createBaseTypes;
        Node IClassesFactory.BaseType(Type type) {
            return baseTypesCache.GetOrAdd(type, createBaseTypes);
        }
        readonly ConcurrentDictionary<Type, BaseTypes> baseTypesRootsCache;
        readonly Func<Type, BaseTypes> createBaseTypesRoot;
        Node IClassesFactory.BaseTypes(Type type) {
            return baseTypesRootsCache.GetOrAdd(type, createBaseTypesRoot);
        }
        Node IClassesFactory.Namespaces(Tuple<string, Assembly, IEnumerable<Node>> types) {
            return new Namespaces(this, types);
        }
        Node IClassesFactory.References(Assembly assembly) {
            return new References(this, assembly);
        }
        public static string GetErrors(Node node) {
            if(node is AssemblyNode a)
                return a.Errors;
            if(node is TypeNode t)
                return t.Errors;
            return string.Empty;
        }
        public int? GetAssemblyNodeID(Node node, out Node? assemblyNode) {
            assemblyNode = node as AssemblyNode;
            if(assemblyNode != null)
                return assemblyNode.NodeID;
            Assembly? assembly = null;
            if(node is References rs)
                assembly = rs.GetSource();
            if(node is Reference r)
                assembly = r.GetAssembly();
            if(node is Namespaces ns)
                assembly = ns.GetAssembly();
            if(node is TypeNode t)
                assembly = t.GetSource().Assembly;
            if(node is MethodNode m)
                assembly = m.GetSource().DeclaringType.Assembly;
            if(assembly == null)
                return null;
            AssemblyNode aNode;
            if(!assembliesCache.TryGetValue(assembly, out aNode))
                return null;
            assemblyNode = aNode;
            return aNode.NodeID;
        }
    }
}