namespace ILSA.Client.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.POCO;
    using ILReader.Readers;
    using ILSA.Core;
    using ILSA.Core.Classes;
    using ILSA.Core.Loader;

    public class ClassesViewModel : IDisposable {
        readonly INodesFactory nodesFactory = new NodesFactory();
        public ClassesViewModel() {
            ShowOffset = true;
            Messenger.Default.Register<IAssembliesSource>(this, "assemblies", OnReloadAssemblies);
        }
        public void Dispose() {
            Messenger.Default.Unregister(this);
        }
        async void OnReloadAssemblies(IAssembliesSource source) {
            await DoReload(source, nodesFactory).ConfigureAwait(false);
        }
        public virtual BindingList<Node> Nodes {
            get;
            protected set;
        }
        public virtual Node SelectedNode {
            get;
            set;
        }
        public virtual IEnumerable<IInstruction> SelectedMethod {
            get;
            protected set;
        }
        public virtual bool ShowOffset {
            get;
            protected set;
        }
        public virtual bool ShowBytes {
            get;
            protected set;
        }
        protected void OnNodesChanged() {
            SelectedMethod = null;
            SelectedNode = null;
        }
        readonly static IInstruction[] Empty = new IInstruction[0];
        protected void OnSelectedNodeChanged() {
            var method = NodesFactory.GetMethod(SelectedNode);
            if(method == null)
                SelectedMethod = Empty;
            else {
                var cfg = ILReader.Configuration.Resolve(method);
                SelectedMethod = cfg.GetReader(method);
            }
            this.RaiseCanExecuteChanged(x => x.Remove());
        }
        public async Task OnLoad() {
            var source = this.GetService<IAssembliesSource>("assemblies");
            await DoReload(source, nodesFactory).ConfigureAwait(false);
        }
        async Task DoReload(IAssembliesSource assembliesSource, INodesFactory factory) {
            var prevNodes = Nodes;
            try {
                var assemblies = assembliesSource.Assemblies;
                var nodes = assemblies.OrderBy(x => x.GetName().Name)
                    .Select(factory.Create).ToList();
                var workload = new NodesFactory.Workload();
                await WorkloadBase.LoadAsync(nodes, workload);
                Nodes = new BindingList<Node>(nodes);
                Messenger.Default.Send(workload);
            }
            catch { Nodes = prevNodes; }
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