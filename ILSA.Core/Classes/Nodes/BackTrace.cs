namespace ILSA.Core.Classes {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    public partial class ClassesFactory {
        sealed class BackTrace : Node<Assembly> {
            readonly AssemblyNode assembly;
            readonly WorkloadBase.Branch branch;
            public BackTrace(IClassesFactory factory, AssemblyNode assembly, WorkloadBase.Branch branch)
                : base(factory, assembly.GetSource()) {
                this.assembly = assembly;
                this.branch = branch;
            }
            protected sealed override string GetName() {
                return assembly.Name;
            }
            protected sealed override string GetGroup() {
                return assembly.Group;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                return branch.GetMatches();
            }
            public sealed override int TypeCode {
                get { return (int)NodeType.Assembly; }
            }
            [Display(AutoGenerateField = false)]
            public string Errors {
                get { return assembly.Errors; }
            }
        }
    }
}