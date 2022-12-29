namespace ILSA.Client.Views {
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using DevExpress.LookAndFeel;
    using DevExpress.Mvvm.Native;
    using DevExpress.Utils;
    using DevExpress.XtraEditors;
    using DevExpress.XtraTreeList;
    using ILSA.Client.ViewModels;
    using ILSA.Core;
    using ILSA.Core.Classes;

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
            NodesFactory.WithNodeTypes((key, value) => svgImages.Add(key, resources[key]));
            classesTree.StateImageList = svgImages;
        }
        protected internal void AttachToSearchControl(SearchControl searchControl) {
            if(searchControl != null) searchControl.Client = classesTree;
        }
        public bool ShowOffset { get; set; }
        public bool ShowBytes { get; set; }
        void InitializeBindings() {
            var fluent = mvvmContext.OfType<ClassesViewModel>();
            fluent.WithEvent<EventArgs>(this, nameof(Load))
                .EventToCommand(x => x.OnLoad);
            fluent.SetBinding(classesTree, tl => tl.DataSource, x => x.Nodes);
            fluent.WithEvent<TreeList, FocusedNodeChangedEventArgs>(classesTree, "FocusedNodeChanged")
                .SetBinding(x => x.SelectedNode, args => classesTree.GetDataRecordByNode(args.Node) as Node,
                    (tree, entity) => { });
            fluent.SetBinding(this, f => f.ShowOffset, x => x.ShowOffset);
            fluent.SetBinding(this, f => f.ShowBytes, x => x.ShowBytes);
            fluent.SetTrigger(x => x.SelectedMethod, m => codeBox.AppendLines(m, ShowOffset, ShowBytes));
            fluent.WithKey(classesTree, Keys.Delete)
                .KeyToCommand(x => x.Remove);
        }
        void GetStateImage(object sender, GetStateImageEventArgs e) {
            var node = classesTree.GetRow(e.Node.Id) as Node;
            if(node != null) e.NodeImageIndex = (int)node.TypeCode;
        }
        void OnCodeBoxCustomHighlightText(object sender, TextEditCustomHighlightTextEventArgs e) {
            if(e.Text.StartsWith(".", StringComparison.Ordinal))
                e.HighlightRange(0, e.Text.IndexOf(' '), Color.Green);
            if(ShowOffset && e.Text.StartsWith("IL_", StringComparison.OrdinalIgnoreCase)) {
                int endOffset = e.Text.IndexOf(':') + 1;
                e.HighlightRange(0, endOffset, Color.Gray);
                int startOpcode = endOffset + e.Text.Skip(endOffset).IndexOf(x => !char.IsWhiteSpace(x));
                int endOpcode = e.Text.IndexOf(' ', startOpcode);
                if(endOpcode < 0) endOpcode = e.Text.Length;
                e.HighlightRange(startOpcode, endOpcode - startOpcode, Color.Blue);
            }
            int paramIndex = e.Text.IndexOf("(@");
            if(paramIndex >= 0)
                e.HighlightRange(paramIndex, e.Text.IndexOf(')', paramIndex), Color.DarkMagenta);
            if(e.Text.StartsWith("@", StringComparison.Ordinal)) {
                int endLabel = e.Text.IndexOf(' ');
                int startLabelComment = endLabel + e.Text.Skip(endLabel).IndexOf(x => !char.IsWhiteSpace(x));
                e.HighlightRange(startLabelComment, e.Text.Length - startLabelComment, Color.Green);
            }
            int strIndex = e.Text.IndexOf('"');
            if(strIndex >= 0)
                e.HighlightRange(strIndex, e.Text.IndexOf('"', strIndex + 1) - strIndex + 1, Color.DarkRed);
        }
        readonly static StringFormat sf = new StringFormat {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };
        readonly string NoCodeMessage =
            "Either there is no method or abstract method is chosen." + Environment.NewLine +
            "Please choose a method to display.";
        void OnCodeBoxNoCodePaint(object sender, TextEditPaintExEventArgs e) {
            if(string.IsNullOrEmpty(codeBox.Text)) {
                int margin = ScaleDPI.ScaleHorizontal(-40);
                Rectangle client = Rectangle.Inflate(codeBox.ClientRectangle, margin, margin);
                var color = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.GrayText);
                e.Cache.DrawString(NoCodeMessage, codeBox.Font, e.Cache.GetSolidBrush(color), client, sf);
            }
        }
    }
}