using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace TextCompare.WPF
{
    public class HighlightingTextBlock : CompareTextBlock
    {
        public HighlightingTextBlock()
        {
            HighlightForeground = Brushes.Black;
            HighlightBackground = Brushes.Transparent;
        }

        protected override IReadOnlyCollection<TextGroup> Compare(string mainText, string compareText)
        {
            return LookUpComparer.CompareText(mainText, compareText).ToArray();
        }
    }
}