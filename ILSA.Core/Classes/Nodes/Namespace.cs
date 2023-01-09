namespace ILSA.Core.Classes {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    partial class ClassesFactory {
        sealed class Namespace : Node<string> {
            readonly IEnumerable<Node> types;
            readonly Assembly assembly;
            public Namespace(IClassesFactory factory, Tuple<string, Assembly, IEnumerable<Node>> types)
                : base(factory, types.Item1) {
                this.assembly = types.Item2;
                this.types = types.Item3;
            }
            internal Assembly GetAssembly() {
                return assembly;
            }
            protected sealed override string GetName() {
                return source;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                var typeNodes = types.OfType<TypeNode>().Where(x => !x.IsNested).ToArray();
                Array.Sort(typeNodes, NodeNamesComparer.Default);
                return typeNodes;
            }
            public sealed override int TypeCode {
                get { return (int)NodeType.Namespace; }
            }
            readonly StringBuilder ErrorsBuilder = new StringBuilder();
            protected internal sealed override StringBuilder GetErrorsBuilder() {
                return ErrorsBuilder;
            }
            string? errors;
            [Display(AutoGenerateField = false)]
            public string Errors {
                get { return errors ?? (errors = ErrorsBuilder.ToString()); }
            }
            protected sealed override void OnVisited() {
                Action<Node> collectErrors = x => CollectTypeErrors(ErrorsBuilder, x);
                foreach(var child in Nodes)
                    child.Visit(collectErrors);
            }
            static void CollectTypeErrors(StringBuilder errors, Node node) {
                if(node is TypeNode t) {
                    if(t.HasErrors) {
                        var typeName = t.GetSource().FullName;
                        errors.Append("# ").AppendLine(typeName).Append(t.Errors);
                    }
                }
            }
            protected internal sealed override void Reset() {
                errors = null;
                ErrorsBuilder.Clear();
            }
        }
    }
}