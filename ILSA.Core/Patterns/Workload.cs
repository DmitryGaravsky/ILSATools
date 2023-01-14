namespace ILSA.Core.Patterns {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using ILSA.Core.Classes;
    using ILSA.Core.Sources;

    public partial class PatternsFactory {
        public sealed class Workload : WorkloadBase {
            readonly static StringBuilder CalleeBuilder = new StringBuilder();
            //
            readonly List<MetadataPattern> metadataPatterns = new List<MetadataPattern>();
            readonly List<MethodBodyPattern> methodBodyPatterns = new List<MethodBodyPattern>();
            Workload(List<Node> nodes)
                : base(nodes) {
            }
            public static async Task<Workload> LoadAsync(IAssembliesSource assembliesSource, IPatternsFactory factory) {
                var nodes = assembliesSource.Assemblies
                    .OrderBy(x => x.GetName().Name)
                    .Select(factory.Create).ToList();
                var workload = new Workload(nodes);
                await LoadAsync(workload, workload.Nodes);
                return workload;
            }
            public sealed override bool IsEmpty {
                get { return (metadataPatterns.Count == 0) && (methodBodyPatterns.Count == 0); }
            }
            protected sealed override void Advance(Node node) {
                node.Reset();
                switch(node) {
                    case AssemblyNode a:
                        OnAssembly(a.GetSource());
                        break;
                    case MethodNode m:
                        var pattern = m.GetPattern();
                        if(pattern is MetadataPattern mp)
                            metadataPatterns.Add(mp);
                        if(pattern is MethodBodyPattern mbp)
                            methodBodyPatterns.Add(mbp);
                        OnMethod(m.GetSource());
                        break;
                }
            }
            protected sealed override void Advance(Node node, Branch branch) {
                throw new NotImplementedException();
            }
            protected sealed override Branch[] BeforeAnalyze(WorkloadBase effects) {
                throw new NotImplementedException();
            }
            protected sealed override void EndAnalyze(Branch[] branches) {
                throw new NotImplementedException();
            }
            public sealed override Node Next(Node node, IClassesFactory factory) {
                throw new NotImplementedException();
            }
            public sealed override Node Previous(Node node, IClassesFactory factory) {
                throw new NotImplementedException();
            }
            //
            HashSet<Assembly>? scope = new HashSet<Assembly>();
            ConcurrentDictionary<MethodBase, HashSet<MethodBase>>? callers;
            protected internal sealed override void SetScope(HashSet<Assembly> scope, ConcurrentDictionary<MethodBase, HashSet<MethodBase>> callers) {
                this.scope = scope ?? new HashSet<Assembly>();
                this.callers = callers ?? new ConcurrentDictionary<MethodBase, HashSet<MethodBase>>();
            }
            protected internal sealed override bool Apply(Node node, Type type) {
                var errors = node.GetErrorsBuilder();
                bool hasMatches = false;
                foreach(var pattern in metadataPatterns) {
                    if(hasMatches |= pattern.Match(type, errors))
                        node.OnPatternMatch(pattern);
                }
                return hasMatches;
            }
            protected internal sealed override bool Apply(Node node, MethodBase method) {
                if(method.IsAbstract)
                    return false;
                var cfg = ILReader.Configuration.Resolve(method);
                var reader = cfg.GetReader(method);
                var errors = node.GetErrorsBuilder();
                bool hasMatches = false;
                foreach(var pattern in methodBodyPatterns) {
                    if(hasMatches |= pattern.Match(reader, errors, out int[] captures))
                        node.OnPatternMatch(pattern, captures);
                }
                if(TrackedMembers.Count > 0) {
                    var pattern = MethodBodyPattern.Tracking;
                    if(hasMatches |= pattern.Match(reader, errors, out int[] captures))
                        node.OnPatternMatch(pattern, captures);
                }
                CalleeBuilder.Clear();
                if(MethodBodyPattern.Callee.Match(reader, CalleeBuilder, out int[] calleeCaptures)) {
                    for(int i = 0; i < calleeCaptures.Length; i++) {
                        var callee = reader[calleeCaptures[i]].Operand as MethodBase;
                        MethodBodyPattern.EnsureCallers(method, callee, scope!, callers!);
                    }
                }
                return hasMatches;
            }
            //
            public sealed override string ToString() {
                return $"Patterns: {methods} patterns from {assemblies} assemblies";
            }
        }
    }
}