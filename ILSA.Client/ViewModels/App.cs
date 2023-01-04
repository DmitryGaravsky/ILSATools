namespace ILSA.Client.ViewModels {
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using CommandLine;
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.DataAnnotations;
    using DevExpress.Mvvm.POCO;
    using ILSA.Core;
    using ILSA.Core.Classes;
    using ILSA.Core.Patterns;
    using ILSA.Core.Sources;

    public class AppViewModel {
        readonly IAssembliesSource classesSource = new AssembliesSourceForClasses();
        readonly IAssembliesSource patternsSource = new AssembliesSourceForPatterns();
        readonly IClassesFactory classesFactory = new ClassesFactory();
        readonly IPatternsFactory patternsFactory = new PatternsFactory();
        public AppViewModel() {
            ServiceContainer.Default.RegisterService("classes", classesSource);
            ServiceContainer.Default.RegisterService("patterns", patternsSource);
            ServiceContainer.Default.RegisterService(classesFactory);
            ServiceContainer.Default.RegisterService(patternsFactory);
            Messenger.Default.Register<ClassesFactory.Workload>(this, OnAssembliesWorkload);
            Messenger.Default.Register<PatternsFactory.Workload>(this, OnPatternsWorkload);
        }
        public void Dispose() {
            SetClassesWorkloadCore(null);
            Messenger.Default.Unregister(this);
            GC.SuppressFinalize(this);
        }
        public string Title {
            get { return "IL Static Analysis Tool"; }
        }
        public string AssembliesWorkload {
            get { return classesWorkload?.ToString() ?? "No assemblies loaded."; }
        }
        public string PatternsWorkload {
            get { return patternsWorkload?.ToString() ?? "No patterns loaded."; }
        }
        IDispatcherService dispatcher;
        public async Task OnLoad() {
            dispatcher = this.GetRequiredService<IDispatcherService>();
            LoadStartupAssemblies();
            await UpdateWorkloads();
            ShowAssemblies();
        }
        async Task UpdateWorkloads() {
            SetClassesWorkloadCore(await ClassesFactory.Workload.LoadAsync(classesSource, classesFactory));
            patternsWorkload = await PatternsFactory.Workload.LoadAsync(patternsSource, patternsFactory);
            this.RaisePropertyChanged(x => x.AssembliesWorkload);
            this.RaisePropertyChanged(x => x.PatternsWorkload);
            this.RaiseCanExecuteChanged(x => x.RunAnalysis());
            this.RaiseCanExecuteChanged(x => x.SaveAssembliesWorkload());
            this.RaiseCanExecuteChanged(x => x.SavePatternsWorkload());
            this.RaiseCanExecuteChanged(x => x.NavigateNext());
            this.RaiseCanExecuteChanged(x => x.NavigatePrevious());
        }
        void LoadStartupAssemblies() {
            var startupArgs = Environment.GetCommandLineArgs();
            if(startupArgs != null && startupArgs.Length > 0) {
                Parser.Default.ParseArguments<CommandLineOptions>(startupArgs)
                    .WithParsed(opt => {
                        var loadAssemblies = new Action<string>(classesSource.Load);
                        if(!opt.IsEmpty) {
                            opt.WithDirectory(loadAssemblies);
                            opt.WithAssemblies(loadAssemblies);
                            opt.WithPatterns(patternsSource.Load);
                        }
                    });
            }
        }
        WorkloadBase classesWorkload;
        void OnAssembliesWorkload(ClassesFactory.Workload workload) {
            SetClassesWorkloadCore(workload);
            this.RaisePropertyChanged(x => x.AssembliesWorkload);
            this.RaiseCanExecuteChanged(x => x.SaveAssembliesWorkload());
            this.RaiseCanExecuteChanged(x => x.RunAnalysis());
            this.RaiseCanExecuteChanged(x => x.NavigateNext());
            this.RaiseCanExecuteChanged(x => x.NavigatePrevious());
        }
        void SetClassesWorkloadCore(ClassesFactory.Workload workload) {
            if(classesWorkload != null)
                classesWorkload.AnalysisProgress -= OnClassesAnalysisProgress;
            this.classesWorkload = workload;
            if(classesWorkload != null)
                classesWorkload.AnalysisProgress += OnClassesAnalysisProgress;
        }
        WorkloadBase patternsWorkload;
        void OnPatternsWorkload(PatternsFactory.Workload workload) {
            this.patternsWorkload = workload;
            this.RaisePropertyChanged(x => x.PatternsWorkload);
            this.RaiseCanExecuteChanged(x => x.SavePatternsWorkload());
            this.RaiseCanExecuteChanged(x => x.RunAnalysis());
            this.RaiseCanExecuteChanged(x => x.NavigateNext());
            this.RaiseCanExecuteChanged(x => x.NavigatePrevious());
        }
        public void AddAssembly() {
            bool isAssemblies = GetIsAssembliesPageActive();
            const string filter = "Assemblies (*.dll)|*.dll|Executable (*.exe)|*.exe";
            var openFile = this.GetService<IOpenFileDialogService>();
            openFile.Title = isAssemblies ? "Open Assembly for analysis" : "Load Patterns library";
            openFile.Multiselect = true;
            openFile.CheckPathExists = true;
            openFile.CheckFileExists = true;
            openFile.Filter = filter;
            if(openFile.ShowDialog()) {
                var aSource = isAssemblies ? classesSource : patternsSource;
                var token = isAssemblies ? "classes" : "patterns";
                foreach(var fileInfo in openFile.Files)
                    aSource.Load(fileInfo.GetFullName());
                Messenger.Default.Send(aSource, token);
            }
        }
        public bool CanRunAnalysis() {
            return
                (classesWorkload != null) && !classesWorkload.IsEmpty &&
                (patternsWorkload != null) && !patternsWorkload.IsEmpty;
        }
        public async Task RunAnalysis() {
            await WorkloadBase.AnalyzeAsync(classesWorkload, patternsWorkload);
            await dispatcher.BeginInvoke(() => {
                this.RaiseCanExecuteChanged(x => x.NavigateNext());
                this.RaiseCanExecuteChanged(x => x.NavigatePrevious());
                AnalysisProgress = 0;
            });
        }
        public virtual int AnalysisProgress {
            get;
            protected set;
        }
        async void OnClassesAnalysisProgress(object sender, ProgressChangedEventArgs e) {
            await dispatcher.BeginInvoke(() => AnalysisProgress = e.ProgressPercentage);
        }
        public async Task Reset() {
            classesSource.Reset();
            patternsSource.Reset();
            LoadStartupAssemblies();
            await UpdateWorkloads();
            Messenger.Default.Send(classesSource, "classes");
            Messenger.Default.Send(patternsSource, "patterns");
        }
        protected IDocumentManagerService DocumentManagerService {
            get { return this.GetService<IDocumentManagerService>(); }
        }
        protected bool GetIsAssembliesPageActive() {
            var active = DocumentManagerService.ActiveDocument;
            return (active == null) || Equals(active.Id, nameof(ShowAssemblies));
        }
        public void ShowAssemblies() {
            var assemblies = DocumentManagerService.FindDocumentByIdOrCreate(nameof(ShowAssemblies), x => {
                var classesViewModel = ViewModelSource.Create<ClassesViewModel>();
                classesViewModel.SetParentViewModel(this);
                ((ISupportParameter)classesViewModel).Parameter = classesWorkload;
                var document = x.CreateDocument("ClassesView", classesViewModel);
                document.Id = nameof(ShowAssemblies);
                return document;
            });
            assemblies.Show();
        }
        public void ShowPatterns() {
            var patterns = DocumentManagerService.FindDocumentByIdOrCreate(nameof(ShowPatterns), x => {
                var patternsViewModel = ViewModelSource.Create<PatternsViewModel>();
                patternsViewModel.SetParentViewModel(this);
                ((ISupportParameter)patternsViewModel).Parameter = patternsWorkload;
                var document = x.CreateDocument("PatternsView", patternsViewModel);
                document.Id = nameof(ShowPatterns);
                return document;
            });
            patterns.Show();
        }
        public bool CanSaveAssembliesWorkload() {
            return (classesWorkload != null) && !classesWorkload.IsEmpty;
        }
        public void SaveAssembliesWorkload() {
            // TODO
        }
        public bool CanSavePatternsWorkload() {
            return (patternsWorkload != null) && !patternsWorkload.IsEmpty;
        }
        public void SavePatternsWorkload() {
            // TODO
        }
        public bool CanNavigate() {
            return (classesWorkload != null) && classesWorkload.CanNavigate;
        }
        [Command(CanExecuteMethodName = nameof(CanNavigate))]
        public void NavigateNext() {
            var classesViewModel = EnsureAssembliesPageIsActive();
            if(classesViewModel != null)
                classesViewModel.SelectedNode = classesWorkload.Next(classesViewModel.SelectedOrFirstNode, classesFactory);
        }
        [Command(CanExecuteMethodName = nameof(CanNavigate))]
        public void NavigatePrevious() {
            var classesViewModel = EnsureAssembliesPageIsActive();
            if(classesViewModel != null)
                classesViewModel.SelectedNode = classesWorkload.Previous(classesViewModel.SelectedOrFirstNode, classesFactory);
        }
        NodesViewModel EnsureAssembliesPageIsActive() {
            if(!GetIsAssembliesPageActive())
                ShowAssemblies();
            return DocumentManagerService.ActiveDocument.Content as NodesViewModel;
        }
    }
}