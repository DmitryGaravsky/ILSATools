namespace ILSA.Core.Patterns {
    using System;

    partial class PatternsFactory {
        public enum NodeType {
            None,
            Assembly,
            Namespace,
            Pattern,
            PatternInformational,
            PatternWarning,
            PatternError,
        }
        //
        abstract class Node<TSource> : Node
            where TSource : class {
            protected readonly IPatternsFactory factory;
            protected readonly TSource source;
            protected Node(IPatternsFactory factory, TSource source) {
                this.factory = factory;
                this.source = source;
                NodeID = Murmur<TSource>.Calc(source, GetType().GetHashCode());
            }
        }
        public static void WithNodeTypes(Action<string, int> action) {
            var nodeTypeValues = Enum.GetValues(typeof(NodeType)) as NodeType[];
            for(int i = 0; i < nodeTypeValues!.Length; i++) {
                string key = nodeTypeValues[i].ToString();
                action(key, (int)nodeTypeValues[i]);
            }
        }
        public static void SetSeverity(Node node, ProcessingSeverity? value) {
            node.Visit(x => {
                if(x is MethodNode m) {
                    var pattern = m.GetPattern();
                    if(value.HasValue)
                        pattern.Severity = value.Value;
                    else pattern.ResetSeverity();
                }
            });
        }
    }
}