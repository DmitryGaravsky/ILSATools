namespace ILSA.Client.Views {
    using System.Collections.Generic;

    public partial class Markdown {
        enum ElementType {
            Heading,
            List,
            ListItem,
            Emphasis,
            URL,
            Code,
            CodeBlock,
            Image,
            Link,
            Blockquote,
            Text,
            Paragraph,
            Ruler,
            CodeLine
        }
        enum BlockElementType {
            Block,
            Image
        }
        interface IElement {
            int Id { get; set; }
            ElementType Type { get; }
            string ToString();
            void PerformClick(BlockElementType blockType);
        }
        interface IBlockElement : IElement {
            List<IElement> Elements { get; }
            int Level { get; set; }
        }
        //
        abstract class BaseElement : IElement {
            int IElement.Id { get; set; }
            ElementType IElement.Type {
                get { return GetElementType(); }
            }
            void IElement.PerformClick(BlockElementType blockType) {
                PerformClick(blockType);
            }
            protected abstract ElementType GetElementType();
            protected virtual void PerformClick(BlockElementType blockType) { }
        }
        abstract class BaseBlockElement : BaseElement, IBlockElement {
            readonly List<IElement> elementsCore = new List<IElement>();
            public List<IElement> Elements {
                get { return elementsCore; }
            }
            int IBlockElement.Level { get; set; }
        }
    }
}