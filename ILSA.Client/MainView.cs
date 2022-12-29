namespace ILSA.Client {
    using System;
    using System.Drawing;
    using DevExpress.LookAndFeel;
    using DevExpress.Utils.Html;
    using DevExpress.Utils.MVVM.Services;
    using DevExpress.XtraBars.Navigation;
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
            fluent.SetBinding(this, frm => frm.AssembliesWorkload, x => x.AssembliesWorkload);
            fluent.SetBinding(this, frm => frm.PatternsWorkload, x => x.PatternsWorkload);
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
                if(e.ViewType == nameof(PatternsView)) {
                    var patternsView = new PatternsView();
                    patternsView.AttachToSearchControl(searchControl);
                    e.Result = patternsView;
                }
            };
            mvvmContext.RegisterService(viewService);
            navigationFrame.SelectedPageChanged += OnSelectedPageChanged;
        }
        void OnSelectedPageChanged(object sender, SelectedPageChangedEventArgs e) {
            var view = ((NavigationPage)e.Page).Controls[0];
            if(view is ClassesView classesView) 
                classesView.AttachToSearchControl(searchControl);
            if(view is PatternsView patternsView)
                patternsView.AttachToSearchControl(searchControl);
        }
        string assembliesWorkloadCore;
        public string AssembliesWorkload {
            get { return assembliesWorkloadCore; }
            set {
                if(assembliesWorkloadCore == value)
                    return;
                assembliesWorkloadCore = value;
                OnWorkLoadChanged();
            }
        }
        string patternsWorkloadCore;
        public string PatternsWorkload {
            get { return patternsWorkloadCore; }
            set {
                if(patternsWorkloadCore == value)
                    return;
                patternsWorkloadCore = value;
                OnWorkLoadChanged();
            }
        }
        void OnWorkLoadChanged() {
            if(IsHandleCreated) {
                FormPainter.UpdateHtmlTemplate();
                DevExpress.Skins.XtraForm.FormPainter.InvalidateNC(this);
                Update();
            }
        }
        protected override string GetHtmlText(string fieldName, DxHtmlElementBase element) {
            if(fieldName == nameof(AssembliesWorkload))
                return AssembliesWorkload;
            if(fieldName == nameof(PatternsWorkload))
                return PatternsWorkload;
            return base.GetHtmlText(fieldName, element);
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
                Views.TabPane.Register();
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