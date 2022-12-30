namespace ILSA.Core.Patterns {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using MatchMetadata = System.Func<System.Type, System.Text.StringBuilder, bool>;

    public enum PatternType {
        None,
        MetadataPattern,
        MethodBodyPattern
    }
    public enum ProcessingSeverity {
        Ignore,
        Informational,
        Warning,
        Error
    }
    //
    public abstract class Pattern {
        protected readonly MethodInfo match;
        protected Pattern(MethodInfo match) {
            this.match = match;
            var sourceType = match.DeclaringType;
            var display = sourceType.GetCustomAttribute<DisplayAttribute>();
            this.Name = display?.GetName() ?? sourceType.Name;
            this.Group = display?.GetGroupName() ?? GetGroup(sourceType.Namespace);
        }
        static string GetGroup(string @namespace) {
            if(string.IsNullOrEmpty(@namespace))
                return string.Empty;
            var extension = Path.GetExtension(@namespace);
            return extension?.Replace(".", string.Empty) ?? string.Empty;
        }
        public string Name {
            get;
            private set;
        }
        public string Group {
            get;
            private set;
        }
        public abstract PatternType Type {
            get;
        }
        public ProcessingSeverity Severity {
            get;
            set;
        }
    }
    //
    public class EmptyPattern : Pattern {
        public readonly static EmptyPattern Instance = new EmptyPattern();
        EmptyPattern()
            : base(new Func<string, Type>(System.Type.GetType).Method) {
        }
        public sealed override PatternType Type {
            get { return PatternType.None; }
        }
    }
    public class MethodBodyPattern : Pattern {
        delegate bool MatchMethodBody(IILReader instructions, StringBuilder errors, out int[] captures);
        public MethodBodyPattern(MethodInfo match)
            : base(match) {
        }
        public sealed override PatternType Type {
            get { return PatternType.MethodBodyPattern; }
        }
        MatchMethodBody? matchMethodBody;
        readonly static int[] NoCaptures = new int[0];
        public bool Match(IILReader instructions, StringBuilder errors, out int[] captures) {
            captures = NoCaptures;
            matchMethodBody = matchMethodBody ?? (MatchMethodBody)Delegate.CreateDelegate(typeof(MatchMethodBody), match!);
            return (matchMethodBody == null) || matchMethodBody(instructions, errors, out captures);
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
                    if(i == 0)
                        errors.Append(' ', 2).Append(instructions.Name).AppendLine(":");
                    errors.Append(' ', 4).AppendLine(instructions[i].ToString());
                }
            }
            else captures = NoCaptures;
            return (index == matches.Length);
        }
    }
    public class MetadataPattern : Pattern {
        public MetadataPattern(MethodInfo match)
            : base(match) {
        }
        public sealed override PatternType Type {
            get { return PatternType.MetadataPattern; }
        }
        MatchMetadata? matchMetadata;
        public bool Match(Type type, StringBuilder errors) {
            matchMetadata = matchMetadata ?? (MatchMetadata)Delegate.CreateDelegate(typeof(MatchMetadata), match!);
            return (matchMetadata == null) || matchMetadata(type, errors);
        }
    }
}