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

    public static class NET6Compatibility {
        readonly internal static HashSet<MethodBase> incompatibleAPI = new HashSet<MethodBase>(Call.MethodsComparer);
        readonly internal static HashSet<Type> incompatibleTypes = new HashSet<Type>(Call.TypesComparer) {
                typeof(System.Net.WebClient),
                typeof(System.Net.WebRequest),
                typeof(System.Net.ServicePoint),
                typeof(System.Security.IPermission),
        };
        static NET6Compatibility() {
            var createDomainMethods = typeof(AppDomain).GetMember(nameof(AppDomain.CreateDomain), BF.Public | BF.Static);
            for(int i = 0; i < createDomainMethods.Length; i++)
                incompatibleAPI.Add((MethodBase)createDomainMethods[i]);
            var reflectionOnlyLoadMethods = typeof(Assembly).GetMember(nameof(Assembly.ReflectionOnlyLoad), BF.Public | BF.Static);
            for(int i = 0; i < reflectionOnlyLoadMethods.Length; i++)
                incompatibleAPI.Add((MethodBase)reflectionOnlyLoadMethods[i]);
            var threadAbortMethods = typeof(System.Threading.Thread).GetMember(nameof(System.Threading.Thread.Abort), BF.Public | BF.Static);
            for(int i = 0; i < threadAbortMethods.Length; i++)
                incompatibleAPI.Add((MethodBase)threadAbortMethods[i]);
            incompatibleAPI.Add(typeof(AppDomain).GetMethod(nameof(AppDomain.Unload)));
            incompatibleAPI.Add(typeof(Type).GetMethod(nameof(Type.ReflectionOnlyGetType), BF.Public | BF.Static));
            incompatibleAPI.Add(typeof(Uri).GetMethod(nameof(Uri.EscapeUriString), BF.Public | BF.Static));
            incompatibleAPI.Add(typeof(System.Threading.Thread).GetMethod(nameof(System.Threading.Thread.ResetAbort), BF.Public | BF.Static));
            incompatibleAPI.Add(typeof(Assembly).GetMethod("get_" + nameof(Assembly.GlobalAssemblyCache)));
            incompatibleAPI.Add(typeof(Encoding).GetMethod("get_" + nameof(Encoding.UTF7), BF.Public | BF.Static));
        }
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCallOrIsNewObj(i.OpCode) && IsIncompatibleAPI(i.Operand)),
        };
        static bool IsIncompatibleAPI(object? operand) {
            return (operand is MethodBase method) && (incompatibleAPI.Contains(method) || incompatibleTypes.Contains(method.DeclaringType));
        }
        //
        [Display(Name = ".NET6 Compatibility", Order = (int)ProcessingSeverity.Warning,
            Description = "ILSA.Core.Assets.MD.NET6Compatibility.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}