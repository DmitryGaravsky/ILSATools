namespace ILSA.Core.Hierarchy {
    using System;
    using System.Collections.Generic;
    using BF = System.Reflection.BindingFlags;

    partial class NodesFactory {
        sealed class TypeNode : Node<Type> {
            public TypeNode(INodesFactory factory, Type type)
                : base(factory, type) {
            }
            protected sealed override string GetName() {
                return source.Name;
            }
            protected override string GetGroup() {
                return source.Namespace ?? string.Empty;
            }
            public bool IsNested {
                get { return source.IsNested; }
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                var flags = BF.Instance | BF.Static | BF.Public | BF.NonPublic | BF.DeclaredOnly;
                var methods = source.GetMethods(flags);
                var ctors = source.GetConstructors(flags);
                var nestedTypes = source.GetNestedTypes(BF.Public | BF.NonPublic);
                var nodes = new List<Node>(nestedTypes.Length + ctors.Length + methods.Length + 1);
                if(source.BaseType != null && source.BaseType != typeof(object))
                    nodes.Insert(0, factory.BaseTypes(source));
                foreach(var n in nestedTypes)
                    nodes.Add(factory.Create(n));
                foreach(var c in ctors)
                    nodes.Add(factory.Create(c));
                foreach(var m in methods)
                    nodes.Add(factory.Create(m));
                return nodes.ToArray();
            }
            public sealed override NodeType Type {
                get {
                    if(source.IsValueType) {
                        if(source.IsEnum)
                            return NodeType.Enumeration;
                        return NodeType.ValueType;
                    }
                    if(source.IsInterface)
                        return NodeType.Interface;
                    return NodeType.Type;
                }
            }
        }
    }
}