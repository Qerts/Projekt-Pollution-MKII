using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;
using System.IO;
using Utils;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using WinRTXamlToolkit.Controls.Extensions;

namespace Pollution.ViewModels
{
    public enum ESortStationType
    {
        ALPHA = 0, DISTANCE, GOOD, POOR
    }

    public enum GPSSTATUS
    {
        GPSactive, GPSdisabled, GPSnotactive,
        GPSenabled
    }

    public class StationViewModel : INotifyPropertyChanged
    {
        #region /// PROPERTIES ///
        public ObservableCollection<Station> Stations { get; private set; }
        private Dictionary<string, MyGeocoordinate> StationPositions { set; get; }
        private Dictionary<int, int> stationCount { set; get; }
        public string RawData { get; set; }
        public PinManager PinManager { get; set; }
        public Pin PinMan { get; set; }
        private DateTime lastPositionTime;
        public DateTime LastPositionTime
        {
            get
            {
                return lastPositionTime;
            }
            set
            {
                if (value != lastPositionTime)
                {
                    lastPositionTime = value;
                    if (value != null)
                    {
                        CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            NotifyPropertyChanged("LastPositionTime");
                        });

                    }

                }

            }
        }
        public bool AutoCurrentNearest { get; set; }
        public bool IsDataLoaded { private set; get; }

        //lokátor gps
        private Geolocator locator;
        //localsettings
        private Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        //resources
        private ResourceLoader _resourceLoader = new ResourceLoader();
        //
        private DateTime dataTime;
        //
        private string pivotPhotosHeader;
        GPSService _gpsService = new GPSService();

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
                    NotifyPropertyChanged("DataTime");
                }
            }
        }
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

        private ESortStationType currentSort;

        private bool isLoaded = false;
        public bool IsLoaded
        {
            get { return isLoaded; }
            set { isLoaded = value; }
        }
        private bool isReady = false;
        public bool IsReady
        { get { return isReady; } set { isReady = value; } }
        private MyGeocoordinate myPosition;
        public MyGeocoordinate MyPosition
        {
            get
            {
                return myPosition;
            }
            set
            {
                if (value != myPosition)
                {
                    myPosition = value;
                    if (value != null)
                    {
                        CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            NotifyPropertyChanged("MyPosition");
                        });
                        
                    }

                }

            }
        }
        private Station nearestStation;
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
                    if (value != null)
                    {
                        NotifyPropertyChanged("NearestStation");

                        //IsolatedStorageSettings.ApplicationSettings["lastNearestStation"] = value.Code;
                        _localSettings.Containers["AppSettings"].Values["lastNearestStation"] = value.Code;
                    }

                }

            }
        }
        private Station currentStation;
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
                    if (value != null && value.Name != "")
                    {
                        currentStation = value;
                        CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            NotifyPropertyChanged("CurrentStation");
                        });
                    }
                }

            }
        }
        private Station detailsStation;
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
                    if (value != null) NotifyPropertyChanged("DetailsStation");

                }
            }
        }
        private Station cameraStation;
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
                    if (value != null) NotifyPropertyChanged("CameraStation");

                }
            }
        }
        private PHistory historyDetailsStation;
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
                    if (value != null) NotifyPropertyChanged("HistoryDetailsStation");
                }

            }
        }
        private List<PPhoto> photosDetailsStation;
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
                    if (value != null) NotifyPropertyChanged("PhotosDetailsStation");
                }

            }
        }
        private List<PPhoto> photosGlobal;
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
                    if (value != null) NotifyPropertyChanged("PhotosGlobal");
                }

            }
        }
        private bool isBusy = false;
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
                    NotifyPropertyChanged("IsBusy");
                }
            }
        }
        private bool isGPSBusy = false;
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
                    NotifyPropertyChanged("IsGPSBusy");
                }
            }
        }
        private bool isGPS = true;
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
                    NotifyPropertyChanged("IsGPS");
                }
            }
        }
        private string gPSStatus = GPSSTATUS.GPSnotactive.ToString();
        public string GPSStatus 
        {
            get
            {
                switch (gPSStatus)
                {
                    case "GPSactive":
                        return @"Assets/gpsavailable-white.png";//_resourceLoader.GetString("GPSisActive");
                    case "GPSdisabled":
                        return @"Assets/reducedgps-white.png";//_resourceLoader.GetString("GPSisDisabled");
                    case "GPSnotactive":
                        return @"Assets/nogps-white.png";//_resourceLoader.GetString("GPSisNotActive");
                    case "GPSenabled":
                        return @"Assets/gpsavailable-white.png";//_resourceLoader.GetString("GPSisEnabled");
                    default:
                        return "error";
                }
            }
            set
            {
                if (value != gPSStatus)
                {
                    gPSStatus = value;
                    NotifyPropertyChanged("GPSStatus");
                }
            }
        }
        #endregion

        public StationViewModel()
        {
            Stations = new ObservableCollection<Station>();
            PinManager = new PinManager();
            PinMan = new Pin(EPinType.MAN);
            PinMan.IsSelected = true;

            stationCount = new Dictionary<int, int>();

            CurrentStation = new Station();
            DetailsStation = new Station();
            NearestStation = new Station();

            pivotPhotosHeader = _resourceLoader.GetString("AppPhotos");

        }
        
        /// <summary>
        /// Tato funkce natáhne data z App.ViewModel.Rawdata do view modelu a inicializuje piny.
        /// </summary>
        public async Task LoadDataToModel()
        {
            this.IsBusy = true;

            await LoadDataGarvis(Stations); //rozparsuje raw data a načte je do modelu
            //LoadPositions();

            InitializePins();

            this.IsBusy = false;

            
        }

        public async Task LoadDataGarvis(ObservableCollection<Station> stations)
        {
            stations.Clear();                                                               //vynulovat seznam stanic
            Dictionary<string, object[]> stationsDict = new Dictionary<string, object[]>(); //vytvořit nový dictionary pro stanice
            stationCount = new Dictionary<int, int>();                                      //vytvořit nový dict pro druhy stanic???
            for(int i = 1; i<=8; i++)
            {
                stationCount.Add(i, 0);
            }

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
                                sta.Position = new MyGeocoordinate(Double.Parse(sl[6], CultureInfo.InvariantCulture), Double.Parse(sl[5], CultureInfo.InvariantCulture));
                                
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

                                stations.Add(sta); // Add to list of stations 
                                stationsDict.Add(sta.Code, new object[] {sta.Name, sta.Quality, sta.Position.Longitude, sta.Position.Latitude});

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
                                throw e;
                            }
                        }
                    }

                    try
                    {
                        //zakomentováno pro účely debuggu
                        //await SerializationStorage.Save("stations.serial", stationsDict);
                    }
                    catch (Exception e)
                    {
                        throw e;                    
                    }

                }
                catch(Exception e)
                {
                    throw e;
                }
            }

            //Z nějakého důvodu vyhazuje nullreference exception, z důvodu testování uzavřeno do try
            try
            {
                stations.Sort(i => i.Name);
            }
            catch (Exception e)
            {
                throw e;
            }

            Stations = stations;
            IsDataLoaded = true; // Data is loaded
        }

        /// <summary>
        /// Load station positions from xml file
        /// </summary>
        private void LoadPositions()
        {
            StationPositions = new Dictionary<string, MyGeocoordinate>();
            XDocument d = XDocument.Load("SampleData/Positions.xml");

            var list = from c in d.Descendants("location")
                       select c;

            string name;
            MyGeocoordinate gc;

            foreach (var l in list)
            {
                gc = new MyGeocoordinate();
                name = l.Attribute("id").Value;                
                gc.Latitude = Double.Parse(l.Attribute("x").Value, CultureInfo.InvariantCulture.NumberFormat);
                gc.Longitude = Double.Parse(l.Attribute("y").Value, CultureInfo.InvariantCulture.NumberFormat);

                StationPositions.Add(name, gc);
            }
        }

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
      
        private void InitializePins()
        {
            //Správce špendlíků (ikon na mapě)
            App.ViewModel.PinManager = new PinManager();
            

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

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        
        
        

        public event PropertyChangedEventHandler PropertyChanged;


        #region /// LOCATION
        /// <summary>
        /// Tato metoda inicializuje a spustí geolocator.
        /// </summary>
        public void SetGeolocator(bool high = true)
        {
            //Nastavení pozice, čistě pro případ
            //MyPosition = null;
            //Zaznačení času posledního určování pozice
            LastPositionTime = DateTime.Now;

            //Pokus o spuštění nastavení watcheru
            try
            {
                if (high)
                {
                    locator = new Geolocator();
                    locator.DesiredAccuracy = PositionAccuracy.High;
                }
                else
                {
                    locator = new Geolocator();
                    locator.DesiredAccuracy = PositionAccuracy.Default;
                }

                locator.ReportInterval = 5000;
                //Když nastane změna pozice, je načtena a uložena nová pozice. Zároveň je volána metoda pro vypočtení vzdálenosti aktuální pozice vůči ostatním stanicím.
                locator.PositionChanged += locator_PositionChanged;
                //Když nastane změna stavu lokátoru, při které bude nefukční, vyskočí hláška.
                locator.StatusChanged += locator_StatusChanged;
                

            }
            catch
            {
                locator = null;
            }
        }
        /// <summary>
        /// Tato metoda vynuluje lokátor.
        /// </summary>
        public void StopGeolocator()
        {

                IsGPSBusy = false;
                locator = null;

        }

        void locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Geoposition position = null;
            var task = Task.Run(async () =>
            {
                position = await sender.GetGeopositionAsync(new TimeSpan(0, 0, 0, 0, 200), new TimeSpan(0, 0, 3));
                var a = 1;
            });
            task.Wait();

            //Definice MyPosition vlastní náhradní třídou pro geopozici a uložení šířky a délky do jejích property.
            MyGeocoordinate coord = new MyGeocoordinate();
            coord.Latitude = position.Coordinate.Point.Position.Latitude;
            coord.Longitude = position.Coordinate.Point.Position.Longitude;
            if (coord != null)
            {
                MyPosition = coord; 
            }
            
            //nastavení času aktualizace
            LastPositionTime = DateTime.Now;

#if WINDOWS_APP
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //Výpočet vzdálenosti vůči všm stanicím.
                _gpsService.SetPosition();
            });
#endif
#if WINDOWS_PHONE_APP
            //Výpočet vzdálenosti vůči všm stanicím.
                _gpsService.SetPosition();
#endif


        }
        /// <summary>
        /// Will calculate distance from given position (my position) to every station
        /// </summary>
        /// <param name="gc"></param>
        public void CalculateDistances()
        {
            var gc = MyPosition;
            /*
            bool nearestWithoutStation = false;
            if (_localSettings.Containers["AppSettings"].Values.ContainsKey("nearestWithoutStation"))
            {
                nearestWithoutStation = (bool)_localSettings.Containers["AppSettings"].Values["nearestWithoutStation"];
            }
            else 
            {
                nearestWithoutStation = false;
            }*/
            object tmpObject = null;
            bool nearestWithoutStation = false;
            _localSettings.Containers["AppSettings"].Values.TryGetValue("nearestWithoutStation", out tmpObject);
            var x = tmpObject as bool?;
            if (x != null)
            {
                nearestWithoutStation = x.Value;
            }

            bool useGPS = false;
            if (_localSettings.Containers["AppSettings"].Values.ContainsKey("useGPS"))
            {
                useGPS = (bool)_localSettings.Containers["AppSettings"].Values["useGPS"];
            }
            else
            {
                useGPS = false;
            }


            Station tmp = null;

            foreach (Station sta in Stations)
            {

                if (useGPS == false)
                {
                    sta.Distance = -1;
                    continue;
                }
                if (gc != null)
                {
                    var abc = gc.GetDistanceTo(sta.Position);
                    sta.Distance = abc; // Calculate distance
                }

                //eliminate nearest stations without quality value
                if (nearestWithoutStation == false && sta.Quality > 6) continue;

                if (tmp == null || sta.Distance < tmp.Distance) // Check if this is the nearest station
                    tmp = sta;
            }
            if (tmp != null && tmp != NearestStation) NearestStation = tmp; // Set the nearest station

            if (AutoCurrentNearest) { CurrentStation = NearestStation; }

            if (useGPS)
            {
                //ESortStationType t = (ESortStationType)IsolatedStorageSettings.ApplicationSettings["sortType"];
                ESortStationType t = (ESortStationType)_localSettings.Containers["AppSettings"].Values["sortType"];
                if (t == ESortStationType.DISTANCE)
                {
                    App.ViewModel.SortStations(t);
                }
            }

        }


        /// <summary>
        /// Je možné, že vyhodí chybu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void locator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            
            string s;
            s = _localSettings.Containers["AppSettings"].Values["lastNearestStation"].ToString();

            //Získání stavu
            var a = sender.LocationStatus.ToString();


            switch (sender.LocationStatus)
            {
                case Windows.Devices.Geolocation.PositionStatus.Disabled:
                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        //await msgGPSDisabled();
                        MessageDialog sss = new MessageDialog(_resourceLoader.GetString("MsgGPSDisabled"),_resourceLoader.GetString("Error"));
                        await sss.ShowAsyncQueue();

                        IsGPSBusy = false;
                        IsGPS = false;

                        App.ViewModel.GPSStatus = GPSSTATUS.GPSdisabled.ToString();
                    });
                    
                    if (s != null)
                    {
                        CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            App.ViewModel.CurrentStation = App.ViewModel.GetStation(s);                        
                            
                        });
                    }
                    
                    
                    break;

                case Windows.Devices.Geolocation.PositionStatus.NoData:
                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        MessageDialog sss = new MessageDialog(_resourceLoader.GetString("MsgGPSUnavailable"), _resourceLoader.GetString("Error"));
                        await sss.ShowAsyncQueue();

                        IsGPSBusy = false;
                        IsGPS = false;
                        App.ViewModel.GPSStatus = GPSSTATUS.GPSnotactive.ToString();
                    });

                    

                    if (s != null) App.ViewModel.CurrentStation = App.ViewModel.GetStation(s);

                    break;
                case Windows.Devices.Geolocation.PositionStatus.Ready:
                    var nearestStation = true;
                    object tmpObject = null;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Containers["AppSettings"].Values.TryGetValue("nearestStation", out tmpObject);            
                    bool? x = tmpObject as bool?;
                    if (x != null)
                    {
                        nearestStation = x.Value;
                    }
                    if (nearestStation)
                    {
                        CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                                    {
                                        App.ViewModel.GPSStatus = GPSSTATUS.GPSactive.ToString();
                                    }); 
                    }
                    break;
            }
        }

        #endregion



    }
}
