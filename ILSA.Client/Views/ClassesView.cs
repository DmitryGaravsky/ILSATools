namespace ILSA.Client.Views {
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using DevExpress.LookAndFeel;
    using DevExpress.Mvvm.Native;
    using DevExpress.Utils;
    using DevExpress.Utils.Svg;
    using DevExpress.XtraEditors;
    using DevExpress.XtraTreeList;
    using ILSA.Client.ViewModels;
    using ILSA.Core;
    using ILSA.Core.Classes;
    using ILSA.Core.Patterns;

    public partial class ClassesView : XtraUserControl {
        public ClassesView() {
            InitializeComponent();
            if(!mvvmContext.IsDesignMode) {
                InitializeStyles();
                InitializeBindings();
            }
        }
        void InitializeStyles() {
            var resources = CoreSvgImages.SvgImages;
            SvgImageCollection svgImages = new SvgImageCollection(components);
            ClassesFactory.WithNodeTypes((key, value) => svgImages.Add(key, resources[key]));
            classesTree.StateImageList = svgImages;
        }
        protected internal void AttachToSearchControl(SearchControl searchControl) {
            if(searchControl != null) searchControl.Client = classesTree;
        }
        void InitializeBindings() {
            var fluent = mvvmContext.OfType<ClassesViewModel>();
            fluent.SetBinding(classesTree, tl => tl.DataSource, x => x.Nodes);
            fluent.WithEvent<TreeList, FocusedNodeChangedEventArgs>(classesTree, "FocusedNodeChanged")
                .SetBinding(x => x.SelectedNode, args => classesTree.GetDataRecordByNode(args.Node) as Node,
                    (tree, entity) => { });
            fluent.SetTrigger(x => x.SelectedMethod, codeBox.AppendLines);
            fluent.WithKey(classesTree, Keys.Delete)
                .KeyToCommand(x => x.Remove);
        }
        void GetStateImage(object sender, GetStateImageEventArgs e) {
            var node = classesTree.GetRow(e.Node.Id) as Node;
            if(node != null) e.NodeImageIndex = node.TypeCode;
        }
        void OnCodeBoxCustomHighlightText(object sender, TextEditCustomHighlightTextEventArgs e) {
            if(e.Text.StartsWith(".", StringComparison.Ordinal))
                e.HighlightRange(0, e.Text.IndexOf(' '), Color.Green);
            if(e.Text.StartsWith("IL_", StringComparison.OrdinalIgnoreCase)) {
                int endOffset = e.Text.IndexOf(':', 3) + 1;
                e.HighlightRange(0, endOffset, Color.Gray);
                int startOpcode = endOffset + e.Text.Skip(endOffset).IndexOf(x => !char.IsWhiteSpace(x));
                int endOpcode = e.Text.IndexOf(' ', startOpcode);
                if(endOpcode < 0) endOpcode = e.Text.Length;
                var fluent = mvvmContext.OfType<ClassesViewModel>();
                var severity = fluent.ViewModel.GetSeverity(e.Text.Substring(3, endOffset - 4));
                e.HighlightRange(startOpcode - 1, 1, x => SetSeverity(x, severity));
                e.HighlightRange(startOpcode, endOpcode - startOpcode, Color.Blue);
            }
            int paramIndex = e.Text.IndexOf("(@", StringComparison.Ordinal);
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
        static void SetSeverity(TextEdit.Block block, ProcessingSeverity severity) {
            block.Tag = severity;
            block.ContentSize = new Size(24, 16);
            block.Painter = SeverityPainter.Instance;
            block.AllowNavigation = false;
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
        sealed class SeverityPainter : TextEdit.TextEditBlockPainter {
            public readonly static SeverityPainter Instance = new SeverityPainter();
            //
            readonly static SvgBitmap Error = SvgBitmap.Create(CoreSvgImages.SvgImages[nameof(Error)]);
            readonly static SvgBitmap Warning = SvgBitmap.Create(CoreSvgImages.SvgImages[nameof(Warning)]);
            readonly static SvgBitmap Information = SvgBitmap.Create(CoreSvgImages.SvgImages[nameof(Information)]);
            //
            public sealed override bool DrawBackground(TextEdit.Block block) {
                if(!(block.Tag is ProcessingSeverity))
                    return true;
                var blockBounds = block.Segments[0].Bounds;
                var imgBounds = PlacementHelper.Arrange(new Size(blockBounds.Height, blockBounds.Height), blockBounds, ContentAlignment.MiddleCenter);
                switch((ProcessingSeverity)block.Tag) {
                    case ProcessingSeverity.Error:
                        DrawSvgBitmap(Error, imgBounds);
                        break;
                    case ProcessingSeverity.Warning:
                        DrawSvgBitmap(Warning, imgBounds);
                        break;
                    case ProcessingSeverity.Informational:
                        DrawSvgBitmap(Information, imgBounds);
                        break;
                }
                return true;
            }
        }
    }
}