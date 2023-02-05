namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;

    public static class BoxingOnDictionaryMethodCalls {
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsDictionaryMethodWithBoxingOfKey(i.Operand)),
        };
        readonly static Type dictionaryType = typeof(System.Collections.Generic.Dictionary<,>);
        readonly static Type cDictionaryType = typeof(System.Collections.Concurrent.ConcurrentDictionary<,>);
        readonly static Type sortedDictionaryType = typeof(System.Collections.Generic.SortedDictionary<,>);
        readonly static Type sortedListType = typeof(System.Collections.Generic.SortedList<,>);
        static bool IsDictionaryMethodWithBoxingOfKey(object? operand) {
            if(operand is MethodInfo method) {
                if(Call.IsMethodOfGenericType(method, dictionaryType) ||
                    Call.IsMethodOfGenericType(method, cDictionaryType) ||
                     Call.IsMethodOfGenericType(method, sortedDictionaryType) ||
                      Call.IsMethodOfGenericType(method, sortedListType)) {
                    return Boxing.HasBoxingOfKeyParameter(method);
                }
            }
            return false;
        }
        //
        [Display(Order = (int)ProcessingSeverity.Error, 
            Name = "Boxing on Dictionary<U,V> method calls",
            Description = "ILSA.Core.Assets.MD.BoxingOnDictionaryMethodCalls.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}