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
            this.rootContainer = new DevExpress.XtraEditors.DirectXFormContainerControl();
            this.navigationFrame = new DevExpress.XtraBars.Navigation.NavigationFrame();
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.appDataContext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbar)).BeginInit();
            this.toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchControl.Properties)).BeginInit();
            this.rootContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navigationFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // mvvmContext
            // 
            this.mvvmContext.ContainerControl = this;
            this.mvvmContext.ViewModelType = typeof(ILSA.Client.ViewModels.AppViewModel);
            // 
            // appDataContext
            // 
            this.appDataContext.DataSource = typeof(ILSA.Client.ViewModels.AppViewModel);
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
            // 
            // searchControl
            // 
            this.searchControl.Location = new System.Drawing.Point(20, 10);
            this.searchControl.Name = "searchControl";
            this.searchControl.Properties.AllowFocused = false;
            this.searchControl.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.searchControl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Repository.ClearButton()});
            this.searchControl.Properties.NullValuePrompt = " ";
            this.searchControl.Properties.ShowSearchButton = false;
            this.searchControl.Size = new System.Drawing.Size(201, 18);
            this.searchControl.TabIndex = 0;
            this.searchControl.GotFocus += new System.EventHandler(OnSearchControlGotFocus);
            this.searchControl.LostFocus += new System.EventHandler(OnSearchControlLostFocus);
            // 
            // rootContainer
            // 
            this.rootContainer.Controls.Add(this.navigationFrame);
            this.rootContainer.Controls.Add(this.toolbar);
            this.rootContainer.Location = new System.Drawing.Point(1, 31);
            this.rootContainer.Name = "rootContainer";
            this.rootContainer.Size = new System.Drawing.Size(1198, 768);
            this.rootContainer.TabIndex = 1;
            // 
            // navigationFrame
            // 
            this.navigationFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationFrame.Location = new System.Drawing.Point(0, 53);
            this.navigationFrame.Name = "navigationFrame";
            this.navigationFrame.SelectedPage = null;
            this.navigationFrame.Size = new System.Drawing.Size(1198, 715);
            this.navigationFrame.TabIndex = 1;
            this.navigationFrame.Text = "navigationFrame1";
            this.navigationFrame.SelectedPageChanged += new DevExpress.XtraBars.Navigation.SelectedPageChangedEventHandler(OnSelectedPageChanged);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ChildControls.Add(this.rootContainer);
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1000, 650);
            this.Name = "MainView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "{Title}";
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.appDataContext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbar)).EndInit();
            this.toolbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.searchControl.Properties)).EndInit();
            this.rootContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navigationFrame)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private DevExpress.Utils.MVVM.MVVMContext mvvmContext;
        private System.Windows.Forms.BindingSource appDataContext;
        private DevExpress.XtraEditors.DirectXFormContainerControl rootContainer;
        private DevExpress.XtraEditors.HtmlContentControl toolbar;
        private DevExpress.XtraEditors.SearchControl searchControl;
        private DevExpress.XtraBars.Navigation.NavigationFrame navigationFrame;
    }
}