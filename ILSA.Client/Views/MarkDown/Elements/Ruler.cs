namespace ILSA.Client.Views {
    using System.Text;

    partial class Markdown {
        sealed class RulerBlock : BaseElement {
            protected sealed override ElementType GetElementType() {
                return ElementType.Ruler;
            }
            public override string ToString() {
                var builder = new StringBuilder();
                builder.Append("<div class=\"separator\"></div>");
                return builder.ToString();
            }
        }
        sealed class RulerBlockParser : IBlockParser {
            enum RulerBlockParserState {
                None,
                Candidate,
                ValidElement
            }
            RulerBlockParserState state;
            public bool CanProcess(IParserResult result, string line) {
                int tokenCount = 0;
                const int tokenCountReq = 3;
                char token = '\0';
                state = RulerBlockParserState.None;
                for(int i = 0; i < line.Length; i++) {
                    char current = line[i];
                    switch(state) {
                        case RulerBlockParserState.None:
                            switch(current) {
                                case '*':
                                case '-':
                                case '_':
                                    token = current;
                                    tokenCount++;
                                    state = RulerBlockParserState.Candidate;
                                    break;
                            }
                            break;
                        case RulerBlockParserState.Candidate:
                            if(current == token) {
                                tokenCount++;
                                if(tokenCount >= tokenCountReq)
                                    state = RulerBlockParserState.ValidElement;
                            }
                            return false;
                        case RulerBlockParserState.ValidElement:
                            return true;
                    }
                }
                return (state == RulerBlockParserState.ValidElement);
            }
            public void Process(IParserContext context, IParserResult result, string line) {
                result.AddElement(new RulerBlock(), directly: true);
            }
            public void Close(IParserResult result) { }
        }
    }
}