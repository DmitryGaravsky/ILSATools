namespace ILSA.Client.Views {
    using System.Text;

    partial class Markdown {
        sealed class LinkElement : BaseElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.Link;
            }
            public LinkElement(string link, string displayText) {
                this.Link = link;
                this.DisplayText = displayText;
            }
            public string DisplayText {
                get; 
                private set;
            }
            public string Link {
                get;
                private set;
            }
            public override string ToString() {
                var builder = new StringBuilder(128);
                builder.Append("<a href=\"" + Link + "\">" + DisplayText + "</a>");
                return builder.ToString();
            }
        }
        sealed class LinkParser : IElementParser {
            enum LinkParserState {
                DisplayText,
                Link,
            }
            string displayText;
            string link;
            LinkParserState state;
            public InlineParserState Process(IParserContext context, char current) {
                switch(state) {
                    case LinkParserState.DisplayText:
                        switch(current) {
                            case '[':
                                state = LinkParserState.DisplayText;
                                break;
                            case ']':
                                state = LinkParserState.Link;
                                break;
                            default:
                                displayText += current;
                                break;
                        }
                        break;
                    case LinkParserState.Link:
                        switch(current) {
                            case '(':
                                break;
                            case ')':
                                return InlineParserState.None;
                            default:
                                link += current;
                                break;
                        }
                        break;
                }
                return InlineParserState.Link;
            }
            public void Close(IParserResult result) {
                if(!string.IsNullOrEmpty(displayText) && !string.IsNullOrEmpty(link))
                    result.AddElement(new LinkElement(link, displayText));
                link = displayText = null;
                state = LinkParserState.DisplayText;
            }
        }
    }
}