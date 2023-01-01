namespace ILSA.Core.Patterns {
    using System;
    using System.Reflection;
    using System.Text;
    using MatchMetadata = System.Func<System.Type, System.Text.StringBuilder, bool>;

    public class MetadataPattern : Pattern {
        public MetadataPattern(MethodInfo match)
            : base(match) {
        }
        protected override ProcessingSeverity GetDefaultSeverity() {
            return ProcessingSeverity.Warning;
        }
        public sealed override ProcessingTarget Target {
            get { return ProcessingTarget.Metadata; }
        }
        MatchMetadata? matchMetadata;
        public bool Match(Type type, StringBuilder errors) {
            if(Severity == ProcessingSeverity.Ignore || Target == ProcessingTarget.None)
                return false;
            matchMetadata = matchMetadata ?? (MatchMetadata)Delegate.CreateDelegate(typeof(MatchMetadata), match!);
            return matchMetadata == null || matchMetadata(type, errors);
        }
    }
}