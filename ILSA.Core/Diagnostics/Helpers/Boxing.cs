﻿namespace ILSA.Core.Diagnostics {
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    public static class Boxing {
        readonly static short box_Value = OpCodes.Box.Value;
        readonly static short unbox_Value = OpCodes.Unbox.Value;
        //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBoxing(OpCode opCode) {
            short opCodeValue = opCode.Value;
            return opCodeValue == box_Value || opCodeValue == unbox_Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValueType(object? operand) {
            return (operand is Type type) && type.IsValueType;
        }
        public static bool HasBoxingOfKeyParameter(MethodInfo method) {
            var parameters = method.GetParameters();
            if(parameters.Length == 0)
                return false;
            var keyType = method.DeclaringType.GetGenericArguments()[0];
            if(!keyType.IsValueType || keyType.IsGenericParameter)
                return false;
            if(!HasBasicHashCodeImpl(keyType))
                return false;
            return parameters.Any(x => Call.IsSameType(x.ParameterType, keyType));
        }
        //
        readonly static Type ValueTypeType = typeof(ValueType);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool HasBasicHashCodeImpl(Type type) {
            var hashCodeMethod = type.GetMethod("GetHashCode", Type.EmptyTypes);
            return Call.IsSameType(hashCodeMethod.DeclaringType, ValueTypeType);
        }
    }
}