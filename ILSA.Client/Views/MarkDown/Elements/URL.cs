namespace ILSA.Client.Views {
    using System.Text;

    partial class Markdown {
        sealed class URLElement : BaseElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.URL;
            }
            public URLElement(string url) {
                URL = url;
            }
            public string URL {
                get;
                private set;
            }
            bool IsWebAddress(string str) {
                return
                    str.StartsWith("http://", System.StringComparison.OrdinalIgnoreCase) ||
                    str.StartsWith("https://", System.StringComparison.OrdinalIgnoreCase) ||
                    str.StartsWith("ftp://", System.StringComparison.OrdinalIgnoreCase) ||
                    str.StartsWith("file://", System.StringComparison.OrdinalIgnoreCase);
            }
            bool IsEmailAddress(string str) {
                int domainSeparatorPos = str.IndexOf('@');
                if(domainSeparatorPos < 0)
                    return false;
                int lastDotPosition = str.LastIndexOf('.');
                return (lastDotPosition > domainSeparatorPos);
            }
            public override string ToString() {
                if(string.IsNullOrEmpty(URL))
                    return string.Empty;
                var builder = new StringBuilder(128);
                bool isEmail = IsEmailAddress(URL);
                if(!isEmail && !IsWebAddress(URL)) {
                    builder.Append("<" + URL + ">");
                    return builder.ToString();
                }
                string link = isEmail ? "mailto:" + URL : URL;
                builder.Append("<a href=\"" + link + "\">" + URL + "</a>");
                return builder.ToString();
            }
        }
        sealed class UrlParser : IElementParser {
            string url;
            public InlineParserState Process(IParserContext context, char current) {
                switch(current) {
                    case '<':
                        break;
                    case '>':
                        break;
                    case ' ':
                        return  context.ProcessText(current);
                    default:
                        url += current;
                        break;
                }
                return InlineParserState.URL;
            }
            public void Close(IParserResult result) {
                if(!string.IsNullOrEmpty(url)) 
                    result.AddElement(new URLElement(url));
                url = null;
            }
        }
    }
}