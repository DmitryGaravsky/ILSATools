namespace ILSA.Core.Classes {
    using System;

    partial class ClassesFactory {
        public enum NodeType {
            None,
            Assembly,
            References,
            Reference,
            Namespace,
            Type,
            BaseTypes,
            Interface,
            Class,
            ValueType,
            Enumeration,
            Method,
            MethodPrivate,
            MethodAbstract,
        }
        public static void WithNodeTypes(Action<string, int> action) {
            var nodeTypeValues = Enum.GetValues(typeof(NodeType)) as NodeType[];
            for(int i = 0; i < nodeTypeValues!.Length; i++) {
                string key = nodeTypeValues[i].ToString();
                action(key, (int)nodeTypeValues[i]);
            }
        }
        //
        abstract class Node<TSource> : Node {
            protected readonly IClassesFactory factory;
            protected readonly TSource source;
            protected Node(IClassesFactory factory, TSource source) {
                this.factory = factory;
                this.source = source;
                NodeID = Murmur<TSource>.Calc(source);
            }
            internal TSource GetSource() {
                return source;
            }
        }
    }
}