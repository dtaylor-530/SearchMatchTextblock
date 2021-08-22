using System;
using System.Collections.Generic;

namespace TextCompare.WPF
{
    class LookUpComparer
    {
        public static IEnumerable<TextGroup> CompareText(string mainText, string CompareText)
        {
            if (string.IsNullOrWhiteSpace(CompareText))
{
                yield return new TextGroup(mainText, 0, Convert.ToInt16(false));
                yield break;
            }

            var find = 0;
            var searchTextLength = CompareText.Length;
            while (find >= 0)
            {
                var oldFind = find;
                find = mainText.IndexOf(CompareText, find, StringComparison.InvariantCultureIgnoreCase);
                yield return GetTextGroup(mainText, ref find, searchTextLength, oldFind);
            }
        }

        static TextGroup GetTextGroup(string mainText, ref int find, int searchTextLength, int oldFind)
        {
            if (find == -1)
            {
                return
                   oldFind > 0
                       ? Create(mainText.Substring(oldFind, mainText.Length - oldFind), find, false)
                       : Create(mainText, find, false);
            }
            else if (oldFind == find)
            {
                find += searchTextLength;
                return Create(mainText.Substring(oldFind, searchTextLength), find, true);
            }
            else
                return Create(mainText.Substring(oldFind, find - oldFind), find, false);

            TextGroup Create(string text, int index, bool isHighlighted)
            {
                return new TextGroup(text, index, Convert.ToInt16(isHighlighted));
            }
        }
    }
}