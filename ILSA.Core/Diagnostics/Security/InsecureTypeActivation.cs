namespace ILSA.Core.Diagnostics.Security {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;
    using BF = System.Reflection.BindingFlags;

    public static class InsecureTypeActivation {
        readonly internal static HashSet<MethodBase> activationAPI = new HashSet<MethodBase>(Call.MethodsComparer);
        static InsecureTypeActivation() {
            var invokeMemberMethods = typeof(Type).GetMember(nameof(Type.InvokeMember), BF.Public | BF.Instance);
            for(int i = 0; i < invokeMemberMethods.Length; i++)
                activationAPI.Add((MethodBase)invokeMemberMethods[i]);
            var activatorCreateInstanceMethods = typeof(Activator).GetMember(nameof(Activator.CreateInstance), BF.Public | BF.Instance | BF.Static);
            for(int i = 0; i < activatorCreateInstanceMethods.Length; i++)
                activationAPI.Add((MethodBase)activatorCreateInstanceMethods[i]);
            var activatorCreateInstanceFromMethods = typeof(Activator).GetMember(nameof(Activator.CreateInstanceFrom), BF.Public | BF.Instance | BF.Static);
            for(int i = 0; i < activatorCreateInstanceFromMethods.Length; i++)
                activationAPI.Add((MethodBase)activatorCreateInstanceFromMethods[i]);
            var assemblyCreateInstanceMethods = typeof(Assembly).GetMember(nameof(Assembly.CreateInstance), BF.Public | BF.Instance | BF.Static);
            for(int i = 0; i < assemblyCreateInstanceMethods.Length; i++)
                activationAPI.Add((MethodBase)assemblyCreateInstanceMethods[i]);
        }
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsActivationAPI(i.Operand)),
        };
        static bool IsActivationAPI(object? operand) {
            return operand is MethodBase method && activationAPI.Contains(method);
        }
        //
        [Display(Order = (int)ProcessingSeverity.Error,
            Description = "ILSA.Core.Assets.MD.InsecureTypeActivation.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}