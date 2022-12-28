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
                int methodsCount = nestedTypes.Length + ctors.Length + methods.Length;
                bool hasBaseTypes = (source.BaseType != null) && (source.BaseType != typeof(object));
                var nodes = new Node[methodsCount + (hasBaseTypes ? 1 : 0)];
                int index = 0;
                if(hasBaseTypes)
                    nodes[index++] = factory.BaseTypes(source);
                foreach(var n in nestedTypes)
                    nodes[index++] = factory.Create(n);
                foreach(var c in ctors)
                    nodes[index++] = factory.Create(c);
                foreach(var m in methods)
                    nodes[index++] = factory.Create(m);
                return nodes;
            }
            public sealed override NodeType Type {
                get {
                    if(source.IsValueType)
                        return source.IsEnum ? NodeType.Enumeration : NodeType.ValueType;
                    if(source.IsInterface)
                        return NodeType.Interface;
                    return NodeType.Type;
                }
            }
        }
    }
}