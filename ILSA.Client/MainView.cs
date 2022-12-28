namespace ILSA.Client {
    using System;
    using System.Drawing;
    using DevExpress.LookAndFeel;
    using DevExpress.Utils.Html;
    using DevExpress.Utils.MVVM;
    using DevExpress.Utils.MVVM.Services;
    using ILSA.Client.ViewModels;
    using ILSA.Client.Views;

    public partial class MainView : DevExpress.XtraEditors.DirectXForm {
        public MainView() {
            InitializeComponent();
            if(!mvvmContext.IsDesignMode) {
                InitializeStyles();
                InitializeBindings();
                InitializeNavigation();
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
            fluent.BindCommandToElement(toolbar, "assemblies-button", x => x.ShowAssemblies);
            fluent.BindCommandToElement(toolbar, "patterns-button", x => x.ShowPatterns);
            fluent.BindCommandToElement(toolbar, "add-assembly", x => x.AddAssembly);
            fluent.BindCommandToElement(toolbar, "run-analysis", x => x.RunAnalysis);
            fluent.BindCommandToElement(toolbar, "reset", x => x.Reset);
        }
        void InitializeNavigation() {
            var viewService = DocumentManagerService.Create(navigationFrame);
            viewService.QueryView += (s, e) => {
                if(e.ViewType == nameof(ClassesView)) {
                    var classesView = new ClassesView();
                    classesView.AttachToSearchControl(searchControl);
                    e.Result = classesView;
                }
            };
            mvvmContext.RegisterService(viewService);
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
            static Styles() {
                TabPane.Register();
            }
            public static Assets.Style App = new AppStyle();
            public static Assets.Style Toolbar = new ToolbarStyle();
            //
            sealed class AppStyle : Assets.Style { }
            sealed class ToolbarStyle : Assets.Style { }
        }
        #endregion Theme
    }
}