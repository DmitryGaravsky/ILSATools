namespace ILSA.Client.ViewModels {
    using System;
    using System.ComponentModel;
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.POCO;
    using ILSA.Core;

    public class NodesViewModel : IDisposable, ISupportParameter {
        public void Dispose() {
            Messenger.Default.Unregister(this);
        }
        object ISupportParameter.Parameter {
            get { return null; }
            set {
                if(value is WorkloadBase workload)
                    Nodes = new BindingList<Node>(workload.Nodes);
            }
        }
        public virtual BindingList<Node> Nodes {
            get;
            protected set;
        }
        public virtual Node SelectedNode {
            get;
            set;
        }
        protected virtual void OnNodesChanged() {
            SelectedNode = null;
        }
        protected virtual void OnSelectedNodeChanged() {
            this.RaiseCanExecuteChanged(x => x.Remove());
        }
        public virtual bool CanRemove() {
            return SelectedNode != null;
        }
        public void Remove() {
            int index = Nodes.IndexOf(SelectedNode);
            if(Nodes.Remove(SelectedNode))
                OnRemoveComplete(index);
        }
        protected virtual void OnRemoveComplete(int index) {
            SelectedNode = (index < Nodes.Count) ? Nodes[index] : null;
        }
    }
}