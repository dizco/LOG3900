using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using PolyPaint.Helpers.Adorners;

namespace PolyPaint.CustomComponents
{
    public class ContentControlDecorator : Control
    {
        public static readonly DependencyProperty ShowDecoratorProperty =
            DependencyProperty.Register("ShowDecorator", typeof(bool), typeof(ContentControlDecorator),
                                        new FrameworkPropertyMetadata(false, ShowDecoratorProperty_Changed));

        private Adorner _adorner;

        public bool ShowDecorator
        {
            get => (bool) GetValue(ShowDecoratorProperty);
            set => SetValue(ShowDecoratorProperty, value);
        }

        private void HideAdorner()
        {
            if (_adorner != null)
            {
                _adorner.Visibility = Visibility.Hidden;
            }
        }

        private void ShowAdorner()
        {
            if (_adorner == null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

                if (adornerLayer != null)
                {
                    ContentControl designerItem = DataContext as ContentControl;
                    _adorner = new ResizeAdorner(designerItem);
                    adornerLayer.Add(_adorner);

                    if (ShowDecorator)
                    {
                        _adorner.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _adorner.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                _adorner.Visibility = Visibility.Visible;
            }
        }

        private static void ShowDecoratorProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentControlDecorator decorator = (ContentControlDecorator) d;
            bool showDecorator = (bool) e.NewValue;

            if (showDecorator)
            {
                decorator.ShowAdorner();
            }
            else
            {
                decorator.HideAdorner();
            }
        }
    }
}
