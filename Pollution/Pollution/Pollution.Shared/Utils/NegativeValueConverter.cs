using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Utils
{
    class NegativeValueConverter : IValueConverter  
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((double)value < 0)
            {
                return null;
            }
            else 
            {
                //toto přetypování řeší zobrazování desetinné tečky/čárky
                var d = (double)value;
                var s = d.ToString();
                value = s;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
