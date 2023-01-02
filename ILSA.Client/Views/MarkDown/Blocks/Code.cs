namespace ILSA.Client.Views {
    using System.Text;

    partial class Markdown {
        sealed class CodeElement : BaseBlockElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.Code;
            }
            public CodeElement(string content) {
                this.Content = content;
            }
            public string Content {
                get;
                private set;
            }
            public override string ToString() {
                var codeBuilder = new StringBuilder(128);
                codeBuilder.Append("<div class=\"code-inline-element\">");
                codeBuilder.Append(Content);
                codeBuilder.Append("</div>");
                return codeBuilder.ToString();
            }
        }
        sealed class CodeElementParser : IElementParser {
            enum ElementState {
                WaitingForOpen,
                WaitingForClose
            }
            enum CodeParserState {
                None,
                Open,
                Value,
                Close
            }
            string content;
            CodeParserState state;
            ElementState elementState;
            public InlineParserState Process(IParserContext context, char current) {
                switch(state) {
                    case CodeParserState.None:
                        if(current == '`')
                            state = CodeParserState.Open;
                        break;
                    case CodeParserState.Open:
                        if(current == '`')
                            state = CodeParserState.None;
                        else {
                            content += current;
                            state = CodeParserState.Value;
                        }
                        break;
                    case CodeParserState.Value:
                        if(current == '`')
                            state = CodeParserState.Close;
                        else
                            content += current;
                        break;
                    case CodeParserState.Close:
                        elementState = ElementState.WaitingForClose;
                        return InlineParserState.Text;
                }
                return InlineParserState.CodeElement;
            }
            public void Close(IParserResult result) {
                if(elementState == ElementState.WaitingForClose && !string.IsNullOrEmpty(content))
                    result.AddElement(new CodeElement(content));
                this.content = null;
                this.state = CodeParserState.None;
            }
        }
    }
}