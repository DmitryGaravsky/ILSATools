namespace ILSA.Client.ViewModels {
    using System.Collections.Generic;
    using System.ComponentModel;
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.POCO;
    using ILReader.Readers;
    using ILSA.Core;
    using ILSA.Core.Classes;
    using ILSA.Core.Sources;

    public class ClassesViewModel : NodesViewModel {
        public ClassesViewModel() {
            ShowOffset = true;
            Messenger.Default.Register<IAssembliesSource>(this, "classes", OnReloadClasses);
        }
        async void OnReloadClasses(IAssembliesSource source) {
            var prevNodes = Nodes;
            try {
                var factory = this.GetService<IClassesFactory>();
                var workload = await ClassesFactory.Workload.LoadAsync(source, factory);
                Nodes = new BindingList<Node>(workload.Nodes);
                Messenger.Default.Send(workload);
            }
            catch { Nodes = prevNodes; }
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
        protected override void OnNodesChanged() {
            SelectedMethod = null;
            base.OnNodesChanged();
        }
        readonly static IInstruction[] Empty = new IInstruction[0];
        protected override void OnSelectedNodeChanged() {
            var method = ClassesFactory.GetMethod(SelectedNode);
            if(method == null || method.IsAbstract)
                SelectedMethod = Empty;
            else {
                var cfg = ILReader.Configuration.Resolve(method);
                SelectedMethod = cfg.GetReader(method);
            }
            base.OnSelectedNodeChanged();
        }
        public override bool CanRemove() {
            return base.CanRemove() && ClassesFactory.GetAssembly(SelectedNode) != null;
        }
    }
}