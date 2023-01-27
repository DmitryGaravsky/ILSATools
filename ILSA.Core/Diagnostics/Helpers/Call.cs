namespace ILSA.Core.Diagnostics {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    public static class Call {
        readonly static short call_Value = OpCodes.Call.Value;
        readonly static short callvirt_Value = OpCodes.Callvirt.Value;
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
            if(target == source)
                return true;
            return IsSameType(target.DeclaringType, source.DeclaringType) && target.ToString() == source.ToString();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMethodOfGenericType(MethodBase method, Type genericType) {
            var declaringType = method.DeclaringType;
            return declaringType.IsGenericType && IsSameType(declaringType.GetGenericTypeDefinition(), genericType);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSameType(Type source, Type target) {
            if(target == source)
                return true;
            return target.AssemblyQualifiedName == source.AssemblyQualifiedName;
        }
        //
        public readonly static IEqualityComparer<MethodBase> MethodsComparer = new MethodBaseComparer();
        public readonly static IEqualityComparer<Type> TypesComparer = new TypeComparer();
        sealed class MethodBaseComparer : IEqualityComparer<MethodBase> {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool IEqualityComparer<MethodBase>.Equals(MethodBase x, MethodBase y) {
                return IsSameMethod(x, y);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IEqualityComparer<MethodBase>.GetHashCode(MethodBase obj) {
                return obj.ToString().GetHashCode();
            }
        }
        sealed class TypeComparer : IEqualityComparer<Type> {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool IEqualityComparer<Type>.Equals(Type x, Type y) {
                return IsSameType(x, y);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IEqualityComparer<Type>.GetHashCode(Type obj) {
                return obj.ToString().GetHashCode();
            }
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