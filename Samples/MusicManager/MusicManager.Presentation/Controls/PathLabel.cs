using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Waf.MusicManager.Applications.Data;

namespace Waf.MusicManager.Presentation.Controls
{
    // Usage: Set the HorizontalContentAlignment of the Parent Control to Stretch.
    public class PathLabel : Label
    {
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register(nameof(Path), typeof(string), typeof(PathLabel), new FrameworkPropertyMetadata("", PathChangedHandler));

        private readonly TextBlock textBlock;
        private double pixelsPerDip = 1;

        public PathLabel()
        {
            textBlock = new TextBlock();
            
            Loaded += LoadedHandler;
            SizeChanged += SizeChangedHandler;
        }

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Content = textBlock;
        }

        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
        }

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            pixelsPerDip = newDpi.PixelsPerDip;
        }

        private void UpdatePathText()
        {
            string path = Path ?? "";
            var pathSize = MeasureString(path);
            if (pathSize.Width < ActualWidth)
            {
                textBlock.Text = path;
                return;
            }

            var pathElements = FolderHelper.GetPathSegments(path).ToArray();
            for (int i = 2; i < pathElements.Length; i++)
            {
                path = string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new[] { pathElements[0], "..." }.Concat(pathElements.Skip(i)));
                pathSize = MeasureString(path);
                if (pathSize.Width < ActualWidth)
                {
                    break;
                }
            }
            textBlock.Text = path;
        }

        private Size MeasureString(string str)
        {
            var typeFace = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            var formattedText = new FormattedText(str, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, FontSize, Foreground, pixelsPerDip);
            return new Size(formattedText.Width, formattedText.Height);
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            UpdatePathText();
        }

        private static void PathChangedHandler(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((PathLabel)obj).UpdatePathText();
        }
    }
}
