using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Pollution.Utils
{
    class BackGroundImageConverter : IValueConverter
    {
        private Random _random = new Random();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var image = new ImageBrush();
            var i = _random.Next();
            if ((int)value == 1)
            {                
                switch (i%2)
                {
                    case 0:
                        image.ImageSource = new BitmapImage(new Uri(@"ms-appx:SharedAssets/velmi-dobra1.jpg", UriKind.RelativeOrAbsolute));
                        break;
                    case 1:
                        image.ImageSource = new BitmapImage(new Uri(@"ms-appx:SharedAssets/velmi-dobra2.jpg", UriKind.RelativeOrAbsolute));
                        break;
                }
                return image;
            }
            if ((int)value == 2)
            {
                switch (i % 2)
                {
                    case 0:
                        image.ImageSource = new BitmapImage(new Uri(@"ms-appx:SharedAssets/dobra1.jpg", UriKind.RelativeOrAbsolute));
                        break;
                    case 1:
                        image.ImageSource = new BitmapImage(new Uri(@"ms-appx:SharedAssets/dobra2.jpg", UriKind.RelativeOrAbsolute));
                        break;
                }
                return image;
            }
            if ((int)value == 3)
            {
                switch (i % 2)
                {
                    case 0:
                        image.ImageSource = new BitmapImage(new Uri(@"ms-appx:SharedAssets/uspokojiva1.jpg", UriKind.RelativeOrAbsolute));
                        break;
                    case 1:
                        image.ImageSource = new BitmapImage(new Uri(@"ms-appx:SharedAssets/uspokojiva2.jpg", UriKind.RelativeOrAbsolute));
                        break;
                }
                return image;
            }
            if ((int)value == 4)
            {
                switch (i % 2)
                {
                    case 0:
                        image.ImageSource = new BitmapImage(new Uri(@"ms-appx:SharedAssets/vyhovujici1.jpg", UriKind.RelativeOrAbsolute));
                        break;
                    case 1:
                        image.ImageSource = new BitmapImage(new Uri(@"ms-appx:SharedAssets/vyhovujici2.jpg", UriKind.RelativeOrAbsolute));
                        break;
                }
                return image;
            }
            if ((int)value == 5)
            {
                switch (i % 2)
                {
                    case 0:
                        image.ImageSource = new BitmapImage(new Uri(@"ms-appx:SharedAssets/spatna1.jpg", UriKind.RelativeOrAbsolute));
                        break;
                    case 1:
                        image.ImageSource = new BitmapImage(new Uri(@"ms-appx:SharedAssets/spatna2.jpg", UriKind.RelativeOrAbsolute));
                        break;
                }
                return image;
            }
            if ((int)value == 6)
            {
                switch (i % 2)
                {
                    case 0:
                        image.ImageSource = new BitmapImage(new Uri(@"ms-appx:SharedAssets/velmi-spatna1.jpg", UriKind.RelativeOrAbsolute));
                        break;
                    case 1:
                        image.ImageSource = new BitmapImage(new Uri(@"ms-appx:SharedAssets/velmi-spatna2.jpg", UriKind.RelativeOrAbsolute));
                        break;
                }
                return image;
            }
            if ((int)value == 7)
            {
                return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
            if ((int)value == 8)
            {
                return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
            return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
