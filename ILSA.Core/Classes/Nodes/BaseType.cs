namespace ILSA.Core.Classes {
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
                Type baseType = source.BaseType;
                bool hasBaseTypes = (baseType != null) && (baseType != typeof(object));
                var interfaces = source.GetInterfaces();
                var nodes = new Node[interfaces.Length + (hasBaseTypes ? 1 : 0)];
                int index = 0;
                if(hasBaseTypes)
                    nodes[index++] = factory.BaseType(baseType!);
                for(int i = 0; i < interfaces.Length; i++)
                    nodes[index++] = factory.BaseType(interfaces[i]);
                return nodes;
            }
            public sealed override int TypeCode {
                get {
                    if(isGroup)
                        return (int)NodeType.BaseTypes;
                    if(source.IsValueType)
                        return source.IsEnum ? (int)NodeType.Enumeration : (int)NodeType.ValueType;
                    if(source.IsInterface)
                        return (int)NodeType.Interface;
                    return (int)NodeType.Class;
                }
            }
        }
    }
}