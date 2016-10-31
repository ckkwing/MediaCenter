using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MediaCenter.Theme.CustomControl
{
    public class EnhancedTextBlock : TextBlock
    {
        protected bool GetIsTextTrimmed(DependencyObject obj)
        {
            try
            {
                TextBlock textBlock = (TextBlock)obj;

                Typeface typeface = new Typeface(textBlock.FontFamily,
                    textBlock.FontStyle,
                    textBlock.FontWeight,
                    textBlock.FontStretch);

                // FormattedText is used to measure the whole width of the text held up by TextBlock container.
                FormattedText formmatedText = new FormattedText(
                    textBlock.Text,
                    System.Threading.Thread.CurrentThread.CurrentCulture,
                    textBlock.FlowDirection,
                    typeface,
                    textBlock.FontSize,
                    textBlock.Foreground);

                return formmatedText.Width > (textBlock.ActualWidth + 0.1);
            }
            catch (Exception e)
            {
                NLogger.LogHelper.UILogger.Debug("EnhancedTextBlock::GetIsTextTrimmed exception", e);
                return true;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            bool v = GetIsTextTrimmed(this);
            SetValue(IsTextTrimmedProperty, v);
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            bool v = GetIsTextTrimmed(this);
            SetValue(IsTextTrimmedProperty, v);
        }

        public bool IsTextTrimmed
        {
            get { return (bool)GetValue(IsTextTrimmedProperty); }
            set { SetValue(IsTextTrimmedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTextTrimmed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTextTrimmedProperty =
            DependencyProperty.Register("IsTextTrimmed", typeof(bool), typeof(EnhancedTextBlock),
            new FrameworkPropertyMetadata(false
                )
            );
    }
}
