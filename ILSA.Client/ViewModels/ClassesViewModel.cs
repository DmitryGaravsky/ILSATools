namespace ILSA.Client.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
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
        void OnReloadAssemblies(IAssembliesSource source) {
            DoReload(source);
            SelectedMethod = null;
            SelectedNode = null;
        }
        public virtual IReadOnlyCollection<Node> Nodes {
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
        public void OnLoad() {
            DoReload(this.GetService<IAssembliesSource>());
        }
        void DoReload(IAssembliesSource assembliesSource) {
            factory = factory ?? new NodesFactory();
            var prevNodes = Nodes;
            try {
                var assemblies = assembliesSource.Assemblies;
                Nodes = new BindingList<Node>(assemblies
                    .OrderBy(x => x.GetName().Name)
                    .Select(factory.Create).ToList());
            }
            catch { Nodes = prevNodes; }
        }
        public bool CanRemove() {
            return SelectedNode != null && NodesFactory.GetAssembly(SelectedNode) != null;
        }
        public void Remove() {
            ((BindingList<Node>)Nodes).Remove(SelectedNode);
        }
    }
}