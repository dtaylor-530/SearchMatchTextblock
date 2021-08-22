namespace TextCompare.WPF
{
    public class TextGroup
    {
        public TextGroup(string text, int index, short difference)
        {
            Text = text;
            Index = index;
            Difference = difference;
        }

        public string Text { get; }
        public int Index { get; }
        public short Difference { get; }
    }
}