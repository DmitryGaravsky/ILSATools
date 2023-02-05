namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    public static class BoxingOnEnumMethodCalls {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Boxing.IsBoxing(i.OpCode) && Boxing.IsValueType(i.Operand)),
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsEnumMethod(i.Operand)),
        };
        static bool IsEnumMethod(object? operand) {
            return (operand is MethodBase method) && (method.DeclaringType == typeof(Enum));
        }
        //
        [Display(Order = (int)ProcessingSeverity.Warning, 
            Name = "Boxing on Enum method calls",
            Description = "ILSA.Core.Assets.MD.BoxingOnEnumMethodCalls.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}