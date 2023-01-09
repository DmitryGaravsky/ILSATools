namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    public static class BoxingOnStringMethodCalls {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Boxing.IsBoxing(i.OpCode) && Boxing.IsValueType(i.Operand)),
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsFormatOrConcatMethod(i.Operand)),
        };
        static bool IsFormatOrConcatMethod(object? operand) {
            if(!(operand is MethodBase method) || method.DeclaringType != typeof(string))
                return false;
            return method.Name == "Format" || method.Name == "Concat";
        }
        //
        [Display(Order = (int)ProcessingSeverity.Informational, 
            Description = "ILSA.Core.Assets.MD.BoxingOnStringMethodCalls.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}