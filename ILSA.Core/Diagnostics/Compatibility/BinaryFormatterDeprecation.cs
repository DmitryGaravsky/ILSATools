namespace ILSA.Core.Diagnostics.Compatibility {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Diagnostics;
    using ILSA.Core.Patterns;
    // Refer to: https://github.com/dotnet/designs/blob/main/accepted/2020/better-obsoletion/binaryformatter-obsoletion.md
    public static class BinaryFormatterDeprecation {
        readonly internal static HashSet<Type> incompatibleTypes = new HashSet<Type>(Call.TypesComparer) {
            // In .NET 8.0, the following APIs will be marked [Obsolete] as error by default. These will utilize the existing SYSLIB0011 warning code.
            // The errors can be converted back to warnings by using the <EnableUnsafeBinaryFormatterSerialization> compatibility-switch within the project file.
            typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter),
            typeof(System.Runtime.Serialization.IFormatter),
            typeof(System.Runtime.Serialization.Formatter),
            // The following APIs will be marked [Obsolete] as warning using a new warning code(tentatively SYSLIB0050).
            // Newly obsolete types
            typeof(System.Runtime.Serialization.IFormatterConverter),
            typeof(System.Runtime.Serialization.FormatterConverter),
            typeof(System.Runtime.Serialization.IObjectReference),
            typeof(System.Runtime.Serialization.ISurrogateSelector),
            typeof(System.Runtime.Serialization.ISerializationSurrogate),
            typeof(System.Runtime.Serialization.SurrogateSelector),
            typeof(System.Runtime.Serialization.ISafeSerializationData),
            typeof(System.Runtime.Serialization.SafeSerializationEventArgs),
            typeof(System.Runtime.Serialization.StreamingContextStates),
            typeof(System.Runtime.Serialization.ObjectIDGenerator),
            typeof(System.Runtime.Serialization.ObjectManager),
            typeof(System.Runtime.Serialization.SerializationObjectManager),
            typeof(System.Runtime.Serialization.Formatters.FormatterTypeStyle),
            typeof(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle),
            typeof(System.Runtime.Serialization.Formatters.TypeFilterLevel),
            typeof(System.Runtime.Serialization.Formatters.IFieldInfo),
            // Newly obsolete members on existing types (we are threat any members for better coverage)
            typeof(System.Runtime.Serialization.SerializationInfo),
            typeof(System.Runtime.Serialization.StreamingContext),
        };
        // Newly obsolete members on existing types
        readonly internal static HashSet<MethodBase> incompatibleAPI = new HashSet<MethodBase>(Call.MethodsComparer) {
            typeof(Type).GetMethod("get_" + nameof(Type.IsSerializable)),
            typeof(FieldInfo).GetMethod("get_" + nameof(FieldInfo.IsNotSerialized)),
            typeof(System.Runtime.Serialization.ISerializable).GetMethod("get_" + nameof(System.Runtime.Serialization.ISerializable.GetObjectData))

        };
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCallOrIsNewObj(i.OpCode) && IsIncompatibleAPI(i.Operand)),
        };
        static bool IsIncompatibleAPI(object? operand) {
            if(operand is not MethodBase method)
                return false;
            return incompatibleAPI.Contains(method) || incompatibleTypes.Contains(method.DeclaringType);
        }
        static string GetMethodSignature(MethodBase method) {
            return method.DeclaringType.FullName + "." + method.Name;
        }
        //
        [Display(Name = ".NET BinaryFormatter deprecation", Order = (int)ProcessingSeverity.Warning,
            Description = "ILSA.Core.Assets.MD.BinaryFormatterDeprecation.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}