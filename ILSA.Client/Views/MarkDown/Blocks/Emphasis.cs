namespace ILSA.Client.Views {
    using System.Text;

    partial class Markdown {
        enum EmphasisType {
            None,
            Italic,
            Bold,
            Strikethrough
        }
        sealed class EmphasisElement : BaseBlockElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.Emphasis;
            }
            public EmphasisElement(EmphasisType type) {
                this.Type = type;
            }
            public EmphasisType Type {
                get;
                private set;
            }
            public override string ToString() {
                var builder = new StringBuilder(128);
                string open = string.Empty;
                string close = string.Empty;
                switch(Type) {
                    case EmphasisType.Italic:
                        open += "<em>";
                        close += "</em>";
                        break;
                    case EmphasisType.Bold:
                        open += "<strong>";
                        close = "</strong>" + close;
                        break;
                    case EmphasisType.Strikethrough:
                        open += "<s>";
                        close = "</s>" + close;
                        break;
                }
                builder.Append(open);
                for(int i = 0; i < Elements.Count; i++)
                    builder.Append(Elements[i]);
                builder.Append(close);
                return builder.ToString();
            }
        }
        sealed class EmphasisParser : IElementParser {
            enum ElementState {
                WaitingForOpen,
                WaitingForClose
            }
            enum EmphasisParserState {
                None,
                Italic,
                Bold,
                Strikethrough
            }
            EmphasisType type = EmphasisType.None;
            EmphasisParserState state;
            ElementState elementState;
            public InlineParserState Process(IParserContext context, char current) {
                switch(state) {
                    case EmphasisParserState.None:
                        switch(current) {
                            case '*':
                                state = EmphasisParserState.Italic;
                                break;
                            case '~':
                                state = EmphasisParserState.Strikethrough;
                                break;
                            default:
                                return context.ProcessText(current);
                        }
                        break;
                    case EmphasisParserState.Italic:
                        switch(current) {
                            case '*':
                                state = EmphasisParserState.Bold;
                                break;
                            default:
                                type = EmphasisType.Italic;
                                return context.ProcessText(current);
                        }
                        break;
                    case EmphasisParserState.Bold:
                        switch(current) {
                            case '*':
                                type = EmphasisType.None;
                                state = EmphasisParserState.None;
                                break;
                            default:
                                type = EmphasisType.Bold;
                                return context.ProcessText(current);
                        }
                        break;
                    case EmphasisParserState.Strikethrough:
                        switch(current) {
                            case '~':
                                type = EmphasisType.None;
                                state = EmphasisParserState.None;
                                break;
                            default:
                                type = EmphasisType.Strikethrough;
                                return context.ProcessText(current);
                        }
                        break;
                }
                return InlineParserState.Emphasis;
            }
            public void Close(IParserResult result) {
                switch(elementState) {
                    case ElementState.WaitingForOpen:
                        var element = new EmphasisElement(type);
                        result.AddElement(element);
                        result.OpenBlock(element);
                        elementState = ElementState.WaitingForClose;
                        break;
                    case ElementState.WaitingForClose:
                        if(result.TryCloseBlock(x => x.Type == ElementType.Emphasis))
                            elementState = ElementState.WaitingForOpen;
                        break;
                }
                this.type = EmphasisType.None;
                this.state = EmphasisParserState.None;
            }
        }
    }
}