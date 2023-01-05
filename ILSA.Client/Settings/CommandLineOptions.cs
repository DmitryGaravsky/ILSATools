namespace ILSA.Client.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommandLine;

    sealed class CommandLineOptions {
        public bool IsEmpty {
            get {
                if(Assemblies != null && Assemblies.Any())
                    return false;
                if(Patterns != null && Patterns.Any())
                    return false;
                return string.IsNullOrEmpty(Directory);
            }
        }
        //
        [Option('d', "directory", HelpText = "Directory for assemblies to be loaded.")]
        public string Directory { get; set; }
        [Option('a', "assemblies", HelpText = "Assemblies to be loaded.")]
        public IEnumerable<string> Assemblies { get; set; }
        [Option('p', "patterns", HelpText = "Assemblies with patterns to be loaded.")]
        public IEnumerable<string> Patterns { get; set; }
        //
        public void WithDirectory(Action<string> load) {
            Loader.LoadDirectory(Directory, load);
        }
        public void WithAssemblies(Action<string> load) {
            Loader.LoadAssemblies(Assemblies, load);
        }
        public void WithPatterns(Action<string> load) {
            Loader.LoadAssemblies(Patterns, load);
        }
    }
}