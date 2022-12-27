namespace ILSA.Client.ViewModels {
    using System.Threading.Tasks;
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.POCO;
    using ILSA.Core.Loader;

    public class AppViewModel {
        public string Title {
            get { return "IL Static Analysis Client"; }
        }
        IAssembliesSource? assembliesSource;
        public void OnLoad() {
            if(assembliesSource == null) {
                assembliesSource = new AssembliesSource();
                ServiceContainer.Default.RegisterService(assembliesSource);
            }
            ShowAssemblies();
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
}