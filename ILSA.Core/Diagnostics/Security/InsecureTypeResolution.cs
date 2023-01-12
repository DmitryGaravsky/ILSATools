namespace ILSA.Core.Diagnostics.Security {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.Serialization;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;
    using BF = System.Reflection.BindingFlags;

    public static class InsecureTypeResolution {
        readonly internal static HashSet<MethodBase> getTypeMethods = new HashSet<MethodBase>();
        static InsecureTypeResolution() {
            var typeGetTypeMethods = typeof(Type).GetMember(nameof(Type.GetType), BF.Public | BF.Static);
            RegisterMembersWithParameters(typeGetTypeMethods);
            var assemblyGetTypeMethods = typeof(Assembly).GetMember(nameof(Assembly.GetType), BF.Public | BF.Instance);
            RegisterMembersWithParameters(assemblyGetTypeMethods);
            var activatorCreateInstanceMethods = typeof(Activator).GetMember(nameof(Activator.CreateInstance), BF.Public | BF.Static);
            RegisterMembersWithStringParameters(activatorCreateInstanceMethods, x => x.Name.Contains("type"));
            getTypeMethods.Add(typeof(FormatterServices).GetMethod(nameof(FormatterServices.GetTypeFromAssembly), BF.Public | BF.Static));
        }
        static void RegisterMembersWithParameters(MemberInfo[] methods) {
            for(int i = 0; i < methods.Length; i++) {
                var method = methods[i] as MethodBase;
                if(method != null) {
                    var mParam = method.GetParameters();
                    if(mParam.Length > 0)
                        getTypeMethods.Add(method);
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
                            getTypeMethods.Add(method);
                    }
                    else {
                        if(mParam.Any(x => x.ParameterType == typeof(string) && filter(x)))
                            getTypeMethods.Add(method);
                    }
                }
            }
        }
        //
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsGetTypeMethod(i.Operand)),
        };
        static bool IsGetTypeMethod(object? operand) {
            return (operand is MethodBase method) && getTypeMethods.Contains(method);
        }
        //
        [Display(Order = (int)ProcessingSeverity.Error,
            Description = "ILSA.Core.Assets.MD.InsecureTypeResolution.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
    public static class HardCodedTypeResolution {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => i.OpCode== OpCodes.Ldstr),
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsGetTypeMethod(i.Operand)),
        };
        static bool IsGetTypeMethod(object? operand) {
            return (operand is MethodBase method) && InsecureTypeResolution.getTypeMethods.Contains(method);
        }
        //
        [Display(Order = (int)ProcessingSeverity.Informational, 
            Description = "ILSA.Core.Assets.MD.InsecureTypeResolution.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}