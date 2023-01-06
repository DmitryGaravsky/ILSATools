namespace ILSA.Core.Classes {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    partial class ClassesFactory {
        public static Assembly? GetAssembly(Node node) {
            var assemblyNode = node as AssemblyNode;
            return (assemblyNode != null) ? assemblyNode.GetSource() : null;
        }
        sealed class AssemblyNode : Node<Assembly> {
            public AssemblyNode(IClassesFactory factory, Assembly assembly)
                : base(factory, assembly) {
            }
            protected sealed override string GetName() {
                return source.GetName().Name;
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
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                var types = GetTypes(source).OfType<Type>().Select(factory.Create);
                var namespaces = types.GroupBy(x => x.Group)
                    .Select(x => Tuple.Create(x.Key, source, (IEnumerable<Node>)x))
                    .Select(factory.Namespace).ToArray();
                var nodes = new Node[namespaces.Length + 1];
                nodes[0] = factory.References(source);
                for(int i = 0; i < namespaces.Length; i++)
                    nodes[i + 1] = namespaces[i];
                return nodes;
            }
            public sealed override int TypeCode {
                get { return (int)NodeType.Assembly; }
            }
            static Type[] GetTypes(Assembly assembly) {
                try { return assembly.GetTypes(); }
                catch(ReflectionTypeLoadException e) { return e.Types; }
            }
            public Node BackTrace(WorkloadBase.Branch branch) {
                return factory.BackTrace(this, branch);
            }
        }
    }
}