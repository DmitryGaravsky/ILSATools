namespace ILSA.Core.Loader {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public sealed class AssembliesSourceForClasses : IAssembliesSource {
        Lazy<AppDomain> appDomain;
        Lazy<Assembly[]> assemblies;
        public AssembliesSourceForClasses() {
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

    public sealed class AssembliesSourceForPatterns : IAssembliesSource {
        readonly HashSet<string> assembliyPaths = new HashSet<string>();
        readonly HashSet<Assembly> assemblies = new HashSet<Assembly>();
        public AssembliesSourceForPatterns() {
            Reset();
        }
        public void Reset() {
            assemblies.Clear();
            assembliyPaths.Clear(); 
            assemblies.Add(typeof(IAssembliesSource).Assembly);
            assembliyPaths.Add(typeof(IAssembliesSource).Assembly.Location);
        }
        Assembly[] IAssembliesSource.Assemblies {
            get { return assemblies.ToArray(); }
        }
        public void Load(string path) {
            if(string.IsNullOrEmpty(path))
                return;
            if(!File.Exists(path))
                return;
            if(assembliyPaths.Contains(path))
                return;
            var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach(var assembly in appDomainAssemblies) {
                if(assembly.Location == path) {
                    assemblies.Add(assembly);
                    assembliyPaths.Add(path);
                    return;
                }
            }
            try {
                var assembly = Assembly.LoadFrom(path);
                assemblies.Add(assembly);
                assembliyPaths.Add(path);
            }
            catch { }
        }
    }
}