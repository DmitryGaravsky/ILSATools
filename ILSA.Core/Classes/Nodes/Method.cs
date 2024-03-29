﻿namespace ILSA.Core.Classes {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    partial class ClassesFactory {
        public static MethodBase? GetMethod(Node node) {
            if(node is Node<MethodBase> m)
                return m.GetSource();
            return null;
        }
        public static Dictionary<string, ProcessingSeverity>? GetSeverityMap(Node node, IILReader instructions) {
            if(node is MethodNode m)
                return m.GetSeverityMap(instructions);
            if(node is BackTraceMethodNode bm)
                return bm.GetSeverityMap(instructions);
            return null;
        }
        sealed class MethodNode : Node<MethodBase> {
            public MethodNode(IClassesFactory factory, MethodBase method)
                : base(factory, method) {
            }
            protected sealed override string GetName() {
                var sb = new StringBuilder(source.ToString());
                var ns = source.DeclaringType?.Namespace;
                if(!string.IsNullOrEmpty(ns))
                    sb.Replace(ns + ".", string.Empty);
                foreach(var item in typeAliases)
                    sb.Replace(item.Key.FullName, item.Value);
                MethodInfo? mInfo = source as MethodInfo;
                ConstructorInfo? mConstructor = source as ConstructorInfo;
                if(mInfo != null || mConstructor != null) {
                    for(int i = 0; i < sb.Length; i++) {
                        if(char.IsWhiteSpace(sb[i])) {
                            sb.Remove(0, i + 1);
                            if(mInfo != null) {
                                sb.Append(" : ").Append(TypeToString(mInfo.ReturnType));
                            }
                            break;
                        }
                    }
                }
                return sb.ToString();
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                return EmptyNodes;
            }
            protected internal sealed override StringBuilder GetErrorsBuilder() {
                var typeNode = factory.Create(source.DeclaringType);
                return typeNode.GetErrorsBuilder();
            }
            protected internal sealed override bool HasErrors {
                get { return (patternMatches != null) && patternMatches.Count > 0; }
            }
            List<Tuple<Pattern, HashSet<int>>>? patternMatches;
            protected internal sealed override void Reset() {
                patternMatches = null;
            }
            protected internal sealed override void OnPatternMatch(Pattern pattern, int[] captures) {
                patternMatches = patternMatches ?? new List<Tuple<Pattern, HashSet<int>>>();
                patternMatches.Add(Tuple.Create(pattern, new HashSet<int>(captures)));
            }
            internal Dictionary<string, ProcessingSeverity>? GetSeverityMap(IILReader instructions) {
                if(patternMatches == null)
                    return null;
                Dictionary<string, ProcessingSeverity>? map = null;
                foreach(IInstruction instruction in instructions) {
                    foreach(var match in patternMatches) {
                        if(match.Item2.Contains(instruction.Index)) {
                            map = map ?? new Dictionary<string, ProcessingSeverity>();
                            string offset = instruction.Offset.ToString("X4");
                            ProcessingSeverity severity;
                            if(!map.TryGetValue(offset, out severity))
                                map.Add(offset, match.Item1.Severity);
                            else map[offset] = (ProcessingSeverity)Math.Max((int)severity, (int)match.Item1.Severity);
                        }
                    }
                }
                return map;
            }
            public sealed override int TypeCode {
                get {
                    if(source.IsAbstract)
                        return (int)NodeType.MethodAbstract;
                    return source.IsPrivate ? (int)NodeType.MethodPrivate : (int)NodeType.Method;
                }
            }
            readonly static Dictionary<Type, string> typeAliases = new Dictionary<Type, string> {
                { typeof(void), "void" },
                { typeof(object), "object" },
                { typeof(string), "string" },
                { typeof(bool), "bool" },
                { typeof(char), "char" },
                { typeof(byte), "byte" },
                { typeof(int), "int" },
                { typeof(long), "long" },
                { typeof(decimal), "decimal" },
                { typeof(float), "float" },
                { typeof(double), "double" },
            };
            readonly HashSet<string> standardNamespaces = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
                typeof(System.Int32).Namespace,
                typeof(System.Collections.ArrayList).Namespace,
                typeof(System.Collections.Generic.List<>).Namespace,
            };
            string TypeToString(Type type) {
                return typeAliases.TryGetValue(type, out string alias) ? alias : TypeToStringCore(type);
            }
            string TypeToStringCore(Type type) {
                var ns = type.Namespace ?? string.Empty;
                if(standardNamespaces.Contains(ns))
                    return type.Name;
                return ns == source.DeclaringType?.Namespace ? type.Name : type.ToString();
            }
        }
    }
}