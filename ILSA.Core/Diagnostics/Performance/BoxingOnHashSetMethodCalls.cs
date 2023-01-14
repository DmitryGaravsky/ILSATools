namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    public static class BoxingOnHashSetMethodCalls {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => i.OpCode == OpCodes.Callvirt && IsHashSetMethodWithBoxingOfKey(i.Operand)),
        };
        readonly static Type hashSetType = typeof(HashSet<>);
        readonly static Type sortedSetType = typeof(SortedSet<>);
        static bool IsHashSetMethodWithBoxingOfKey(object? operand) {
            if(operand is MethodInfo method) {
                if(Call.IsMethodOfGenericType(method, hashSetType) ||
                    Call.IsMethodOfGenericType(method, sortedSetType)) {
                    return Boxing.HasBoxingOfKeyParameter(method);
                }
            }
            return false;
        }
        //
        [Display(Order = (int)ProcessingSeverity.Error, Description = "ILSA.Core.Assets.MD.BoxingOnHashSetMethodCalls.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}