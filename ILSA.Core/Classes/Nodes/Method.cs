namespace ILSA.Core.Hierarchy {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    partial class NodesFactory {
        public static MethodBase? GetMethod(Node node) {
            var methodNode = node as MethodNode;
            return (methodNode != null) ? methodNode.GetMethod() : null;
        } 
        sealed class MethodNode : Node<MethodBase> {
            public MethodNode(INodesFactory factory, MethodBase method)
                : base(factory, method) {
            }
            public MethodBase? GetMethod() {
                return !source.IsAbstract ? source : null;
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
                                sb.Append(" : ");
                                sb.Append(TypeToString(mInfo.ReturnType));
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
            public sealed override NodeType Type {
                get {
                    if(source.IsAbstract)
                        return NodeType.MethodAbstract;
                    return source.IsPrivate ? NodeType.MethodPrivate : NodeType.Method;
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
                typeof(int).Namespace,
                typeof(System.Collections.ArrayList).Namespace,
                typeof(List<>).Namespace,
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