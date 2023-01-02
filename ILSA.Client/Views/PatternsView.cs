namespace ILSA.Client.Views {
    using System.Windows.Forms;
    using DevExpress.Utils;
    using DevExpress.Utils.Html;
    using DevExpress.XtraEditors;
    using DevExpress.XtraTreeList;
    using ILSA.Client.ViewModels;
    using ILSA.Core;
    using ILSA.Core.Patterns;

    public partial class PatternsView : XtraUserControl {
        public PatternsView() {
            InitializeComponent();
            if(!mvvmContext.IsDesignMode) {
                InitializeStyles();
                InitializeBindings();
            }
        }
        void InitializeStyles() {
            var resources = CoreSvgImages.SvgImages;
            SvgImageCollection svgImages = new SvgImageCollection(components);
            PatternsFactory.WithNodeTypes((key, value) => svgImages.Add(key, resources[key]));
            patternsTree.StateImageList = svgImages;
        }
        void InitializeBindings() {
            var fluent = mvvmContext.OfType<PatternsViewModel>();
            fluent.SetBinding(patternsTree, tl => tl.DataSource, x => x.Nodes);
            fluent.WithEvent<TreeList, FocusedNodeChangedEventArgs>(patternsTree, "FocusedNodeChanged")
                .SetBinding(x => x.SelectedNode, args => patternsTree.GetDataRecordByNode(args.Node) as Node,
                    (tree, entity) => { });
            fluent.WithKey(patternsTree, Keys.Delete)
                .KeyToCommand(x => x.Remove);
            fluent.SetTrigger(x => x.SelectedPattern, p => {
                descriptionBox.MarkdownImageResourcesAssembly = p?.GetAssembly();
                descriptionBox.Markdown = p?.Description ?? string.Empty;
            });
        }
        void OnCopyClick(object sender, DxHtmlElementMouseEventArgs args) {
            this.descriptionBox.PerformClick(args);
        }
        void GetStateImage(object sender, GetStateImageEventArgs e) {
            var node = patternsTree.GetRow(e.Node.Id) as Node;
            if(node != null) e.NodeImageIndex = node.TypeCode;
        }
        protected internal void AttachToSearchControl(SearchControl searchControl) {
            if(searchControl != null) searchControl.Client = patternsTree;
        }
    }
}