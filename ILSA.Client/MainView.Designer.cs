namespace ILSA.Client {
    partial class MainView {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.mvvmContext = new DevExpress.Utils.MVVM.MVVMContext(this.components);
            this.appDataContext = new System.Windows.Forms.BindingSource(this.components);
            this.toolbar = new DevExpress.XtraEditors.HtmlContentControl();
            this.searchControl = new DevExpress.XtraEditors.SearchControl();
            this.classesView = new Client.Views.ClassesView();
            this.rootContainer = new DevExpress.XtraEditors.DirectXFormContainerControl();
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchControl.Properties)).BeginInit();
            this.rootContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // appDataContext
            // 
            this.appDataContext.DataSource = typeof(ILSA.Client.ViewModels.AppViewModel);
            // 
            // mvvmContext
            // 
            this.mvvmContext.ContainerControl = this;
            this.mvvmContext.ViewModelType = typeof(ILSA.Client.ViewModels.AppViewModel);
            // 
            // toolbar
            // 
            this.toolbar.Controls.Add(this.searchControl);
            this.toolbar.DataContext = this.appDataContext;
            this.toolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolbar.Location = new System.Drawing.Point(0, 0);
            this.toolbar.Margin = new System.Windows.Forms.Padding(0);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(1198, 53);
            this.toolbar.TabIndex = 0;
            this.toolbar.ElementMouseClick += OnToolbarElementClick;
            // 
            // searchControl
            // 
            this.searchControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.searchControl.Location = new System.Drawing.Point(20, 10);
            this.searchControl.Name = "searchControl";
            this.searchControl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Repository.ClearButton()});
            this.searchControl.Properties.AllowFocused = false;
            this.searchControl.Properties.NullValuePrompt = " ";
            this.searchControl.Size = new System.Drawing.Size(201, 20);
            this.searchControl.TabIndex = 0;
            this.searchControl.GotFocus += OnSearchControlGotFocus;
            this.searchControl.LostFocus += OnSearchControlLostFocus;
            // 
            // classesView
            // 
            this.classesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.classesView.Location = new System.Drawing.Point(0, 40);
            this.classesView.Margin = new System.Windows.Forms.Padding(6);
            this.classesView.Name = "classesView";
            this.classesView.Size = new System.Drawing.Size(1198, 728);
            this.classesView.TabIndex = 0;
            // 
            // rootContainer
            // 
            this.rootContainer.Controls.Add(this.classesView);
            this.rootContainer.Controls.Add(this.toolbar);
            this.rootContainer.Location = new System.Drawing.Point(1, 31);
            this.rootContainer.Name = "rootContainer";
            this.rootContainer.Size = new System.Drawing.Size(1198, 768);
            this.rootContainer.TabIndex = 1;
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1198, 800);
            this.MinimumSize = new System.Drawing.Size(1198, 650);
            this.Controls.Add(this.rootContainer);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Name = "MainView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "{Title}";
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchControl.Properties)).EndInit();
            this.rootContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private DevExpress.Utils.MVVM.MVVMContext mvvmContext;
        private System.Windows.Forms.BindingSource appDataContext;
        private DevExpress.XtraEditors.DirectXFormContainerControl rootContainer;
        private DevExpress.XtraEditors.HtmlContentControl toolbar;
        private DevExpress.XtraEditors.SearchControl searchControl;
        private Client.Views.ClassesView classesView;
    }
}