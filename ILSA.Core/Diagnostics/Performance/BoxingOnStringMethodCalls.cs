namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    public static class BoxingOnStringMethodCalls {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Boxing.IsBoxing(i.OpCode) && Boxing.IsValueType(i.Operand as Type)),
            new Func<IInstruction, bool>(i => i.OpCode == OpCodes.Call && IsStringMethod(i.Operand as MethodInfo)),
        };
        static bool IsStringMethod(MethodBase? method) {
            if(method == null || method.DeclaringType != typeof(string))
                return false;
            return IsFormatOrConcatMethod(method);
        }
        static bool IsFormatOrConcatMethod(MethodBase method) {
            return method.Name == "Format" || method.Name == "Concat";
        }
        //
        [Display(Order = (int)ProcessingSeverity.Informational, Description = "ILSA.Core.Assets.MD.BoxingOnStringMethodCalls.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}