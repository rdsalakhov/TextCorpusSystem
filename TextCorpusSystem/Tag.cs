using System;
using System.Windows.Media;

namespace TextCorpusSystem
{
    public class Tag
    {
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
}
