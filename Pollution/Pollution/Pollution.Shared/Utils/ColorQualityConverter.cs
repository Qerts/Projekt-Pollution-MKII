using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
//using Resources;

namespace Utils
{
    public class ColorQualityConverter:IValueConverter
    {

        ResourceLoader _resourceLoader = new ResourceLoader();
        
        public string GetText(int value)
        {

            if (value == 1)
                return _resourceLoader.GetString("QualityState1/Text");

            if (value == 2)
                return _resourceLoader.GetString("QualityState2/Text");

            if (value == 3)
                return _resourceLoader.GetString("QualityState3/Text");

            if (value == 4)
                return _resourceLoader.GetString("QualityState4/Text");

            if (value == 5)
                return _resourceLoader.GetString("QualityState5/Text");

            if (value == 6)
                return _resourceLoader.GetString("QualityState6/Text");

            if (value == 7)
                return _resourceLoader.GetString("QualityState7/Text");

            if (value == 8)
                return _resourceLoader.GetString("QualityState8/Text");

            return "N/A";
        }
        
        
        public object CultureConvert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            if (targetType == typeof(string))
                return GetTextSimple((int)value);
            
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
        
        public object CultureConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType == typeof(string))
                return GetText((int)value);

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
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public Brush ConvertSimple(int value)
        {

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

        public string GetTextSimple(int value)
        {

            if (value == 1)
                return "velmi dobré";

            if (value == 2)
                return "dobré";

            if (value == 3)
                return "uspokojivá";

            if (value == 4)
                return "vyhovující";

            if (value == 5)
                return "špatná";

            if (value == 6)
                return "velmi špatná";

            if (value == 7)
                return "neměří se";

            if (value == 8)
                return "nejsou data";

            return "N/A";
        }

    }
}
