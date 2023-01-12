namespace ILSA.Client.Views {
    using System.Text;

    partial class Markdown {
        sealed class CodeBlockElement : BaseBlockElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.CodeBlock;
            }
            public override string ToString() {
                var builder = new StringBuilder();
                int id = ((IElement)this).Id;
                builder.Append("<pre><div class=\"code-block\" id=\"block:" + id + "\">");
                builder.Append("<div class=\"code-block-code\">");
                for(int i = 0; i < Elements.Count; i++)
                    builder.AppendLine(Elements[i].ToString());
                builder.Append("</div>");
                builder.Append("<img class=\"copy\" src=\"Copy\" id=\"block-img:" + id + "\" onclick=\"OnCopyClick\">");
                builder.Append("</div></pre>");
                return builder.ToString();
            }
            protected override void PerformClick(BlockElementType type) {
                switch(type) {
                    case BlockElementType.Block:
                        break;
                    case BlockElementType.Image:
                        var builder = new StringBuilder();
                        for(int i = 0; i < Elements.Count; i++) {
                            var element = Elements[i] as CodeLineElement;
                            if(element == null)
                                continue;
                            builder.AppendLine(element.Content);
                        }
                        try {
                            System.Windows.Forms.Clipboard.SetText(builder.ToString());
                        }
                        catch { }
                        break;
                }
            }
        }
        sealed class CodeBlockBlockParser : IBlockParser {
            int level;
            int position;
            const int requiredTabs = 1;
            const int requiredSpaces = 2;
            const int requiredQuotes = 3;
            //
            enum BlockPrefixParserState {
                None,
                Indent,
                Quote,
                ValidPrefix
            }
            enum ElementState {
                WaitingForOpen,
                WaitingForClose
            }
            ElementState state = ElementState.WaitingForOpen;
            public bool CanProcess(IParserResult result, string line) {
                switch(state) {
                    case ElementState.WaitingForOpen:
                        return ProcessWaitingForOpen(line);
                    case ElementState.WaitingForClose:
                        return ProcessWaitingForClose(line);
                }
                return false;
            }
            bool ProcessWaitingForClose(string line) {
                int quoteCount = 0;
                for(int i = 0; i < line.Length; i++) {
                    char current = line[i];
                    if(current == '`') {
                        quoteCount++;
                        if(quoteCount == requiredQuotes) {
                            state = ElementState.WaitingForOpen;
                            return false;
                        }
                    }
                    else return true;
                }
                return true;
            }
            bool ProcessWaitingForOpen(string line) {
                int tabCount = 0;
                int spaceCount = 0;
                int quoteCount = 0;
                BlockPrefixParserState state = BlockPrefixParserState.None;
                for(int i = 0; i < line.Length; i++) {
                    char current = line[i];
                    switch(state) {
                        case BlockPrefixParserState.None:
                            if(current == ' ' || current == '\t')
                                state = BlockPrefixParserState.Indent;
                            else if(current == '`') {
                                state = BlockPrefixParserState.Quote;
                                quoteCount++;
                            }
                            else return false;
                            break;
                        case BlockPrefixParserState.Indent:
                            if(current == '\t') {
                                tabCount++;
                                state = BlockPrefixParserState.ValidPrefix;
                                if(tabCount % requiredTabs == 0)
                                    level = tabCount / requiredTabs;
                            }
                            else if(current == ' ') {
                                spaceCount++;
                                if(spaceCount % requiredSpaces == 0) {
                                    state = BlockPrefixParserState.ValidPrefix;
                                    level = spaceCount / requiredSpaces;
                                }
                            }
                            break;
                        case BlockPrefixParserState.Quote:
                            if(current == '`') {
                                quoteCount++;
                                if(quoteCount >= requiredQuotes) {
                                    this.state = ElementState.WaitingForClose;
                                    this.position = 0;
                                    return false;
                                }
                                state = BlockPrefixParserState.Quote;
                            }
                            break;
                        case BlockPrefixParserState.ValidPrefix:
                            position = i;
                            return true;
                    }
                }
                level = 0;
                return false;
            }
            public void Process(IParserContext context, IParserResult result, string line) {
                if(!(result.CurrentBlock is CodeBlockElement)) {
                    IBlockElement block = new CodeBlockElement();
                    block.Level = level;
                    result.AddElement(block);
                    result.OpenBlock(block);
                }
                InlineParserState previousState, currentState = InlineParserState.None;
                for(int i = position; i < line.Length; i++) {
                    char current = line[i];
                    switch(currentState) {
                        case InlineParserState.None:
                            currentState = context.ProcessInline(InlineParserState.CodeLine, current);
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
            public void Close(IParserResult result) {
                result.CloseBlocks();
            }
        }
    }
}