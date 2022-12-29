namespace ILSA.Core.Classes {
    using System;

    partial class NodesFactory {
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
        //
        abstract class Node<TSource> : Node {
            protected readonly INodesFactory factory;
            protected readonly TSource source;
            protected Node(INodesFactory factory, TSource source) {
                this.factory = factory;
                this.source = source;
            }
        }
        public static void WithNodeTypes(Action<string, int> action) {
            var nodeTypeValues = Enum.GetValues(typeof(NodeType)) as NodeType[];
            for(int i = 0; i < nodeTypeValues!.Length; i++) {
                string key = nodeTypeValues[i].ToString();
                action(key, (int)nodeTypeValues[i]);
            }
        }
    }
}