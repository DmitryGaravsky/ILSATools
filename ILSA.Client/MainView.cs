namespace ILSA.Client {
    using System;
    using System.Drawing;
    using DevExpress.LookAndFeel;
    using DevExpress.Utils.Html;
    using DevExpress.Utils.MVVM;
    using ILSA.Client.ViewModels;

    public partial class MainView : DevExpress.XtraEditors.DirectXForm {
        public MainView() {
            InitializeComponent();
            if(!mvvmContext.IsDesignMode) {
                InitializeStyles();
                InitializeBindings();
                InitializeRelationship();
            }
        }
        void InitializeStyles() {
            this.IconOptions.SvgImage = ILSAClient.SvgImages["Class"];
            this.HtmlImages = ILSAClient.SvgImages;
            toolbar.HtmlImages = ILSAClient.SvgImages;
            Styles.Toolbar.Apply(toolbar.HtmlTemplate);
        }
        void InitializeBindings() {
            var fluent = mvvmContext.OfType<AppViewModel>();
            fluent.SetObjectDataSourceBinding(appDataContext);
            fluent.WithEvent(this, nameof(HandleCreated))
                .EventToCommand(x => x.OnLoad);
            fluent.SetBinding(this, frm => frm.Text, x => x.Title);
        }
        void InitializeRelationship() {
            var childContext = MVVMContext.FromControl(classesView);
            childContext.ParentViewModel = mvvmContext.GetViewModel<AppViewModel>();
            //classesView.AttachToSearchControl(searchControl);
        }
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            UpdateSearchBoxBackground();
        }
        protected override void OnLookAndFeelChangedCore() {
            base.OnLookAndFeelChangedCore();
            UpdateSearchBoxBackground();
        }
        protected override HtmlTemplate CreateHtmlTemplate() {
            return new HtmlTemplate(Styles.App.Html, Styles.App.Css);
        }
        #region Search Behavior
        void OnToolbarElementClick(object sender, DxHtmlElementMouseEventArgs e) {
            if(e.ElementId == "search-button" || e.ParentHasId("search-button")) {
                this.searchControl.Focus();
                return;
            }
        }
        void OnSearchControlGotFocus(object sender, EventArgs e) {
            this.searchControl.Properties.NullValuePrompt = "Type keywords here...";
        }
        void OnSearchControlLostFocus(object sender, EventArgs e) {
            this.searchControl.Properties.NullValuePrompt = " ";
        }
        void UpdateSearchBoxBackground() {
            this.searchControl.Properties.Appearance.BackColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
        }
        #endregion Search Behavior
        #region Theme
        internal sealed class Styles {
            public static Assets.Style App = new AppStyle();
            public static Assets.Style Toolbar = new ToolbarStyle();
            //
            sealed class AppStyle : Assets.Style { }
            sealed class ToolbarStyle : Assets.Style { }
        }
        #endregion Theme
    }
}