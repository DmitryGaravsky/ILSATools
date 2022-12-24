namespace ILSA.Core.Hierarchy {
    using System;
    using System.Collections.Generic;

    partial class NodesFactory {
        sealed class BaseTypes : Node<Type> {
            readonly bool isGroup;
            public BaseTypes(INodesFactory factory, Type type, bool isRoot)
                : base(factory, type) {
                this.isGroup = isRoot;
            }
            protected sealed override string GetName() {
                return isGroup ? "Base Types" : source.Name;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                var interfaces = source.GetInterfaces();
                var nodes = new List<Node>(interfaces.Length + 1);
                var baseType = source.BaseType;
                if(baseType != null && baseType != typeof(object))
                    nodes.Add(factory.BaseType(baseType));
                for(int i = 0; i < interfaces.Length; i++)
                    nodes.Add(factory.BaseType(interfaces[i]));
                return nodes.ToArray();
            }
            public sealed override NodeType Type {
                get {
                    if(isGroup)
                        return NodeType.BaseTypes;
                    if(source.IsValueType) {
                        if(source.IsEnum)
                            return NodeType.Enumeration;
                        return NodeType.ValueType;
                    }
                    if(source.IsInterface)
                        return NodeType.Interface;
                    return NodeType.Class;
                }
            }
        }
    }
}