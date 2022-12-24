namespace ILSA.Client.Views {
    using System;
    using System.Windows.Forms;
    using DevExpress.XtraTreeList;
    using ILSA.Client.ViewModels;
    using ILSA.Core.Hierarchy;

    public partial class ClassesView : UserControl {
        public ClassesView() {
            InitializeComponent();
            if(!mvvmContext.IsDesignMode) {
                InitializeBindings();
            }
        }
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            classesTree.ExpandToLevel(2);
        }
        void InitializeBindings() {
            var fluent = mvvmContext.OfType<ClassesViewModel>();
            fluent.WithEvent<EventArgs>(this, nameof(Load))
                .EventToCommand(x => x.OnLoad);
            fluent.SetBinding(classesTree, tl => tl.DataSource, x => x.Nodes);
        }
        void GetStateImage(object sender, GetStateImageEventArgs e) {
            var node = classesTree.GetRow(e.Node.Id) as Node;
            if(node != null) e.NodeImageIndex = (int)node.Type;
        }
    }
}