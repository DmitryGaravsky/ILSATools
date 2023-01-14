namespace ILSA.Core.Classes {
    using System;
    using System.Collections.Generic;

    partial class ClassesFactory {
        sealed class BaseTypes : Node<Type> {
            readonly bool isGroup;
            public BaseTypes(IClassesFactory factory, Type type, bool isRoot)
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
                    if(source.IsValueType) {
                        if(source.IsEnum)
                            return source.IsPublic ? (int)NodeType.Enumeration : (int)NodeType.EnumerationPrivate;
                        else
                            return source.IsPublic ? (int)NodeType.ValueType : (int)NodeType.ValueTypePrivate;
                    }
                    if(source.IsInterface)
                        return source.IsPublic ? (int)NodeType.Interface : (int)NodeType.InterfacePrivate;
                    else
                        return source.IsPublic ? (int)NodeType.Class : (int)NodeType.ClassPrivate;
                }
            }
        }
    }
}