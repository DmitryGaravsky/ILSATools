namespace ILSA.Core.Patterns {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    public interface IPatternsFactory {
        Node Create(Assembly assembly);
        Node Create(Tuple<MethodInfo, Type> methodInfo);
        Node Namespaces(Tuple<string, Assembly, IEnumerable<Node>> methods);
    }
    //
    public partial class PatternsFactory : IPatternsFactory {
        public PatternsFactory() {
            createAssemblyNode = x => new AssemblyNode(this, x);
            assembliesCache = new ConcurrentDictionary<Assembly, AssemblyNode>();
            createMethodNode = x => new MethodNode(this, x.Item1, x.Item2);
            methodsCache = new ConcurrentDictionary<Tuple<MethodInfo, Type>, MethodNode>();
        }
        readonly ConcurrentDictionary<Assembly, AssemblyNode> assembliesCache;
        readonly Func<Assembly, AssemblyNode> createAssemblyNode;
        Node IPatternsFactory.Create(Assembly assembly) {
            return assembliesCache.GetOrAdd(assembly, createAssemblyNode);
        }
        readonly ConcurrentDictionary<Tuple<MethodInfo, Type>, MethodNode> methodsCache;
        readonly Func<Tuple<MethodInfo, Type>, MethodNode> createMethodNode;
        Node IPatternsFactory.Create(Tuple<MethodInfo, Type> methodInfo) {
            return methodsCache.GetOrAdd(methodInfo, createMethodNode);
        }
        Node IPatternsFactory.Namespaces(Tuple<string, Assembly, IEnumerable<Node>> methods) {
            return new Namespaces(this, methods);
        }
        public string GetTOC(Node node) {
            var toc = new StringBuilder();
            if(node is AssemblyNode a)
                a.BuildTOC(toc);
            if(node is Namespaces ns) 
                ns.BuildTOC(toc);
            return toc.ToString();
        }
    }
}