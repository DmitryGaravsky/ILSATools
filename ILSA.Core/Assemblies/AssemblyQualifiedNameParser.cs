namespace ILSA.Core.Sources {
    using System;
    using System.Collections.Generic;

    public sealed partial class AssembliesSourceForClasses {
        static AssemblyQualifiedNameParser ParseAssemblyQualifiedName(string name) {
            int startIndex = 0, sectionStart, sectionEnd;
            var parser = new AssemblyQualifiedNameParser(name);
            while((sectionEnd = parser.GetSectionEnd(startIndex)) > 0) {
                sectionStart = parser.GetSectionStart(startIndex);
                switch(name[sectionStart]) {
                    case 'V':
                    case 'v':
                        if(!parser.TryExtractVersion(sectionStart, sectionEnd))
                            parser.TryExtractName(sectionStart, sectionEnd);
                        break;
                    case 'C':
                    case 'c':
                        if(!parser.TryExtractCulture(sectionStart, sectionEnd))
                            parser.TryExtractName(sectionStart, sectionEnd);
                        break;
                    case 'P':
                    case 'p':
                        if(!parser.TryExtractPublicKeyToken(sectionStart, sectionEnd))
                            parser.TryExtractName(sectionStart, sectionEnd);
                        break;
                    default:
                        parser.TryExtractName(sectionStart, sectionEnd);
                        break;
                }
                startIndex = sectionEnd + 1;
            }
            return parser;
        }
        //
        sealed class AssemblyQualifiedNameParser {
            readonly string name;
            readonly Dictionary<Part, Tuple<int, int>> bounds;
            Part parts;
            public AssemblyQualifiedNameParser(string name) {
                this.name = name;
                this.bounds = new Dictionary<Part, Tuple<int, int>>(5);
            }
            [Flags]
            public enum Part {
                None = 0,
                TypeName = 1,
                AssemblyName = 2,
                Version = 4, Culture = 8, PublicKeyToken = 16,
                Unknown = 32
            }
            public bool TryExtractVersion(int start, int end) {
                return TryExtractPart(Part.Version, "version=", start, end);
            }
            public bool TryExtractCulture(int start, int end) {
                return TryExtractPart(Part.Culture, "culture=", start, end);
            }
            public bool TryExtractPublicKeyToken(int start, int end) {
                return TryExtractPart(Part.PublicKeyToken, "publickeytoken=", start, end);
            }
            bool TryExtractPart(Part partToExtract, string key, int start, int end) {
                if(name.IndexOf(key, start, end - start, StringComparison.OrdinalIgnoreCase) == start) {
                    if((parts & partToExtract) == Part.None) {
                        bounds.Add(partToExtract, new Tuple<int, int>(start, end - start));
                        parts |= partToExtract;
                    }
                    return true;
                }
                return false;
            }
            public void TryExtractName(int start, int end) {
                if((parts & Part.AssemblyName) == Part.None) {
                    bounds.Add(Part.AssemblyName, new Tuple<int, int>(start, end - start));
                    parts |= Part.AssemblyName;
                }
                else if((parts & Part.Unknown) == Part.None) 
                    parts |= Part.Unknown;
            }
            public string GetVersion(int partsCount) {
                if((parts & Part.Version) == Part.Version) {
                    var section = bounds[Part.Version];
                    if(Version.TryParse(name.Substring(section.Item1 + 8, section.Item2 - 8), out Version v))
                        return v.ToString(partsCount);
                }
                return string.Empty;
            }
            public string BuildAssemblyShortName() {
                if((parts & Part.AssemblyName) == Part.AssemblyName) {
                    var section = bounds[Part.AssemblyName];
                    return name.Substring(section.Item1, section.Item2);
                }
                return name;
            }
            public int GetSectionStart(int start) {
                int sIndex = start;
                while(SkipSectionLeadingCharacter(name[sIndex]))
                    sIndex++;
                return sIndex;
            }
            public int GetSectionEnd(int start) {
                if(start >= name.Length)
                    return -1;
                int eIndex = start;
                while(eIndex < name.Length) {
                    if(name[eIndex] == ',')
                        return eIndex;
                    if(name[eIndex] == '[')
                        eIndex = SkipGenericDefinition(name, eIndex + 1);
                    else eIndex++;
                }
                return eIndex;
            }
            static bool SkipSectionLeadingCharacter(char leadingCharacter) {
                return char.IsWhiteSpace(leadingCharacter) || leadingCharacter == '.';
            }
            static int SkipGenericDefinition(string name, int start) {
                int bracketsCount = 1;
                for(int i = start; i < name.Length; ++i) {
                    if(name[i] == '[')
                        ++bracketsCount;
                    if(name[i] == ']') {
                        if(--bracketsCount == 0)
                            return i;
                    }
                }
                return start;
            }
        }
    }
}