namespace ILSA.Core.Diagnostics.Security {
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    static class Call {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCall(OpCode opCode) {
            return opCode == OpCodes.Call || opCode == OpCodes.Callvirt;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSameMethod(MethodBase source, MethodBase target) {
            if(source == target)
                return true;
            return source.DeclaringType == target.DeclaringType && source.MethodHandle == target.MethodHandle;
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