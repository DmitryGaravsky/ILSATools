namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    public static class BoxingOnNonGenericCollectionMethodCalls {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Boxing.IsBoxing(i.OpCode) && Boxing.IsValueType(i.Operand)),
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsNonGenericCollectionMethod(i.Operand)),
        };
        //
        readonly static Type collectionBaseType = typeof(System.Collections.CollectionBase);
        readonly static Type arrayListType = typeof(System.Collections.ArrayList);
        readonly static Type sortedListType = typeof(System.Collections.SortedList);
        readonly static Type hashtableType = typeof(System.Collections.Hashtable);
        readonly static Type queueType = typeof(System.Collections.Queue);
        readonly static Type stackType = typeof(System.Collections.Stack);
        static bool IsNonGenericCollectionMethod(object? operand) {
            if(!(operand is MethodBase method))
                return false;
            return
                Call.IsSameType(collectionBaseType, method.DeclaringType) ||
                Call.IsSameType(arrayListType, method.DeclaringType) ||
                Call.IsSameType(sortedListType, method.DeclaringType) ||
                Call.IsSameType(hashtableType, method.DeclaringType) ||
                Call.IsSameType(queueType, method.DeclaringType) ||
                Call.IsSameType(stackType, method.DeclaringType);
        }
        //
        [Display(Order = (int)ProcessingSeverity.Error,
            Name = "Boxing on non-generic collection method calls",
            Description = "ILSA.Core.Assets.MD.BoxingOnNonGenericCollectionMethodCalls.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}