namespace ILSA.Core.Loader {
    using System;
    using System.IO;
    using System.Reflection;

    public sealed class AssembliesSource : IAssembliesSource {
        Lazy<AppDomain> appDomain;
        Lazy<Assembly[]> assemblies;
        public AssembliesSource() {
            appDomain = new Lazy<AppDomain>(CreateDomain);
            assemblies = new Lazy<Assembly[]>(GetAssemblies);
        }
        public void Load(string path) {
            var command = new LoadCommand(path);
            appDomain.Value.DoCallBack(command.Execute);
            ResetAssemblies();
        }
        public void Reset() {
            if(appDomain.IsValueCreated) {
                appDomain.Value.ReflectionOnlyAssemblyResolve -= OnReflectionOnlyAssemblyResolve;
                AppDomain.Unload(appDomain.Value);
                appDomain = new Lazy<AppDomain>(CreateDomain);
            }
            ResetAssemblies();
        }
        void ResetAssemblies() {
            if(assemblies.IsValueCreated)
                assemblies = new Lazy<Assembly[]>(GetAssemblies);
        }
        Assembly[] GetAssemblies() {
            return appDomain.Value.ReflectionOnlyGetAssemblies();
        }
        Assembly[] IAssembliesSource.Assemblies {
            get { return assemblies.Value; }
        }
        static AppDomain CreateDomain() {
            var appDomain = AppDomain.CreateDomain("ReflectionOnlyLoadContext");
            appDomain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyAssemblyResolve;
            return appDomain;
        }
        static Assembly OnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args) {
            // TODO
            return args.RequestingAssembly;
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