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


namespace Pollution
{
    public sealed partial class MainPage : Page
    {
        DateTime lastDownload;
        private Popup popup;
        private const string URL = "http://data.garvis.cz/pollution/timeshot";
        private const string URLphotos = "http://data.garvis.cz/pollution/stationphotos";
        private const string RAW_DATA_FILE = "rawdata.txt";
                
           
        //funkce pro nacteni obsahu
        private async void contentLoad() 
        {
            bool newDownload = true;

            if (!NetworkInterface.GetIsNetworkAvailable())//overeni pro overeni pripojeni k internetu
            {
                MessageDialog msg = new MessageDialog("Nebylo možné nalézt internetové připojení");//v contentu by melo byt AppResources.MsgDownloadError, AppResources.Error, MessageBoxButton.OK
                msg.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler(CommandHandlers)));
                //await msg.ShowAsync(); bez tohohle to nejde
                
                newDownload = false;
            }

            //podminka, ktera prejde na stahovani nebo pouziti stareho obsahu
            if (newDownload)//stazeni noveho stringu
            {
                HttpClient wc = new HttpClient();

                string l = "";
                //ziskani posledni nejblizsi stanice IsolatedStorageSettings.ApplicationSettings.TryGetValue("lastNearestStation", out l);
                
                if (l == null) l = "";

                //create URL request
                string ut = DateTime.Now.Ticks.ToString();
                string ms = "0";
                string manuf = "Nokia"; //ziskani vyrobce Utils.WPVersions.GetManufacturer();
                long r = 158;

                for (int i = 0; i < l.Length; i++)
                {
                    r = r + (int)(l[i]);
                }

                for (int i = 0; i < ms.Length; i++)
                {
                    r = r + (int)(ms[i]);
                }

                for (int i = 0; i < ut.Length; i++)
                {
                    r = r + (int)(ut[i]);
                }
                for (int i = 0; i < manuf.Length; i++)
                {
                    r = r + (int)(manuf[i]);
                }

                

                try
                {
                    
                    var response = await wc.GetAsync(new Uri(URL + "?ns=" + l + "&ms=" + ms + "&t=" + ut + "&m=" + manuf + "&key=" + r.ToString()));
                    var content = await response.Content.ReadAsStringAsync();
                    App.ViewModel.RawData = content;
                    stringDownloadComplete(content);//po dokonceni stahovani zavolat dalsi funkci k ulozeni do local storage
                    
                }
                catch (Exception e)
                {
                    stringDownloadInterrupted();//pri preruseni stahovani zavolat dalsi funkci k nacteni z local storage, pouziti stareho stringu
                }                
            }
            else//pouziti stareho stringu
            {
                
            }
            

        }

        private async void stringDownloadComplete(String responseString) 
        {
            //ulozeni stazenych dat do lokalniho uloziste
            StorageFolder rdfo = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile rdfi = await rdfo.CreateFileAsync(RAW_DATA_FILE, CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(rdfi, responseString);
            
            LoadDataAsync();
        }
        private async void stringDownloadInterrupted() 
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync(RAW_DATA_FILE);
            App.ViewModel.RawData = await FileIO.ReadTextAsync(file);
            LoadDataAsync();
        }
        
        public void LoadDataAsync()
        {
            //Task myTask1 = new Task(ViewModel.DoDataLoad);//App.ViewModel.DoDataLoad);
            //continuewith
            //StationViewModel hasdf = new StationViewModel();
            
            /*
            StorageFolder rdfo = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile rdfi = await rdfo.GetFileAsync("RAW_DATA_FILE");
            legendTxt1.Text = await FileIO.ReadTextAsync(rdfi);*/

            Task loadDataTask = Task.Run(() =>
                {
                    StationViewModel st = new StationViewModel();
                    st.DoLoadData();
                }
                );
            
        }

        
        public void CommandHandlers(IUICommand commandLabel)
        {
            var Actions = commandLabel.Label;
            switch (Actions)
            {
                
                case "Ok":
                    rootPage.Focus(Windows.UI.Xaml.FocusState.Pointer);
                    break;                
                case "Quit":
                    Application.Current.Exit();
                    break;                
            }
        }
        
    }
}
