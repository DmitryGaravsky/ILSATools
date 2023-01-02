namespace ILSA.Client.Views {
    partial class Markdown {
        sealed class TextElement : BaseElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.Text;
            }
            public TextElement(string content) {
                this.Content = content;
            }
            public string Content {
                get; 
                private set;
            }
            public override string ToString() { 
                return Content;
            }
        }
        sealed class TextElementParser : IElementParser {
            string content;
            public InlineParserState Process(IParserContext context, char current) {
                switch(current) {
                    case '!':
                        return context.ProcessInline(InlineParserState.Image, current);
                    case '[':
                        return context.ProcessInline(InlineParserState.Link, current);
                    case '<':
                        return context.ProcessInline(InlineParserState.URL, current);
                    case '*':
                    case '~':
                        return context.ProcessInline(InlineParserState.Emphasis, current);
                    case '`':
                        return context.ProcessInline(InlineParserState.CodeElement, current);
                    default:
                        content += current;
                        return InlineParserState.Text;
                }
            }
            public void Close(IParserResult result) {
                if(!string.IsNullOrEmpty(content)) 
                    result.AddElement(new TextElement(content));
                content = null;
            }
        }
    }
}