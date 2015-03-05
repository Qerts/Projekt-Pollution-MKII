using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using System.Net.Http;
using Microsoft.VisualBasic.CompilerServices;
using Windows.Storage;
using Pollution;
using Pollution.ViewModels;
using Utils;
using System.Xml.Linq;
using Windows.UI.Xaml.Media;
using System.IO;
using Windows.ApplicationModel.Resources;
using System.ComponentModel;
using Bing.Maps;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel;
using System.Collections.ObjectModel;
using Windows.UI.Core;


namespace Pollution
{
    public sealed partial class MainPage : Page
    {
        DateTime lastDownload;
        private Popup popup;
        
        
        private const string RAW_DATA_FILE = "rawdata.txt";
        private ResourceLoader _myResourceLoader = new ResourceLoader();

        //connection classes
        DownloadService _downloadService = new DownloadService();
        FileService _fileService = new FileService();
        GPSService _gpsService = new GPSService();

        //localsettings
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private async Task LoadData() 
        {            

            //příznak možnosti připojení a stažení
            bool _newDownload = true;

            //kontrola připojení k internetu
            if (NetworkInterface.GetIsNetworkAvailable() == false)
            {
                MessageDialog msg = new MessageDialog("Nebylo možné nalézt internetové připojení");
                msg.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler(CommandHandlers)));
                await msg.ShowAsync();
                _newDownload = false;
            }

            //stažení nových dat
            if (_newDownload)
            {
                try
                {
                    var content = await _downloadService.DownloadData();        //stažení dat
                    App.ViewModel.RawData = content;                            //uložení dat do zdroje
                    await _fileService.SaveDataToFile(content, RAW_DATA_FILE);  //uložení stažených dat do souboru
                }
                catch (Exception e)
                {
                    _fileService.LoadDataFromFile(RAW_DATA_FILE);               //načtení dat ze souboru a uložení jako zdroj do view modelu
                }

                //původně load data async
                await App.ViewModel.LoadDataToModel();    //zavedení dat do modelu
                LoadData_RunWorkerCompleted();
               
                try
                {
                    var content = await _downloadService.DownloadPhotos();      //stažení fotek
                    _downloadService.ProcessDonwloadedPhotos(content);          //zpracování stažených fotek
                }
                catch
                {
                   //photosDownloadInterrupted();
                }                
            }
            else
            {
                await _fileService.LoadDataFromFile(RAW_DATA_FILE);         //načtení dat ze souboru a uložení jako zdroj do view modelu

                if (App.ViewModel.NearestStation == null)                   //nastavení nejbližší stanice, jelikož není možné jí dosáhnout skrze wifi
                {
                    App.ViewModel.NearestStation = App.ViewModel.GetStation("AKALA");

                }
                if (App.ViewModel.CurrentStation == null)                   //nastavení aktuální stanice, jelikož není možné jí dosáhnout skrze wifi
                {
                    App.ViewModel.CurrentStation = App.ViewModel.GetStation("AKALA");
                }
                
                
                await App.ViewModel.LoadDataToModel();      //načtení dat do modelu

                

                //vytvořit vlákno, které bude kontrolovat připojení k internetu a při připojení načte data
                //todo
                
            }

            _gpsService.SetGeolocator();
            setMapDetails();    //nastavení mapy
        }

        /// <summary>
        /// Tato metoda poskytne mapě zdroj pinů a zvolí vybraný pin dle aktuální stanice.
        /// </summary>
        private void setMapDetails()
        {
            //poskytnutí pinů jako zdroje mapě
            StationsDetailPinLayer.ItemsSource = App.ViewModel.PinManager.Pins;
            //poskytnutí momentální stanice
            Station tmp = App.ViewModel.CurrentStation;

            //procházení všech pinů
            foreach (Pin pin in App.ViewModel.PinManager.Pins)
            {
                //přeskočení všech nestanicových pinů
                if (pin.PinType != EPinType.STATION) continue;

                //pokud je tento pin vybrán, uloží se i jako poslední vybraný
                if (pin.IsSelected) oldSelected = pin;

                //pokud je tento pin zároveň i aktuální stanicí
                if (pin.Station == tmp)
                {
                    pin.IsSelected = true;
                    newSelected = pin;

                }
                else
                {
                    pin.IsSelected = false;
                }
            }

        }

        /// <summary>
        /// Tao funkce by měla přednačíst data, jelikož aplikace kolabovala kvůli nedostatku dat, nebo žádná data nezobrazila.
        /// Je to způsobeno tím, že metody stahování dat jsou asynchronní. Tato metoda by neměla být vůbec potřeba a slouží spíše
        /// jako testovací.
        /// </summary>
        public async Task DataPreLoad() 
        {

            await _fileService.LoadDataFromFile(RAW_DATA_FILE);            
            await App.ViewModel.LoadDataToModel();

            //udání defaultní polohy z důvodu nemožnosti načtení aktuální polohy
            if ((App.ViewModel.NearestStation == null) || (App.ViewModel.NearestStation.Name.Length == 0))                   //nastavení nejbližší stanice, jelikož není možné jí dosáhnout skrze wifi
            {
                App.ViewModel.NearestStation = App.ViewModel.GetStation("AKALA");

            }
            if ( (App.ViewModel.CurrentStation == null) || (App.ViewModel.CurrentStation.Name.Length == 0))                   //nastavení aktuální stanice, jelikož není možné jí dosáhnout skrze wifi
            {
                App.ViewModel.CurrentStation = App.ViewModel.GetStation("AKALA");
            }

            //spuštění lokátoru na začátku programu
            _gpsService.SetGeolocator();
            //nastavení mapy
            setMapDetails();

        }

        /// <summary>
        /// Metoda zpracující fotky a jejich detaily.
        /// </summary>
        /// <param name="content"></param>
        private void photoStringDownloadComplete(string content)
        {
            //zpracovani fotek
            App.ViewModel.PhotosGlobal = null;
            if (content.Length == 0)//bylo tu e
            {
                App.ViewModel.PhotosGlobal = null;
            }
            else
            {

                List<PPhoto> pp = new List<PPhoto>();

                //process history data
                string s = content;//bylo tu e
                string[] sl;

                PPhoto p;


                if (s != null && s.Length != 0)
                {
                    try
                    {
                        using (StringReader reader = new StringReader(s))
                        {
                            string line;

                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.Length == 0)
                                {
                                    continue;
                                }
                                try
                                {

                                    sl = line.Split('|');
                                    if (sl.Length != 5) continue;


                                    p = new PPhoto();
                                    p.Time = new DateTime(Int64.Parse(sl[1]));
                                    p.URL = sl[0];
                                    p.Smile = Int32.Parse(sl[2]);
                                    p.Note = sl[3];
                                    p.StationCode = sl[4]; //unable to set Station, list is not ready 

                                    pp.Add(p);

                                }
                                catch (Exception exp)
                                {
                                }

                            }
                        }

                        App.ViewModel.PhotosGlobal = pp;
                        
                    }
                    catch (Exception exp)
                    {
                    }

                }

                if (App.ViewModel.PhotosGlobal != null)
                {

                    DateTime d = DateTime.MinValue;
                    //IsolatedStorageSettings.ApplicationSettings.TryGetValue("lastPhotoTime", out d);

                    int c = 0;
                    foreach (PPhoto photo in App.ViewModel.PhotosGlobal)
                    {
                        if (photo.Time > d) c++;
                    }


                    if (c == 0)
                    {
                        //App.ViewModel.PivotPhotosHeader = AppResources.AppPhotos;
                        App.ViewModel.PivotPhotosHeader = _myResourceLoader.GetString("AppPhotos");
                    }
                    else
                    {
                        if (App.ViewModel.PhotosGlobal[App.ViewModel.PhotosGlobal.Count - 1].Time > d)
                        {
                            //všechny načtené jsou novější
                            //App.ViewModel.PivotPhotosHeader = AppResources.AppPhotos + " (+" + App.ViewModel.PhotosGlobal.Count + ")";
                            App.ViewModel.PivotPhotosHeader = _myResourceLoader.GetString("AppPhotos") + " (+" + App.ViewModel.PhotosGlobal.Count + ")";
                        }
                        else
                        {
                            //App.ViewModel.PivotPhotosHeader = AppResources.AppPhotos + " (" + c + ")";
                            App.ViewModel.PivotPhotosHeader = _myResourceLoader.GetString("AppPhotos") + " (" + c + ")";
                        }
                    }
                    


                }


            }

            setMapDetails();
        }

        /// <summary>
        /// Uložení dat do souboru a zároveň zavolání matody pro parsnutí dat z raw do celého view modelu.
        /// </summary>
        /// <param name="responseString"></param>
        private async Task stringDownloadComplete(String responseString) 
        {
            //ulozeni stazenych dat do lokalniho uloziste
            StorageFolder rdfo = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile rdfi = await rdfo.CreateFileAsync(RAW_DATA_FILE, CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(rdfi, responseString);
        }

        /// <summary>
        /// Načtení dat ze starého souboru a přiřazení jich do view modelu. Dále je zavolána funkce pro parsnutí dat z raw do celého modelu.
        /// </summary>
        private async Task stringDownloadInterrupted() 
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync(RAW_DATA_FILE);
            App.ViewModel.RawData = await FileIO.ReadTextAsync(file);
        }

        /// <summary>
        /// Command handlers pro použití v alert oknech.
        /// </summary>
        /// <param name="commandLabel"></param>
        public void CommandHandlers(IUICommand commandLabel)
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

        void LoadData_RunWorkerCompleted()
        {
            //this.DataContext = App.ViewModel;
            
            object obj;
            string l;
            //if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("lastNearestStation", out l))
            if (!localSettings.Containers["AppSettings"].Values.TryGetValue("lastNearestStation", out obj))
            {
                l = "AKALA";
                //IsolatedStorageSettings.ApplicationSettings.Add("lastNearestStation", l);
                localSettings.Containers["AppSettings"].Values.Add("lastNearestStation", l);
            }
            else 
            {
                l = obj.ToString();
            }
            
            string s;
            if (!localSettings.Containers["AppSettings"].Values.TryGetValue("infoStation", out obj))
            {
                localSettings.Containers["AppSettings"].Values.Add("infoStation", l);
            }
            else
            {
                s = obj.ToString();
            }

            //nedává smysl
            bool b = true;
            /*
            if (!localSettings.Containers["AppSettings"].Values.TryGetValue("nearestStation", out obj))
            {
                //IsolatedStorageSettings.ApplicationSettings.Add("nearestStation", true);
                localSettings.Containers["AppSettings"].Values.Add("nearestStation", true);
                b = true;
            }
            */
            localSettings.Containers["AppSettings"].Values.TryGetValue("nearestStation", out obj);
            var x = obj as bool?;
            if (x != null)
            {
                b = x.Value;
            }

            bool g = false;
            //if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("useGPS", out g))
            if (!localSettings.Containers["AppSettings"].Values.TryGetValue("useGPS", out obj))
            {
                //IsolatedStorageSettings.ApplicationSettings.Add("useGPS", true);
                localSettings.Containers["AppSettings"].Values.Add("useGPS", true);
                g = true;
            }
            else
            {
                //g = (bool)obj;
                localSettings.Containers["AppSettings"].Values["useGPS"] = true;
                g = true;
            }
            bool useLiveTile = false;
            
            //if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("useLiveTile", out useLiveTile))
            if (!localSettings.Containers["AppSettings"].Values.TryGetValue("useLiveTile", out obj))
            {
                //IsolatedStorageSettings.ApplicationSettings.Add("useLiveTile", true);
                localSettings.Containers["AppSettings"].Values.Add("useLiveTile", useLiveTile);
                useLiveTile = true;
            }
            else
            {
                useLiveTile = (bool)obj;
            }
            ESortStationType t;
            
            //if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("sortType", out t))
            if (!localSettings.Containers["AppSettings"].Values.TryGetValue("sortType", out obj))
            {
                t = ESortStationType.DISTANCE;
                //IsolatedStorageSettings.ApplicationSettings.Add("sortType", (int)t);
                localSettings.Containers["AppSettings"].Values["sortType"] = (int)t;
            }
            else
            {
                t = (ESortStationType)obj;
            }
            //IsolatedStorageSettings.ApplicationSettings.Save();         
            
            

            App.ViewModel.SortStations(t);


            if (l != null) App.ViewModel.NearestStation = App.ViewModel.GetStation(l);

            if (b)
            {
                App.ViewModel.CurrentStation = App.ViewModel.NearestStation;
            }
            else
            {
                s = null;
                if (localSettings.Containers["AppSettings"].Values.ContainsKey("infoStation")) 
                {
                    s = localSettings.Containers["AppSettings"].Values["infoStation"].ToString();
                }
                if (s != null) App.ViewModel.CurrentStation = App.ViewModel.GetStation(s);
                else
                {
                    App.ViewModel.CurrentStation = App.ViewModel.NearestStation;
                }
            }
            
            StationsDetailPinLayer.ItemsSource = App.ViewModel.PinManager.Pins;

            //current map update

            mapSmall.UpdateStations();

            //statistics
            
            ColorQualityConverter ccq = new ColorQualityConverter();
            
            this.UpdateLayout();
            App.ViewModel.Notify();



            App.ViewModel.AutoCurrentNearest = b; //if first time is manual or auto

            if (g)
            {
                App.ViewModel.SetGeolocator();
            }


            //use livetile
            string agentName = "GARVIS-PollutionAgent";

            
            
            //start background agent 
            /*
            PeriodicTask periodicTask = new PeriodicTask(agentName);

            periodicTask.Description = "Task for Pollution tile update";
            periodicTask.ExpirationTime = System.DateTime.Now.AddDays(14);

            try
            {
                // If the agent is already registered with the system,
                if (ScheduledActionService.Find(agentName) != null)
                {
                    ScheduledActionService.Remove(agentName);
                }

                //not supported in current version
                //periodicTask.BeginTime = DateTime.Now.AddHours(1);
                if (useLiveTile)
                {
                    ScheduledActionService.Add(periodicTask);

                    //for debug only
                    //ScheduledActionService.LaunchForTest(periodicTask.Name, TimeSpan.FromSeconds(10));
                }
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    //MessageBox.Show(AppResources.TextTaskDisabled);
                }
                else
                {
                    //MessageBox.Show(AppResources.TextTaskError);
                }
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
            }
            */

            string versionNews = null;
            obj = null;
            //IsolatedStorageSettings.ApplicationSettings.TryGetValue("versionNews", out versionNews);
            if (localSettings.Containers["AppSettings"].Values.TryGetValue("versionNews", out obj))
            {
                versionNews = obj.ToString();
            }






            string versionApp = Package.Current.Id.Version.Major.ToString() + Package.Current.Id.Version.Minor.ToString() + Package.Current.Id.Version.Build.ToString() + Package.Current.Id.Version.Revision.ToString();
            //string versionApp = xd.Root.Element("App").Attribute("Version").Value;

            
            if (versionNews != versionApp)
            {                   
                var versionMsg = Task.Run(async delegate
                {     
                    MessageDialog msg = new MessageDialog("Novinky");

                    obj = string.Empty;
                    localSettings.Containers["AppSettings"].Values.TryGetValue("WhatIsNew", out obj);
                    string msgcont = "Nothing new";
                    if (obj != null)
                    {
                        msgcont = obj.ToString();
                    }
                    msg.Content = msgcont;
                    msg.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler(CommandHandlers)));
                    this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        msg.ShowAsync();
                    });
                    //MessageBox.Show(AppResources._news, AppResources.WhatIsNew, MessageBoxButton.OK);
                    localSettings.Containers["AppSettings"].Values["versionNews"] = versionApp;
                });
            }

            /*
            //restore default Livetile
            if (useLiveTile == false)
            {
                ShellTile appTile = ShellTile.ActiveTiles.FirstOrDefault();
                StandardTileData newTileData = new StandardTileData
                {
                    BackgroundImage = new Uri("Background.png", UriKind.RelativeOrAbsolute)

                };
                appTile.Update(newTileData);
            }
            else
            {
                if (App.ViewModel.CurrentStation != null)
                {
                    ScheduledAgent sa = new ScheduledAgent();
                    sa.CreateTileDirect(App.ViewModel.DataTime, App.ViewModel.CurrentStation.Quality, App.ViewModel.CurrentStation.Name, App.ViewModel.CurrentStation.LastPhoto);
                }
            }
            */
            

        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            // This occurs when my position is gained. It marks me on the big map, and enable button
            if (e.PropertyName == "MyPosition")
            {
                //tady to padá
                App.ViewModel.PinMan.Location = App.ViewModel.MyPosition;
                if (App.ViewModel.MyPosition != null)
                {
                    GpsAngle la = GpsAngle.FromDouble(App.ViewModel.MyPosition.Latitude);
                    GpsAngle lo = GpsAngle.FromDouble(App.ViewModel.MyPosition.Longitude);
                }
                

                string s = "";
                double ac = 0;
                if (App.ViewModel.MyPosition != null)
                {
                    ac = App.ViewModel.MyPosition.HorizontalAccuracy; 
                }
                if (ac < 1000) s = "±" + (int)ac + " m";
                else s = string.Format("±{0:0} km", (int)(ac / 1000));
            }

            // This handles mini map on the InfoPage. The rest is done by DataBinding
            if (e.PropertyName == "CurrentStation")
            {
                Station tmp = App.ViewModel.CurrentStation;

                foreach (Pin p in App.ViewModel.PinManager.Pins)
                {
                    if (p.PinType != EPinType.STATION) continue;

                    if (p.Station == tmp)
                    {
                        p.IsSelected = true;

                    }
                    else
                    {
                        p.IsSelected = false;
                    }
                }

                Bing.Maps.Location tmpPos = new Bing.Maps.Location();
                tmpPos.Longitude = tmp.Position.Longitude;
                tmpPos.Latitude = tmp.Position.Latitude;
                infoMap.SetView(tmpPos, 9);
            }

            if (e.PropertyName == "NearestStation")
            {
                bool b = false;
                //localSettings.Containers["AppSettings"].Values.TryGetValue("nearestStation", out b);
                if (localSettings.Containers["AppSettings"].Values.ContainsKey("nearestStation"))
                {
                    b = true;
                }               

                if (b && App.ViewModel.CurrentStation != App.ViewModel.NearestStation)
                {
                    App.ViewModel.CurrentStation = App.ViewModel.NearestStation;
                }
            }     
        }
    }
}
