namespace ILSA.Core.Sources {
    using System;
    using System.IO;
    using System.Reflection;

    public sealed partial class AssembliesSourceForClasses {
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
                        max = v; maxDir = versionDirs[i];
                    }
                }
                return maxDir;
            }
        }
    }
}