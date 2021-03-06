﻿using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Pollution
{
    public static class Data
    {

        private static Tuple<int, StatusName, string> SO2 = Tuple.Create(53, StatusName.Good, "SO\x2082");
        private static Tuple<int, StatusName, string> O3 = Tuple.Create(70, StatusName.Satisfying, "O\x2083");
        private static Tuple<int, StatusName, string> NO2 = Tuple.Create(600, StatusName.Bad, "NO\x2082");
        private static Tuple<int, StatusName, string> CO = Tuple.Create(23, StatusName.VeryGood, "CO");
        private static Tuple<int, StatusName, string> PM10 = Tuple.Create(45568, StatusName.VeryBad, "PM\x2081\x2080");

        public static int sample1State = 1;


        private static string STATION_NAME = "Ostrava-Fifejdy";
        private static string STATION_REGION = "Moravskoslezský kraj";
        private static string STATION_COORDS = "49°50'51\"N, 19°20'21\"E (+6 km)";
        
        public static string StationName { set { STATION_NAME = value; } get { return STATION_NAME; } }
        public static string StationRegion { set { STATION_REGION = value; } get { return STATION_REGION; } }
        public static string StationCoordinates { set { STATION_COORDS = value; } get { return STATION_COORDS; } }

        //FONT SIZES
        public static double getFontSize_StatuValue()
        {
            if (Window.Current.Bounds.Height > Window.Current.Bounds.Width)
            {
                return Window.Current.Bounds.Width / 18;
            }
            else
            {
                return Window.Current.Bounds.Height / 18;
            }
        }
        public static double getFontSize_Title()
        {
            if (Window.Current.Bounds.Height > Window.Current.Bounds.Width)
            {
                return Window.Current.Bounds.Width / 24;
            }
            else 
            {
                return Window.Current.Bounds.Height / 24;
            }
        }
        public static double getFontSize_LargeText()
        {
            if (Window.Current.Bounds.Height > Window.Current.Bounds.Width)
            {
                return Window.Current.Bounds.Width / 36;
            }
            else
            {
                return Window.Current.Bounds.Height / 36;
            }
        }
        public static double getFontSize_CommonText()
        {
            if (Window.Current.Bounds.Height > Window.Current.Bounds.Width)
            {
                return Window.Current.Bounds.Width / 48;
            }
            else
            {
                return Window.Current.Bounds.Height / 48;
            }
        }
        public static double getFontSize_SmallText()
        {
            if (Window.Current.Bounds.Height > Window.Current.Bounds.Width)
            {
                return Window.Current.Bounds.Width / 60;
            }
            else
            {
                return Window.Current.Bounds.Height / 60;
            }
        }


        public enum StatusName 
        {
            VeryGood,
            Good,
            Satisfying,
            Suitable,
            Bad,
            VeryBad,
            NoData,
            NoMeasurement
        }


        public static SolidColorBrush getMainColor()
        {
            return getColorAndStatus(getMainMood()).Item1;
        }

        public static StatusName getMainMood() 
        {
            switch (App.ViewModel.CurrentStation.Quality)
            {
                case 1: return StatusName.VeryGood;
                case 2: return StatusName.Good;
                case 3: return StatusName.Suitable;
                case 4: return StatusName.Satisfying;
                case 5: return StatusName.Bad;
                case 6: return StatusName.VeryBad;
                case 7: return StatusName.NoData;
                case 8: return StatusName.NoMeasurement;
            }
            return StatusName.NoMeasurement;
        }

        public static string getString_NO2() { return NO2.Item3; }
        public static string getString_SO2() { return SO2.Item3; }
        public static string getString_O3() { return O3.Item3; }
        public static string getString_PM10() { return PM10.Item3; }
        public static string getString_CO() { return CO.Item3; }

        public static StatusName getStatus_NO2() { return NO2.Item2; }
        public static StatusName getStatus_SO2() { return SO2.Item2; }
        public static StatusName getStatus_O3() { return O3.Item2; }
        public static StatusName getStatus_PM10() { return PM10.Item2; }
        public static StatusName getStatus_CO() { return CO.Item2; }

        public static int getValue_NO2() { return NO2.Item1; }
        public static int getValue_SO2() { return SO2.Item1; }
        public static int getValue_O3() { return O3.Item1; }
        public static int getValue_PM10() { return PM10.Item1; }
        public static int getValue_CO() { return CO.Item1; }











        public static Tuple<SolidColorBrush,string,string> getColorAndStatus(StatusName value) 
        {
            
            switch (value) 
            {
                case StatusName.VeryGood:
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 0, 138, 0)), "velmi dobré", @"ms-appx:SharedAssets/Smiley7.gif");
                case StatusName.Good:
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 132, 161, 47)), "dobré", @"ms-appx:SharedAssets/Smiley6.gif");
                case StatusName.Satisfying:
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 184, 177, 0)), "uspokojivé", @"ms-appx:SharedAssets/Smiley5.gif");
                case StatusName.Suitable:
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 141, 19)), "vyhovující", @"ms-appx:SharedAssets/Smiley4.gif");
                case StatusName.Bad:
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 84, 23)), "špatné", @"ms-appx:SharedAssets/Smiley3.gif");
                case StatusName.VeryBad:
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 51, 27)), "velmi špatné", @"ms-appx:SharedAssets/Smiley2.gif");
                case StatusName.NoData:
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 143, 143, 143)), "nejsou data", @"ms-appx:SharedAssets/Smiley0.gif");
                case StatusName.NoMeasurement:
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 138, 137, 156)), "neměří se", @"ms-appx:SharedAssets/Smiley0.gif");
                default:
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)), "chyba", @"ms-appx:SharedAssets/Smiley0.gif");
            }
        }

        public static Tuple<SolidColorBrush,string> GetSO2ColorAndStatus() 
        {
            int value = SO2.Item1;
            if (value < 120)
            {
                if (value < 50)
                {
                    if (value < 25)
                    {
                        //zbarvi se na velmi dobre
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 0, 138, 0)),"velmi dobré");                        
                    }
                    else
                    {
                        //zbarvi se na dobre
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 132, 161, 47)), "dobré");
                    }
                }
                else
                {
                    //zbarvi se na uspokojiva
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 184, 177, 0)), "uspokojivé");
                }
            }
            else
            {
                if (value < 350)
                {
                    //zbarvi se na vyhovujici
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 141, 19)), "vyhovující");
                }
                else
                {
                    if (value < 500)
                    {
                        //zbarvi se na spatna
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 84, 23)), "špatné");
                    }
                    else
                    {
                        //zbarvi se na velmi spatna
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 51, 27)), "velmi špatné");
                    }
                }
            }
            
        }         
        public static Tuple<SolidColorBrush, string> GetO3ColorAndStatus() 
        {
            int value = O3.Item1;

            if (value < 120)
            {
                if (value < 65)
                {
                    if (value < 33)
                    {
                        //zbarvi se na velmi dobre
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 0, 138, 0)), "velmi dobré");                        
                    }
                    else
                    {
                        //zbarvi se na dobre
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 132, 161, 47)), "dobré");
                    }
                }
                else
                {
                    //zbarvi se na uspokojiva
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 184, 177, 0)), "uspokojivé");
                }
            }
            else
            {
                if (value < 180)
                {
                    //zbarvi se na vyhovujici
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 141, 19)), "vyhovující");
                }
                else
                {
                    if (value < 240)
                    {
                        //zbarvi se na spatna
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 84, 23)), "špatné");
                    }
                    else
                    {
                        //zbarvi se na velmi spatna
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 51, 27)), "velmi špatné");
                    }
                }
            }

        }
        public static Tuple<SolidColorBrush, string> GetNO2ColorAndStatus() 
        {
            int value = NO2.Item1;
            if (value < 100)
            {
                if (value < 50)
                {
                    if (value < 25)
                    {
                        //zbarvi se na velmi dobre
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 0, 138, 0)), "velmi dobré");
                    }
                    else
                    {
                        //zbarvi se na dobre
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 132, 161, 47)), "dobré");
                    }
                }
                else
                {
                    //zbarvi se na uspokojiva
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 184, 177, 0)), "uspokojivé");
                }
            }
            else
            {
                if (value < 200)
                {
                    //zbarvi se na vyhovujici
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 141, 19)), "vyhovující");
                }
                else
                {
                    if (value < 400)
                    {
                        //zbarvi se na spatna
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 84, 23)), "špatné");
                    }
                    else
                    {
                        //zbarvi se na velmi spatna
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 51, 27)), "velmi špatné");
                    }
                }
            }
        }
        public static Tuple<SolidColorBrush, string> GetCOColorAndStatus()
        {
            int value = CO.Item1;
            if (value < 4000)
            {
                if (value < 2000)
                {
                    if (value < 1000)
                    {
                        //zbarvi se na velmi dobre
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 0, 138, 0)), "velmi dobré");
                    }
                    else
                    {
                        //zbarvi se na dobre
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 132, 161, 47)), "dobré");
                    }
                }
                else
                {
                    //zbarvi se na uspokojiva
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 184, 177, 0)), "uspokojivé");
                }
            }
            else
            {
                if (value < 10000)
                {
                    //zbarvi se na vyhovujici
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 141, 19)), "vyhovující");
                }
                else
                {
                    if (value < 30000)
                    {
                        //zbarvi se na spatna
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 84, 23)), "špatné");
                    }
                    else
                    {
                        //zbarvi se na velmi spatna
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 51, 27)), "velmi špatné");
                    }
                }
            }
        }
        public static Tuple<SolidColorBrush, string> GetPM10ColorAndStatus() 
        {
            int value = PM10.Item1;
            if (value < 70)
            {
                if (value < 40)
                {
                    if (value < 20)
                    {
                        //zbarvi se na velmi dobre
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 0, 138, 0)), "velmi dobré");
                    }
                    else
                    {
                        //zbarvi se na dobre
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 132, 161, 47)), "dobré");
                    }
                }
                else
                {
                    //zbarvi se na uspokojiva
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 184, 177, 0)), "uspokojivé");
                }
            }
            else
            {
                if (value < 90)
                {
                    //zbarvi se na vyhovujici
                    return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 141, 19)), "vyhovující");
                }
                else
                {
                    if (value < 180)
                    {
                        //zbarvi se na spatna
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 84, 23)), "špatné");
                    }
                    else
                    {
                        //zbarvi se na velmi spatna
                        return Tuple.Create(new SolidColorBrush(Color.FromArgb(255, 207, 51, 27)), "velmi špatné");
                    }
                }
            }

        }


        public static void CommandHandlers(IUICommand commandLabel)
        {
            var Actions = commandLabel.Label;
            switch (Actions)
            {

                case "Ok":
                    //rootPage.Focus(Windows.UI.Xaml.FocusState.Pointer);
                    break;
                case "Quit":
                    Application.Current.Exit();
                    break;
            }
        }
        
    }
}
