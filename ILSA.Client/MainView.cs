namespace ILSA.Client {
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using DevExpress.LookAndFeel;
    using DevExpress.Utils.Html;
    using DevExpress.Utils.MVVM.Services;
    using DevExpress.XtraBars;
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
            new BarManager(components) {
                DockingEnabled = false,
                Form = this
            };
            this.IconOptions.SvgImage = Assets.Style.SvgImages["Class"];
            this.HtmlImages = Assets.Style.SvgImages;
            toolbar.HtmlImages = Assets.Style.SvgImages;
            Styles.Toolbar.Apply(toolbar.HtmlTemplate);
        }
        void InitializeBindings() {
            var fluent = mvvmContext.OfType<AppViewModel>();
            fluent.SetObjectDataSourceBinding(appDataContext);
            fluent.WithEvent(this, nameof(HandleCreated))
                .EventToCommand(x => x.OnLoad);
            fluent.WithEvent(this, nameof(FormClosed))
                .EventToCommand(x => x.OnClose);
            fluent.SetBinding(this, frm => frm.Text, x => x.Title);
            fluent.SetBinding(this, frm => frm.AssembliesWorkload, x => x.AssembliesWorkload);
            fluent.SetBinding(this, frm => frm.PatternsWorkload, x => x.PatternsWorkload);
            fluent.SetBinding(this, frm => frm.AnalysisProgress, x => x.AnalysisProgress);
            fluent.BindCommandToElement(toolbar, "assemblies-button", x => x.ShowAssemblies);
            fluent.BindCommandToElement(toolbar, "patterns-button", x => x.ShowPatterns);
            fluent.BindCommandToElement(toolbar, "add-assembly", x => x.AddAssembly);
            fluent.BindCommandToElement(toolbar, "run-analysis", x => x.RunAnalysis);
            fluent.BindCommandToElement(toolbar, "reset", x => x.Reset);
            fluent.BindCommandToElement(toolbar, "navigate-prev", x => x.NavigatePrevious);
            fluent.BindCommandToElement(toolbar, "navigate-next", x => x.NavigateNext);
            fluent.BindCommandToElement(toolbar, "classes-button", x => x.ShowClasses);
            fluent.BindCommandToElement(toolbar, "backtrace-button", x => x.ShowBackTrace);
            fluent.BindCommandToElement(this, "assemblies-workload-button", x => x.SaveAssembliesWorkload);
            fluent.BindCommandToElement(this, "patterns-workload-button", x => x.SavePatternsWorkload);
            fluent.BindCommandToElement(this, "load-button", x => x.LoadAssembliesOrPatternsWorkload);
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
                OnFooterLayoutChanged();
            }
        }
        string patternsWorkloadCore;
        public string PatternsWorkload {
            get { return patternsWorkloadCore; }
            set {
                if(patternsWorkloadCore == value)
                    return;
                patternsWorkloadCore = value;
                OnFooterLayoutChanged();
            }
        }
        int analysisProgressCore;
        public int AnalysisProgress {
            get { return analysisProgressCore; }
            set {
                if(analysisProgressCore == value)
                    return;
                analysisProgressCore = value;
                if(FormPainter != null) {
                    var root = ((IDxHtmlClient)FormPainter).Element;
                    var progress = root?.FindElementById("progress");
                    progress?.SetAttribute("value", value);
                }
                OnFooterLayoutChanged();
            }
        }
        void OnFooterLayoutChanged() {
            if(IsHandleCreated) {
                var root = ((IDxHtmlClient)FormPainter).Element;
                root?.LayoutChanged(DevExpress.Utils.Html.Base.DxHtmlLayoutChangeActions.BindingDataChanged);
                FormPainter.UpdateHtmlTemplate();
                DevExpress.Skins.XtraForm.FormPainter.InvalidateNC(this);
                Update();
            }
        }
        protected override object GetHtmlValue(string fieldName, DxHtmlElementBase element) {
            if(fieldName == nameof(AnalysisProgress))
                return AnalysisProgress;
            return base.GetHtmlValue(fieldName, element);
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
        protected override Padding MaximizedContentMargin {
            get { return new Padding(0, 0, 0, 80); }
        }
        protected override HtmlTemplate CreateHtmlTemplate() {
            return new HtmlTemplate(Styles.App.Html, Styles.App.Css);
        }
        #region Search Behavior
        void OnToolbarElementClick(object sender, DxHtmlElementMouseEventArgs e) {
            if(e.ElementId == "search-button" || e.ParentHasId("search-button"))
                this.searchControl.Focus();
        }
        void OnSearchControlGotFocus(object sender, EventArgs e) {
            this.searchControl.Properties.NullValuePrompt = "Type keywords here...";
        }
        void OnSearchControlLostFocus(object sender, EventArgs e) {
            this.searchControl.Properties.NullValuePrompt = " ";
        }
        void UpdateSearchBoxBackground() {
            var controlColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
            this.searchControl.Properties.Appearance.BackColor = controlColor;
        }
        #endregion Search Behavior
        #region Theme
        internal sealed class Styles {
            static Styles() {
                Views.ProgressIndicator.Register();
                Views.ChipGroup.Register();
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