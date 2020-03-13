using Npgsql;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;

namespace TextCorpusSystem
{
    class Tagname
    {
        
        public string Name { get; }

        public Color Color { get; }

        public Tagname(string name)
        {
            this.Name = name;
            var rnd = new Random(Name.GetHashCode());
            var colorBytes = new byte[3];
            rnd.NextBytes(colorBytes);
            Color = Color.FromArgb(185, colorBytes[0], colorBytes[1], colorBytes[2]);
        }
    }
    
    class Tag
    {
        Random rnd;
        public int StartPos { get; }
        public int EndPos { get; }
        public string Name { get; }
        public Color Color { get; }


        public Tag(string name, int startPos, int endPos)
        {
            Random rnd;
            var colorBytes = new byte[3];
            this.Name = name;
            this.StartPos = startPos;
            this.EndPos = endPos;
            rnd = new Random(Name.GetHashCode());
            rnd.NextBytes(colorBytes);
            Color = Color.FromArgb(185, colorBytes[0], colorBytes[1], colorBytes[2]);
        }
    }
    public class TextHighlighter
    {
        public static void GetHighlightedText(int textId, RichTextBox richTextBox)
        {
            string text = GetText(textId);
            var tags = GetTags(textId);
            
            TextRange initialText  = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            initialText.Text = text;
            
            foreach (var tag in tags)
            {
                
                TextRange range = GetTextRange(tag.StartPos, tag.EndPos, richTextBox.Document);

                var oldColor = range.GetPropertyValue(TextElement.BackgroundProperty);
                range.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(tag.Color));
            }
        }

        private static string GetText(int textId)
        {
            string text;
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                db.Open();
                var getTextQuery = new NpgsqlCommand("select plaintext from texts where id = @textId", db);
                getTextQuery.Parameters.Add("@textId", NpgsqlTypes.NpgsqlDbType.Integer);
                getTextQuery.Parameters["@textId"].Value = textId;
                text = (string)getTextQuery.ExecuteScalar();             
            }
            return text;
        }

        private static List<Tag> GetTags(int textId)
        {
            var tagList = new List<Tag>();
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                db.Open();
                var getTagsQuery = new NpgsqlCommand("" +
                    "select tags.startpos, tags.endpos, tagnames.tagname " +
                    "from tags inner " +
                    "join tagnames on tagnames.id = tags.nameid " +
                    "where tags.textid = @textId", db);
                getTagsQuery.Parameters.Add("@textId", NpgsqlTypes.NpgsqlDbType.Integer);
                getTagsQuery.Parameters["@textId"].Value = textId;
                var tagsReader = getTagsQuery.ExecuteReader();


                if (tagsReader.HasRows)
                {
                    while (tagsReader.Read())
                    {
                        tagList.Add(new Tag((string)tagsReader.GetValue(2), (int)tagsReader.GetValue(0), (int)tagsReader.GetValue(1)));
                    }
                }
            }
            return tagList;
        }

        
        private static TextRange GetTextRange(int start, int end, FlowDocument doc)
        {
            var startPointer = GetTextPointerAtOffset(doc, start);
            var endPointer = GetTextPointerAtOffset(doc, end);

            return new TextRange(startPointer, endPointer);
        }

        public static TextPointer GetTextPointerAtOffset(FlowDocument doc, int offset)
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
    }
}
