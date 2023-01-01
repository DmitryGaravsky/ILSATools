namespace ILSA.Core.Patterns {
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Reflection;

    public enum ProcessingTarget {
        None,
        Metadata,
        MethodBody
    }
    public enum ProcessingSeverity {
        Ignore,
        Informational,
        Warning,
        Error
    }
    public abstract class Pattern {
        protected readonly MethodInfo match;
        ProcessingSeverity? severityCore;
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
        public abstract ProcessingTarget Target {
            get;
        }
        public ProcessingSeverity Severity {
            get { return severityCore.GetValueOrDefault(GetDefaultSeverity()); }
            set { severityCore = value; }
        }
        protected virtual ProcessingSeverity GetDefaultSeverity() {
            return ProcessingSeverity.Ignore;
        }
        #region Empty
        public readonly static Pattern Empty = new EmptyPattern();
        //
        sealed class EmptyPattern : Pattern {
            internal EmptyPattern()
                : base(new Func<string, Type>(System.Type.GetType).Method) {
            }
            public sealed override ProcessingTarget Target {
                get { return ProcessingTarget.None; }
            }
        }
        #endregion
    }
}