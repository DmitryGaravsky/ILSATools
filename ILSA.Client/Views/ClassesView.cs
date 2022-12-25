namespace ILSA.Client.Views {
    using System;
    using DevExpress.Utils;
    using DevExpress.XtraEditors;
    using DevExpress.XtraTreeList;
    using ILSA.Client.ViewModels;
    using ILSA.Core.Hierarchy;

    public partial class ClassesView : XtraUserControl {
        public ClassesView() {
            InitializeComponent();
            if(!mvvmContext.IsDesignMode) {
                InitializeStyles();
                InitializeBindings();
            }
        }
        void InitializeStyles() {
            var resources = SvgImageCollection.FromResources("ILSA.Core.Assets.Svg", typeof(INodesFactory).Assembly);
            SvgImageCollection svgImages = new SvgImageCollection(components);
            var nodeTypeValues = typeof(NodeType).GetValues() as NodeType[];
            for(int i = 0; i < nodeTypeValues.Length; i++) {
                string key = nodeTypeValues[i].ToString();
                svgImages.Add(key, resources[key]);
            }
            classesTree.StateImageList = svgImages;
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