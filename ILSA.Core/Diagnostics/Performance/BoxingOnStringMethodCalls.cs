namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection.Emit;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    public static class BoxingOnStringMethodCalls {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(x => x.OpCode == OpCodes.Box || x.OpCode == OpCodes.Unbox || x.OpCode == OpCodes.Unbox_Any),
        };
        [Display(Order = (int)ProcessingSeverity.Informational, Description = "ILSA.Core.Assets.MD.BoxingOnStringMethodCalls.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}