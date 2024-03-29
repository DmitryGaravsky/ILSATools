﻿namespace ILSA.Core {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using ILSA.Core.Classes;

    public abstract class WorkloadBase {
        readonly List<Node> nodesCore;
        protected WorkloadBase(List<Node> nodes) {
            this.nodesCore = nodes;
        }
        public List<Node> Nodes {
            get { return nodesCore; }
        }
        protected int assemblies, types, methods;
        readonly List<string> assemblyLocations = new List<string>();
        public virtual bool IsEmpty {
            get { return assemblyLocations.Count == 0; }
        }
        public virtual bool CanNavigate {
            get { return !IsEmpty; }
        }
        protected void OnAssembly(Assembly assembly) {
            if(assembly != null && !assembly.IsDynamic)
                assemblyLocations.Add(assembly.Location);
            Interlocked.Increment(ref assemblies);
        }
        protected void OnType(Type type) {
            Interlocked.Increment(ref types);
        }
        protected void OnMethod(MethodBase method) {
            Interlocked.Increment(ref methods);
        }
        protected static async Task LoadAsync<TWorkload>(TWorkload workload, List<Node> nodes)
            where TWorkload : WorkloadBase {
            Action<Node> advance = workload.Advance;
            Task[] visits = new Task[nodes.Count];
            for(int i = 0; i < visits.Length; i++) {
                var node = nodes[i];
                visits[i] = Task.Run(() => node.Visit(advance));
            }
            await Task.WhenAll(visits);
        }
        public static async Task AnalyzeAsync(WorkloadBase target, WorkloadBase effects) {
            Task[] visits = new Task[target.nodesCore.Count];
            var branches = target.BeforeAnalyze(effects);
            for(int i = 0; i < visits.Length; i++) {
                var node = target.nodesCore[i];
                var branch = branches[i];
                Action<Node> advance = x => target.Advance(x, branch);
                visits[i] = Task.Run(() => node.Visit(advance));
            }
            await Task.WhenAll(visits);
            target.EndAnalyze(branches);
        }
        EventHandler<ProgressChangedEventArgs>? AnalysisProgressCore;
        public event EventHandler<ProgressChangedEventArgs> AnalysisProgress {
            add { AnalysisProgressCore += value; }
            remove { AnalysisProgressCore -= value; }
        }
        protected void RaiseAnalysisProgress(int percent) {
            AnalysisProgressCore?.Invoke(this, new ProgressChangedEventArgs(percent, null));
        }
        protected abstract Branch[] BeforeAnalyze(WorkloadBase effects);
        protected abstract void EndAnalyze(Branch[] branches);
        protected abstract void Advance(Node node);
        protected abstract void Advance(Node node, Branch branch);
        protected internal virtual bool Apply(Node node, Assembly assembly) {
            return false;
        }
        protected internal virtual bool Apply(Node node, Type type) {
            return false;
        }
        protected internal virtual bool Apply(Node node, MethodBase method) {
            return false;
        }
        public abstract Node Next(Node node, IClassesFactory factory);
        public abstract Node Previous(Node node, IClassesFactory factory);
        protected internal abstract void SetScope(HashSet<Assembly> scope, ConcurrentDictionary<MethodBase, HashSet<MethodBase>> callers);
        //
        public sealed class Branch {
            readonly Node root;
            readonly WorkloadBase effects;
            readonly List<Node> nodesInNavigationOrder = new List<Node>();
            readonly Dictionary<Node, int> nodeIdToIndex = new Dictionary<Node, int>();
            readonly HashSet<int> matchedNodeIndices = new HashSet<int>();
            readonly List<Node> matches = new List<Node>();
            readonly ConcurrentDictionary<MethodBase, HashSet<MethodBase>> callers;
            public Branch(Node root, WorkloadBase effects, ConcurrentDictionary<MethodBase, HashSet<MethodBase>> callers) {
                this.root = root;
                this.effects = effects;
                this.callers = callers;
            }
            public void Apply(Node node, Assembly assembly) {
                int index = EnsureNodeIndexAndNavigationOrder(node);
                if(effects.Apply(node, assembly))
                    matchedNodeIndices.Add(index);
            }
            public void Apply(Node node, Type type) {
                int index = EnsureNodeIndexAndNavigationOrder(node);
                if(effects.Apply(node, type)) {
                    matchedNodeIndices.Add(index);
                    matches.Add(node);
                }
            }
            public void Apply(Node node, MethodBase method) {
                int index = EnsureNodeIndexAndNavigationOrder(node);
                if(effects.Apply(node, method)) {
                    matchedNodeIndices.Add(index);
                    matches.Add(node);
                }
            }
            int EnsureNodeIndexAndNavigationOrder(Node node) {
                int index = nodesInNavigationOrder.Count;
                nodeIdToIndex.Add(node, index);
                nodesInNavigationOrder.Add(node);
                return index;
            }
            public int GetID() {
                return root.NodeID;
            }
            public Node GetRoot() {
                return root;
            }
            public IReadOnlyCollection<Node> GetMatches() {
                return matches;
            }
            readonly static HashSet<MethodBase> NoCallers = new HashSet<MethodBase>();
            public HashSet<MethodBase> GetCallers(MethodBase callee) {
                return callers.TryGetValue(callee, out HashSet<MethodBase> c) ? c : NoCallers;
            }
            public bool HasMatches {
                get { return matchedNodeIndices.Count > 0; }
            }
            public Node? NextMatch(Node node) {
                int currentIndex = nodeIdToIndex[node];
                for(int i = currentIndex + 1; i < nodesInNavigationOrder.Count; i++) {
                    if(matchedNodeIndices.Contains(i))
                        return nodesInNavigationOrder[i];
                }
                for(int i = 0; i < currentIndex; i++) {
                    if(matchedNodeIndices.Contains(i))
                        return nodesInNavigationOrder[i];
                }
                return null;
            }
            public Node? PrevMatch(Node node) {
                int currentIndex = nodeIdToIndex[node];
                for(int i = currentIndex - 1; i > 0; i--) {
                    if(matchedNodeIndices.Contains(i))
                        return nodesInNavigationOrder[i];
                }
                for(int i = nodesInNavigationOrder.Count - 1; i > currentIndex; i--) {
                    if(matchedNodeIndices.Contains(i))
                        return nodesInNavigationOrder[i];
                }
                return null;
            }
        }
    }
}