namespace ILSA.Client.Views {
    using System.Text;

    partial class Markdown {
        sealed class CodeLineElement : BaseElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.CodeLine;
            }
            public CodeLineElement(string code, string content) {
                this.Code = code;
                this.Content = content;
            }
            public string Code { get; }
            public string Content { get; }
            public override string ToString() {
                return Code ?? string.Empty;
            }
        }
        sealed class CodeLineParser : IElementParser {
            StringBuilder code = new StringBuilder(128);
            StringBuilder content = new StringBuilder(128);
            public InlineParserState Process(IParserContext context, char current) {
                switch(current) {
                    case '<':
                        code.Append("&lt;");
                        content.Append('<');
                        break;
                    case '>':
                        code.Append("&gt;");
                        content.Append('>');
                        break;
                    default:
                        code.Append(current);
                        content.Append(current);
                        break;
                }
                return InlineParserState.CodeLine;
            }
            public void Close(IParserResult result) {
                CodeLineElement element = new CodeLineElement(code.ToString(), content.ToString());
                result.AddElement(element);
                code.Clear();
                content.Clear();
            }
        }
    }
}