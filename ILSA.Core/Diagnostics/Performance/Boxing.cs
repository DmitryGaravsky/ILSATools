namespace ILSA.Core.Diagnostics.Performance {
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    static class Boxing {
        readonly static short box_Value = OpCodes.Box.Value;
        readonly static short unbox_Value = OpCodes.Unbox.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBoxing(OpCode opCode) {
            short opCodeValue = opCode.Value;
            return opCodeValue == box_Value || opCodeValue == unbox_Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValueType(Type? type) {
            return (type != null) && type.IsValueType;
        }
        //
        public static bool HasBoxingOfKeyParameter(MethodInfo method) {
            var parameters = method.GetParameters();
            if(parameters.Length == 0)
                return false;
            var keyType = method.DeclaringType.GetGenericArguments()[0];
            if(!keyType.IsValueType || keyType.IsGenericParameter)
                return false;
            if(!HasBasicHashCodeImpl(keyType))
                return false;
            return parameters.Any(x => x.ParameterType == keyType);
        }
        public static bool IsMethodOfGenericType(MethodInfo method, Type genericType) {
            var declaringType = method.DeclaringType;
            return
                declaringType.IsGenericType &&
                declaringType.GetGenericTypeDefinition() != genericType;
        }
        //
        readonly static Type ValueTypeType = typeof(ValueType);
        static bool HasBasicHashCodeImpl(Type type) {
            var hashCodeMethod = type.GetMethod("GetHashCode", Type.EmptyTypes);
            return hashCodeMethod.DeclaringType == ValueTypeType;
        }
    }
}