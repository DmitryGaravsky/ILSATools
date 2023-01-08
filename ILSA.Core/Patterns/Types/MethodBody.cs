namespace ILSA.Core.Patterns {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;

    public class MethodBodyPattern : Pattern {
        delegate bool MatchMethodBody(IILReader instructions, StringBuilder errors, out int[] captures);
        //
        public MethodBodyPattern(MethodInfo match)
            : base(match) {
        }
        public sealed override ProcessingTarget Target {
            get { return ProcessingTarget.MethodBody; }
        }
        protected sealed override ProcessingSeverity GetDefaultSeverity() {
            return ProcessingSeverity.Warning;
        }
        MatchMethodBody? matchMethodBody;
        readonly static int[] NoCaptures = new int[0];
        public bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            captures = NoCaptures;
            if(Severity == ProcessingSeverity.Ignore || Target == ProcessingTarget.None)
                return false;
            matchMethodBody = matchMethodBody ?? (MatchMethodBody)Delegate.CreateDelegate(typeof(MatchMethodBody), match!);
            return matchMethodBody == null || matchMethodBody(instructions, errors, out captures);
        }
        public static bool Match(Func<IInstruction, bool>[] matches, IILReader instructions, ref int startIndex, StringBuilder errors, out int[] captures) {
            captures = NoCaptures;
            if(matches.Length == 0 || startIndex >= instructions.Count)
                return false;
            if(matches.Length == 1)
                return Match(matches[0], instructions, ref startIndex, errors, out captures);
            if(matches.Length == 2)
                return Match(matches[0], matches[1], instructions, ref startIndex, errors, out captures);
            int i, index = 0;
            Func<IInstruction, bool> match = matches[index];
            for(i = startIndex; i < instructions.Count - matches.Length; i++) {
                var instruction = instructions[i];
                if(!match(instruction))
                    continue;
                captures = (captures.Length == 0) ? new int[matches.Length] : captures;
                captures[index] = instruction.Index;
                if(++index == matches.Length)
                    break;
                match = matches[index];
            }
            bool isFirstMatchInMethod = (startIndex == 0);
            startIndex = i + 1;
            if(index == matches.Length) {
                for(i = 0; i < captures.Length; i++) {
                    if(i == 0 && isFirstMatchInMethod) {
                        errors.Append(' ', 2)
                            .Append(instructions.Name).AppendLine(":");
                    }
                    errors.Append(' ', 4)
                        .AppendLine(instructions[captures[i]].ToString());
                }
            }
            else captures = NoCaptures;
            return index == matches.Length;
        }
        static bool Match(Func<IInstruction, bool> match, IILReader instructions, ref int startIndex, StringBuilder errors, out int[] captures) {
            int i = startIndex;
            for(; i < instructions.Count - 1; i++) {
                var instruction = instructions[i];
                if(!match(instruction))
                    continue;
                captures = new int[] { instruction.Index };
                if(startIndex == 0)
                    errors.Append(' ', 2).Append(instructions.Name).AppendLine(":");
                errors.Append(' ', 4).AppendLine(instruction.ToString());
                startIndex = i + 1;
                return true;
            }
            startIndex = i + 1;
            captures = NoCaptures;
            return false;
        }
        static bool Match(Func<IInstruction, bool> first, Func<IInstruction, bool> last,
            IILReader instructions, ref int startIndex, StringBuilder errors, out int[] captures) {
            int lastIndex = startIndex + 1;
            for(; lastIndex < instructions.Count; lastIndex++) {
                if(!last(instructions[lastIndex]))
                    continue;
                for(int i = lastIndex - 1; i >= startIndex; i--) {
                    var firstInstruction = instructions[i];
                    if(first(firstInstruction)) {
                        var lastInstruction = instructions[lastIndex];
                        captures = new int[] { firstInstruction.Index, lastInstruction.Index };
                        if(startIndex == 0)
                            errors.Append(' ', 2).Append(instructions.Name).AppendLine(":");
                        errors.Append(' ', 4).AppendLine(firstInstruction.ToString());
                        errors.Append(' ', 4).AppendLine(lastInstruction.ToString());
                        startIndex = lastIndex + 1;
                        return true;
                    }
                }
            }
            startIndex = lastIndex + 1;
            captures = NoCaptures;
            return false;
        }
        public static bool Match(Func<IInstruction, bool>[] matches, IILReader instructions, StringBuilder errors, out int[] captures) {
            int index = 0;
            if(!Match(matches, instructions, ref index, errors, out captures))
                return false;
            List<int>? allCaptures = null; int[] nextCaptures;
            while(Match(matches, instructions, ref index, errors, out nextCaptures)) {
                allCaptures = allCaptures ?? new List<int>(captures.Length * 4);
                allCaptures.AddRange(captures);
                allCaptures.AddRange(nextCaptures);
            }
            if(allCaptures != null)
                captures = allCaptures.ToArray();
            return (captures != null) && captures.Length > 0;
        }
        //
        public static readonly MethodBodyPattern Callee = new MethodBodyPattern(new MatchMethodBody(CalleeImpl.Match).Method);
        static class CalleeImpl {
            static readonly Func<IInstruction, bool>[] matches = new Func<IInstruction, bool>[] {
                    new Func<IInstruction, bool>(i => Diagnostics.Security.Call.IsCallOrIsNewObj(i.OpCode)),
                };
            public static bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
                return MethodBodyPattern.Match(matches, instructions, errors, out captures);
            }
            readonly static Type objectType = typeof(object);
            readonly static Type valueType = typeof(ValueType);
            internal static bool IsCallee(MethodBase? callee, HashSet<Assembly> scope) {
                if(callee == null)
                    return false;
                var declaringType = callee.DeclaringType;
                if(declaringType == null || declaringType == objectType || declaringType == valueType)
                    return false;
                return scope.Contains(declaringType.Assembly);
            }
        }
        public static void EnsureCallers(MethodBase caller, MethodBase? callee, HashSet<Assembly> scope,
            ConcurrentDictionary<MethodBase, HashSet<MethodBase>> callers) {
            if(CalleeImpl.IsCallee(callee, scope)) {
                callers.AddOrUpdate(callee!, 
                        (c) => new HashSet<MethodBase>() { caller }, 
                        (c, callers) => { callers.Add(caller); return callers; }
                    );
            }
        }
    }
}