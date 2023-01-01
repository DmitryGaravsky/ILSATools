namespace ILSA.Core.Patterns {
    using System;
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
        public static bool Match(Func<IInstruction, bool>[] matches, IILReader instructions, StringBuilder errors, out int[] captures) {
            captures = NoCaptures;
            if(matches.Length == 0)
                return false;
            int index = 0;
            Func<IInstruction, bool> match = matches[index];
            foreach(IInstruction instruction in instructions) {
                if(!match(instruction))
                    continue;
                captures = new int[matches.Length];
                captures[index] = instruction.Index;
                if(++index == matches.Length)
                    break;
                match = matches[index];
            }
            if(index == matches.Length) {
                for(int i = 0; i < captures.Length; i++) {
                    if(i == 0) {
                        errors
                            .Append(' ', 2)
                            .Append(instructions.Name)
                            .AppendLine(":");
                    }
                    errors
                        .Append(' ', 4)
                        .AppendLine(instructions[captures[i]].ToString());
                }
            }
            else captures = NoCaptures;
            return index == matches.Length;
        }
    }
}