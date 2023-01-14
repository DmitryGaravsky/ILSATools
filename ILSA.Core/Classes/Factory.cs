namespace ILSA.Core.Classes {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public interface IClassesFactory {
        Node Create(Assembly assembly);
        Node Create(Tuple<AssemblyName, Assembly> reference);
        Node Create(Type type);
        Node Create(MethodBase method);
        Node BaseTypes(Type type);
        Node BaseType(Type type);
        Node References(Assembly assembly);
        Node Namespace(Tuple<string, Assembly, IEnumerable<Node>> types);
        int? GetRootNodeID(Node node, out Node? navigationNode);
        Node BackTrace(Node assembly, WorkloadBase.Branch branch);
        Node Callee(Node method, WorkloadBase.Branch branch);
        Node Caller(Node method, Node callee, WorkloadBase.Branch branch);
    }
    //
    public partial class ClassesFactory : IClassesFactory {
        static ClassesFactory() {
            ILReader.Configuration.DisableUsingRuntimeHelpersPrepareMethod();
        }
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
            calleeCache = new ConcurrentDictionary<MethodBase, Callee>();
            callersCache = new ConcurrentDictionary<Tuple<MethodBase, MethodBase>, Caller>();
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
        Node IClassesFactory.Namespace(Tuple<string, Assembly, IEnumerable<Node>> types) {
            return new Namespace(this, types);
        }
        Node IClassesFactory.References(Assembly assembly) {
            return new References(this, assembly);
        }
        Node IClassesFactory.BackTrace(Node assembly, WorkloadBase.Branch branch) {
            return new BackTrace(this, (assembly as AssemblyNode)!, branch);
        }
        readonly ConcurrentDictionary<MethodBase, Callee> calleeCache;
        Node IClassesFactory.Callee(Node method, WorkloadBase.Branch branch) {
            var mNode = (method as MethodNode)!;
            return calleeCache.GetOrAdd(mNode.GetSource(), c => new Callee(this, mNode, branch));
        }
        readonly ConcurrentDictionary<Tuple<MethodBase, MethodBase>, Caller> callersCache;
        Node IClassesFactory.Caller(Node method, Node callee, WorkloadBase.Branch branch) {
            var mNode = (method as MethodNode)!;
            var mCalee = (callee as BackTraceMethodNode)!;
            return callersCache.GetOrAdd(Tuple.Create(mNode.GetSource(), mCalee.GetSource()), c => new Caller(this, mNode, mCalee, branch));
        }
        int? IClassesFactory.GetRootNodeID(Node node, out Node? navigationNode) {
            navigationNode = node as AssemblyNode;
            if(navigationNode != null)
                return navigationNode.NodeID;
            Assembly? assembly = null;
            if(node is Node<Assembly> a)
                assembly = a.GetSource();
            if(node is TypeNode t) {
                navigationNode = node;
                assembly = t.GetSource().Assembly;
            }
            if(node is Node<MethodBase> m) {
                navigationNode = node;
                assembly = m.GetSource().DeclaringType.Assembly;
            }
            if(node is Reference r)
                assembly = r.GetAssembly();
            if(node is Namespace ns) {
                navigationNode = ns.Nodes.FirstOrDefault();
                assembly = ns.GetAssembly();
            }
            if(assembly == null || !assembliesCache.TryGetValue(assembly, out AssemblyNode aNode))
                return null;
            navigationNode = navigationNode ?? aNode;
            return aNode.NodeID;
        }
        public static string GetErrors(Node node) {
            if(node is AssemblyNode a)
                return a.Errors;
            if(node is BackTrace b)
                return b.Errors;
            if(node is Namespace ns)
                return ns.Errors;
            if(node is TypeNode t)
                return t.Errors;
            return string.Empty;
        }
        public static string[] GetAssemblies(WorkloadBase workload) {
            List<string> assemblies = new List<string>(workload.Nodes.Count);
            foreach(AssemblyNode aNode in workload.Nodes) {
                if(aNode == null)
                    continue;
                var asm = aNode.GetSource();
                assemblies.Add(asm.Location);
            }
            return assemblies.ToArray();
        }
        public static Node CreateBackTrace(WorkloadBase.Branch branch) {
            var root = branch.GetRoot();
            if(root is AssemblyNode a)
                return a.BackTrace(branch);
            return root;
        }
        public static bool AllowTracking(Node node, out MemberInfo? trackingMember) {
            trackingMember = null;
            if(node is Node<Type> t)
                trackingMember = t.GetSource();
            if(node is Node<MethodBase> m)
                trackingMember = m.GetSource();
            return trackingMember != null;
        }
        //
        static Type[] GetTypes(Assembly assembly) {
            try { return assembly.GetTypes(); }
            catch(ReflectionTypeLoadException e) { return e.Types; }
        }
    }
}