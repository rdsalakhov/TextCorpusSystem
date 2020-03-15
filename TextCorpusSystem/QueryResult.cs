using System.Collections.Generic;
using System.Linq;

namespace TextCorpusSystem
{
    public class QueryResult
    {
        static int _cutOffset = 90;
        public string TextName { get; }
        public string Text { get; private set; }


        public List<int> StartPositions { get; private set; }
        public List<int> EndPositions { get; private set; }


        public QueryResult(string textName, string text, List<int> startPositions, List<int> endPositions)
        {
            this.TextName = textName;
            this.Text = text;
            this.StartPositions = startPositions;
            this.EndPositions = endPositions;
            CutText();
        }

        private void CutText()
        {
            int lesserHighlightIndex = StartPositions.Min();
            int greaterHighlightIndex = EndPositions.Max();

            int startCutIndex = lesserHighlightIndex - _cutOffset < 0 ? 0 : lesserHighlightIndex - _cutOffset;
            int cutLenght = greaterHighlightIndex - lesserHighlightIndex + startCutIndex + _cutOffset * 2 > Text.Length ? Text.Length - startCutIndex : greaterHighlightIndex - lesserHighlightIndex + _cutOffset * 2;
            
            Text = Text.Substring(startCutIndex, cutLenght);
            if (startCutIndex != 0)
            {
                Text = Text.Insert(0, "...");
                startCutIndex -= 3;
            }
            for (int i = 0; i < StartPositions.Count; i++)
            {
                StartPositions[i] -= startCutIndex;
                EndPositions[i] -= startCutIndex;
            }

            if (cutLenght == greaterHighlightIndex - lesserHighlightIndex + _cutOffset * 2)
            {
                Text = Text.Insert(Text.Length, "...");
            }

        }


    }
} 
