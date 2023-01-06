namespace ILSA.Core.Diagnostics.Security {
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    static class Call {
        readonly static short call_Value = OpCodes.Call.Value;
        readonly static short callvirt_Value = OpCodes.Callvirt.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCall(OpCode opCode) {
            short opCodeValue = opCode.Value;
            return opCodeValue == call_Value || opCodeValue == callvirt_Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSameMethod(MethodBase source, MethodBase target) {
            if(source == target)
                return true;
            if(source.DeclaringType != target.DeclaringType)
                return false;
            return source.MethodHandle == target.MethodHandle;
        }
        //
        public struct MethodBaseComparer : IEqualityComparer<MethodBase> {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(MethodBase source, MethodBase target) {
                return IsSameMethod(source, target);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetHashCode(MethodBase source) {
                return source.GetHashCode();
            }
        }
    }
}