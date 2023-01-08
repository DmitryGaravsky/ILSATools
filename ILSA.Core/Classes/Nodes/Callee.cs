namespace ILSA.Core.Classes {
    using System.Collections.Generic;

    public partial class ClassesFactory {
        sealed class Callee : BackTraceMethodNode {
            readonly WorkloadBase.Branch branch;
            public Callee(IClassesFactory factory, MethodNode method, WorkloadBase.Branch branch)
                 : base(factory, method) {
                this.branch = branch;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                var callers = branch.GetCallers(source);
                if(callers.Count == 0)
                    return EmptyNodes;
                var nodes = new List<Node>(callers.Count);
                foreach(var caller in callers) {
                    var mCaller = factory.Create(caller) as MethodNode;
                    nodes.Add(factory.Caller(mCaller!, this, branch));
                }
                return nodes;
            }
        }
    }
}