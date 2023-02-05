namespace ILSA.Core.Diagnostics.Security {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using ILSA.Core.Patterns;
    using BF = System.Reflection.BindingFlags;

    public static class PossiblePathTraversalPoint {
        readonly internal static HashSet<MethodBase> pathRelatedMethods = new HashSet<MethodBase>(Call.MethodsComparer);
        static PossiblePathTraversalPoint() {
            var flags = BF.Public | BF.Static | BF.Instance;
            var pathRelated = new Predicate<ParameterInfo>(x => x.Name.Contains("path"));
            var fileMembers = typeof(File).GetMembers(flags);
            RegisterMembersWithStringParameters(fileMembers, pathRelated);
            var fileInfoMembers = typeof(FileInfo).GetMembers(flags);
            RegisterMembersWithStringParameters(fileInfoMembers, pathRelated);
            var streamReaderMembers = typeof(StreamReader).GetMembers(flags);
            RegisterMembersWithStringParameters(streamReaderMembers, pathRelated);
            var streamWriterMembers = typeof(StreamWriter).GetMembers(flags);
            RegisterMembersWithStringParameters(streamWriterMembers, pathRelated);
            var fileStreamMembers = typeof(FileStream).GetMembers(flags);
            RegisterMembersWithStringParameters(fileStreamMembers, pathRelated);
            pathRelatedMethods.Remove(typeof(File).GetMethod(nameof(File.Exists), flags));
            var pathMembers = typeof(Path).GetMember(nameof(Path.Combine), flags);
            RegisterMembersWithParameters(pathMembers);
        }
        static void RegisterMembersWithParameters(MemberInfo[] methods) {
            for(int i = 0; i < methods.Length; i++) {
                var method = methods[i] as MethodBase;
                if(method != null) {
                    var mParam = method.GetParameters();
                    if(mParam.Length > 0)
                        pathRelatedMethods.Add(method);
                }
            }
        }
        static void RegisterMembersWithStringParameters(MemberInfo[] methods, Predicate<ParameterInfo> filter) {
            for(int i = 0; i < methods.Length; i++) {
                var method = methods[i] as MethodBase;
                if(method != null) {
                    var mParam = method.GetParameters();
                    if(mParam.Any(x => x.ParameterType == typeof(string) && filter(x)))
                        pathRelatedMethods.Add(method);
                }
            }
        }
        static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
            new Func<IInstruction, bool>(i => Call.IsCall(i.OpCode) && IsPathBasedMethod(i.Operand)),
        };
        static bool IsPathBasedMethod(object? operand) {
            return operand is MethodBase method && pathRelatedMethods.Contains(method);
        }
        //
        [Display(Order = (int)ProcessingSeverity.Informational,
            Name = "Possible PathTraversal point",
            Description = "ILSA.Core.Assets.MD.PossiblePathTraversalPoint.md")]
        public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            return MethodBodyPattern.Match(matches, instructions, errors, out captures);
        }
    }
}