using System.Windows;
using System;
using System.Net;
using Utils;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;
using System.Globalization;
using Pollution.PollutionAgent;

namespace PollutionAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;
        /*
        private string stationCode;
        private GeoCoordinateWatcher watcher;
        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        private Dictionary<string, object[]> stations;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {

            if (ShellTile.ActiveTiles.Count() == 0) { NotifyComplete();  return; }


            //set lang of app
            string lang = "";
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("appLang", out lang);

            if (lang == null) lang = "";
            if (lang != "")
            {
                CultureInfo c = new CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = c;
            }
                       

            settings["lastTaskTime"] = DateTime.Now;
           

            if (task is PeriodicTask)
            {

                try
                {
                    stations = (Dictionary<string, object[]>)SerializationStorage.Load("stations.serial", typeof(Dictionary<string, object[]>));                        


                    bool g = false;
                    if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("useGPS", out g))
                    {
                        IsolatedStorageSettings.ApplicationSettings.Add("useGPS", true);

                        g = true;
                    }

                    bool b = false;
                    if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("nearestStation", out b))
                    {
                        IsolatedStorageSettings.ApplicationSettings.Add("nearestStation", true);

                        b = true;
                    }

                    stationCode = "";
                    if (b == true && g == true)
                    {
                        stationCode = (string)IsolatedStorageSettings.ApplicationSettings["lastNearestStation"];
                    }
                    else
                    {
                        stationCode = (string)IsolatedStorageSettings.ApplicationSettings["infoStation"];
                    }

                    if (b && g)
                    {
                        
                        watcher = new GeoCoordinateWatcher();
                        watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                        watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);

                        watcher.Start();
                    }
                    else
                    {
                        CreateTile("without GPS");
                    }

                }
                catch (Exception e)
                {
                    settings["lastTaskFailedTime"] = DateTime.Now;
                    settings["lastTaskFailedDetail"] = e.Message;
                    settings.Save();
                    NotifyComplete();
                }
            }
            else
            {
                // Execute resource-intensive task actions here.
            }


        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    CreateTile("GPS disabled");
                    break;

                case GeoPositionStatus.NoData:
                    CreateTile("GPS noData");
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {

            string nc = "";
            string ncn = "";
            double d = Double.MaxValue;
            double dp;

            if (e.Position == null) return;

            bool nearestWithoutStation = false;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("nearestWithoutStation", out nearestWithoutStation);

            
            foreach (var v in stations)
            {
                if (nearestWithoutStation == false && (int)v.Value[1] > 6) continue;
                
                dp = e.Position.Location.GetDistanceTo(new GeoCoordinate((double)v.Value[3], (double)v.Value[2]));
                    
                if (d > dp)
                {
                    d = dp;
                    nc = v.Key;
                }
            }

            stationCode = nc;
            

            watcher.Stop();

            CreateTile("GPS OK");
        }

        void wc_DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {

            if (e.Error != null || e.Result == null || e.Result == "")
            {
                stationCode = null;
                CreateTile("no data");
                return;
            }

            DateTime dt;
            int q;
            DateTime? lastPhoto;
            try
            { 

                using (StringReader reader = new StringReader(e.Result))
                {
                    string line;

                    line = reader.ReadLine();
                    //datetime of data
                    dt = new DateTime(Int64.Parse(line));

                    line = reader.ReadLine();
                    q = Int32.Parse(line);

                    line = reader.ReadLine();
                    if (line.Length == 0) lastPhoto = null;
                    else
                        lastPhoto = new DateTime(Int64.Parse(line));

                }

            
                string sn = "";
                if (stations != null) sn = (string)stations[stationCode][0];

                ShellTile appTile = ShellTile.ActiveTiles.FirstOrDefault();

                string filename = SaveTile(dt, q, sn, true, lastPhoto); //null if nothing changed


                if (appTile != null && filename != null)
                {
                    StandardTileData newTileData = new StandardTileData
                    {
                        BackgroundImage = new Uri("isostore:" + filename, UriKind.Absolute)

                    };
                    appTile.Update(newTileData);
                }

                settings["lastTaskOKTime"] = DateTime.Now;
                settings.Save();
                NotifyComplete();
            }
            catch (Exception exp)
            {
                settings["lastTaskFailedTime"] = DateTime.Now;
                settings["lastTaskFailedDetail"] = exp.Message;                                
                settings.Save();
                NotifyComplete();
            }
        }

        public void CreateTile(string status)
        {
            try
            {
                if (stationCode != null)
                {

                    string URL = "http://data.garvis.cz/pollution/stationquality";
                    //create URL request
                    string ut = DateTime.Now.Ticks.ToString();
                    long r = 158;
                    string manuf = Utils.WPVersions.GetManufacturer();

                    for (int i = 0; i < stationCode.Length; i++)
                    {
                        r = r + (int)(stationCode[i]);
                    }

                    for (int i = 0; i < ut.Length; i++)
                    {
                        r = r + (int)(ut[i]);
                    }

                    for (int i = 0; i < manuf.Length; i++)
                    {
                        r = r + (int)(manuf[i]);
                    }

                    WebClient wc = new WebClient();

                    wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                    wc.DownloadStringAsync(new Uri(URL + "?ns=" + stationCode + "&t=" + ut + "&m=" + manuf + "&key=" + r.ToString()));
                }
                else
                {
                    ShellTile appTile = ShellTile.ActiveTiles.FirstOrDefault();

                    string filename = SaveTile(DateTime.Now, -1, status, true); //null if nothing changed


                    if (appTile != null && filename != null)
                    {
                        StandardTileData newTileData = new StandardTileData
                        {
                            BackgroundImage = new Uri("isostore:" + filename, UriKind.Absolute)

                        };
                        appTile.Update(newTileData);
                    }

                    settings["lastTaskOKTime"] = DateTime.Now;
                    settings.Save();
                    NotifyComplete();
                }
            }
            catch (Exception exp)
            {
                settings["lastTaskFailedTime"] = DateTime.Now;
                settings["lastTaskFailedDetail"] = exp.Message;
                settings.Save();
                NotifyComplete();
            }
        }

        public void CreateTileDirect(DateTime dt, int quality, string station, DateTime? lastPhoto)
        {
            ShellTile appTile = ShellTile.ActiveTiles.FirstOrDefault();

            string filename = SaveTile(dt, quality, station, false, lastPhoto); //null if nothing changed


            if (appTile != null && filename != null)
            {
                StandardTileData newTileData = new StandardTileData
                {
                    BackgroundImage = new Uri("isostore:" + filename, UriKind.Absolute)

                };
                appTile.Update(newTileData);
            }
        }


        public WriteableBitmap RenderTile(DateTime dt, int quality, string station, out bool done, DateTime? lastPhoto=null)
        {

            if (Utils.WPVersions.IsTargetedVersion)
            {
                if (Utils.ResolutionHelper.CurrentResolution == Resolutions.WVGA)
                { 

                PollutionTile8 pt = new PollutionTile8();
                ColorQualityConverter ccq = new ColorQualityConverter();

                if (quality >= 0)
                {
                    pt.StaName.Text = station;
                    if (pt.StaName.Text.Length > 21) pt.StaName.Text = pt.StaName.Text.Substring(0, 21);

                    if (quality >= 7) pt.StaQuality.Text = "?";
                    else pt.StaQuality.Text = quality.ToString();

                    pt.StaQualityText.Text = (string)ccq.Convert(quality, typeof(string), null, null);
                    pt.Rect.Fill = (Brush)ccq.Convert(quality, typeof(Brush), null, null);

                    if (lastPhoto != null && ((DateTime)lastPhoto).AddDays(2) > DateTime.Now)
                    {
                        pt.iconLastPhoto.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        pt.iconLastPhoto.Visibility = Visibility.Collapsed;
                    }

                    string week;
                    if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "cs")
                    {
                        week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + ".týden";

                    }
                    else
                    {
                        week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + "th week";

                    }

                    pt.StaDate.Text = dt.ToString() + ", " + week;
                    if (pt.StaDate.Text.Length > 29) pt.StaDate.Text = pt.StaDate.Text.Substring(0, 29);                
                }
                else
                {
                    pt.StaName.Text = "";
                    pt.StaQuality.Text = "?!";
                    if (station == "no data")
                        pt.StaQualityText.Text = AppResources.TextNoData;
                    else
                        pt.StaQualityText.Text = "";

                    pt.iconLastPhoto.Visibility = Visibility.Collapsed;
                    pt.Rect.Fill = (Brush)ccq.Convert(quality, typeof(Brush), null, null);
                    pt.StaDate.Text = "";
                }

                pt.Measure(new Size(210, 210));
                pt.Arrange(new Rect(0, 0, 210, 210));

                WriteableBitmap bmp = new WriteableBitmap(pt, null);
                bmp.Invalidate();

                done = true;
                return bmp;
                }
                else
                {
                    PollutionTileHD pt = new PollutionTileHD();
                    ColorQualityConverter ccq = new ColorQualityConverter();

                    if (quality >= 0)
                    {
                        pt.StaName.Text = station;
                        //if (pt.StaName.Text.Length > 21) pt.StaName.Text = pt.StaName.Text.Substring(0, 21);

                        if (quality >= 7) pt.StaQuality.Text = "?";
                        else pt.StaQuality.Text = quality.ToString();

                        pt.StaQualityText.Text = (string)ccq.Convert(quality, typeof(string), null, null);
                        pt.Rect.Fill = (Brush)ccq.Convert(quality, typeof(Brush), null, null);

                        if (lastPhoto != null && ((DateTime)lastPhoto).AddDays(2) > DateTime.Now)
                        {
                            pt.iconLastPhoto.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            pt.iconLastPhoto.Visibility = Visibility.Collapsed;
                        }

                        string week;
                        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "cs")
                        {
                            week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + ".týden";

                        }
                        else
                        {
                            week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + "th week";

                        }

                        pt.StaDate.Text = dt.ToString() + ", " + week;
                        //if (pt.StaDate.Text.Length > 29) pt.StaDate.Text = pt.StaDate.Text.Substring(0, 29);
                    }
                    else
                    {
                        pt.StaName.Text = "";
                        pt.StaQuality.Text = "?!";
                        if (station == "no data")
                            pt.StaQualityText.Text = AppResources.TextNoData;
                        else
                            pt.StaQualityText.Text = "";

                        pt.iconLastPhoto.Visibility = Visibility.Collapsed;
                        pt.Rect.Fill = (Brush)ccq.Convert(quality, typeof(Brush), null, null);
                        pt.StaDate.Text = "";
                    }

                    pt.Measure(new Size(336, 336));
                    pt.Arrange(new Rect(0, 0, 336, 336));

                    WriteableBitmap bmp = new WriteableBitmap(pt, null);
                    bmp.Invalidate();

                    done = true;
                    return bmp;
                }

            }
            else
            {
                PollutionTile pt = new PollutionTile();
                ColorQualityConverter ccq = new ColorQualityConverter();

                if (quality >= 0)
                {
                    pt.StaName.Text = station;

                    if (quality >= 7) pt.StaQuality.Text = "?";
                    else pt.StaQuality.Text = quality.ToString();

                    pt.StaQualityText.Text = (string)ccq.Convert(quality, typeof(string), null, null);
                    pt.Rect.Fill = (Brush)ccq.Convert(quality, typeof(Brush), null, null);

                    pt.StaDate.Text = dt.ToString();
                }
                else
                {
                    pt.StaName.Text = "";
                    pt.StaQuality.Text = "?!";
                    if (station == "no data")
                        pt.StaQualityText.Text = AppResources.TextNoData;
                    else
                        pt.StaQualityText.Text = "";

                    pt.Rect.Fill = (Brush)ccq.Convert(quality, typeof(Brush), null, null);
                    pt.StaDate.Text = "";
                }

                pt.Measure(new Size(173, 173));
                pt.Arrange(new Rect(0, 0, 173, 173));

                WriteableBitmap bmp = new WriteableBitmap(pt, null);
                bmp.Invalidate();

                done = true;
                return bmp;
            }
        }

        public string SaveTile(DateTime dt, int quality, string station, bool background, DateTime? lastPhoto=null)
        {
            string filename = "/Shared/ShellContent/pollution-tile.png";


            try
            {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                IsolatedStorageFileStream fileStream;

                //check if new version is neccessary

                var settings = IsolatedStorageSettings.ApplicationSettings;

                WriteableBitmap b = null;
                bool bDone = false;

                if (background)
                {

                    Deployment.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        b = RenderTile(dt, quality, station, out bDone, lastPhoto);
                    }));
                    while (bDone != true)
                        Thread.Sleep(500);
                }
                else
                {
                    b = RenderTile(dt, quality, station, out bDone, lastPhoto);
                }

                //WriteableBitmap b = RenderTile(dt);

                if (b == null) return null;

                EditableImage eiImage = new EditableImage(b.PixelWidth, b.PixelHeight);

                for (int y = 0; y < b.PixelHeight; ++y)
                {
                    for (int x = 0; x < b.PixelWidth; ++x)
                    {
                        int pixel = b.Pixels[b.PixelWidth * y + x];
                        eiImage.SetPixel(x, y,
                        (byte)((pixel >> 16) & 0xFF),
                        (byte)((pixel >> 8) & 0xFF),
                        (byte)(pixel & 0xFF), (byte)((pixel >> 24) & 0xFF)
                        );
                    }
                }

                // Save it to disk
                Stream streamPNG = eiImage.GetStream();
                StreamReader srPNG = new StreamReader(streamPNG);
                byte[] baBinaryData = new Byte[streamPNG.Length];
                long bytesRead = streamPNG.Read(baBinaryData, 0, (int)streamPNG.Length);

                fileStream = new IsolatedStorageFileStream(filename, FileMode.Create, myIsolatedStorage);
                fileStream.Write(baBinaryData, 0, baBinaryData.Length);
                //b.SaveJpeg(fileStream, b.PixelWidth, b.PixelHeight, 0, 100);
                fileStream.Close();
            }

            return filename;

            }
            catch (Exception exp)
            {
                settings["lastTaskFailedTime"] = DateTime.Now;
                settings["lastTaskFailedDetail"] = exp.Message;
                settings.Save();
                NotifyComplete();
                return null;
            }
        }*/
    }
}