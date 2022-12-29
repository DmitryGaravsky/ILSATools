namespace ILSA.Client.ViewModels {
    using System;
    using System.Threading.Tasks;
    using CommandLine;
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.POCO;
    using ILSA.Core;
    using ILSA.Core.Loader;

    public class AppViewModel {
        readonly IAssembliesSource assembliesSource = new AssembliesSourceForClasses();
        readonly IAssembliesSource patternsSource = new AssembliesSourceForPatterns();
        public AppViewModel() {
            ServiceContainer.Default.RegisterService("assemblies", assembliesSource);
            ServiceContainer.Default.RegisterService("patterns", patternsSource);
            Messenger.Default.Register<Core.Classes.NodesFactory.Workload>(this, OnAssembliesWorkload);
            Messenger.Default.Register<Core.Patterns.NodesFactory.Workload>(this, OnPatternsWorkload);
        }
        public void Dispose() {
            Messenger.Default.Unregister(this);
        }
        public string Title {
            get { return "IL Static Analysis Client"; }
        }
        public string AssembliesWorkload {
            get { return assembliesWorkload?.ToString() ?? "No assemblies loaded."; }
        }
        public string PatternsWorkload {
            get { return patternsWorkload?.ToString() ?? "No patterns loaded."; }
        }
        public void OnLoad() {
            LoadStartupAssemblies();
            ShowAssemblies();
        }
        void LoadStartupAssemblies() {
            var startupArgs = Environment.GetCommandLineArgs();
            if(startupArgs != null && startupArgs.Length > 0) {
                Parser.Default.ParseArguments<CommandLineOptions>(startupArgs)
                    .WithParsed(opt => {
                        var loadAssemblies = new Action<string>(assembliesSource.Load);
                        if(!opt.IsEmpty) {
                            opt.WithDirectory(loadAssemblies);
                            opt.WithAssemblies(loadAssemblies);
                            opt.WithPatterns(patternsSource.Load);
                        }
                        else CommandLineOptions.WithDirectory(startupArgs, loadAssemblies);
                    });
            }
        }
        WorkloadBase assembliesWorkload;
        void OnAssembliesWorkload(Core.Classes.NodesFactory.Workload workload) {
            this.assembliesWorkload = workload;
            this.RaisePropertyChanged(x => x.AssembliesWorkload);
            this.RaiseCanExecuteChanged(x => x.SaveAssembliesWorkload());
        }
        WorkloadBase patternsWorkload;
        void OnPatternsWorkload(Core.Patterns.NodesFactory.Workload workload) {
            this.patternsWorkload = workload;
            this.RaisePropertyChanged(x => x.PatternsWorkload);
            this.RaiseCanExecuteChanged(x => x.SavePatternsWorkload());
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
                var aSource = isAssemblies ? assembliesSource : patternsSource;
                var token = isAssemblies ? "assemblies" : "patterns";
                foreach(var fileInfo in openFile.Files)
                    aSource.Load(fileInfo.GetFullName());
                Messenger.Default.Send(aSource, token);
            }
        }
        public async Task RunAnalysis() {
            await Task.Delay(5000);
        }
        public void Reset() {
            assembliesSource.Reset();
            patternsSource.Reset();
            LoadStartupAssemblies();
            Messenger.Default.Send(assembliesSource, "assemblies");
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
                var document = x.CreateDocument("ClassesView", null, this);
                document.Id = nameof(ShowAssemblies);
                return document;
            });
            assemblies.Show();
        }
        public void ShowPatterns() {
            var patterns = DocumentManagerService.FindDocumentByIdOrCreate(nameof(ShowPatterns), x => {
                var document = x.CreateDocument("PatternsView", null, this);
                document.Id = nameof(ShowPatterns);
                return document;
            });
            patterns.Show();
        }
        public bool CanSaveAssembliesWorkload() {
            return (assembliesWorkload != null) && !assembliesWorkload.IsEmpty;
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
    }
}