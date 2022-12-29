namespace ILSA.Core.Patterns {
    using System;

    partial class NodesFactory {
        public enum NodeType {
            None,
            Assembly,
            _2,
            _3,
            _4,
            Type,
            _6,
            _7,
            _8,
            _9,
            _10,
            _11,
            _12,
            _13,
            Pattern
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
                if(key[0] != '_')
                    action(key, (int)nodeTypeValues[i]);
            }
        }
    }
}