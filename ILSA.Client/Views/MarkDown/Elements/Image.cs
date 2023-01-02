namespace ILSA.Client.Views {
    using System.Text;

    partial class Markdown {
        sealed class ImageElement : BaseElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.Image;
            }
            public ImageElement(string uri, string name) {
                this.Uri = uri;
                this.Name = name;
            }
            public string Name {
                get;
                private set;
            }
            public string Uri {
                get;
                private set;
            }
            public override string ToString() {
                var builder = new StringBuilder(128);
                builder.Append("<img src=\"" + Uri + "\" title=\"" + Name + "\" >");
                return builder.ToString();
            }
        }
        sealed class ImageParser : IElementParser {
            enum ImageParserState {
                Img,
                Name,
                Uri,
            }
            string name;
            string uri;
            ImageParserState state;
            public InlineParserState Process(IParserContext context, char current) {
                switch(state) {
                    case ImageParserState.Img:
                        if(current == '!') {
                            state = ImageParserState.Name;
                            break;
                        }
                        return InlineParserState.None;
                    case ImageParserState.Name:
                        switch(current) {
                            case '[':
                                state = ImageParserState.Name;
                                break;
                            case ']':
                                state = ImageParserState.Uri;
                                break;
                            default:
                                name += current;
                                break;
                        }
                        break;
                    case ImageParserState.Uri:
                        switch(current) {
                            case '(':
                                break;
                            case ')':
                                return InlineParserState.None;
                            default:
                                uri += current;
                                break;
                        }
                        break;
                }
                return InlineParserState.Image;
            }
            public void Close(IParserResult result) {
                if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(uri))
                    result.AddElement(new ImageElement(uri, name));
                uri = name = null;
                state = ImageParserState.Img;
            }
        }
    }
}