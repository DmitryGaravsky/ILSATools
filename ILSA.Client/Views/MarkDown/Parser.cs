namespace ILSA.Client.Views {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class Markdown {
        public static Result ToHtml(string markdown) {
            var parserResult = new Result();
            if(string.IsNullOrEmpty(markdown))
                return parserResult;
            new Context(markdown, parserResult);
            return parserResult;
        }
        //
        enum InlineParserState {
            None,
            Text,
            CodeLine,
            CodeElement,
            Link,
            Image,
            URL,
            Emphasis,
        }
        enum BlockParserState {
            None,
            Heading,
            List,
            CodeBlock,
            Blockquote,
            Paragraph,
            Ruler
        }
        //
        interface IElementParser {
            InlineParserState Process(IParserContext context, char current);
            void Close(IParserResult result);
        }
        interface IBlockParser {
            bool CanProcess(IParserResult result, string line);
            void Process(IParserContext context, IParserResult result, string line);
            void Close(IParserResult result);
        }
        interface IParserContext {
            InlineParserState ProcessText(char current);
            InlineParserState ProcessInline(InlineParserState state, char current);
            void CloseInline(InlineParserState state, IParserResult result);
        }
        interface IParserResult {
            IBlockElement CurrentBlock { get; }
            void AddElement(IElement element, bool directly = false);
            void OpenBlock(IBlockElement element);
            bool TryCloseBlock(Predicate<IBlockElement> filter);
            void CloseBlocks();
            void CloseBlocks(Predicate<IBlockElement> filter);
        }
        //
        sealed class Reader {
            readonly string markdown;
            int position = 0;
            public Reader(string markdown) {
                this.markdown = markdown;
            }
            public int Position {
                get { return position; }
            }
            readonly static char[] crlf_chars = new char[] { '\r', '\n' };
            public string ReadLine() {
                int end = markdown.Length, newPosition = int.MaxValue;
                if(position >= end)
                    return null;
                int crlf = markdown.IndexOfAny(crlf_chars, position);
                string current;
                if(crlf > -1) {
                    current = markdown.Substring(position, crlf - position);
                    end = position + current.Length;
                    newPosition = end + 1;
                    if(end < markdown.Length && markdown[end] == '\r')
                        if(newPosition < markdown.Length && markdown[newPosition] == '\n')
                            newPosition++;
                }
                else {
                    current = markdown.Substring(position, markdown.Length - position);
                }
                position = newPosition;
                return current;
            }
        }
        public sealed class Result : IParserResult {
            int currentElementId = -1;
            List<IElement> elements = new List<IElement>();
            public void PerformElementClick(int elementId) {
                if(elements.Count == 0)
                    return;
                var element = elements.FirstOrDefault(x => x.Id == elementId);
                if(element != null)
                    element.PerformClick(BlockElementType.Image);
            }
            void IParserResult.AddElement(IElement element, bool addDirectly) {
                if(openedBlocks.Count == 0 || addDirectly) {
                    element.Id = (currentElementId++);
                    elements.Add(element);
                }
                else {
                    element.Id = -1;
                    var block = openedBlocks.Peek();
                    block.Elements.Add(element);
                }
            }
            Stack<IBlockElement> openedBlocks = new Stack<IBlockElement>(4);
            void IParserResult.OpenBlock(IBlockElement element) {
                openedBlocks.Push(element);
            }
            bool IParserResult.TryCloseBlock(Predicate<IBlockElement> filter) {
                if(openedBlocks.Count == 0)
                    return false;
                var block = openedBlocks.Peek();
                if(filter(block)) {
                    openedBlocks.Pop();
                    return true;
                }
                return false;
            }
            void IParserResult.CloseBlocks(Predicate<IBlockElement> filter) {
                while(openedBlocks.Count > 0 && filter(openedBlocks.Peek()))
                    openedBlocks.Pop();
            }
            void IParserResult.CloseBlocks() {
                while(openedBlocks.Count > 0)
                    openedBlocks.Pop();
            }
            IBlockElement IParserResult.CurrentBlock {
                get {
                    if(openedBlocks.Count == 0)
                        return null;
                    return openedBlocks.Peek();
                }
            }
            public sealed override string ToString() {
                var builder = new StringBuilder(1024);
                builder.Append("<div class=\"container\" id=\"container\">");
                for(int i = 0; i < elements.Count; i++)
                    builder.AppendLine(elements[i].ToString());
                builder.Append("</div>");
                return builder.ToString();
            }
        }
        sealed class Context : IParserContext {
            readonly Dictionary<InlineParserState, IElementParser> inlineParsers = new Dictionary<InlineParserState, IElementParser> {
                { InlineParserState.Text, new TextElementParser() },
                { InlineParserState.Link, new LinkParser() },
                { InlineParserState.Image, new ImageParser() },
                { InlineParserState.URL, new UrlParser() },
                { InlineParserState.Emphasis, new EmphasisParser() },
                { InlineParserState.CodeLine, new CodeLineParser() },
                { InlineParserState.CodeElement, new CodeElementParser() },
            };
            readonly Dictionary<BlockParserState, IBlockParser> blockParsers = new Dictionary<BlockParserState, IBlockParser> {
                { BlockParserState.Heading, new HeadingBlockParser() },
                { BlockParserState.Blockquote, new BlockquoteBlockParser() },
                { BlockParserState.List, new ListBlockParser() },
                { BlockParserState.CodeBlock, new CodeBlockBlockParser() },
                { BlockParserState.Ruler, new RulerBlockParser() },
                { BlockParserState.Paragraph, new ParagraphBlockParser() },
            };
            //
            public Context(string markdown, IParserResult result) {
                var reader = new Reader(markdown);
                BlockParserState blockType = BlockParserState.None;
                string line;
                while((line = reader.ReadLine()) != null) {
                    foreach(var parser in blockParsers) {
                        if(parser.Value.CanProcess(result, line)) {
                            if(blockType != BlockParserState.None && blockType != parser.Key)
                                blockParsers[blockType].Close(result);
                            parser.Value.Process(this, result, line);
                            blockType = parser.Key;
                            break;
                        }
                    }
                }
            }
            InlineParserState IParserContext.ProcessText(char current) {
                return inlineParsers[InlineParserState.Text].Process(this, current);
            }
            InlineParserState IParserContext.ProcessInline(InlineParserState state, char current) {
                return inlineParsers[state].Process(this, current);
            }
            void IParserContext.CloseInline(InlineParserState state, IParserResult result) {
                inlineParsers[state].Close(result);
            }
        }
    }
}