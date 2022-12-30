namespace ILSA.Client.ViewModels {
    using System.ComponentModel;
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.POCO;
    using ILSA.Core;
    using ILSA.Core.Patterns;
    using ILSA.Core.Sources;

    public class PatternsViewModel : NodesViewModel {
        public PatternsViewModel() {
            Messenger.Default.Register<IAssembliesSource>(this, "patterns", OnReloadAssemblies);
        }
        async void OnReloadAssemblies(IAssembliesSource source) {
            var prevNodes = Nodes;
            try {
                var factory = this.GetService<IPatternsFactory>();
                var workload = await PatternsFactory.Workload.LoadAsync(source, factory);
                Nodes = new BindingList<Node>(workload.Nodes);
                Messenger.Default.Send(workload);
            }
            catch { Nodes = prevNodes; }
        }
        public virtual Pattern SelectedPattern {
            get;
            protected set;
        }
        protected override void OnNodesChanged() {
            SelectedPattern = null;
            base.OnNodesChanged();
        }
        protected override void OnSelectedNodeChanged() {
            SelectedPattern = PatternsFactory.GetPattern(SelectedNode);
            base.OnSelectedNodeChanged();
        }
        public override bool CanRemove() {
            return base.CanRemove() && PatternsFactory.GetAssembly(SelectedNode) != null;
        }
    }
}