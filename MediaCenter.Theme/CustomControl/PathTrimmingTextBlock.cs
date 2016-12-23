using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MediaCenter.Theme.CustomControl
{
    static class PathExtensions
    {
        public static string FormatWith(this string s, params object[] args)
        {
            return string.Format(s, args);
        }
    }

    public class PathTrimmingTextBlock : TextBlock
    {

        FrameworkElement _container;


        public PathTrimmingTextBlock()
        {
            this.Loaded += new RoutedEventHandler(PathTrimmingTextBlock_Loaded);
        }

        void PathTrimmingTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Parent == null) throw new InvalidOperationException("PathTrimmingTextBlock must have a container such as a Grid.");

            _container = (FrameworkElement)this.Parent;
            _container.SizeChanged += new SizeChangedEventHandler(container_SizeChanged);

            Text = GetTrimmedPath(_container.ActualWidth);
        }

        void container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Text = GetTrimmedPath(_container.ActualWidth);
        }

        public bool IsTextTrimmed
        {
            get { return (bool)GetValue(IsTextTrimmedProperty); }
            set { SetValue(IsTextTrimmedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTextTrimmed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTextTrimmedProperty =
            DependencyProperty.Register("IsTextTrimmed", typeof(bool), typeof(PathTrimmingTextBlock),
            new FrameworkPropertyMetadata(false
                )
            );

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(PathTrimmingTextBlock), new PropertyMetadata(OnPathPropertyChanged));

        private static void OnPathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PathTrimmingTextBlock;
            if (null != control)
                control.UpdateText();
        }

        public void UpdateText()
        {
            if (null == _container)
                return;
            Text = GetTrimmedPath(_container.ActualWidth);
        }

        string GetTrimmedPath(double width)
        {
            string str = string.Empty;
            if (null == Path)
                return str;
            if (Path.StartsWith("/"))
                str = GetTrimmedOnlinePath(width);
            else
                str = GetTrimmedLocalPath(width);
            return str;
        }

        private string GetTrimmedOnlinePath(double width)
        {
            try
            {
                string filename = System.IO.Path.GetFileName(Path);
                string directory = Path.Replace(filename, string.Empty);
                FormattedText formatted;
                bool widthOK = false;
                bool changedWidth = false;
                string placeholder = string.Empty;
                if (!directory.EndsWith("/"))
                    directory += "/";
                do
                {
                    TextBlock textBlock = this;

                    Typeface typeface = new Typeface(textBlock.FontFamily,
                        textBlock.FontStyle,
                        textBlock.FontWeight,
                        textBlock.FontStretch);

                    // FormattedText is used to measure the whole width of the text held up by TextBlock container.
                    formatted = new FormattedText(
                         "{0}{1}{2}".FormatWith(directory, placeholder, filename),
                        System.Threading.Thread.CurrentThread.CurrentCulture,
                        textBlock.FlowDirection,
                        typeface,
                        textBlock.FontSize,
                        textBlock.Foreground);
                    widthOK = formatted.Width < width;

                    if (!widthOK)
                    {
                        changedWidth = true;
                        int iIndex = directory.LastIndexOf("/");
                        string directoryInfo = directory.Remove(iIndex);
                        if (string.IsNullOrEmpty(directoryInfo))
                        {
                            if (filename.Length == 0)
                                return "...";
                            filename = filename.Substring(1);
                        }
                        else
                        {
                            directory = directoryInfo;
                        }
                        if (directory.EndsWith("/"))
                            placeholder = ".../";
                        else
                            placeholder = "/.../";
                    }

                } while (!widthOK);

                if (!changedWidth)
                {
                    return Path;
                }
                SetValue(IsTextTrimmedProperty, changedWidth);
                return "{0}{1}{2}".FormatWith(directory, placeholder, filename);
            }
            catch (Exception ex)
            {
                NLogger.LogHelper.UILogger.Debug("Failed", ex);
                return string.Empty;
            }
        }

        private string GetTrimmedLocalPath(double width)
        {
            try
            {
                string filename = System.IO.Path.GetFileName(Path);
                string directory = System.IO.Path.GetDirectoryName(Path);
                FormattedText formatted;
                bool widthOK = false;
                bool changedWidth = false;
                string placeholder = string.Empty;
                if (!directory.EndsWith("\\"))
                    directory += "\\";
                do
                {
                    TextBlock textBlock = this;

                    Typeface typeface = new Typeface(textBlock.FontFamily,
                        textBlock.FontStyle,
                        textBlock.FontWeight,
                        textBlock.FontStretch);

                    // FormattedText is used to measure the whole width of the text held up by TextBlock container.
                    formatted = new FormattedText(
                         "{0}{1}{2}".FormatWith(directory, placeholder, filename),
                        System.Threading.Thread.CurrentThread.CurrentCulture,
                        textBlock.FlowDirection,
                        typeface,
                        textBlock.FontSize,
                        textBlock.Foreground);

                    //formatted = new FormattedText(
                    //    "{0}{1}{2}".FormatWith(directory, placeholder, filename),
                    //    CultureInfo.CurrentCulture,
                    //    FlowDirection.LeftToRight,
                    //    FontFamily.GetTypefaces().First(),
                    //    FontSize,
                    //    Foreground
                    //    );

                    widthOK = formatted.Width < width;

                    if (!widthOK)
                    {
                        changedWidth = true;
                        System.IO.DirectoryInfo directoryInfo = System.IO.Directory.GetParent(directory);
                        if (null == directoryInfo)
                        {
                            if (filename.Length == 0)
                                return "...";
                            filename = filename.Substring(1);
                        }
                        else
                        {
                            directory = directoryInfo.ToString();
                        }
                        if (directory.EndsWith("\\"))
                            placeholder = "...\\";
                        else
                            placeholder = "\\...\\";
                    }

                } while (!widthOK);

                if (!changedWidth)
                {
                    return Path;
                }
                SetValue(IsTextTrimmedProperty, changedWidth);
                return "{0}{1}{2}".FormatWith(directory, placeholder, filename);
            }
            catch (Exception ex)
            {
                NLogger.LogHelper.UILogger.Debug("Failed", ex);
                return string.Empty;
            }
        }
    }
}
