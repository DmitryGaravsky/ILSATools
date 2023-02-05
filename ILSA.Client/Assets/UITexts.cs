namespace ILSA.Client.Assets {
    using System.Collections.Generic;

    public sealed class UITexts {
        readonly static Dictionary<string, string> uiTexts = new Dictionary<string, string> {
            { nameof(AppTitle), "Static Analysis (Visual Client)" },
            // Toolbar Buttons
            { nameof(AddDescription), "Load assembly from disk (Ctrl+N)."},
            { nameof(ResetDescription), "Reload all assemblies and clean all the analysis results (Ctrl+R)."},
            { nameof(RunAnalysisName), "Run Analysis"},
            { nameof(RunAnalysisDescription), "Run analysis for loaded types and methods with specified subset of patterns.\r\nYou can configure patterns set and their severity level using Patterns page."},
            { nameof(PrevResultDescription), "Navigate to previous result (Ctrl+LeftArrow)."},
            { nameof(NextResultDescription), "Navigate to next result (F3 or Ctrl+RightArrow)."},
            { nameof(ClassesName), "Classes"},
            { nameof(ClassesDescription), "Show all classes and methods from loaded assemblies."},
            { nameof(BacktraceName), "Backtrace"},
            { nameof(BacktraceDescription), "Show only affected methods and their callers."},
            // Tabs
            { nameof(AssembliesName), "Assemblies"},
            { nameof(AssembliesDescription), "At this page you can load and inspect types and methods."},
            { nameof(PatternsName), "Patterns"},
            { nameof(PatternsDescription), "At this page you can load and activate\\deactivate available patterns."},
            // Footer Buttons
            { nameof(SaveAssembliesDescription), "Save loaded assemblies configuration into *.assemblies file."},
            { nameof(SavePatternsDescription), "Save patterns configuration into *.patterns file."},
            { nameof(LoadName), "Load Assemblies or Patterns" },
            { nameof(LoadDescription), "Load assemblies or patterns configuration from *.assemblies or *.patterns file."},
        };
        public string AppTitle {
            get { return uiTexts[nameof(AppTitle)]; }
        }
        // Toolbar Buttons
        public string AddDescription {
            get { return uiTexts[nameof(AddDescription)]; }
        }
        public string ResetDescription {
            get { return uiTexts[nameof(ResetDescription)]; }
        }
        public string RunAnalysisName {
            get { return uiTexts[nameof(RunAnalysisName)]; }
        }
        public string RunAnalysisDescription {
            get { return uiTexts[nameof(RunAnalysisDescription)]; }
        }
        public string PrevResultDescription {
            get { return uiTexts[nameof(PrevResultDescription)]; }
        }
        public string NextResultDescription {
            get { return uiTexts[nameof(NextResultDescription)]; }
        }
        public string ClassesName {
            get { return uiTexts[nameof(ClassesName)]; }
        }
        public string ClassesDescription {
            get { return uiTexts[nameof(ClassesDescription)]; }
        }
        public string BacktraceName {
            get { return uiTexts[nameof(BacktraceName)]; }
        }
        public string BacktraceDescription {
            get { return uiTexts[nameof(BacktraceDescription)]; }
        }
        // Tabs
        public string AssembliesName {
            get { return uiTexts[nameof(AssembliesName)]; }
        }
        public string AssembliesDescription {
            get { return uiTexts[nameof(AssembliesDescription)]; }
        }
        public string PatternsName {
            get { return uiTexts[nameof(PatternsName)]; }
        }
        public string PatternsDescription {
            get { return uiTexts[nameof(PatternsDescription)]; }
        }
        // Footer Buttons
        public string SaveAssembliesDescription {
            get { return uiTexts[nameof(SaveAssembliesDescription)]; }
        }
        public string SavePatternsDescription {
            get { return uiTexts[nameof(SavePatternsDescription)]; }
        }
        public string LoadName {
            get { return uiTexts[nameof(LoadName)]; }
        }
        public string LoadDescription {
            get { return uiTexts[nameof(LoadDescription)]; }
        }
    }
}