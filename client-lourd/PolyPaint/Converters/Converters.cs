using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PolyPaint.Models.ApiModels;

namespace PolyPaint.Converters
{
    /// <summary>
    ///     Permet de générer une couleur en fonction de la chaine passée en paramètre.
    ///     Par exemple, pour chaque bouton d'un groupe d'options on compare son nom avec l'élément actif (sélectionné) du
    ///     groupe.
    ///     S'il y a correspondance, la bordure du bouton aura une teinte bleue, sinon elle sera transparente.
    ///     Cela permet de mettre l'option sélectionnée dans un groupe d'options en évidence.
    /// </summary>
    internal class BorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == parameter.ToString() ? "#FF58BDFA" : "#00000000";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    /// Permet de générer une couleur en fonction de la chaine passée en paramètre.
    /// Par exemple, pour chaque bouton d'un groupe d'option on compare son nom avec l'élément actif (sélectionné) du groupe.
    /// S'il y a correspondance, la couleur de fond du bouton aura une teinte bleue, sinon elle sera transparente.
    /// Cela permet de mettre l'option sélectionnée dans un groupe d'options en évidence.
    internal class BackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == parameter.ToString() ? "#3F58BDFA" : "#00000000";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    ///     Permet au InkCanvas de définir son mode d'édition en fonction de l'outil sélectionné.
    /// </summary>
    internal class EditingToolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "lasso":
                    return InkCanvasEditingMode.Select;
                case "efface_segment":
                    return InkCanvasEditingMode.EraseByPoint;
                case "efface_trait":
                    return InkCanvasEditingMode.EraseByStroke;
                case "shapes":
                    return InkCanvasEditingMode.GestureOnly;
                default:
                    return InkCanvasEditingMode.Ink;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    internal class DrawingProtectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProtectionModel protection)
            {
                return protection.Active ? "🔒" : "🔓";
            }

            return "🔓";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool active = false;

            if (value is string protectionBool)
            {
                active = protectionBool.Equals("🔒");
            }

            return new ProtectionModel {Active = active};
        }
    }
}
