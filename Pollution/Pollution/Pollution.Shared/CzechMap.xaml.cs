using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Pollution.ViewModels;
using Pollution;
using Utils;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;

namespace Pollution
{
    public partial class CzechMap : UserControl
    {
        public CzechMap()
        {
            InitializeComponent();
        }

        public void UpdateStations()
        {

            double minlon = 12.18;
            double minlat = 48.50;
            double maxlon = 18.93;
            double maxlat = 51.02;
            //lat - zespodu - nahoru
            //lon - zleva - doprava

            double lonC = overlay.Width / (maxlon - minlon);
            double latC = overlay.Height / (maxlat - minlat);

            double y;

            ColorQualityConverter ccq = new ColorQualityConverter();

            overlay.Children.Clear();

            foreach (Station s in App.ViewModel.Stations)
            {
                Ellipse el = new Ellipse();
                el.Width = 32;
                el.Height = 32;
                el.Fill = (Brush)ccq.Convert(s.Quality, typeof(Brush), null, null);
                if (s.Quality >= 7) el.Opacity = 0.5;
                Canvas.SetLeft(el, (s.Position.Longitude - minlon) * lonC);
                Canvas.SetTop(el, overlay.Height - ((s.Position.Latitude - minlat) * latC));
                overlay.Children.Add(el);
            }

        }

    }
}
