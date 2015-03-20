using Pollution.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace Pollution.Flyouts
{
    public sealed partial class FlyoutSettings : SettingsFlyout
    {
        public FlyoutSettings()
        {
            this.InitializeComponent();
            this.Loaded += OnNavigatedTo;
            this.Unloaded += OnNavigatingFrom;
        }
        
        //resources
        private ResourceLoader _resourceLoader = new ResourceLoader();
        //localsettings
        private Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private string infoStation;
        private bool nearestStation;
        private bool nearestWithoutStation;
        private bool useGPS;
        private bool useLiveTile;

        private string lang; //"" - system, "cs-CZ"-czech, "en"-english

        private ESortStationType sortType;

        /// <summary>
        /// When selection is changed, than save what is selected, and change the infoPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listStations.SelectedItem == null) return;

            infoStation = (listStations.SelectedItem as Station).Code;
            //if(checkNearest.IsChecked == false)
            App.ViewModel.CurrentStation = (listStations.SelectedItem as Station);
            _localSettings.Containers["AppSettings"].Values["infoStation"] = infoStation;
        }

        private void checkNearest_Checked(object sender, RoutedEventArgs e)
        {
            nearestStation = checkNearest.IsChecked ?? false;
            _localSettings.Containers["AppSettings"].Values["nearestStation"] = nearestStation;
        }

        private void checkNearestWithoutQuality_Checked(object sender, RoutedEventArgs e)
        {
            nearestWithoutStation = checkNearestWithoutQuality.IsChecked ?? false;
            _localSettings.Containers["AppSettings"].Values["nearestWithoutStation"] = nearestWithoutStation;
        }
        private void checkLiveTile_Checked(object sender, RoutedEventArgs e)
        {
            useLiveTile = checkLiveTile.IsChecked ?? false;
            //IsolatedStorageSettings.ApplicationSettings["useLiveTile"] = useLiveTile;
            //IsolatedStorageSettings.ApplicationSettings.Save();
            _localSettings.Containers["AppSettings"].Values["useLiveTile"] = useLiveTile;            
        }

        private void OnNavigatedTo(object sender, RoutedEventArgs e)
        {
            object tmpObject = null;     

            infoStation = null;
            _localSettings.Containers["AppSettings"].Values.TryGetValue("infoStation", out tmpObject);
            infoStation = tmpObject.ToString();

            nearestStation = true;
            _localSettings.Containers["AppSettings"].Values.TryGetValue("nearestStation", out tmpObject);            
            bool? x = tmpObject as bool?;
            if (x != null)
            {
                nearestStation = x.Value;
            }

            tmpObject = null;
            nearestWithoutStation = false;
            _localSettings.Containers["AppSettings"].Values.TryGetValue("nearestWithoutStation", out tmpObject);
            x = tmpObject as bool?;
            if (x != null)
            {
                nearestWithoutStation = x.Value;
            }

            checkNearestWithoutQuality.IsChecked = nearestWithoutStation;

            tmpObject = null;
            useGPS = true;
            //IsolatedStorageSettings.ApplicationSettings.TryGetValue("useGPS", out useGPS);
            _localSettings.Containers["AppSettings"].Values.TryGetValue("useGPS", out tmpObject);
            x = tmpObject as bool?;
            if (x != null)
            {
                useGPS = x.Value;
            }

            useLiveTile = true;
            //IsolatedStorageSettings.ApplicationSettings.TryGetValue("useLiveTile", out useLiveTile);
            _localSettings.Containers["AppSettings"].Values.TryGetValue("useLiveTile", out tmpObject);
            x = tmpObject as bool?;
            if (x != null)
            {
                useLiveTile = x.Value;
            }
            
            lang = "cs-CZ";
            //IsolatedStorageSettings.ApplicationSettings.TryGetValue("appLang", out lang);
            _localSettings.Containers["AppSettings"].Values.TryGetValue("appLang", out tmpObject);
            if (tmpObject != null)
            {
                lang = tmpObject.ToString();
            }

            if (lang == null) lang = "";
            //if (lang == "") listLang.SelectedIndex = 0;
            if (lang == "cs-CZ") listLang.SelectedIndex = 0;
            if (lang == "en-US") listLang.SelectedIndex = 1;

            int i = 0;

                //listStations.Items.Clear();

                sortType = (ESortStationType)_localSettings.Containers["AppSettings"].Values["sortType"];

                App.ViewModel.SortStations(ESortStationType.ALPHA);

                foreach (Station sta in App.ViewModel.Stations)
                {

                    if (sta.Code == infoStation)
                    {
                        break;
                    }
                    i++;
                }
                if (i >= App.ViewModel.Stations.Count()) i = 0;
                
                listStations.ItemsSource = App.ViewModel.Stations;
                if (App.ViewModel.Stations.Count() != 0)
                {                
                    listStations.SelectedIndex = i;
                    listStations.UpdateLayout();
                    listStations.ScrollIntoView(listStations.Items[i]);
                    listStations.UpdateLayout();
                }

            checkNearest.IsChecked = nearestStation;

            //checkGPS.IsChecked = useGPS;

            checkLiveTile.IsChecked = useLiveTile;

            

        }

        protected void OnNavigatingFrom(object sender, RoutedEventArgs e)
        {            

            //use livetile
            string agentName = "GARVIS-PollutionAgent";

            //GPSService service = new GPSService();
            //service.SetPosition();

            if (nearestStation)
            {
                App.ViewModel.CurrentStation = App.ViewModel.NearestStation;
            }
            else
            {
                App.ViewModel.CurrentStation = listStations.SelectedItem as Station;
            }
            
            

            

            App.ViewModel.SortStations(sortType);

        }
        

        
        private void listLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listLang == null || listLang.SelectedItem == null) return;

            lang = "";
            if (listLang.SelectedIndex == 0) lang = "cs-CZ";
            if (listLang.SelectedIndex == 1) lang = "en-US";
            
            _localSettings.Containers["AppSettings"].Values["appLang"] = lang;


            if (lang != "")
            {
                ApplicationLanguages.PrimaryLanguageOverride = lang;
                
            }
            else
            {
                ApplicationLanguages.PrimaryLanguageOverride = CultureInfo.InvariantCulture.TwoLetterISOLanguageName; //teoreticky vložít do kutury kulturu systému
            }


        }
        
    }
}
