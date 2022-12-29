namespace ILSA.Client.ViewModels {
    using CommandLine;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    sealed class CommandLineOptions {
        [Option('d', "directory", HelpText = "Directory for assemblies to be loaded.")]
        public string Directory { get; set; }
        [Option('a', "assemblies", HelpText = "Assemblies to be loaded.")]
        public IEnumerable<string> Assemblies { get; set; }
        [Option('p', "patterns", HelpText = "Assemblies with patterns to be loaded.")]
        public IEnumerable<string> Patterns { get; set; }
        //
        public bool IsEmpty {
            get {
                if(Assemblies != null && Assemblies.Any())
                    return false;
                if(Patterns != null && Patterns.Any())
                    return false;
                return string.IsNullOrEmpty(Directory);
            }
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
            WithAssembliesCore(Assemblies, action);
        }
        public void WithPatterns(Action<string> action) {
            WithAssembliesCore(Patterns, action);
        }
        static void WithAssembliesCore(IEnumerable<string> assemblies, Action<string> action) {
            if(assemblies == null || action == null)
                return;
            foreach(var assemblyOrExecutable in assemblies) {
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