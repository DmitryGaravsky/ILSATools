namespace ILSA.Client.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.IO;

    static class Loader {
        public static void LoadDirectory(string path, Action<string> load) {
            if(string.IsNullOrEmpty(path) || load == null)
                return;
            if(Directory.Exists(path)) {
                string[] directoryAssemblies = Directory.GetFiles(path, "*.dll");
                foreach(string assembly in directoryAssemblies) {
                    load(assembly);
                }
                string[] directoryExecutables = Directory.GetFiles(path, "*.exe");
                foreach(string executable in directoryExecutables) {
                    load(executable);
                }
            }
        }
        public static void LoadAssemblies(IEnumerable<string> assemblies, Action<string> load) {
            if(assemblies == null || load == null)
                return;
            foreach(var assemblyOrExecutable in assemblies) {
                if(!File.Exists(assemblyOrExecutable))
                    continue;
                var ext = Path.GetExtension(assemblyOrExecutable);
                if(string.Equals(ext, ".dll", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(ext, ".exe", StringComparison.OrdinalIgnoreCase)) {
                    load(assemblyOrExecutable);
                }
            }
        }
    }
}