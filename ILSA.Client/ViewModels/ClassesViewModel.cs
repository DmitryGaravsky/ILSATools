namespace ILSA.Client.ViewModels {
    using System.Collections.Generic;
    using System.ComponentModel;
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.POCO;
    using ILReader.Readers;
    using ILSA.Core;
    using ILSA.Core.Classes;
    using ILSA.Core.Patterns;
    using ILSA.Core.Sources;

    public class ClassesViewModel : NodesViewModel {
        readonly static IInstruction[] Empty = new IInstruction[0];
        //
        public ClassesViewModel() {
            Messenger.Default.Register<IAssembliesSource>(this, "classes", OnReloadClasses);
            Messenger.Default.Register<ClassesFactory.Workload>(this, "classes", OnClasses);
            Messenger.Default.Register<ClassesFactory.Workload>(this, "backtrace", OnBackTrace);
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
        void OnClasses(ClassesFactory.Workload workload) {
            var prevNodes = Nodes;
            try {
                Nodes = new BindingList<Node>(workload.Nodes);
            }
            catch { Nodes = prevNodes; }
        }
        void OnBackTrace(ClassesFactory.Workload workload) {
            var prevNodes = Nodes;
            try {
                Nodes = new BindingList<Node>(workload.BackTrace);
            }
            catch { Nodes = prevNodes; }
        }
        public virtual IEnumerable<IInstruction> SelectedMethod {
            get;
            protected set;
        }
        public virtual string SelectedNodeErrors {
            get;
            protected set;
        }
        public virtual Dictionary<string, ProcessingSeverity> SeverityMap {
            get;
            protected set;
        }
        protected internal ProcessingSeverity GetSeverity(string offset) {
            if(SeverityMap == null)
                return ProcessingSeverity.Ignore;
            return SeverityMap.TryGetValue(offset, out ProcessingSeverity severity) ? severity : ProcessingSeverity.Ignore;
        }
        protected override void OnNodesChanged() {
            SelectedMethod = null;
            base.OnNodesChanged();
        }
        protected override void OnSelectedNodeChanged() {
            var method = ClassesFactory.GetMethod(SelectedNode);
            if(method == null || method.IsAbstract) {
                SeverityMap = null;
                SelectedMethod = Empty;
                SelectedNodeErrors = ClassesFactory.GetErrors(SelectedNode);
            }
            else {
                var cfg = ILReader.Configuration.Resolve(method);
                var reader = cfg.GetReader(method);
                SelectedNodeErrors = string.Empty;
                SeverityMap = ClassesFactory.GetSeverityMap(SelectedNode, reader);
                SelectedMethod = reader;
            }
            base.OnSelectedNodeChanged();
        }
        public override bool CanRemove() {
            return base.CanRemove() && ClassesFactory.GetAssembly(SelectedNode) != null;
        }
    }
}