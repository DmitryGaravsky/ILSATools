namespace ILSA.Client.Views {
    using System.Text;

    partial class Markdown {
        sealed class BlockquoteElement : BaseBlockElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.Blockquote;
            }
            public override string ToString() {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("<div class=\"blockquote\">");
                builder.AppendLine("<div class=\"stripe\"></div>");
                builder.Append("<p>");
                for(int i = 0; i < Elements.Count; i++)
                    builder.Append(Elements[i]);
                builder.Append("</p></div>");
                return builder.ToString();
            }
        }
        sealed class BlockquoteBlockParser: IBlockParser {
            public bool CanProcess(IParserResult result, string line) {
                return line.StartsWith(">", System.StringComparison.Ordinal);
            }
            public void Process(IParserContext context, IParserResult result, string line) {
                result.CloseBlocks();
                InlineParserState previousState, currentState = InlineParserState.None;
                BlockquoteElement block = new BlockquoteElement();
                result.AddElement(block);
                result.OpenBlock(block);
                for(int i = 0; i < line.Length; i++) {
                    char current = line[i];
                    switch(currentState) {
                        case InlineParserState.None:
                            switch(current) {
                                case '>':
                                    break;
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
                                    currentState = context.ProcessInline(InlineParserState.Text, current);
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
                result.CloseBlocks();
            }
            public void Close(IParserResult result) {
                result.CloseBlocks();
            }
        }
    }
}