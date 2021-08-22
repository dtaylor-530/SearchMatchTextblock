using System.Collections.Generic;
using System.Text;
using Netsoft.Diff;

namespace TextCompare.WPF
{
    class DiffComparer
    {


        public static IEnumerable<TextGroup> CompareText(string mainText, string CompareText)
        {
            var differences = Differences.Between(mainText, CompareText);
            int i = 0;
            int changeIndex = 0;
            StringBuilder stringBuilder = new StringBuilder();
            short? action = 0;
            foreach (var difference in differences)
            {
                if (action.HasValue && difference.Action != action)
                {
                    yield return new TextGroup(stringBuilder.ToString(), changeIndex, action.Value);
                    stringBuilder.Clear();
                    changeIndex = i;
                }
                stringBuilder.Append(difference.Value);
                i++;
                action = difference.Action;
            }

            if (stringBuilder.Length > 0)
                yield return new TextGroup(stringBuilder.ToString(), changeIndex, action.Value);
        }
    }
}