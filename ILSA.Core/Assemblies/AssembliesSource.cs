namespace ILSA.Core.Loader {
    using System;
    using System.IO;
    using System.Reflection;

    public sealed class AssembliesSource : IAssembliesSource {
        readonly Lazy<AppDomain> appDomain;
        Lazy<Assembly[]> assemblies;
        public AssembliesSource() {
            appDomain = new Lazy<AppDomain>(CreateDomain);
            assemblies = new Lazy<Assembly[]>(appDomain.Value.ReflectionOnlyGetAssemblies);
        }
        public void Load(string path) {
            var command = new LoadCommand(path);
            appDomain.Value.DoCallBack(command.Execute);
            ResetAssemblies();
        }
        public void Reset() {
            if(appDomain.IsValueCreated)
                AppDomain.Unload(appDomain.Value);
            ResetAssemblies();
        }
        void ResetAssemblies() {
            if(assemblies.IsValueCreated)
                assemblies = new Lazy<Assembly[]>(appDomain.Value.ReflectionOnlyGetAssemblies);
        }
        Assembly[] IAssembliesSource.Assemblies {
            get { return assemblies.Value; }
        }
        AppDomain CreateDomain() {
            var startupArgs = Environment.GetCommandLineArgs();
            return AppDomain.CreateDomain("ReflectionOnlyLoadContext", null, new AppDomainSetup {
                // TODO BaseDir from args
            });
        }
        [Serializable]
        sealed class LoadCommand {
            readonly string path;
            public LoadCommand(string path) {
                this.path = path;
            }
            public void Execute() {
                if(!File.Exists(path))
                    return;
                try { Assembly.ReflectionOnlyLoadFrom(path); }
                catch { }
            }
        }
    }
}