using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Xaml.Data;
//using Resources;

namespace Utils
{
    public class ColorQualityConverter:IValueConverter
    {


        /*
        private string GetText(int value)
        {

            if (value == 1)
                return AppResources.QualityState1;

            if (value == 2)
                return AppResources.QualityState2;

            if (value == 3)
                return AppResources.QualityState3;

            if (value == 4)
                return AppResources.QualityState4;

            if (value == 5)
                return AppResources.QualityState5;

            if (value == 6)
                return AppResources.QualityState6;

            if (value == 7)
                return AppResources.QualityState7;

            if (value == 8)
                return AppResources.QualityState8;

            return "N/A";
        }
        */
        /*
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(string))
                return GetText((int)value);

            if (targetType != typeof(Brush))
                throw new InvalidOperationException("Wrong target");

            if ((int)value == 1)
                return new SolidColorBrush(Color.FromArgb(255, 0, 138, 0));// "#008a00";

            if ((int)value == 2)
                return new SolidColorBrush(Color.FromArgb(255, 132, 161, 47)); //"#84a12f";

            if ((int)value == 3)
                return new SolidColorBrush(Color.FromArgb(255, 184, 177, 0)); //"#b8b100";

            if ((int)value == 4)
                return new SolidColorBrush(Color.FromArgb(255, 207, 141, 19)); //"#cf8d13";

            if ((int)value == 5)
                return new SolidColorBrush(Color.FromArgb(255, 207, 84, 23)); //"#cf5417";

            if ((int)value == 6)
                return new SolidColorBrush(Color.FromArgb(255, 207, 51, 27)); //"#cf331b";

            if ((int)value == 7)
                return new SolidColorBrush(Color.FromArgb(255, 143, 143, 143)); //"#8f8f8f";

            if ((int)value == 8)
                return new SolidColorBrush(Color.FromArgb(255, 138, 137, 156)); //"#8a899c";

            return new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)); //gray";
        }
        */
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
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
