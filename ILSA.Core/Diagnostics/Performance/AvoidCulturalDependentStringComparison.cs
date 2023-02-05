namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;
    using BF = System.Reflection.BindingFlags;

    public static class AvoidCulturalDependentStringComparison {
        readonly static MethodInfo mi_startsWith, mi_endsWith,
            mi_IndexOf_1, mi_IndexOf_2, mi_IndexOf_3,
            mi_LastIndexOf_1, mi_LastIndexOf_2, mi_LastIndexOf_3;
        static AvoidCulturalDependentStringComparison() {
            var strParam = new Type[] { typeof(string) };
            var strParams1 = new Type[] { typeof(string), typeof(int) };
            var strParams2 = new Type[] { typeof(string), typeof(int), typeof(int) };
            mi_startsWith = typeof(string).GetMethod("StartsWith", BF.Public | BF.Instance, null, strParam, null);
            mi_endsWith = typeof(string).GetMethod("EndsWith", BF.Public | BF.Instance, null, strParam, null);
            mi_IndexOf_1 = typeof(string).GetMethod("IndexOf", BF.Public | BF.Instance, null, strParam, null);
            mi_IndexOf_2 = typeof(string).GetMethod("IndexOf", BF.Public | BF.Instance, null, strParams1, null);
            mi_IndexOf_3 = typeof(string).GetMethod("IndexOf", BF.Public | BF.Instance, null, strParams2, null);
            mi_LastIndexOf_1 = typeof(string).GetMethod("LastIndexOf", BF.Public | BF.Instance, null, strParam, null);
            mi_LastIndexOf_2 = typeof(string).GetMethod("LastIndexOf", BF.Public | BF.Instance, null, strParams1, null);
            mi_LastIndexOf_3 = typeof(string).GetMethod("LastIndexOf", BF.Public | BF.Instance, null, strParams2, null);
        }
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsCulturalDependentStringComparison(i.Operand)),
        };
        static bool IsCulturalDependentStringComparison(object? operand) {
            if(!(operand is MethodBase method))
                return false;
            return
                Call.IsSameMethod(method, mi_startsWith) || Call.IsSameMethod(method, mi_endsWith) ||
                Call.IsSameMethod(method, mi_IndexOf_1) ||
                Call.IsSameMethod(method, mi_IndexOf_2) ||
                Call.IsSameMethod(method, mi_IndexOf_3) ||
                Call.IsSameMethod(method, mi_LastIndexOf_1) ||
                Call.IsSameMethod(method, mi_LastIndexOf_2) ||
                Call.IsSameMethod(method, mi_LastIndexOf_3);
        }
        //
        [Display(Order = (int)ProcessingSeverity.Warning,
            Name = "Avoid cultural-dependent string comparison",
            Description = "ILSA.Core.Assets.MD.AvoidCulturalDependentStringComparison.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}