namespace ILSA.Client.Views {
    using System;
    using System.Windows.Forms;
    using DevExpress.Utils;
    using DevExpress.Utils.Html;
    using DevExpress.Utils.Menu;
    using DevExpress.XtraEditors;
    using DevExpress.XtraTreeList;
    using DevExpress.XtraTreeList.Menu;
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
            nodeBindingSource.ListChanged += OnNodeBindingSourceListChanged;
            var fluent = mvvmContext.OfType<PatternsViewModel>();
            fluent.SetBinding(nodeBindingSource, tl => tl.DataSource, x => x.Nodes);
            fluent.WithEvent<TreeList, FocusedNodeChangedEventArgs>(patternsTree, "FocusedNodeChanged")
                .SetBinding(x => x.SelectedNode, args => patternsTree.GetDataRecordByNode(args.Node) as Node,
                    (tree, entity) => { });
            fluent.WithKey(patternsTree, Keys.Delete)
                .KeyToCommand(x => x.Remove);
            fluent.SetTrigger(x => x.SelectedPattern, p => {
                descriptionBox.MarkdownImageResourcesAssembly = p?.GetAssembly();
                descriptionBox.Markdown = p?.Description ?? string.Empty;
            });
            fluent.SetTrigger(x => x.SelectedNodeTOC, toc => descriptionBox.Markdown = toc);
        }
        void OnNodeBindingSourceListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
            if(e.ListChangedType == System.ComponentModel.ListChangedType.Reset)
                patternsTree.ExpandAll();
        }
        void OnCopyClick(object sender, DxHtmlElementMouseEventArgs args) {
            this.descriptionBox.PerformClick(args);
        }
        void GetStateImage(object sender, GetStateImageEventArgs e) {
            var node = patternsTree.GetRow(e.Node.Id) as Node;
            if(node != null) e.NodeImageIndex = node.TypeCode;
        }
        void OnNodeMenu(object sender, PopupMenuShowingEventArgs e) {
            if(e.MenuType == TreeListMenuType.Node) {
                var nodeMenu = e.Menu as TreeListNodeMenu;
                if(nodeMenu != null) {
                    var node = patternsTree.GetDataRecordByNode(nodeMenu.Node) as Node;
                    if(node != null) {
                        e.Menu.Items.Clear();
                        var svg = CoreSvgImages.SvgImages;
                        e.Menu.Items.Add(new DXMenuItem("Ignore", OnSetSeverity,
                            svg[nameof(PatternsFactory.NodeType.Pattern)], DXMenuItemPriority.Normal) {
                            Tag = Tuple.Create(node, (ProcessingSeverity?)ProcessingSeverity.Ignore)
                        });
                        e.Menu.Items.Add(new DXMenuItem("Informational", OnSetSeverity,
                            svg[nameof(PatternsFactory.NodeType.PatternInformational)], DXMenuItemPriority.Normal) {
                            Tag = Tuple.Create(node, (ProcessingSeverity?)ProcessingSeverity.Informational)
                        });
                        e.Menu.Items.Add(new DXMenuItem("Warning", OnSetSeverity,
                            svg[nameof(PatternsFactory.NodeType.PatternWarning)], DXMenuItemPriority.Normal) {
                            Tag = Tuple.Create(node, (ProcessingSeverity?)ProcessingSeverity.Warning)
                        });
                        e.Menu.Items.Add(new DXMenuItem("Error", OnSetSeverity,
                            svg[nameof(PatternsFactory.NodeType.PatternError)], DXMenuItemPriority.Normal) {
                            Tag = Tuple.Create(node, (ProcessingSeverity?)ProcessingSeverity.Error)
                        });
                        e.Menu.Items.Add(new DXMenuItem("Default", OnSetSeverity, null, DXMenuItemPriority.Normal) {
                            Tag = Tuple.Create(node, (ProcessingSeverity?)null),
                            BeginGroup = true
                        });
                    }
                }
            }
        }
        void OnSetSeverity(object sender, EventArgs args) {
            var tag = (sender as DXMenuItem).Tag as Tuple<Node, ProcessingSeverity?>;
            PatternsFactory.SetSeverity(tag.Item1, tag.Item2);
            patternsTree.Refresh(false);
        }
        internal void AttachToSearchControl(SearchControl searchControl) {
            if(searchControl != null) searchControl.Client = patternsTree;
        }
    }
}