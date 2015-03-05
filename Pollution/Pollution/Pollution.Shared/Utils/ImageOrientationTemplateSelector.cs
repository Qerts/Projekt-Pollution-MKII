using Pollution;
using Pollution.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Diagnostics;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Storage;
using Pollution.Utils;

namespace Utils
{
    public class ImageOrientationTemplateSelector : DataTemplateSelector
    {

        BitmapImage bitmap;
        int pixelWidth = 0, pixelHeight = 0;
        
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            // cast item to your custom item class
            var customItem = item as PPhoto;
            if (customItem == null)
                return null;

            string templateName = String.Empty;

            

            bitmap = new BitmapImage();
            bitmap.CreateOptions = BitmapCreateOptions.None;
            bitmap.ImageOpened +=bitmap_ImageOpened;
            bitmap = new BitmapImage(new Uri(customItem.FullURL));

            Image im = new Image();
            im.Source = bitmap;
      
            /*
            ////////////////////////////////////////////////////////
            var myBitmap = new BitmapImage();
            myBitmap.CreateOptions = BitmapCreateOptions.None;
            myBitmap.ImageOpened +=bitmap_ImageOpened;
            myBitmap = new BitmapImage(new Uri(customItem.FullURL));

            Image myImage = new Image();
            myImage.Source = myBitmap;

            ImageVisualisation myTest = new ImageVisualisation(myBitmap);
            double myHeight = myTest.GetHeight();
            double myWidth = myTest.GetWidth();
            

            ////////////////////////////////////////////////////////
            */


            if (pixelWidth > pixelHeight)
            {
                // image is horizontal
                templateName = "HorizontalItemTemplate";
            }
            else
            {
                templateName = "VerticalItemTemplate";
            }

            object template = null;
            // find template in App.xaml
            Application.Current.Resources.TryGetValue(templateName, out template);
            // find template in StationPage.xaml            
            return template as DataTemplate;
        }

        void image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void bitmap_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void image_ImageOpened(object sender, RoutedEventArgs e)
        {
            pixelWidth = bitmap.PixelWidth;
            pixelHeight = bitmap.PixelHeight;
        }

        private void bitmap_ImageOpened(object sender, RoutedEventArgs e)
        {
            pixelWidth = bitmap.PixelWidth;
            pixelHeight = bitmap.PixelHeight;
        }



    }
}
