using Npgsql;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;

namespace TextCorpusSystem
{
    public class TextHighlighter
    {
        static Color rangeHighlitingColor = Color.FromArgb(185, 250, 230, 0);
        public static void GetHighlightedText(int textId, RichTextBox textRTB, RichTextBox legendRTB)
        {
            string text = TextManager.GetText(textId);
            var tags = TextManager.GetTags(textId);
            
            TextRange initialText  = new TextRange(textRTB.Document.ContentStart, textRTB.Document.ContentEnd);
            initialText.Text = text;
            
            foreach (var tag in tags)
            {
                TextRange range = GetTextRange(tag.StartPos, tag.EndPos, textRTB.Document);
                var oldColor = range.GetPropertyValue(TextElement.BackgroundProperty);
                range.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(tag.Color));
            }

            GetTagsLegend(tags, legendRTB);
        }

        public static void GetTagsLegend(List<Tag> tags, RichTextBox legendRTB)
        {
            foreach (var tag in tags.GroupBy(x => x.Name).Select(g => g.First()))
            { 
                Run tagRun = new Run(tag.Name);
                TextRange range = new TextRange(tagRun.ContentStart, tagRun.ContentEnd);
                range.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(tag.Color));
                Paragraph highlightedTag = new Paragraph(tagRun);
                legendRTB.Document.Blocks.Add(highlightedTag);
            }
        }
        
        private static TextRange GetTextRange(int start, int end, FlowDocument doc)
        {
            var startPointer = GetTextPointerAtOffset(doc, start);
            var endPointer = GetTextPointerAtOffset(doc, end);
            return new TextRange(startPointer, endPointer);
        }

        private static TextPointer GetTextPointerAtOffset(FlowDocument doc, int offset)
        {
            var navigator = doc.ContentStart;
            int cnt = 0;

            while (navigator.CompareTo(doc.ContentEnd) < 0)
            {
                switch (navigator.GetPointerContext(LogicalDirection.Forward))
                {
                    case TextPointerContext.ElementStart:
                        break;
                    case TextPointerContext.ElementEnd:
                        if (navigator.GetAdjacentElement(LogicalDirection.Forward) is Paragraph)
                            cnt += 2;
                        break;
                    case TextPointerContext.EmbeddedElement:
                        cnt++;
                        break;
                    case TextPointerContext.Text:
                        int runLength = navigator.GetTextRunLength(LogicalDirection.Forward);

                        if (runLength > 0 && runLength + cnt < offset)
                        {
                            cnt += runLength;
                            navigator = navigator.GetPositionAtOffset(runLength);
                            if (cnt > offset)
                                break;
                            continue;
                        }
                        cnt++;
                        break;
                }

                if (cnt > offset)
                    break;

                navigator = navigator.GetPositionAtOffset(1, LogicalDirection.Forward);

            } // End while.

            return navigator;
        }

        public static FlowDocument HighlighAtRange(FlowDocument doc, int startPos, int endPos)
        {

            TextRange tr = new TextRange(GetTextPointerAtOffset(doc, startPos), GetTextPointerAtOffset(doc, endPos));
            tr.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(rangeHighlitingColor));
            return doc;
        }
    }

    
}
