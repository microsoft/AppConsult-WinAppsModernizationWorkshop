using System;
using System.Windows;
using System.Windows.Controls;

namespace Waf.MusicManager.Presentation.Controls
{
    /// <summary>
    /// Represents a control that can be used to enter a search or filter text.
    /// </summary>
    [TemplatePart(Name = searchTextBoxPartName, Type = typeof(TextBox))]
    public class SearchBox : Control
    {
        private const string searchTextBoxPartName = "PART_SearchTextBox";
        
        /// <summary>
        /// Identifies the Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SearchBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies the HintText dependency property.
        /// </summary>
        public static readonly DependencyProperty HintTextProperty =
            DependencyProperty.Register(nameof(HintText), typeof(string), typeof(SearchBox), new FrameworkPropertyMetadata(""));

        private TextBox searchTextBox;

        static SearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(typeof(SearchBox)));
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Gets or sets the hint text. This text is shown in the background if no search text is entered.
        /// </summary>
        public string HintText
        {
            get => (string)GetValue(HintTextProperty);
            set => SetValue(HintTextProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            searchTextBox = (TextBox)GetTemplateChild(searchTextBoxPartName);
            if (searchTextBox == null) { throw new InvalidOperationException("The part could not be found: " + searchTextBoxPartName); }
        }

        public new void Focus()
        {
            searchTextBox.Focus();
        }
    }
}
