namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection.Emit;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    public static class WeakHashCodeImplementation {
        readonly static short xor_value = OpCodes.Xor.Value;
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => i.OpCode.Value == xor_value),
        };
        //
        [Display(Order = (int)ProcessingSeverity.Informational,
            Name = "Weak GetHashCode implementation",
            Description = "ILSA.Core.Assets.MD.WeakHashCodeImplementation.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            if(instructions.Name != nameof(GetHashCode)) {
                captures = MethodBodyPattern.NoCaptures;
                return false;
            }
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}