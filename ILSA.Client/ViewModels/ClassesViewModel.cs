namespace ILSA.Client.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.POCO;
    using ILReader.Readers;
    using ILSA.Core.Hierarchy;
    using ILSA.Core.Loader;

    public class ClassesViewModel : IDisposable {
        public ClassesViewModel() {
            ShowOffset = true;
            Messenger.Default.Register<IAssembliesSource>(this, OnReloadAssemblies);
        }
        public void Dispose() {
            Messenger.Default.Unregister(this);
        }
        async void OnReloadAssemblies(IAssembliesSource source) {
            await DoReload(source).ConfigureAwait(false);
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
        INodesFactory factory;
        public async Task OnLoad() {
            var source = this.GetService<IAssembliesSource>();
            await DoReload(source).ConfigureAwait(false);
        }
        async Task DoReload(IAssembliesSource assembliesSource) {
            factory = factory ?? new NodesFactory();
            var prevNodes = Nodes;
            try {
                var assemblies = assembliesSource.Assemblies;
                var nodes = assemblies.OrderBy(x => x.GetName().Name)
                    .Select(factory.Create).ToList();
                Nodes = new BindingList<Node>(await StartAsyncNodesLoading(nodes));
            }
            catch { Nodes = prevNodes; }
        }
        static int count = 0;
        static async Task<IList<Node>> StartAsyncNodesLoading(IList<Node> nodes) {
            count = 0;
            Action<Node> counter = (x) => Interlocked.Increment(ref count);
            Task[] visits = new Task[nodes.Count];
            for(int i = 0; i < visits.Length; i++) {
                var node = nodes[i];
                visits[i] = Task.Run(() => node.Visit(counter));
            }
            await Task.WhenAll(visits);
            return nodes;
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