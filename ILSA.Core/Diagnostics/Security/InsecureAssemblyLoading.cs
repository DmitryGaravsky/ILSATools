namespace ILSA.Core.Diagnostics.Security {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;
    using BF = System.Reflection.BindingFlags;

    public static class InsecureAssemblyLoading {
        readonly internal static HashSet<MethodBase> loadMethods = new HashSet<MethodBase>(Call.MethodsComparer);
        static InsecureAssemblyLoading() {
            var loadMethods = typeof(Assembly).GetMember(nameof(Assembly.Load), BF.Public | BF.Static);
            RegisterMembersWithParameters(loadMethods);
            var loadFromMethods = typeof(Assembly).GetMember(nameof(Assembly.LoadFrom), BF.Public | BF.Static);
            RegisterMembersWithStringParameters(loadFromMethods, x => x.Name.Contains("assembly"));
            var loadFileMethods = typeof(Assembly).GetMember(nameof(Assembly.LoadFile), BF.Public | BF.Static);
            RegisterMembersWithStringParameters(loadFileMethods);
            var loadWithPartialNameMethods = typeof(Assembly).GetMember(nameof(Assembly.LoadWithPartialName), BF.Public | BF.Static);
            RegisterMembersWithStringParameters(loadWithPartialNameMethods);
            InsecureAssemblyLoading.loadMethods.Add(typeof(Assembly).GetMethod(nameof(Assembly.UnsafeLoadFrom), BF.Public | BF.Static));
        }
        static void RegisterMembersWithParameters(MemberInfo[] methods) {
            for(int i = 0; i < methods.Length; i++) {
                var method = methods[i] as MethodBase;
                if(method != null) {
                    var mParam = method.GetParameters();
                    if(mParam.Length > 0)
                        loadMethods.Add(method);
                }
            }
        }
        static void RegisterMembersWithStringParameters(MemberInfo[] methods, Predicate<ParameterInfo>? filter = null) {
            for(int i = 0; i < methods.Length; i++) {
                var method = methods[i] as MethodBase;
                if(method != null) {
                    var mParam = method.GetParameters();
                    if(filter == null) {
                        if(mParam.Any(x => x.ParameterType == typeof(string)))
                            loadMethods.Add(method);
                    }
                    else {
                        if(mParam.Any(x => x.ParameterType == typeof(string) && filter(x)))
                            loadMethods.Add(method);
                    }
                }
            }
        }
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsGetTypeMethod(i.Operand)),
        };
        static bool IsGetTypeMethod(object? operand) {
            return (operand is MethodBase method) && loadMethods.Contains(method);
        }
        //
        [Display(Order = (int)ProcessingSeverity.Error,
            Name = "Insecure assembly loading",
            Description = "ILSA.Core.Assets.MD.InsecureAssemblyLoading.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
    public static class HardCodedAssemblyLoading {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => i.OpCode== OpCodes.Ldstr),
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsAssemblyLoadMethod(i.Operand)),
        };
        static bool IsAssemblyLoadMethod(object? operand) {
            return (operand is MethodBase method) && InsecureAssemblyLoading.loadMethods.Contains(method);
        }
        //
        [Display(Order = (int)ProcessingSeverity.Informational,
            Name = "Hard-coded assembly loading",
            Description = "ILSA.Core.Assets.MD.HardCodedAssemblyLoading.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}