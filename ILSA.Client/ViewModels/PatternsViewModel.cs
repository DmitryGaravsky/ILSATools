namespace ILSA.Client.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.POCO;
    using ILSA.Core;
    using ILSA.Core.Loader;
    using ILSA.Core.Patterns;

    public class PatternsViewModel : IDisposable {
        readonly INodesFactory nodesFactory = new NodesFactory();
        public PatternsViewModel() {
            Messenger.Default.Register<IAssembliesSource>(this, "patterns", OnReloadAssemblies);
        }
        public void Dispose() {
            Messenger.Default.Unregister(this);
        }
        async void OnReloadAssemblies(IAssembliesSource source) {
            await DoReload(source, nodesFactory).ConfigureAwait(false);
        }
        async Task DoReload(IAssembliesSource assembliesSource, INodesFactory factory) {
            var prevNodes = Nodes;
            try {
                var assemblies = assembliesSource.Assemblies;
                var nodes = assemblies
                    .Where(x => !x.IsDynamic)
                    .OrderBy(x => x.GetName().Name)
                    .Select(factory.Create).ToList();
                var workload = new NodesFactory.Workload();
                await WorkloadBase.LoadAsync(nodes, workload);
                Nodes = new BindingList<Node>(nodes);
                Messenger.Default.Send(workload);
            }
            catch { Nodes = prevNodes; }
        }
        public virtual BindingList<Node> Nodes {
            get;
            protected set;
        }
        public virtual Node SelectedNode {
            get;
            set;
        }
        public virtual Pattern SelectedPattern {
            get;
            protected set;
        }
        public async Task OnLoad() {
            var source = this.GetService<IAssembliesSource>("patterns");
            await DoReload(source, nodesFactory).ConfigureAwait(false);
        }
        protected void OnNodesChanged() {
            SelectedPattern = null;
            SelectedNode = null;
        }
        protected void OnSelectedNodeChanged() {
            SelectedPattern = NodesFactory.GetPattern(SelectedNode);
            this.RaiseCanExecuteChanged(x => x.Remove());
        }
        public bool CanRemove() {
            return (SelectedNode != null) && NodesFactory.GetAssembly(SelectedNode) != null;
        }
        public void Remove() {
            Nodes.Remove(SelectedNode);
            SelectedNode = null;
        }
    }
}