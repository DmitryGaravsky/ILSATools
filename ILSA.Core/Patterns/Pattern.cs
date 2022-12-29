namespace ILSA.Core.Patterns {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using ILReader.Readers;
    using Match = System.Func<ILReader.Readers.IILReader, System.Text.StringBuilder, bool>;

    public enum PatternType {
        MetadataPattern,
        MethodBodyPattern
    }
    public enum ProcessingSeverity {
        Ignore,
        Informational,
        Warning,
        Error
    }
    public class Pattern {
        readonly Match match;
        public Pattern(MethodInfo match) {
            var sourceType = match.DeclaringType;
            var display = sourceType.GetCustomAttribute<DisplayAttribute>();
            this.Name = display?.GetName() ?? sourceType.Name;
            this.Group = display?.GetGroupName() ?? Path.GetExtension(sourceType.Namespace) ?? string.Empty;
            this.match = (Match)Delegate.CreateDelegate(typeof(Match), match!);
        }
        public string Name {
            get;
            private set;
        }
        public string Group {
            get;
            private set;
        }
        public PatternType Type {
            get;
            private set;
        }
        public ProcessingSeverity Severity {
            get;
            set;
        }
        public bool Match(IILReader instructions, StringBuilder errors) {
            return match(instructions, errors);
        }
    }
}