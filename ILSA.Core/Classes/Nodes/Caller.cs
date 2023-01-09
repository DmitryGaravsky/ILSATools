namespace ILSA.Core.Classes {
    using System.Collections.Generic;
    using System.Reflection;
    using ILReader.Readers;
    using ILSA.Core.Diagnostics;
    using ILSA.Core.Diagnostics.Security;
    using ILSA.Core.Patterns;

    public partial class ClassesFactory {
        sealed class Caller : BackTraceMethodNode {
            readonly WorkloadBase.Branch branch;
            readonly BackTraceMethodNode callee;
            readonly static HashSet<MethodBase> allCallers = new HashSet<MethodBase>();
            public Caller(IClassesFactory factory, MethodNode method, BackTraceMethodNode callee, WorkloadBase.Branch branch)
                 : base(factory, method) {
                allCallers.Add(source);
                this.callee = callee;
                this.branch = branch;
            }
            protected sealed override IReadOnlyCollection<Node> GetNodes() {
                var callers = branch.GetCallers(source);
                if(callers.Count == 0)
                    return EmptyNodes;
                var nodes = new List<Node>(callers.Count);
                foreach(var caller in callers) {
                    if(allCallers.Contains(caller))
                        continue;
                    var mCaller = factory.Create(caller) as MethodNode;
                    nodes.Add(factory.Caller(mCaller!, this, branch));
                }
                return nodes;
            }
            protected internal sealed override Dictionary<string, ProcessingSeverity>? GetSeverityMap(IILReader instructions) {
                Dictionary<string, ProcessingSeverity>? map = null;
                var mCallee = callee.GetSource();
                foreach(IInstruction instruction in instructions) {
                    if(!Call.IsCallOrIsNewObj(instruction.OpCode))
                        continue;
                    var call = instruction.Operand as MethodBase;
                    if(call != null && Call.IsSameMethod(mCallee, call)) {
                        map = map ?? new Dictionary<string, ProcessingSeverity>();
                        string offset = instruction.Offset.ToString("X4");
                        map.Add(offset, ProcessingSeverity.Callee);
                    }
                }
                return map;
            }
        }
    }
}