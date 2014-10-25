﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
//using System.Windows.Media.Imaging;

namespace Pollution.ViewModels
{
    public class PPhoto
    {

        bool darkTheme = ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible);

        public string URL;
        public int Smile;
        public string Note;
        public DateTime Time;
        public Station Station; 
        public string StationCode;
        public string FullURL
        {
            get { return "http://data.garvis.cz/pollution/Uploads/" + URL; }
            set { }
        }
        /*
        public string TimeText
        {
            get { return Time.ToShortDateString() + " " + Time.ToLongTimeString(); }
        }
        */
        public string StationText
        {
            get { if (Station == null) return "";  return Station.Name; }
        }

        public string NoteText
        {
            get { return Note; }
        }


        public BitmapImage IconSmile
        {
            get
            {
                string c = "black";
                if (darkTheme) { c = "white"; }
                return new BitmapImage(new Uri("/icons/icon-smile"+Smile+"-"+(darkTheme?"white":"black")+".png", UriKind.RelativeOrAbsolute));
            }

        }



        public PPhoto()
        {

        }        
    }
}
