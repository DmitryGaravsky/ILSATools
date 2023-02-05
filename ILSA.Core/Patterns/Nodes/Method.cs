namespace ILSA.Core.Patterns {
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    partial class PatternsFactory {
        public static Pattern? GetPattern(Node node) {
            var methodNode = node as MethodNode;
            return (methodNode != null) ? methodNode.GetPattern() : null;
        }
        sealed class MethodNode : Node<MethodInfo> {
            readonly Pattern pattern;
            public MethodNode(IPatternsFactory factory, MethodInfo method, Type argumentType)
                : base(factory, method) {
                if(argumentType == typeof(Type))
                    pattern = new MetadataPattern(method);
                if(argumentType == typeof(ILReader.Readers.IILReader))
                    pattern = new MethodBodyPattern(method);
                pattern = pattern ?? Pattern.Empty;
            }
            public MethodNode(IPatternsFactory factory, MethodBodyPattern pattern)
                : base(factory, pattern.match) {
                this.pattern = pattern;
            }
            public MethodBase GetSource() {
                return source;
            }
            public Pattern GetPattern() {
                return pattern;
            }
            protected override string GetName() {
                return pattern.Name;
            }
            protected override string GetGroup() {
                return pattern.Group;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                return EmptyNodes;
            }
            public sealed override int TypeCode {
                get { return (int)NodeType.Pattern + (int)pattern.Severity; }
            }
            public string GetReadMeLinkUrl() {
                var fileName = pattern.GetReadMeFileName();
                int fileNameStart = fileName.LastIndexOf('.', fileName.Length - 4);
                return "https://github.com/DmitryGaravsky/ILSATools/tree/main/Docs/" + fileName.Substring(fileNameStart > 0 ? fileNameStart + 1 : 0);
            }
        }
    }
}