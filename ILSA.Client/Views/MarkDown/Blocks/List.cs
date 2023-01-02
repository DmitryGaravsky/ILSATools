namespace ILSA.Client.Views {
    using System.Text;

    partial class Markdown {
        enum ListType {
            None,
            Ordered,
            Unordered
        }
        sealed class ListBlockElement : BaseBlockElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.List;
            }
            public ListBlockElement(ListType type) {
                ListType = type;
            }
            public ListType ListType {
                get;
                private set;
            }
            public override string ToString() {
                if(ListType == ListType.None)
                    return string.Empty;
                var builder = new StringBuilder();
                bool isOrdered = (ListType == ListType.Ordered);
                string open = isOrdered ? "<ol>" : "<ul>";
                builder.Append(open);
                for(int i = 0; i < Elements.Count; i++)
                    builder.Append(Elements[i]);
                string close = isOrdered ? "</ol>" : "</ul>";
                builder.Append(close);
                return builder.ToString();
            }
        }
        sealed class ListItemElement : BaseBlockElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.ListItem;
            }
            public override string ToString() {
                var builder = new StringBuilder();
                builder.Append("<li>");
                for(int i = 0; i < Elements.Count; i++)
                    builder.Append(Elements[i].ToString());
                builder.Append("</li>");
                return builder.ToString();
            }
        }
        sealed class ListBlockParser : IBlockParser {
            ListType type;
            int level;
            int position;
            public enum PrefixParserState {
                None,
                Indent,
                PrefixCandidateOrdered,
                PrefixCandidateUnordered,
                ValidPrefix
            }
            public bool CanProcess(IParserResult result, string line) {
                if(string.IsNullOrEmpty(line))
                    return false;
                PrefixParserState state = PrefixParserState.None;
                int tabCount = 0, spaceCount = 0;
                const int requiredTabs = 1;
                const int requiredSpaces = 2;
                level = 0;
                for(int i = 0; i < line.Length; i++) {
                    char ch = line[i];
                    int num = ch - '0';
                    switch(state) {
                        case PrefixParserState.None:
                            if(num > 0 && num < 10)
                                state = PrefixParserState.PrefixCandidateOrdered;
                            else if(ch == '-' || ch == '+' || ch == '*')
                                state = PrefixParserState.PrefixCandidateUnordered;
                            else if(ch == '\t') {
                                tabCount++;
                                state = PrefixParserState.Indent;
                            }
                            else if(ch == ' ') {
                                spaceCount++;
                                state = PrefixParserState.Indent;
                            }
                            else return false;
                            break;
                        case PrefixParserState.Indent:
                            if(ch == '\t') {
                                tabCount++;
                                if(tabCount % requiredTabs == 0) {
                                    state = PrefixParserState.None;
                                    level = tabCount / requiredTabs;
                                }
                            }
                            else if(ch == ' ') {
                                spaceCount++;
                                if(spaceCount % requiredSpaces == 0) {
                                    state = PrefixParserState.None;
                                    level = spaceCount / requiredSpaces;
                                }
                            }
                            break;
                        case PrefixParserState.PrefixCandidateOrdered:
                            if(ch == '.')
                                state = PrefixParserState.PrefixCandidateOrdered;
                            else if(ch == ' ') {
                                type = ListType.Ordered;
                                state = PrefixParserState.ValidPrefix;
                            }
                            else return false;
                            break;
                        case PrefixParserState.PrefixCandidateUnordered:
                            if(ch == ' ') {
                                type = ListType.Unordered;
                                state = PrefixParserState.ValidPrefix;
                            }
                            else return false;
                            break;
                        case PrefixParserState.ValidPrefix:
                            position = i;
                            return true;
                    }
                }
                return false;
            }
            public void Process(IParserContext context, IParserResult result, string line) {
                var currentListBlock = result.CurrentBlock;
                if(currentListBlock != null) {
                    result.CloseBlocks(x => x.Type != ElementType.List);
                    var currentBlock = result.CurrentBlock;
                    if(currentBlock != null) {
                        if(currentBlock.Level < level) {
                            IBlockElement block = new ListBlockElement(type);
                            block.Level = level;
                            currentBlock.Elements.Add(block);
                            result.OpenBlock(block);
                        }
                        else if(currentBlock.Level > level) {
                            result.CloseBlocks(x => x.Level != level);
                        }
                    }
                    else CreateBlock(result);
                }
                else CreateBlock(result);

                IBlockElement element = new ListItemElement();
                element.Level = level;
                result.CurrentBlock.Elements.Add(element);
                result.OpenBlock(element);

                InlineParserState previousState, currentState = InlineParserState.None;
                for(int i = position; i < line.Length; i++) {
                    char current = line[i];
                    switch(currentState) {
                        case InlineParserState.None:
                            switch(current) {
                                case '!':
                                    currentState = context.ProcessInline(InlineParserState.Image, current);
                                    break;
                                case '[':
                                    currentState = context.ProcessInline(InlineParserState.Link, current);
                                    break;
                                case '<':
                                    currentState = context.ProcessInline(InlineParserState.URL, current);
                                    break;
                                case '*':
                                case '~':
                                    currentState = context.ProcessInline(InlineParserState.Emphasis, current);
                                    break;
                                case '`':
                                    currentState = context.ProcessInline(InlineParserState.CodeElement, current);
                                    break;
                                default:
                                    currentState = context.ProcessText(current);
                                    break;
                            }
                            break;
                        default:
                            previousState = currentState;
                            currentState = context.ProcessInline(currentState, current);
                            if(currentState != previousState)
                                context.CloseInline(previousState, result);
                            break;
                    }
                }
                if(currentState != InlineParserState.None)
                    context.CloseInline(currentState, result);
                this.level = 0;
            }
            void CreateBlock(IParserResult result) {
                IBlockElement block = new ListBlockElement(type);
                block.Level = level;
                result.AddElement(block, directly: true);
                result.OpenBlock(block);
            }
            public void Close(IParserResult result) {
                result.CloseBlocks();
            }
        }
    }
}