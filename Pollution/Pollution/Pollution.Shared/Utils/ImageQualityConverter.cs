using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Pollution.Utils
{
    class ImageQualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((int)value == 1)
                return new BitmapImage(new Uri(@"ms-appx:SharedAssets/Smiley7.png"));

            if ((int)value == 2)
                return new BitmapImage(new Uri(@"ms-appx:SharedAssets/Smiley6.png"));

            if ((int)value == 3)
                return new BitmapImage(new Uri(@"ms-appx:SharedAssets/Smiley5.png"));

            if ((int)value == 4)
                return new BitmapImage(new Uri(@"ms-appx:SharedAssets/Smiley4.png"));

            if ((int)value == 5)
                return new BitmapImage(new Uri(@"ms-appx:SharedAssets/Smiley2.png"));

            if ((int)value == 6)
                return new BitmapImage(new Uri(@"ms-appx:SharedAssets/Smiley3.png"));

            if ((int)value == 7)
                return new BitmapImage(new Uri(@"ms-appx:SharedAssets/Smiley0.png"));

            if ((int)value == 8)
                return new BitmapImage(new Uri(@"ms-appx:SharedAssets/Smiley0.png"));

            return new BitmapImage(new Uri(@"ms-appx:SharedAssets/Smiley0.png")); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
