namespace ILSA.Core.Patterns {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using ILSA.Core.Sources;

    public partial class PatternsFactory {
        public sealed class Workload : WorkloadBase {
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
                await LoadAsync(workload);
                return workload;
            }
            //
            public override bool IsEmpty {
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
            protected sealed override void Advance(Node node, AdvanceContext context) {
                throw new NotImplementedException();
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
                return hasMatches;
            }
            public sealed override string ToString() {
                return $"Patterns: {methods} patterns from {assemblies} assemblies";
            }
        }
    }
}