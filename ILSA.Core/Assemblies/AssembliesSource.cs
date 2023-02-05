namespace ILSA.Core.Sources {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public sealed partial class AssembliesSourceForClasses : IAssembliesSource {
        public enum IsolationMode { CurrentDomain, SeparateDomain }
        Lazy<AppDomain> appDomain;
        Lazy<Assembly[]> assemblies;
        readonly IsolationMode isolationMode;
        readonly HashSet<string> assembliesFilter = new HashSet<string>();
        readonly HashSet<string> referencePaths = new HashSet<string>();
        public AssembliesSourceForClasses(IsolationMode mode = IsolationMode.CurrentDomain) {
            this.isolationMode = mode;
            appDomain = new Lazy<AppDomain>(CreateDomain);
            assemblies = new Lazy<Assembly[]>(GetAssemblies);
        }
        public void Load(string path) {
            assembliesFilter.Add(Path.GetFileName(path));
            referencePaths.Add(Path.GetDirectoryName(path));
            var command = new LoadCommand(path);
            appDomain.Value.DoCallBack(command.Execute);
            ResetAssemblies();
        }
        public void Reset() {
            if(appDomain.IsValueCreated) {
                var domain = appDomain.Value;
                domain.ReflectionOnlyAssemblyResolve -= OnReflectionOnlyAssemblyResolve;
                if(isolationMode == IsolationMode.SeparateDomain)
                    AppDomain.Unload(domain);
                appDomain = new Lazy<AppDomain>(CreateDomain);
            }
            ResetAssemblies();
        }
        void ResetAssemblies() {
            if(assemblies.IsValueCreated)
                assemblies = new Lazy<Assembly[]>(GetAssemblies);
        }
        Assembly[] GetAssemblies() {
            var assemblies = appDomain.Value.ReflectionOnlyGetAssemblies();
            List<Assembly> filteredAssemblies = new List<Assembly>(assemblies.Length);
            for(int i = 0; i < assemblies.Length; i++) {
                if(assembliesFilter.Contains(Path.GetFileName(assemblies[i].Location)))
                    filteredAssemblies.Add(assemblies[i]);
            }
            return filteredAssemblies.ToArray();
        }
        Assembly[] IAssembliesSource.Assemblies {
            get { return assemblies.Value; }
        }
        AppDomain CreateDomain() {
            var domain = (isolationMode == IsolationMode.CurrentDomain) ?
                AppDomain.CurrentDomain : CreateIsolatedDomain();
            domain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyAssemblyResolve;
            return domain;
        }
        static AppDomain CreateIsolatedDomain() {
            return AppDomain.CreateDomain("ReflectionOnlyLoadContext");
        }
        readonly static NetCoreAssemblyResolver netCoreAssemblyResolver = new NetCoreAssemblyResolver();
        Assembly OnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args) {
            var aqnParser = ParseAssemblyQualifiedName(args.Name);
            string asmFileName = aqnParser.BuildAssemblyShortName() + ".dll";
            foreach(string directory in referencePaths) {
                var refAssembly = LoadAssemblyFrom(asmFileName, directory);
                if(refAssembly != null)
                    return refAssembly;
            }
            var startupDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var assembly = LoadAssemblyFrom(asmFileName, startupDirectory);
            if(assembly != null)
                return assembly;
            assembly = netCoreAssemblyResolver.Load(asmFileName, aqnParser);
            if(assembly != null)
                return assembly;
            try {
                var alreadyLoaded = appDomain.Value.ReflectionOnlyGetAssemblies();
                for(int i = 0; i < alreadyLoaded.Length; i++) {
                    if(Path.GetFileName(alreadyLoaded[i].Location) == asmFileName)
                        return alreadyLoaded[i];
                }
                return Assembly.ReflectionOnlyLoad(args.Name);
            }
            catch { }
            return args.RequestingAssembly;
        }
        static Assembly? LoadAssemblyFrom(string assemblyFileName, string directory) {
            if(Directory.Exists(directory)) {
                var fileName = Path.Combine(directory, assemblyFileName);
                if(File.Exists(fileName)) {
                    try { return Assembly.ReflectionOnlyLoadFrom(fileName); }
                    catch { }
                }
            }
            return null;
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
        sealed class NetCoreAssemblyResolver {
            static readonly string CoreRoot;
            static readonly string WinDesktopRoot;
            static readonly string AspNetRoot;
            static NetCoreAssemblyResolver() {
                var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + Path.DirectorySeparatorChar;
                CoreRoot = Path.Combine(programFiles, "dotnet\\shared\\Microsoft.NETCore.App\\");
                WinDesktopRoot = Path.Combine(programFiles, "dotnet\\shared\\Microsoft.WindowsDesktop.App\\");
                AspNetRoot = Path.Combine(programFiles, "dotnet\\shared\\Microsoft.AspNetCore.App\\");
            }
            public Assembly? Load(string asmFileName, AssemblyQualifiedNameParser aqnParser) {
                var asmVersionShort = aqnParser.GetVersion(2);
                if(!string.IsNullOrEmpty(asmVersionShort)) {
                    string maxCoreVDir = FindMaxVersion(asmVersionShort, CoreRoot);
                    if(!string.IsNullOrEmpty(maxCoreVDir)) {
                        var assembly = LoadAssemblyFrom(asmFileName, maxCoreVDir);
                        if(assembly != null)
                            return assembly;
                    }
                    string maxWinDesktopVDir = FindMaxVersion(asmVersionShort, WinDesktopRoot);
                    if(!string.IsNullOrEmpty(maxWinDesktopVDir)) {
                        var assembly = LoadAssemblyFrom(asmFileName, maxWinDesktopVDir);
                        if(assembly != null)
                            return assembly;
                    }
                    string maxAspNetVDir = FindMaxVersion(asmVersionShort, AspNetRoot);
                    if(!string.IsNullOrEmpty(maxAspNetVDir)) {
                        var assembly = LoadAssemblyFrom(asmFileName, maxAspNetVDir);
                        if(assembly != null)
                            return assembly;
                    }
                }
                return null;
            }
            static string FindMaxVersion(string asmVersion, string root) {
                var versionDirs = Directory.GetDirectories(root, asmVersion + ".*");
                Version max = new Version(asmVersion);
                string maxDir = string.Empty;
                for(int i = 0; i < versionDirs.Length; i++) {
                    var vDir = Path.GetFileName(versionDirs[i]);
                    if(Version.TryParse(vDir, out Version v) && v > max) {
                        max = v; 
                        maxDir = versionDirs[i];
                    }
                }
                return maxDir;
            }
        }
    }
    //
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
                if(!assembly.IsDynamic && assembly.Location == path) {
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