namespace ILSA.Core.Patterns {
    using System;

    partial class PatternsFactory {
        public enum NodeType {
            None,
            Assembly,
            Namespace,
            Pattern
        }
        //
        abstract class Node<TSource> : Node {
            protected readonly IPatternsFactory factory;
            protected readonly TSource source;
            protected Node(IPatternsFactory factory, TSource source) {
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