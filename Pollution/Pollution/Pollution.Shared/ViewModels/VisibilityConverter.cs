using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Globalization;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace Pollution.ViewModels
{
    /// <summary>
    /// Konvertor pro převod Boolean hodnoty na viditelnost při bindingu.
    /// </summary>
    public class VisibilityConverter : IValueConverter
        {
         
        /// <summary>
        /// Metoda pro převod z Boolean na Visibility
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
         {
             bool visibility = (bool)value;
             return visibility ? Visibility.Visible : Visibility.Collapsed;
         }

        /// <summary>
        /// Metoda pro převod z Visibility na Boolean, není použita ani implementována
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
         public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
         {
             Visibility visibility = (Visibility)value;
             return (visibility == Visibility.Visible);
         }

         public object Convert(object value, Type targetType, object parameter, string language)
         {
             throw new NotImplementedException();
         }

         public object ConvertBack(object value, Type targetType, object parameter, string language)
         {
             throw new NotImplementedException();
         }
        }
}
