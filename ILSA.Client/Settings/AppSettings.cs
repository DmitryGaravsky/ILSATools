namespace ILSA.Client.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using DevExpress.Utils;
    using DevExpress.Utils.Design;
    using DevExpress.XtraEditors;
    using ILSA.Core.Patterns;

    public sealed class AppSettings {
        const string settingsXml = "ILSA.Client.Settings.Settings.xml";
        const string settingsXmlExt = "*.settings";
        const string settingsXmlDefaultFileName = "user.settings";
        const string userSettingsRegistryPath = "Software\\ILSA\\Client\\";
        const string userSettingsRegistryPathEntry = "UserSettingsRoot";
        const string userSettingsRoot = @"ILSA\Client\";
        //
        static string CommonDocumentsPath {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments); }
        }
        static string UserSettingsPath {
            get {
                PropertyStore store = new PropertyStore(userSettingsRegistryPath);
                if(store == null)
                    return GetDefaultUserSettingsPath();
                store.Restore();
                return store.RestoreProperty(userSettingsRegistryPathEntry, GetDefaultUserSettingsPath());
            }
        }
        static AppSettings predefinedAppSettings;
        public static AppSettings CurrentAppSettings {
            get {
                if(predefinedAppSettings == null)
                    predefinedAppSettings = LoadPredefinedAppSettings();
                return predefinedAppSettings;
            }
        }
        static AppSettings LoadPredefinedAppSettings() {
            using(var stream = typeof(AppSettings).Assembly.GetManifestResourceStream(settingsXml))
                return LoadAppSettings(stream);
        }
        public static AppSettings LoadAppSettings() {
            return MergeUserSettings(CurrentAppSettings, UserSettingsPath);
        }
        public static AppSettings LoadAppSettings(string path) {
            var userSettings = LoadUserAppSettings(path);
            CurrentAppSettings.Merge(userSettings, path);
            return userSettings;
        }
        public static void SaveAppSettings(string path, string[] assemblies, Pattern[] patterns) {
            var settings = new AppSettings();
            settings.PathToUserSettings = path;
            SaveSettingsCore(assemblies, patterns, settings);
        }
        public static void SaveAppSettings(string[] assemblies, Pattern[] patterns) {
            var userSettings = EnsureUserAppSettings();
            SaveSettingsCore(assemblies, patterns, userSettings);
        }
        static void SaveSettingsCore(string[] assemblies, Pattern[] patterns, AppSettings userSettings) {
            userSettings.Assemblies = assemblies;
            userSettings.Patterns = patterns
                .Select(x => x.GetAssembly().Location).Distinct().ToArray();
            PatternInfo[] infos = new PatternInfo[patterns.Length];
            for(int i = 0; i < infos.Length; i++) {
                infos[i] = new PatternInfo {
                    Name = patterns[i].GetNameInAssembly(),
                    Severity = patterns[i].GetSeverity()
                };
            }
            userSettings.PatternInfos = infos;
            SaveAppSettings(userSettings);
        }

        [XmlArray]
        public string[] Assemblies;
        [XmlArray]
        public string[] Patterns;
        [XmlArray, XmlArrayItem(typeof(PatternInfo))]
        public PatternInfo[] PatternInfos;
        [XmlIgnore]
        string PathToUserSettings;
        static string GetDefaultUserSettingsPath() {
            return Path.Combine(CommonDocumentsPath, userSettingsRoot);
        }
        [XmlIgnore]
        static readonly IList<AppSettings> userSettings = new List<AppSettings>();
        static AppSettings MergeUserSettings(AppSettings settings, string userSettingsPath) {
            if(Directory.Exists(userSettingsPath)) {
                var files = Directory.GetFiles(userSettingsPath, settingsXmlExt);
                for(int i = 0; i < files.Length; i++) {
                    var userSettings = LoadUserAppSettings(files[i]);
                    if(userSettings == null)
                        continue;
                    settings.Merge(userSettings, files[i]);
                    AppSettings.userSettings.Add(userSettings);
                }
            }
            return settings;
        }
        [XmlIgnore]
        static Type[] ExtraTypes = new Type[] {
            typeof(PatternInfo)
        };
        static AppSettings LoadUserAppSettings(string path) {
            try {
                using(var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    return LoadAppSettings(fs);
            }
            catch { return null; }
        }
        static AppSettings LoadAppSettings(Stream stream) {
            try {
                return SafeXml.Deserialize<AppSettings>(stream, ExtraTypes);
            }
            catch { return null; }
        }
        static AppSettings EnsureUserAppSettings() {
            var settings = userSettings.FirstOrDefault();
            if(settings == null) {
                settings = new AppSettings();
                settings.PathToUserSettings = UserSettingsPath + settingsXmlDefaultFileName;
                userSettings.Add(settings);
            }
            return settings;
        }
        static void SaveAppSettings(AppSettings settings) {
            try {
                var file = GetFile(settings.PathToUserSettings);
                using(var writer = file.OpenWrite()) {
                    writer.SetLength(0);
                    SafeXml.Serialize<AppSettings>(writer, settings, ExtraTypes);
                }
            }
            catch(Exception e) {
                XtraMessageBox.Show($"Unable to save settings. {e.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        static FileInfo GetFile(string fileName) {
            var file = new FileInfo(fileName);
            if(!file.Exists)
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            else file.IsReadOnly = false;
            return file;
        }
        void Merge(AppSettings userSettings, string path) {
            if(userSettings == null)
                return;
            userSettings.PathToUserSettings = path;
            var aItems = new List<string>(Assemblies ?? new string[0]);
            var assemblies = userSettings.Assemblies ?? new string[0];
            for(int i = 0; i < assemblies.Length; i++) 
                aItems.Add(assemblies[i]);
            Assemblies = aItems.ToArray();
            var pItems = new List<string>(Patterns ?? new string[0]);
            var patterns = userSettings.Patterns ?? new string[0];
            for(int i = 0; i < patterns.Length; i++)
                pItems.Add(patterns[i]);
            Patterns = pItems.ToArray();
            var pInfos = new List<PatternInfo>(PatternInfos ?? new PatternInfo[0]);
            var patternInfos = userSettings.PatternInfos ?? new PatternInfo[0];
            for(int i = 0; i < patternInfos.Length; i++)
                pInfos.Add(patternInfos[i]);
            PatternInfos = pInfos.ToArray();
        }
        public sealed class PatternInfo {
            [XmlAttribute]
            public string Name;
            [XmlAttribute]
            public string Severity;
        }
    }
}