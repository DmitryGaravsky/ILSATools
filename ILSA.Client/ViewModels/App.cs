namespace ILSA.Client.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using CommandLine;
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.POCO;
    using ILSA.Core.Loader;

    public class AppViewModel {
        readonly IAssembliesSource assembliesSource = new AssembliesSource();
        public AppViewModel() {
            ServiceContainer.Default.RegisterService(assembliesSource);
        }
        public string Title {
            get { return "IL Static Analysis Client"; }
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
                        var load = new Action<string>(assembliesSource.Load);
                        if(!opt.IsEmpty) {
                            opt.WithDirectory(load);
                            opt.WithAssemblies(load);
                        }
                        else CommandLineOptions.WithDirectory(startupArgs, load);
                    });
            }
        }
        public void AddAssembly() {
            const string filter = "Assemblies (*.dll)|*.dll|Executable (*.exe)|*.exe";
            var openFile = this.GetService<IOpenFileDialogService>();
            openFile.Multiselect = true;
            openFile.CheckPathExists = true;
            openFile.CheckFileExists = true;
            openFile.Filter = filter;
            if(openFile.ShowDialog()) {
                foreach(var fileInfo in openFile.Files)
                    assembliesSource.Load(fileInfo.GetFullName());
                Messenger.Default.Send(assembliesSource);
            }
        }
        public async Task RunAnalysis() {
            await Task.Delay(5000);
        }
        public void Reset() {
            assembliesSource.Reset();
            LoadStartupAssemblies();
            Messenger.Default.Send(assembliesSource);
        }
        protected IDocumentManagerService DocumentManagerService {
            get { return this.GetService<IDocumentManagerService>(); }
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
    }
    sealed class CommandLineOptions {
        [Option('d', "directory", HelpText = "Directory for assemblies to be loaded.")]
        public string Directory { get; set; }
        [Option('a', "assemblies", HelpText = "Assemblies to be loaded.")]
        public IEnumerable<string> Assemblies { get; set; }
        //
        public bool IsEmpty {
            get { return string.IsNullOrEmpty(Directory) && (Assemblies == null || !Assemblies.Any()); }
        }
        public void WithDirectory(Action<string> action) {
            WithDirectoryCore(Directory, action);
        }
        public static void WithDirectory(string[] startArgs, Action<string> action) {
            WithDirectoryCore(Path.GetDirectoryName(startArgs[0]), action);
        }
        static void WithDirectoryCore(string path, Action<string> action) {
            if(string.IsNullOrEmpty(path) || action == null)
                return;
            if(System.IO.Directory.Exists(path)) {
                string[] directoryAssemblies = System.IO.Directory.GetFiles(path, "*.dll");
                foreach(string assembly in directoryAssemblies)
                    action(assembly);
                string[] directoryExecutables = System.IO.Directory.GetFiles(path, "*.exe");
                foreach(string executable in directoryExecutables)
                    action(executable);
            }
        }
        public void WithAssemblies(Action<string> action) {
            if(Assemblies == null || action == null)
                return;
            foreach(var assemblyOrExecutable in Assemblies) {
                if(!File.Exists(assemblyOrExecutable))
                    continue;
                var ext = Path.GetExtension(assemblyOrExecutable);
                if(string.Equals(ext, ".dll", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(ext, ".exe", StringComparison.OrdinalIgnoreCase)) {
                    action(assemblyOrExecutable);
                }
            }
        }
    }
}