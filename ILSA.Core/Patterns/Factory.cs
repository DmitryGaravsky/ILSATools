namespace ILSA.Core.Patterns {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    public interface IPatternsFactory {
        Node Create(Assembly assembly);
        Node Create(Tuple<MethodInfo, Type> methodInfo);
        Node Namespace(Tuple<string, Assembly, IEnumerable<Node>> methods);
    }
    //
    public partial class PatternsFactory : IPatternsFactory {
        public PatternsFactory() {
            createAssemblyNode = x => new AssemblyNode(this, x);
            assembliesCache = new ConcurrentDictionary<Assembly, AssemblyNode>();
            createMethodNode = x => new MethodNode(this, x.Item1, x.Item2);
            methodsCache = new ConcurrentDictionary<Tuple<MethodInfo, Type>, MethodNode>();
        }
        readonly ConcurrentDictionary<Assembly, AssemblyNode> assembliesCache;
        readonly Func<Assembly, AssemblyNode> createAssemblyNode;
        Node IPatternsFactory.Create(Assembly assembly) {
            return assembliesCache.GetOrAdd(assembly, createAssemblyNode);
        }
        readonly ConcurrentDictionary<Tuple<MethodInfo, Type>, MethodNode> methodsCache;
        readonly Func<Tuple<MethodInfo, Type>, MethodNode> createMethodNode;
        Node IPatternsFactory.Create(Tuple<MethodInfo, Type> methodInfo) {
            return methodsCache.GetOrAdd(methodInfo, createMethodNode);
        }
        Node IPatternsFactory.Namespace(Tuple<string, Assembly, IEnumerable<Node>> methods) {
            return new Namespace(this, methods);
        }
        public string GetTOC(Node node) {
            var toc = new StringBuilder();
            if(node is AssemblyNode a)
                a.BuildTOC(toc);
            if(node is Namespace ns)
                ns.BuildTOC(toc);
            return toc.ToString();
        }
        public static Pattern[] GetPatterns(WorkloadBase workload) {
            List<Pattern> patterns = new List<Pattern>();
            foreach(AssemblyNode aNode in workload.Nodes) {
                if(aNode == null)
                    continue;
                aNode.Visit(x => {
                    if(x is MethodNode m)
                        patterns.Add(m.GetPattern());
                });
            }
            return patterns.ToArray();
        }
        internal static readonly HashSet<MemberInfo> TrackedMembers = new HashSet<MemberInfo>();
        public static bool IsTracked(MemberInfo member) {
            return TrackedMembers.Contains(member);
        }
        public static void StartTracking(MemberInfo member) {
            TrackedMembers.Add(member);
            trackedMemberDescriptionCore = null;
        }
        public static void StopTracking(MemberInfo member) {
            TrackedMembers.Remove(member);
            trackedMemberDescriptionCore = null;
        }
        static string? trackedMemberDescriptionCore;
        internal static string TrackedMembersDescription {
            get { return trackedMemberDescriptionCore ?? (trackedMemberDescriptionCore = BuildTrackedMembersDescription()); }
        }
        static string BuildTrackedMembersDescription() {
            var builder = new StringBuilder();
            var tracked = PatternsFactory.TrackedMembers;
            builder.AppendLine("# Dynamically tracked members").AppendLine();
            builder.AppendLine("You can perform interactive analysis via dynamically created subsets of dangerous members.");
            builder.AppendLine();
            builder.AppendLine("### Tracked API");
            builder.AppendLine();
            builder.AppendLine("```");
            if(tracked.Count > 0) {
                foreach(var member in tracked) {
                    if(member is Type t)
                        builder.AppendLine(t.FullName);
                    if(member is MethodBase m)
                        builder.Append(m.DeclaringType.FullName).Append('.').AppendLine(m.Name);
                }
            }
            else {
                builder.AppendLine("There are no tracked types or methods.");
                builder.AppendLine("Use the Classes tree and \"Start Tracking\" action in context menu to add some.");
            }
            builder.AppendLine("```");
            return builder.ToString();
        }
    }
}