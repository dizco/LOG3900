using System.Windows;
using System.Windows.Controls;

namespace PolyPaint.CustomComponents
{
    /// <summary>
    ///     Based on
    ///     https://blogs.msdn.microsoft.com/atc_avalon_team/2006/04/10/fixed-width-column-in-listview-a-column-that-cannot-be-resized/
    /// </summary>
    internal class FixedWidthGridViewColumn : GridViewColumn
    {
        public static readonly DependencyProperty FixedWidthProperty =
            DependencyProperty.Register("FixedWidth", typeof(double), typeof(FixedWidthGridViewColumn),
                                        new FrameworkPropertyMetadata(double.NaN,
                                                                      OnFixedWidthChanged));

        static FixedWidthGridViewColumn()
        {
            WidthProperty.OverrideMetadata(typeof(FixedWidthGridViewColumn),
                                           new FrameworkPropertyMetadata(null, OnCoerceWidth));
        }

        public double FixedWidth
        {
            get => (double) GetValue(FixedWidthProperty);
            set => SetValue(FixedWidthProperty, value);
        }

        private static void OnFixedWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FixedWidthGridViewColumn column)
            {
                column.CoerceValue(WidthProperty);
            }
        }

        private static object OnCoerceWidth(DependencyObject d, object baseValue)
        {
            if (d is FixedWidthGridViewColumn column)
            {
                return column.FixedWidth;
            }

            return baseValue;
        }
    }
}
