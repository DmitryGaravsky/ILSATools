namespace ILSA.Client.Views {
    partial class ClassesView {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClassesView));
            this.classesTree = new DevExpress.XtraTreeList.TreeList();
            this.colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.nodeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.mvvmContext = new DevExpress.Utils.MVVM.MVVMContext(this.components);
            this.svgImages = new DevExpress.Utils.SvgImageCollection(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.classesTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nodeBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgImages)).BeginInit();
            this.SuspendLayout();
            // 
            // classesTree
            // 
            this.classesTree.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.classesTree.ChildListFieldName = "Nodes";
            this.classesTree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colName});
            this.classesTree.DataSource = this.nodeBindingSource;
            this.classesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.classesTree.FixedLineWidth = 1;
            this.classesTree.HorzScrollStep = 1;
            this.classesTree.HtmlImages = this.svgImages;
            this.classesTree.Location = new System.Drawing.Point(0, 0);
            this.classesTree.MinWidth = 16;
            this.classesTree.Name = "classesTree";
            this.classesTree.OptionsBehavior.Editable = false;
            this.classesTree.OptionsFilter.ExpandNodesOnFiltering = true;
            this.classesTree.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.classesTree.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.classesTree.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
            this.classesTree.OptionsView.RowImagesShowMode = DevExpress.XtraTreeList.RowImagesShowMode.InCell;
            this.classesTree.Size = new System.Drawing.Size(300, 600);
            this.classesTree.StateImageList = this.svgImages;
            this.classesTree.TabIndex = 1;
            this.classesTree.TreeLevelWidth = 12;
            this.classesTree.TreeViewColumn = this.colName;
            this.classesTree.ViewStyle = DevExpress.XtraTreeList.TreeListViewStyle.TreeView;
            this.classesTree.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.GetStateImage);
            // 
            // colName
            // 
            this.colName.FieldName = "Name";
            this.colName.MinWidth = 16;
            this.colName.Name = "colName";
            this.colName.OptionsColumn.ReadOnly = true;
            this.colName.Visible = true;
            this.colName.VisibleIndex = 0;
            this.colName.Width = 37;
            // 
            // nodeBindingSource
            // 
            this.nodeBindingSource.DataSource = typeof(ILSA.Core.Hierarchy.Node);
            // 
            // mvvmContext
            // 
            this.mvvmContext.ContainerControl = this;
            this.mvvmContext.ViewModelType = typeof(ILSA.Client.ViewModels.ClassesViewModel);
            // 
            // svgImages
            // 
            this.svgImages.Add("None", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.None"))));
            this.svgImages.Add("Assembly", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.Assembly"))));
            this.svgImages.Add("References", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.References"))));
            this.svgImages.Add("Reference", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.Reference"))));
            this.svgImages.Add("Namespace", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.Namespace"))));
            this.svgImages.Add("Type", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.Type"))));
            this.svgImages.Add("BaseTypes", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.BaseTypes"))));
            this.svgImages.Add("Interface", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.Interface"))));
            this.svgImages.Add("Class", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.Class"))));
            this.svgImages.Add("ValueType", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.ValueType"))));
            this.svgImages.Add("Enumeration", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.Enumeration"))));
            this.svgImages.Add("Method", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.Method"))));
            this.svgImages.Add("MethodPrivate", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImages.MethodPrivate"))));
            // 
            // ClassesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.classesTree);
            this.Name = "ClassesView";
            this.Size = new System.Drawing.Size(300, 600);
            ((System.ComponentModel.ISupportInitialize)(this.classesTree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nodeBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgImages)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList classesTree;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colName;
        private System.Windows.Forms.BindingSource nodeBindingSource;
        private DevExpress.Utils.MVVM.MVVMContext mvvmContext;
        private DevExpress.Utils.SvgImageCollection svgImages;
    }
}
