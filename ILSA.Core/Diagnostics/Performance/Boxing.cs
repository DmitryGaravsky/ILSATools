namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.Reflection.Emit;
    using System.Text;
    using ILReader.Readers;

    public static class Boxing {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(x=>x.OpCode == OpCodes.Box),
        };
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return Patterns.MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}