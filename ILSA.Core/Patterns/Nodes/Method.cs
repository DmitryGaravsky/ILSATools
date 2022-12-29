namespace ILSA.Core.Patterns {
    using System.Collections.Generic;
    using System.Reflection;

    partial class NodesFactory {
        public static Pattern? GetPattern(Node node) {
            var methodNode = node as MethodNode;
            return (methodNode != null) ? methodNode.GetPattern() : null;
        }
        sealed class MethodNode : Node<MethodInfo> {
            readonly Pattern pattern;
            public MethodNode(INodesFactory factory, MethodInfo method)
                : base(factory, method) {
                pattern = new Pattern(method);
            }
            public Pattern GetPattern() {
                return pattern;
            }
            protected override string GetName() {
                return pattern.Name;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                return EmptyNodes;
            }
            public sealed override int TypeCode {
                get { return (int)NodeType.Pattern; }
            }
        }
    }
}