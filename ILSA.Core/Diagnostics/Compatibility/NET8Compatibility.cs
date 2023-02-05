namespace ILSA.Core.Diagnostics.Compatibility {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Diagnostics;
    using ILSA.Core.Patterns;

    public static class NET8Compatibility {
        readonly internal static HashSet<MethodBase> incompatibleAPI = new HashSet<MethodBase>(Call.MethodsComparer);
        readonly internal static HashSet<string> incompatibleStringAPI = new HashSet<string>(StringComparer.Ordinal);
        readonly internal static HashSet<string> incompatibleStringTypes = new HashSet<string>(StringComparer.Ordinal);
        static NET8Compatibility() {
            incompatibleAPI.Add(typeof(RSA).GetMethod(nameof(RSA.EncryptValue)));
            incompatibleAPI.Add(typeof(RSA).GetMethod(nameof(RSA.DecryptValue)));
            incompatibleAPI.Add(typeof(RSACryptoServiceProvider).GetMethod(nameof(RSACryptoServiceProvider.EncryptValue)));
            incompatibleAPI.Add(typeof(RSACryptoServiceProvider).GetMethod(nameof(RSACryptoServiceProvider.DecryptValue)));
            incompatibleStringTypes.Add("System.Windows.Forms.DomainUpDown.DomainUpDownAccessibleObject");
            incompatibleStringAPI.Add("System.Windows.Forms.DomainUpDown.CreateAccessibilityInstance");
        }
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCallOrIsNewObj(i.OpCode) && IsIncompatibleAPI(i.Operand)),
        };
        static bool IsIncompatibleAPI(object? operand) {
            if(!(operand is MethodBase method))
                return false;
            return
                incompatibleAPI.Contains(method) ||
                incompatibleStringTypes.Contains(method.DeclaringType.FullName) || 
                incompatibleStringAPI.Contains(GetMethodSignature(method));
        }
        static string GetMethodSignature(MethodBase method) {
            return method.DeclaringType.FullName + "." + method.Name;
        }
        //
        [Display(Name = ".NET 8 Compatibility", Order = (int)ProcessingSeverity.Warning,
            Description = "ILSA.Core.Assets.MD.NET8Compatibility.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}