namespace ILSA.Core.Patterns {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    partial class PatternsFactory {
        sealed class Namespace : Node<string> {
            readonly IEnumerable<Node> methods;
            readonly Assembly assembly;
            public Namespace(IPatternsFactory factory, Tuple<string, Assembly, IEnumerable<Node>> methods)
                : base(factory, methods.Item1) {
                this.assembly = methods.Item2;
                this.methods = methods.Item3;
            }
            protected sealed override string GetName() {
                return source;
            }
            internal void BuildTOC(StringBuilder toc) {
                toc.Append("## ").AppendLine(Name).AppendLine();
                foreach(MethodNode m in Nodes) {
                    var pattern = m.GetPattern();
                    toc.Append("- ")
                      .Append('[').Append(pattern.Name).Append(']')
                      .Append('(').Append(m.GetReadMeLinkUrl()).AppendLine(") ");
                }
                toc.AppendLine();
            }
            internal Assembly GetAssembly() {
                return assembly;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                return methods.ToArray();
            }
            public sealed override int TypeCode {
                get { return (int)NodeType.Namespace; }
            }
        }
    }
}