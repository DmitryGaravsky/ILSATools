namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    public static class AvoidUsingHashSetAddRemoveAfterContainsCheck {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsHashSetContains(i.Operand)),
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsHashSetAddOrRemove(i.Operand))
        };
        //
        static readonly Type hashsetType = typeof(HashSet<>);
        static bool IsHashSetContains(object? operand) {
            return operand is MethodBase method && Call.IsMethodOfGenericType(method, hashsetType) && (method.Name == "Contains");
        }
        static bool IsHashSetAddOrRemove(object? operand) {
            return operand is MethodBase method && Call.IsMethodOfGenericType(method, hashsetType) && (method.Name == "Add" || method.Name == "Remove");
        }
        //
        [Display(Order = (int)ProcessingSeverity.Informational, 
            Name = @"Avoid using HashSet.Add\HashSet.Remove after HashSet.Contains call",
            Description = "ILSA.Core.Assets.MD.AvoidUsingHashSetAddRemoveAfterContainsCheck.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}