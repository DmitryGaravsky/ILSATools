namespace ILSA.Core.Diagnostics.Compatibility {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Diagnostics;
    using ILSA.Core.Patterns;
    using BF = System.Reflection.BindingFlags;

    public static class NET7Compatibility {
        readonly internal static HashSet<MethodBase> incompatibleAPI = new HashSet<MethodBase>();
        readonly internal static HashSet<Type> incompatibleTypes = new HashSet<Type>() {
                typeof(System.Xml.XmlSecureResolver),
        };
        static NET7Compatibility() {
            incompatibleAPI.Add(typeof(AssemblyName).GetMethod("get_" + nameof(AssemblyName.HashAlgorithm)));
            incompatibleAPI.Add(typeof(AssemblyName).GetMethod("get_" + nameof(AssemblyName.ProcessorArchitecture)));
            incompatibleAPI.Add(typeof(AssemblyName).GetMethod("get_" + nameof(AssemblyName.VersionCompatibility)));
            incompatibleAPI.Add(typeof(AssemblyName).GetMethod("get_" + nameof(AssemblyName.CodeBase)));
            incompatibleAPI.Add(typeof(AssemblyName).GetMethod("get_" + nameof(AssemblyName.EscapedCodeBase)));
            var compileToAssemblyMethods = typeof(System.Text.RegularExpressions.Regex).GetMember(nameof(System.Text.RegularExpressions.Regex.CompileToAssembly), BF.Public | BF.Static);
            for(int i = 0; i < compileToAssemblyMethods.Length; i++)
                incompatibleAPI.Add((MethodBase)compileToAssemblyMethods[i]);
        }
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCallOrIsNewObj(i.OpCode) && IsIncompatibleAPI(i.Operand)),
        };
        static bool IsIncompatibleAPI(object? operand) {
            return operand is MethodBase method && (incompatibleAPI.Contains(method) || incompatibleTypes.Contains(method.DeclaringType));
        }
        //
        [Display(Name = ".NET7 Compatibility", Order = (int)ProcessingSeverity.Warning,
            Description = "ILSA.Core.Assets.MD.NET7Compatibility.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}