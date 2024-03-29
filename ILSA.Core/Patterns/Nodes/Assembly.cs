﻿namespace ILSA.Core.Patterns {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;

    partial class PatternsFactory {
        public static Assembly? GetAssembly(Node node) {
            var assemblyNode = node as AssemblyNode;
            return (assemblyNode != null) ? assemblyNode.GetSource() : null;
        }
        sealed class AssemblyNode : Node<Assembly> {
            public AssemblyNode(IPatternsFactory factory, Assembly assembly)
                : base(factory, assembly) {
            }
            public Assembly GetSource() {
                return source;
            }
            internal void BuildTOC(StringBuilder toc) {
                foreach(Namespace ns in Nodes)
                    ns.BuildTOC(toc);
            }
            protected sealed override string GetName() {
                return source.GetName().Name;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                var types = GetTypes(source).OfType<Type>()
                    .Where(x => x.IsSealed && x.IsAbstract);
                var matchMethods = types
                    .Select(FindMatchMethod)
                    .OfType<Tuple<MethodInfo, Type>>()
                    .Select(factory.Create);
                var namespaces = matchMethods
                    .GroupBy(x => x.Group)
                    .Select(g => Tuple.Create(g.Key, source, (IEnumerable<Node>)g))
                    .Select(factory.Namespace).ToList();
                if(source == typeof(Node).Assembly) {
                    var pTracking = MethodBodyPattern.Tracking;
                    var tMethods = new Node[] { new MethodNode(factory, pTracking) } as IEnumerable<Node>;
                    namespaces.Add(factory.Namespace(Tuple.Create(pTracking.Group, source, tMethods)));
                }
                var nodes = new Node[namespaces.Count];
                for(int i = 0; i < namespaces.Count; i++)
                    nodes[i] = namespaces[i];
                return nodes;
            }
            public sealed override int TypeCode {
                get { return (int)NodeType.Assembly; }
            }
            readonly static Type sbType = typeof(StringBuilder);
            readonly static Type readerType = typeof(IILReader);
            readonly static Type tpType = typeof(Type);
            readonly static Type iaType = typeof(int[]).MakeByRefType();
            static Tuple<MethodInfo, Type>? FindMatchMethod(Type type) {
                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                for(int i = 0; i < methods.Length; i++) {
                    var method = methods[i];
                    if(method.Name != "Match")
                        continue;
                    if(method.ReturnType != typeof(bool))
                        continue;
                    var parameters = method.GetParameters();
                    if(parameters == null || parameters.Length < 2 || parameters.Length > 3)
                        continue;
                    if(parameters[1].ParameterType != sbType)
                        continue;
                    if(parameters.Length == 2) {
                        if(parameters[0].ParameterType != tpType)
                            continue;
                        return Tuple.Create(method, tpType);
                    }
                    if(parameters.Length == 3) {
                        if(parameters[0].ParameterType != readerType)
                            continue;
                        if(parameters[2].ParameterType != iaType)
                            continue;
                        return Tuple.Create(method, readerType);
                    }
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