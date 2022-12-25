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
        public async Task OnLoad() {
            var dispatcher = this.GetService<IDispatcherService>();
            if(assembliesSource == null) {
                assembliesSource = new AssembliesSource();

                await dispatcher.BeginInvoke(() => {

                });
            }
        }
    }
}