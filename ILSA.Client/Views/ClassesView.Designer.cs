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
            this.classesTree = new DevExpress.XtraTreeList.TreeList();
            this.colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colNodeID = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.nodeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.mvvmContext = new DevExpress.Utils.MVVM.MVVMContext(this.components);
            this.sidePanel = new DevExpress.XtraEditors.SidePanel();
            this.codeBox = new DevExpress.XtraEditors.MemoEdit();
            ((System.ComponentModel.ISupportInitialize)(this.classesTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nodeBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).BeginInit();
            this.sidePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.codeBox.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // classesTree
            // 
            this.classesTree.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.classesTree.ChildListFieldName = "Nodes";
            this.classesTree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colName,
            this.colNodeID});
            this.classesTree.DataSource = this.nodeBindingSource;
            this.classesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.classesTree.EnableDynamicLoading = false;
            this.classesTree.FixedLineWidth = 1;
            this.classesTree.HorzScrollStep = 1;
            this.classesTree.Location = new System.Drawing.Point(0, 0);
            this.classesTree.MinWidth = 16;
            this.classesTree.Name = "classesTree";
            this.classesTree.OptionsBehavior.Editable = false;
            this.classesTree.OptionsFilter.ExpandNodesOnFiltering = true;
            this.classesTree.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.classesTree.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.classesTree.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
            this.classesTree.OptionsView.RowImagesShowMode = DevExpress.XtraTreeList.RowImagesShowMode.InIndent;
            this.classesTree.Size = new System.Drawing.Size(299, 715);
            this.classesTree.TabIndex = 1;
            this.classesTree.TreeLevelWidth = 12;
            this.classesTree.TreeViewColumn = this.colName;
            this.classesTree.ViewStyle = DevExpress.XtraTreeList.TreeListViewStyle.TreeView;
            this.classesTree.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.GetStateImage);
            this.classesTree.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.OnNodeMenu);
            // 
            // colName
            // 
            this.colName.FieldName = "Name";
            this.colName.Name = "colName";
            this.colName.OptionsColumn.ReadOnly = true;
            this.colName.Visible = true;
            this.colName.VisibleIndex = 0;
            // 
            // colNodeID
            // 
            this.colNodeID.FieldName = "NodeID";
            this.colNodeID.Name = "colNodeID";
            this.colNodeID.OptionsColumn.ReadOnly = true;
            this.colNodeID.Visible = false;
            // 
            // nodeBindingSource
            // 
            this.nodeBindingSource.DataSource = typeof(ILSA.Core.Node);
            // 
            // mvvmContext
            // 
            this.mvvmContext.ContainerControl = this;
            this.mvvmContext.ViewModelType = typeof(ILSA.Client.ViewModels.ClassesViewModel);
            // 
            // sidePanel
            // 
            this.sidePanel.Controls.Add(this.classesTree);
            this.sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePanel.Location = new System.Drawing.Point(0, 0);
            this.sidePanel.MinimumSize = new System.Drawing.Size(110, 0);
            this.sidePanel.Name = "sidePanel";
            this.sidePanel.Size = new System.Drawing.Size(300, 715);
            this.sidePanel.TabIndex = 2;
            // 
            // codeBox
            // 
            this.codeBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeBox.Location = new System.Drawing.Point(300, 0);
            this.codeBox.Name = "codeBox";
            this.codeBox.Properties.Appearance.Font = new System.Drawing.Font("Cascadia Code", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.codeBox.Properties.Appearance.Options.UseFont = true;
            this.codeBox.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.codeBox.Properties.ReadOnly = true;
            this.codeBox.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.codeBox.Properties.WordWrap = false;
            this.codeBox.Size = new System.Drawing.Size(898, 715);
            this.codeBox.TabIndex = 3;
            this.codeBox.CustomHighlightText += new DevExpress.XtraEditors.TextEditCustomHighlightTextEventHandler(this.OnCodeBoxCustomHighlightText);
            this.codeBox.PaintEx += new DevExpress.XtraEditors.TextEditPaintExEventHandler(this.OnCodeBoxNoCodePaint);
            // 
            // ClassesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.codeBox);
            this.Controls.Add(this.sidePanel);
            this.Name = "ClassesView";
            this.Size = new System.Drawing.Size(1198, 715);
            ((System.ComponentModel.ISupportInitialize)(this.classesTree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nodeBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).EndInit();
            this.sidePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.codeBox.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList classesTree;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colNodeID;
        private System.Windows.Forms.BindingSource nodeBindingSource;
        private DevExpress.Utils.MVVM.MVVMContext mvvmContext;
        private DevExpress.XtraEditors.SidePanel sidePanel;
        private DevExpress.XtraEditors.MemoEdit codeBox;
    }
}
