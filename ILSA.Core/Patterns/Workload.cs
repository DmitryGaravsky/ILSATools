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
            protected sealed override void Advance(Node node, WorkloadBase effects) {
                throw new NotImplementedException();
            }
            protected internal sealed override void Apply(Node node, Type type) {
                foreach(var pattern in metadataPatterns) {
                    if(pattern.Match(type, ErrorsBuilder)) 
                        node.OnPatternMatch(pattern);
                }
            }
            protected internal sealed override void Apply(Node node, MethodBase method) {
                if(method.IsAbstract)
                    return;
                var cfg = ILReader.Configuration.Resolve(method);
                var reader = cfg.GetReader(method);
                foreach(var pattern in methodBodyPatterns) {
                    if(pattern.Match(reader, ErrorsBuilder, out int[] captures)) 
                        node.OnPatternMatch(pattern, captures);
                }
            }
            public sealed override string ToString() {
                return $"Patterns: {methods} patterns from {assemblies} assemblies";
            }
        }
    }
}