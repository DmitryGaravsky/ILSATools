namespace ILSA.Client.Views {
    using System.Text;

    partial class Markdown {
        sealed class ParagraphBlock : BaseBlockElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.Paragraph;
            }
            public override string ToString() {
                var builds = new StringBuilder();
                builds.Append("<p>");
                for(int i = 0; i < Elements.Count; i++)
                    builds.AppendLine(Elements[i].ToString());
                builds.Append("</p>");
                return builds.ToString();
            }
        }
        sealed class ParagraphBlockParser : IBlockParser {
            public bool CanProcess(IParserResult result, string line) {
                if(!string.IsNullOrEmpty(line))
                    return true;
                result.CloseBlocks();
                return false;
            }
            public void Process(IParserContext context, IParserResult result, string line) {
                InlineParserState previousState, currentState = InlineParserState.None;
                if(!(result.CurrentBlock is ParagraphBlock)) {
                    ParagraphBlock block = new ParagraphBlock();
                    result.AddElement(block, directly: true);
                    result.OpenBlock(block);
                }
                for(int i = 0; i < line.Length; i++) {
                    char current = line[i];
                    switch(currentState) {
                        case InlineParserState.None:
                            switch(current) {
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
            }
            public void Close(IParserResult result) {
                result.CloseBlocks();
            }
        }
    }
}