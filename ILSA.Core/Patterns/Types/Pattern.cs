namespace ILSA.Core.Patterns {
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Reflection;
    using System.Text;

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
        ProcessingSeverity? severityCore, defaultSeverityCore;
        protected Pattern(MethodInfo match) {
            this.match = match;
            var sourceType = match.DeclaringType;
            var display = match.GetCustomAttribute<DisplayAttribute>();
            this.Name = display?.GetName() ?? GetName(sourceType.Name);
            this.Group = display?.GetGroupName() ?? GetGroup(sourceType.Namespace);
            int? order = display?.GetOrder();
            if(order.HasValue)
                this.defaultSeverityCore = (ProcessingSeverity)order.GetValueOrDefault();
            var descriptionResource = display?.GetDescription() ?? 
                GetDescriptionResource(sourceType.FullName);
            Description = ReadText(descriptionResource, sourceType.Assembly);
        }
        static string GetName(string name) {
            var camelCaseSplit = new StringBuilder(name.Length);
            foreach(char c in name) {
                if(char.IsUpper(c) && camelCaseSplit.Length > 0)
                    camelCaseSplit.Append(' ');
                camelCaseSplit.Append(c);
            }
            return camelCaseSplit.ToString();
        }
        static string GetGroup(string @namespace) {
            if(string.IsNullOrEmpty(@namespace))
                return string.Empty;
            var extension = Path.GetExtension(@namespace);
            return extension?.Replace(".", string.Empty) ?? string.Empty;
        }
        static string GetDescriptionResource(string name) {
            return name + ".md";
        }
        public string Name {
            get;
            private set;
        }
        public string Group {
            get;
            private set;
        }
        public string Description {
            get;
            private set;
        }
        public abstract ProcessingTarget Target {
            get;
        }
        public ProcessingSeverity Severity {
            get {
                var defaultSeverity = defaultSeverityCore.GetValueOrDefault(GetDefaultSeverity());
                return severityCore.GetValueOrDefault(defaultSeverity);
            }
            set { severityCore = value; }
        }
        public void ResetSeverity() {
            severityCore = null;
        }
        protected virtual ProcessingSeverity GetDefaultSeverity() {
            return ProcessingSeverity.Ignore;
        }
        public Assembly GetAssembly() {
            return match.DeclaringType.Assembly;
        }
        public string GetNameInAssembly() {
            return match.DeclaringType.FullName + "." + match.Name;
        }
        public string GetSeverity() {
            return severityCore.HasValue ? severityCore.Value.ToString() : string.Empty;
        }
        #region ReadText
        readonly static ConcurrentDictionary<string, string> texts = new ConcurrentDictionary<string, string>();
        static string ReadText(string resourceName, Assembly resourcesAssembly) {
            return texts.GetOrAdd(resourceName, x => {
                using(var stream = GetResourceStream(x, resourcesAssembly))
                    return (stream != null) ? new StreamReader(stream).ReadToEnd() : string.Empty;
            });
        }
        readonly static ConcurrentDictionary<string, string> mappings =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        static Stream? GetResourceStream(string key, Assembly resourcesAssembly) {
            string resourceName;
            if(!mappings.TryGetValue(key, out resourceName))
                TryAddResourceNameMappings(resourcesAssembly.GetManifestResourceNames(), key, out resourceName);
            return !string.IsNullOrEmpty(resourceName) ? resourcesAssembly.GetManifestResourceStream(resourceName) : null;
        }
        static void TryAddResourceNameMappings(string[] names, string key, out string resourceName) {
            resourceName = string.Empty;
            for(int i = 0; i < names.Length; i++) {
                if(names[i].EndsWith(".md", StringComparison.OrdinalIgnoreCase)) {
                    if(mappings.TryAdd(names[i], names[i]) && StringComparer.OrdinalIgnoreCase.Compare(key, names[i]) == 0)
                        resourceName = names[i];
                }
            }
        }
        #endregion
        #region Empty
        public readonly static Pattern Empty = new EmptyPattern();
        //
        sealed class EmptyPattern : Pattern {
            internal EmptyPattern()
                : base(new Func<string, Type>(Type.GetType).Method) {
            }
            public sealed override ProcessingTarget Target {
                get { return ProcessingTarget.None; }
            }
        }
        #endregion
    }
}