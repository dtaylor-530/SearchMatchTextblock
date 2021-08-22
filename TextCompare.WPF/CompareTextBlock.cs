using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace TextCompare.WPF
{

    /// <summary>
    /// based on <a href="https://github.com/deanchalk/SearchMatchTextblock"></a>
    /// </summary>
    [TemplatePart(Name = CompareTextblockName, Type = typeof(TextBlock))]
    public class CompareTextBlock : Control
    {
        private const string CompareTextblockName = "PART_CompareTextblock";

        private static readonly DependencyPropertyKey AdditionsCountPropertyKey = DependencyProperty.RegisterReadOnly("AdditionsCount", typeof(int), typeof(CompareTextBlock), new PropertyMetadata(0));
        public static readonly DependencyProperty AdditionsCountProperty = AdditionsCountPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey SubtractionsCountPropertyKey = DependencyProperty.RegisterReadOnly("SubtractionsCount", typeof(int), typeof(CompareTextBlock), new PropertyMetadata(0));
        public static readonly DependencyProperty SubtractionsCountProperty = SubtractionsCountPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey OtherCountPropertyKey = DependencyProperty.RegisterReadOnly("OtherCount", typeof(int), typeof(CompareTextBlock), new PropertyMetadata(0));
        public static readonly DependencyProperty OtherCountProperty = OtherCountPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey Other2CountPropertyKey = DependencyProperty.RegisterReadOnly("Other2Count", typeof(int), typeof(CompareTextBlock), new PropertyMetadata(0));
        public static readonly DependencyProperty Other2CountProperty = Other2CountPropertyKey.DependencyProperty;


        public static readonly DependencyProperty CompareTextProperty = DependencyProperty.Register("CompareText", typeof(string), typeof(CompareTextBlock), new PropertyMetadata(string.Empty, OnCompareTextPropertyChanged));
        public static readonly DependencyProperty TextProperty = TextBlock.TextProperty.AddOwner(typeof(CompareTextBlock), new PropertyMetadata(string.Empty, OnTextPropertyChanged));
        public static readonly DependencyProperty TextWrappingProperty = TextBlock.TextWrappingProperty.AddOwner(typeof(CompareTextBlock), new PropertyMetadata(TextWrapping.NoWrap));
        public static readonly DependencyProperty TextTrimmingProperty = TextBlock.TextTrimmingProperty.AddOwner(typeof(CompareTextBlock), new PropertyMetadata(TextTrimming.None));

        public static readonly DependencyProperty HighlightForegroundProperty = DependencyProperty.Register("HighlightForeground", typeof(Brush), typeof(CompareTextBlock), new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty HighlightForeground2Property = DependencyProperty.Register("HighlightForeground2", typeof(Brush), typeof(CompareTextBlock), new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty HighlightForeground3Property = DependencyProperty.Register("HighlightForeground3", typeof(Brush), typeof(CompareTextBlock), new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty HighlightForeground4Property = DependencyProperty.Register("HighlightForeground4", typeof(Brush), typeof(CompareTextBlock), new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty HighlightBackgroundProperty = DependencyProperty.Register("HighlightBackground", typeof(Brush), typeof(CompareTextBlock), new PropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty HighlightBackground2Property = DependencyProperty.Register("HighlightBackground2", typeof(Brush), typeof(CompareTextBlock), new PropertyMetadata(Brushes.LightPink));
        public static readonly DependencyProperty HighlightBackground3Property = DependencyProperty.Register("HighlightBackground3", typeof(Brush), typeof(CompareTextBlock), new PropertyMetadata(Brushes.Yellow));
        public static readonly DependencyProperty HighlightBackground4Property = DependencyProperty.Register("HighlightBackground4", typeof(Brush), typeof(CompareTextBlock), new PropertyMetadata(Brushes.Purple));

        private TextBlock compareTextBlock;

        static CompareTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CompareTextBlock), new FrameworkPropertyMetadata(typeof(CompareTextBlock)));
        }

        #region properties
        public int AdditionsCount
        {
            get => (int)GetValue(AdditionsCountProperty);
            protected set => SetValue(AdditionsCountPropertyKey, value);
        }

        public Brush HighlightBackground
        {
            get => (Brush)GetValue(HighlightBackgroundProperty);
            set => SetValue(HighlightBackgroundProperty, value);
        }

        public Brush HighlightForeground
        {
            get => (Brush)GetValue(HighlightForegroundProperty);
            set => SetValue(HighlightForegroundProperty, value);
        }

        public Brush HighlightBackground2
        {
            get => (Brush)GetValue(HighlightBackground2Property);
            set => SetValue(HighlightBackground2Property, value);
        }

        public Brush HighlightForeground2
        {
            get => (Brush)GetValue(HighlightForeground2Property);
            set => SetValue(HighlightForeground2Property, value);
        }

        public Brush HighlightBackground3
        {
            get => (Brush)GetValue(HighlightBackground3Property);
            set => SetValue(HighlightBackground3Property, value);
        }

        public Brush HighlightForeground3
        {
            get => (Brush)GetValue(HighlightForeground3Property);
            set => SetValue(HighlightForeground3Property, value);
        }

        public Brush HighlightBackground4
        {
            get => (Brush)GetValue(HighlightBackground4Property);
            set => SetValue(HighlightBackground4Property, value);
        }

        public Brush HighlightForeground4
        {
            get => (Brush)GetValue(HighlightForeground4Property);
            set => SetValue(HighlightForeground4Property, value);
        }

        public string CompareText
        {
            get => (string)GetValue(CompareTextProperty);
            set => SetValue(CompareTextProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        public TextTrimming TextTrimming
        {
            get => (TextTrimming)GetValue(TextTrimmingProperty);
            set => SetValue(TextTrimmingProperty, value);
        }

        #endregion properties

        private static void OnCompareTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textblock = (CompareTextBlock)d;
            textblock.ProcessTextChanged(textblock.Text, e.NewValue as string);
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textblock = (CompareTextBlock)d;
            textblock.ProcessTextChanged(e.NewValue as string, textblock.CompareText);
        }
        CancellationTokenSource tokenSource = new();
        private async void ProcessTextChanged(string mainText, string compareText)
        {
            int count = 0;
            Reset();

            if (Validate(mainText, CompareText) == false)
                return;

            tokenSource.Cancel(false);
            tokenSource = new();
            var task = Task.Run(() =>
            {
                return Compare(mainText, compareText);
            }, tokenSource.Token);
            foreach (var group in await task)
            {
                var (foreground, background) = GetGrounds(group.Difference);
                if (group.Difference == 1)
                {
                    count++;
                }
                compareTextBlock.Inlines.Add(new Run(group.Text) { Background = background, Foreground = foreground });
            }
            SetValue(AdditionsCountPropertyKey, count);
        }


        private (Brush foreground, Brush background) GetGrounds(short diff)
        {
            return diff switch
            {
                0 => (HighlightForeground, HighlightBackground),
                1 => (HighlightForeground2, HighlightBackground2),
                -1 => (HighlightForeground3, HighlightBackground3),
                2 => (HighlightForeground4, HighlightBackground4),
                _ => throw new Exception("dfssdf  sdfsdf"),
            };
        }

        private bool Validate(string mainText, string CompareText)
        {
            if (compareTextBlock == null)
                return false;
            if (compareTextBlock == null || string.IsNullOrWhiteSpace(mainText))
                return false;
        
            return true;
        }

        private void Reset()
        {
            compareTextBlock?.Inlines.Clear();
            SetValue(AdditionsCountPropertyKey, 0);
        }


        public override void OnApplyTemplate()
        {
            compareTextBlock = GetTemplateChild(CompareTextblockName) as TextBlock;
            if (compareTextBlock == null)
                return;
            ProcessTextChanged(Text, CompareText);
        }

        
        protected virtual IReadOnlyCollection<TextGroup> Compare(string mainText, string compareText)
        {
            return DiffComparer.CompareText(mainText, compareText).ToArray();
        }
    }
}