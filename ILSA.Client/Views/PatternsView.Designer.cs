namespace ILSA.Client.Views {
    partial class PatternsView {
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
            this.patternsTree = new DevExpress.XtraTreeList.TreeList();
            this.colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.nodeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.mvvmContext = new DevExpress.Utils.MVVM.MVVMContext(this.components);
            this.descriptionBox = new MarkdownViewer();
            this.sidePanel = new DevExpress.XtraEditors.SidePanel();
            ((System.ComponentModel.ISupportInitialize)(this.patternsTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nodeBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).BeginInit();
            this.sidePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // patternsTree
            // 
            this.patternsTree.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.patternsTree.ChildListFieldName = "Nodes";
            this.patternsTree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colName});
            this.patternsTree.DataSource = this.nodeBindingSource;
            this.patternsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patternsTree.EnableDynamicLoading = false;
            this.patternsTree.FixedLineWidth = 1;
            this.patternsTree.HorzScrollStep = 1;
            this.patternsTree.Location = new System.Drawing.Point(0, 0);
            this.patternsTree.MinWidth = 16;
            this.patternsTree.Name = "patternsTree";
            this.patternsTree.OptionsBehavior.Editable = false;
            this.patternsTree.OptionsFilter.ExpandNodesOnFiltering = true;
            this.patternsTree.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.patternsTree.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.patternsTree.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
            this.patternsTree.OptionsView.RowImagesShowMode = DevExpress.XtraTreeList.RowImagesShowMode.InIndent;
            this.patternsTree.Size = new System.Drawing.Size(299, 715);
            this.patternsTree.TabIndex = 1;
            this.patternsTree.TreeLevelWidth = 12;
            this.patternsTree.TreeViewColumn = this.colName;
            this.patternsTree.ViewStyle = DevExpress.XtraTreeList.TreeListViewStyle.TreeView;
            this.patternsTree.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.GetStateImage);
            this.patternsTree.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.OnNodeMenu);
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
            this.nodeBindingSource.DataSource = typeof(ILSA.Core.Node);
            // 
            // mvvmContext
            // 
            this.mvvmContext.ContainerControl = this;
            this.mvvmContext.ViewModelType = typeof(ILSA.Client.ViewModels.PatternsViewModel);
            // 
            // sidePanel
            // 
            this.sidePanel.Controls.Add(this.patternsTree);
            this.sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePanel.Location = new System.Drawing.Point(0, 0);
            this.sidePanel.MinimumSize = new System.Drawing.Size(150, 0);
            this.sidePanel.Name = "sidePanel";
            this.sidePanel.Size = new System.Drawing.Size(300, 715);
            this.sidePanel.TabIndex = 2;
            // 
            // descriptionBox
            // 
            this.descriptionBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descriptionBox.Location = new System.Drawing.Point(300, 0);
            this.descriptionBox.Name = "descriptionBox";
            this.descriptionBox.Size = new System.Drawing.Size(898, 715);
            this.descriptionBox.TabIndex = 3;
            // 
            // PatternsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.descriptionBox);
            this.Controls.Add(this.sidePanel);
            this.Name = "PatternsView";
            this.Size = new System.Drawing.Size(1198, 715);
            ((System.ComponentModel.ISupportInitialize)(this.patternsTree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nodeBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).EndInit();
            this.sidePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList patternsTree;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colName;
        private System.Windows.Forms.BindingSource nodeBindingSource;
        private DevExpress.Utils.MVVM.MVVMContext mvvmContext;
        private DevExpress.XtraEditors.SidePanel sidePanel;
        private MarkdownViewer descriptionBox;
    }
}
