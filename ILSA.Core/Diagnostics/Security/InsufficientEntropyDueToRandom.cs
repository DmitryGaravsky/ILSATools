namespace ILSA.Core.Diagnostics.Security {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    public static class InsufficientEntropyDueToRandom {
        readonly static Type systemRandom = typeof(Random);
        //
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsRandomMethod(i.Operand)),
        };
        static bool IsRandomMethod(object? operand) {
            return operand is MethodBase method && Call.IsSameType(method.DeclaringType, systemRandom);
        }
        //
        [Display(Order = (int)ProcessingSeverity.Informational,
            Description = "ILSA.Core.Assets.MD.InsufficientEntropyDueToRandom.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}