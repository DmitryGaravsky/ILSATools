namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;
    using BF = System.Reflection.BindingFlags;

    public static class AvoidUsingGetExecutingAssembly {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsGetExecutingAssemblyMethod(i.Operand)),
        };
        readonly static MethodBase m_GetExecutingAssembly = typeof(Assembly).GetMethod(nameof(Assembly.GetExecutingAssembly), BF.Public | BF.Instance | BF.Static);
        static bool IsGetExecutingAssemblyMethod(object? operand) {
            return (operand is MethodBase method) && Call.IsSameMethod(method, m_GetExecutingAssembly);
        }
        //
        [Display(Order = (int)ProcessingSeverity.Informational, 
            Description = "ILSA.Core.Assets.MD.AvoidUsingGetExecutingAssembly.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}