namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    public static class BoxingOnEnumMethodCalls {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Boxing.IsBoxing(i.OpCode) && Boxing.IsValueType(i.Operand as Type)),
            new Func<IInstruction, bool>(i => i.OpCode == OpCodes.Call && IsEnumMethod(i.Operand as MethodInfo)),
        };
        static bool IsEnumMethod(MethodBase? method) {
            return (method != null) && (method.DeclaringType == typeof(Enum));
        }
        //
        [Display(Order = (int)ProcessingSeverity.Warning, Description = "ILSA.Core.Assets.MD.BoxingOnEnumMethodCalls.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}