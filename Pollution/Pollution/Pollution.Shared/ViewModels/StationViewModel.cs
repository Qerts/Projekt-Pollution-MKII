using System;
using System.Net;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Linq;
//using System.Device.Location;
using System.Collections.Generic;
using System.Xml;
//using Microsoft.Phone.Controls.Maps;
//using Microsoft.Phone.Shell;
//using System.IO.IsolatedStorage;
using System.Globalization;
//using System.Windows.Threading;
//using Resources;
using System.IO;
using Utils;
//using Utils;

namespace Pollution.ViewModels
{
    public enum ESortStationType
    {
        ALPHA = 0, DISTANCE, GOOD, POOR
    }

    public class StationViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Station> Stations { get; private set; }
        //private Dictionary<string, GeoCoordinate> StationPositions { set; get; }
        private Dictionary<int, int> stationCount { set; get; }
        //private GeoCoordinateWatcher watcher;

        private ESortStationType currentSort;

        //public GeoCoordinate MyPosition { set; get; }

        private Station nearestStation;
        private Station currentStation;
        private Station detailsStation;

        private Station cameraStation;

        private PHistory historyDetailsStation;
        private List<PPhoto> photosDetailsStation;
        private List<PPhoto> photosGlobal;


        private DateTime dataTime;

        //private string pivotPhotosHeader = AppResources.AppPhotos;

        public string RawData { get; set; }
        //public PinManager PinManager { get; set; }
        //public Pin PinMan { get; set; }

        public DateTime LastPositionTime { get; set; }
        public bool AutoCurrentNearest { get; set; }

        public StationViewModel()
        {
            Stations = new ObservableCollection<Station>();
            //PinManager = new PinManager();
            //PinMan = new Pin(EPinType.MAN);
            //PinMan.IsSelected = true;

            stationCount = new Dictionary<int, int>();

            /*CurrentStation = new Station();
            DetailsStation = new Station();
            NearestStation = new Station();
            MyPosition = null;*/

        }


        // Indicates if data is loaded
        public bool IsDataLoaded { private set; get; }
        private bool isBusy = false;
        /// <summary>
        /// Indikace, že se provádí operace nad zdroji
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (value != isBusy)
                {
                    isBusy = value;
                    //NotifyPropertyChanged("IsBusy");
                }
            }
        }

        private bool isGPSBusy = false;
        /// <summary>
        /// Indikace, že se provádí operace nad zdroji
        /// </summary>
        public bool IsGPSBusy
        {
            get
            {
                return isGPSBusy;
            }
            set
            {
                if (value != isGPSBusy)
                {
                    isGPSBusy = value;
                    //NotifyPropertyChanged("IsGPSBusy");
                }
            }
        }

        private bool isGPS = true;
        /// <summary>
        /// Indikace, že se provádí operace nad zdroji
        /// </summary>
        public bool IsGPS
        {
            get
            {
                return isGPS;
            }
            set
            {
                if (value != isGPS)
                {
                    isGPS = value;
                    //NotifyPropertyChanged("IsGPS");
                }
            }
        }

        public Station NearestStation
        {
            get
            {
                return nearestStation;
            }
            set
            {
                if (value != nearestStation)
                {
                    nearestStation = value;
                    if (value != null) NotifyPropertyChanged("NearestStation");

                    //IsolatedStorageSettings.ApplicationSettings["lastNearestStation"] = value.Code;

                }

            }
        }
        public Station CurrentStation
        {
            get
            {
                return currentStation;
            }
            set
            {
                if (value != currentStation)
                {
                    currentStation = value;
                    //if (value != null) NotifyPropertyChanged("CurrentStation");

                }

            }
        }

        public Station DetailsStation
        {
            get
            {
                return detailsStation;
            }
            set
            {
                if (value != detailsStation)
                {
                    detailsStation = value;
                    //if (value != null) NotifyPropertyChanged("DetailsStation");

                }
            }
        }

        public Station CameraStation
        {
            get
            {
                return cameraStation;
            }
            set
            {
                if (value != cameraStation)
                {
                    cameraStation = value;
                    //if (value != null) NotifyPropertyChanged("CameraStation");

                }
            }
        }

        public PHistory HistoryDetailsStation
        {
            get
            {
                return historyDetailsStation;
            }
            set
            {
                if (value != historyDetailsStation)
                {
                    historyDetailsStation = value;
                    //if (value != null) NotifyPropertyChanged("HistoryDetailsStation");
                }

            }
        }

        public List<PPhoto> PhotosDetailsStation
        {
            get
            {
                return photosDetailsStation;
            }
            set
            {
                if (value != photosDetailsStation)
                {
                    photosDetailsStation = value;
                    //if (value != null) NotifyPropertyChanged("PhotosDetailsStation");
                }

            }
        }

        public List<PPhoto> PhotosGlobal
        {
            get
            {
                return photosGlobal;
            }
            set
            {
                if (value != photosGlobal)
                {
                    photosGlobal = value;
                    //if (value != null) NotifyPropertyChanged("PhotosGlobal");
                }

            }
        }

        public Dictionary<int, int> StationCount
        {
            get
            {
                return stationCount;
            }
        }

        public DateTime DataTime
        {
            get
            {
                return dataTime;
            }
            set
            {
                if (value != dataTime)
                {
                    dataTime = value;
                    //NotifyPropertyChanged("DataTime");
                }
            }
        }

        /*
        public string PivotPhotosHeader
        {
            get
            {
                return pivotPhotosHeader;
            }
            set
            {
                if (value != pivotPhotosHeader)
                {
                    pivotPhotosHeader = value;
                    NotifyPropertyChanged("PivotPhotosHeader");
                }
            }
        }
        */
        public void DoLoadData()
        {
            this.IsBusy = true;
            //LoadPositions();
            //LoadData();

            LoadDataGarvis();

            //InitializePins();
            this.IsBusy = false;
        }


        /*
        /// <summary>
        /// Load station positions from xml file
        /// </summary>
        private void LoadPositions()
        {
            StationPositions = new Dictionary<string, GeoCoordinate>();
            XDocument d = XDocument.Load("SampleData/Positions.xml");

            var list = from c in d.Descendants("location")
                       select c;

            string name;
            GeoCoordinate gc;

            foreach (var l in list)
            {
                gc = new GeoCoordinate();
                name = l.Attribute("id").Value;
                gc.Latitude = Double.Parse(l.Attribute("x").Value, CultureInfo.InvariantCulture.NumberFormat);
                gc.Longitude = Double.Parse(l.Attribute("y").Value, CultureInfo.InvariantCulture.NumberFormat);

                StationPositions.Add(name, gc);
            }
        }
        */

        async void LoadDataGarvis()
        {
            //Stations = new ObservableCollection<Station>();


            Dictionary<string, object[]> stationsDict = new Dictionary<string, object[]>();

            stationCount = new Dictionary<int, int>();
            for(int i = 1; i<=8; i++)
                stationCount.Add(i, 0);


            string s = RawData;
            if (s != null && s.Length != 0)
            {
                try
                {
                    using (StringReader reader = new StringReader(s))
                    {
                        string line;

                        line = reader.ReadLine();
                        //datetime of data
                        DataTime = new DateTime(Int64.Parse(line));

                        Station sta;
                        string[] sl;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Length == 0) continue;
                            try
                            {
                                sl = line.Split('|');

                                sta = new Station();

                                sta.Id = Int32.Parse(sl[0]);
                                sta.Code = sl[1];
                                sta.Name = sl[2];
                                sta.Classification = GetClassification(sl[3]);
                                sta.Owner = sl[4];
                                //AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
                                //sta.Position = new GeoCoordinate(Double.Parse(sl[6], CultureInfo.InvariantCulture), Double.Parse(sl[5], CultureInfo.InvariantCulture));
                                sta.Quality = Int32.Parse(sl[7]);

                                if (sl[8] == "") sta.So2.Value = -1; else sta.So2.Value = Double.Parse(sl[8], CultureInfo.InvariantCulture);
                                sta.So2.State = Int32.Parse(sl[9]);

                                if (sl[10] == "") sta.No2.Value = -1; else sta.No2.Value = Double.Parse(sl[10], CultureInfo.InvariantCulture);
                                sta.No2.State = Int32.Parse(sl[11]);

                                if (sl[12] == "") sta.Co.Value = -1; else sta.Co.Value = Double.Parse(sl[12], CultureInfo.InvariantCulture);
                                sta.Co.State = Int32.Parse(sl[13]);

                                if (sl[14] == "") sta.O3.Value = -1; else sta.O3.Value = Double.Parse(sl[14], CultureInfo.InvariantCulture);
                                sta.O3.State = Int32.Parse(sl[15]);

                                if (sl[16] == "") sta.Pm10.Value = -1; else sta.Pm10.Value = Double.Parse(sl[16], CultureInfo.InvariantCulture);
                                sta.Pm10.State = Int32.Parse(sl[17]);

                                if (sl[18] == "") sta.Pm24.Value = -1; else sta.Pm24.Value = Double.Parse(sl[18], CultureInfo.InvariantCulture);
                                sta.Pm24.State = Int32.Parse(sl[19]);

                                //new Time of Last Image
                                if (sl[20] == "") sta.LastPhoto = null; else sta.LastPhoto = new DateTime(Int64.Parse(sl[20]));

                                sta.UpdateStatesBasedOnValues();

                                Stations.Add(sta); // Add to list of stations        
                                //AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
                               // stationsDict.Add(sta.Code, new object[] {sta.Name, sta.Quality, sta.Position.Longitude, sta.Position.Latitude});

                                // add count
                                if (stationCount.ContainsKey(sta.Quality))
                                {
                                    stationCount[sta.Quality]++;
                                }
                                else
                                {
                                    stationCount.Add(sta.Quality, 1);
                                }

                            }
                            catch (Exception e)
                            {
                            }

                        }
                    }

                    try
                    {
                        await SerializationStorage.Save("stations.serial", stationsDict);
                    }
                    catch (Exception e)
                    {
                    
                    }

                }
                catch(Exception e)
                {
                    //vyhodit ulozeni vyjimky

                }


            }

            Stations.Sort(i => i.Name);

            



            //watcher.Start(); // Start locating my position
            IsDataLoaded = true; // Data is loaded

            // Load settings
            //int infoStation = (int)IsolatedStorageSettings.ApplicationSettings["infoStation"];

            // If user wants particular station, then set it on the infoPage,
            // if he wants the nearest station, it will be set when the position of device is gained
            //if (infoStation > 0)
            //    SetInfoPage(Stations[infoStation-1]);

            //NotifyPropertyChanged("Stations"); // Notify change
        }
        /*
        public void SortStations(ESortStationType type)
        {

            
            currentSort = type;

            switch (type)
            {
                case ESortStationType.ALPHA:
                    Stations.Sort(i => i.Name);
                    break;
                case ESortStationType.DISTANCE:
                    Stations.Sort(i => i.Distance);
                    break;
                case ESortStationType.GOOD:
                    Stations.Sort2Levels(i => i.Quality, i => i.Distance);
                    break;
                case ESortStationType.POOR:
                    Stations.SortDesc2Levels(i => i.Quality, i => i.Distance);
                    break;

            }
        }
        */
        /*
        public bool StartGeoWatcher(bool high = true)
        {
            MyPosition = null;
            LastPositionTime = DateTime.Now;
            try
            {
                if(high)
                    watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                else
                    watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);

                watcher.PositionChanged += this.watcher_PositionChanged;
                watcher.StatusChanged += this.watcher_StatusChanged;
                IsGPSBusy = true;
                watcher.Start();
                //if (!watcher.TryStart(false, TimeSpan.FromMilliseconds(2000)))
                //{
                 //   watcher = null;
                  //  return false;
                //}
            }
            catch
            {
                watcher = null;
                return false;
            }

            return true;

        }

        public void StopGeoWatcher()
        {
            if (IsGPSBusy == true && watcher != null)
            {
                IsGPSBusy = false;
                watcher.Stop();
                watcher = null;
            }
        }

        private void InitializePins()
        {
            //Správce špendlíků (ikon na mapě)
            App.ViewModel.PinManager = new PinManager();
            //pinManager.Load();

            App.ViewModel.PinManager.Pins = new System.Collections.ObjectModel.ObservableCollection<Pin>();
            App.ViewModel.PinManager.Pins.Add(App.ViewModel.PinMan);

            Pin pinStation;
            foreach (Station sta in App.ViewModel.Stations)
            {
                pinStation = new Pin(EPinType.STATION);
                pinStation.Location = sta.Position;
                pinStation.Station = sta;
                App.ViewModel.PinManager.Pins.Add(pinStation);
            }
        }


        /// <summary>
        /// This method will convert the color on the website, to a number indicating state of the air
        /// </summary>
        /// <param name="state">CSS attribute from the website</param>
        /// <returns>Number indicating the state</returns>
        public int GetState(string state)
        {
            

            if (state == "_ix1")
               return 1;

           if (state == "_ix2")
               return 2;

           if (state == "_ix3")
               return 3;

           if (state == "_ix4")
               return 4;

           if (state == "_ix5")
               return 5;

           if (state == "_ix6")
               return 6;

           if (state == "_gr")
               return 7;

           if (state == "_wh")
               return 8;

            return 0;
        }
        */
        /// <summary>
        /// This method will convert the color on the website, to a number indicating state of the air
        /// </summary>
        /// <param name="state">CSS attribute from the website</param>
        /// <returns>Number indicating the state</returns>
        public EStationClassification GetClassification(string state)
        {
            
         
            if (state == "T")
                return EStationClassification.TRAFFIC;

            if (state == "IC")
                return EStationClassification.INTERCITY;

            if (state == "AC")
                return EStationClassification.AROUNDCITY;

            if (state == "V")
                return EStationClassification.VILLAGE;

            if (state == "I")
                return EStationClassification.INDUSTRY;

            return EStationClassification.NONE;
        }
        /*
        /// <summary>
        /// This method will convert the value from the website to float. There is a need for proccessing
        /// because not every value is filled for every station.
        /// </summary>
        /// <param name="value">Value read from the website</param>
        /// <returns></returns>
        public float GetValue(string value)
        {
            string v = value;
            v = v.Replace(" ", "");
            if (v == "")
                v = "-1,0";

            return Convert.ToSingle(v, new CultureInfo("cs-CZ"));
        }



        public void Notify()
        {
            NotifyPropertyChanged("Stations");

        }

        public Station GetStation(string s)
        {
            foreach (Station st in Stations)
            {
                if (st.Code == s) return st;
            }
            return null;

        }


        public event PropertyChangedEventHandler PropertyChanged;
        */
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /*
        /// <summary>
        /// When device is located, proccess his location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            
            var pos = e.Position.Location; // Get location

            //if (pos.HorizontalAccuracy <= 1000) // Accuracy must be up to 2 km
            if (MyPosition != null)
            {
                if (LastPositionTime.AddSeconds(5) <= DateTime.Now || (MyPosition.Longitude == pos.Longitude && MyPosition.Latitude == pos.Latitude))
                {
                    watcher.Stop(); // We dont need positioning anymore      
                    IsGPSBusy = false;
                    return;
                }
              
              
            }
                        
            MyPosition = pos;
            NotifyPropertyChanged("MyPosition");
            CalculateDistances(pos); // Calculate distances to every station
            

        }

        /// <summary>
        /// When locating status is changed. For handling states like GPS DISABLED, ..
        /// This will inform user if he has forbid GPS locating to this application, or 
        /// currently device cant locate itself. This has only informative function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            string s;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("lastNearestStation", out s);

            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    
                    IsGPSBusy = false;
                    IsGPS = false;

                    MessageBox.Show(AppResources.MsgGPSDisabled, AppResources.Warning, MessageBoxButton.OK);

                    if (s != null) App.ViewModel.CurrentStation = App.ViewModel.GetStation(s);

                    break;

                case GeoPositionStatus.NoData:
                    //MessageBox.Show(AppResources.MsgGPSUnavailable, AppResources.Warning, MessageBoxButton.OK);
                    IsGPSBusy = false;
                    IsGPS = false;

                    if (s != null) App.ViewModel.CurrentStation = App.ViewModel.GetStation(s);

                    break;
            }


        }

        /// <summary>
        /// Will calculate distance from given position (my position) to every station
        /// </summary>
        /// <param name="gc"></param>
        public void CalculateDistances(GeoCoordinate gc)
        {
           
            bool nearestWithoutStation = false;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("nearestWithoutStation", out nearestWithoutStation);

            bool useGPS = false;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("useGPS", out useGPS);

            Station tmp = null;

            foreach (Station sta in Stations)
            {

                if (useGPS == false)
                {
                    sta.Distance = -1;
                    continue;
                }
                sta.Distance = gc.GetDistanceTo(sta.Position); // Calculate distance

                //eliminate nearest stations without quality value
                if (nearestWithoutStation == false && sta.Quality > 6) continue;

                if (tmp == null || sta.Distance < tmp.Distance) // Check if this is the nearest station
                    tmp = sta;
            }
            if (tmp!= null && tmp != NearestStation) NearestStation = tmp; // Set the nearest station

            if (AutoCurrentNearest) { CurrentStation = NearestStation; }

            if (useGPS)
            {
                ESortStationType t = (ESortStationType)IsolatedStorageSettings.ApplicationSettings["sortType"];

                if (t == ESortStationType.DISTANCE)
                {
                    App.ViewModel.SortStations(t);
                }
            }

        }
        */

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
