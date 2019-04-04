using Microsoft.Toolkit.Win32.UI.XamlHost;
using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Windows;

namespace ContosoExpenses
{
    public class CalendarViewWrapper : WindowsXamlHostBase
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.ChildInternal = UWPTypeFactory.CreateXamlContentByType("Windows.UI.Xaml.Controls.CalendarView");

            SetContent();

            Windows.UI.Xaml.Controls.CalendarView calendarView = this.ChildInternal as Windows.UI.Xaml.Controls.CalendarView;
            calendarView.SelectedDatesChanged += CalendarView_SelectedDatesChanged;
        }

        public DateTimeOffset SelectedDate
        {
            get { return (DateTimeOffset)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTimeOffset), typeof(CalendarViewWrapper), new PropertyMetadata(DateTimeOffset.Now));

        private void CalendarView_SelectedDatesChanged(Windows.UI.Xaml.Controls.CalendarView sender, Windows.UI.Xaml.Controls.CalendarViewSelectedDatesChangedEventArgs args)
        {
            SelectedDate = args.AddedDates[0];
        }
    }
}