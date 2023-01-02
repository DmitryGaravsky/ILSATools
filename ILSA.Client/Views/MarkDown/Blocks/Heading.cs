namespace ILSA.Client.Views {
    using System.Text;

    partial class Markdown {
        enum HeadingType {
            H1,
            H2,
            H3,
            H4,
            H5,
            H6
        }
        sealed class HeadingElement : BaseBlockElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.Heading;
            }
            public HeadingElement() {
                HeadingType = (Markdown.HeadingType)(-1);
            }
            public HeadingType HeadingType {
                get;
                set;
            }
            public override string ToString() {
                var builder = new StringBuilder();
                if(HeadingType > HeadingType.H6) {
                    builder.Append("<p>");
                    for(int i = 0; i < (int)HeadingType; i++)
                        builder.Append('#');
                    for(int i = 0; i < Elements.Count; i++)
                        builder.Append(Elements[i]);
                    builder.Append("</p>");
                }
                else {
                    builder.Append("<" + HeadingType + ">");
                    for(int i = 0; i < Elements.Count; i++)
                        builder.Append(Elements[i]);
                    builder.Append("</" + HeadingType + ">");
                }
                return builder.ToString();
            }
        }
        sealed class HeadingBlockParser : IBlockParser {
            enum HeadingElementParserState {
                Prefix,
                Value,
            }
            public bool CanProcess(IParserResult result, string line) {
                return line.StartsWith("#", System.StringComparison.Ordinal);
            }
            public void Process(IParserContext context, IParserResult result, string line) {
                InlineParserState previousState, currentState = InlineParserState.None;
                HeadingElement element = new HeadingElement();
                result.AddElement(element);
                result.OpenBlock(element);
                var blockState = HeadingElementParserState.Prefix;
                for(int i = 0; i < line.Length; i++) {
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
                                    switch(blockState) {
                                        case HeadingElementParserState.Prefix:
                                            switch(current) {
                                                case '#':
                                                    element.HeadingType++;
                                                    blockState = HeadingElementParserState.Prefix;
                                                    break;
                                                case ' ':
                                                    currentState = context.ProcessText(current);
                                                    blockState = HeadingElementParserState.Value;
                                                    break;
                                            }
                                            break;
                                        case HeadingElementParserState.Value:
                                            currentState = context.ProcessText(current);
                                            break;
                                    }
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