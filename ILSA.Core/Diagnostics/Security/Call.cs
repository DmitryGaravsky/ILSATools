namespace ILSA.Core.Diagnostics {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    static class Call {
        readonly static short call_Value = OpCodes.Call.Value;
        readonly static short callvirt_Value = OpCodes.Callvirt.Value;
        //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCall(OpCode opCode) {
            short opCodeValue = opCode.Value;
            return opCodeValue == call_Value || opCodeValue == callvirt_Value;
        }
        readonly static short newobj_Value = OpCodes.Newobj.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InNewObj(OpCode opCode) {
            return opCode.Value == newobj_Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCallOrIsNewObj(OpCode opCode) {
            short opCodeValue = opCode.Value;
            return opCodeValue == call_Value || opCodeValue == callvirt_Value || opCodeValue == newobj_Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSameMethod(MethodBase source, MethodBase target) {
            return source == target;
        }
    }
    sealed class TypeNamesComparer : IComparer<Type> {
        public readonly static IComparer<Type> Instance = new TypeNamesComparer();
        TypeNamesComparer() { }
        //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(Type x, Type y) {
            return string.CompareOrdinal(x.Name, y.Name);
        }
    }
    sealed class MethodNamesComparer : IComparer<MethodBase> {
        public readonly static IComparer<MethodBase> Instance = new MethodNamesComparer();
        MethodNamesComparer() { }
        //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(MethodBase x, MethodBase y) {
            return string.CompareOrdinal(x.Name, y.Name);
        }
    }
}