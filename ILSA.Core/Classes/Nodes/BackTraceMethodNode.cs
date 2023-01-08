namespace ILSA.Core.Classes {
    using System.Collections.Generic;
    using System.Reflection;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    public partial class ClassesFactory {
        abstract class BackTraceMethodNode : Node<MethodBase> {
            protected readonly MethodNode method;
            protected BackTraceMethodNode(IClassesFactory factory, MethodNode method)
                 : base(factory, method.GetSource()) {
                this.method = method;
            }
            protected sealed override string GetName() {
                return method.Name;
            }
            protected sealed override string GetGroup() {
                return method.Group;
            }
            public sealed override int TypeCode {
                get { return method.TypeCode; }
            }
            protected internal virtual Dictionary<string, ProcessingSeverity>? GetSeverityMap(IILReader instructions) {
                return method.GetSeverityMap(instructions);
            }
        }
    }
}