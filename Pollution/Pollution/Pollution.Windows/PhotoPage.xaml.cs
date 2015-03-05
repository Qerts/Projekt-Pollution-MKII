using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Pollution.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace Pollution
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class PhotoPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private ResourceLoader _resourceLoader = new ResourceLoader();

        Stream photoStream;

        private const string URL = "http://data.garvis.cz/pollution/photoupload";
        private bool isApplicable;

        bool nearestWithoutStation = false;

        //localsettings
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public PhotoPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;

            
            object obj;
            isApplicable = false;
            if (!localSettings.Containers["AppSettings"].Values.TryGetValue("useGPS", out obj))
            {
                localSettings.Containers["AppSettings"].Values.Add("useGPS", true);

                isApplicable = true;
            }
            else
            {
                bool? x = obj as bool?;
                if (x != null)
                {
                    isApplicable = x.Value;
                }
            }
        }



        #region NavigationHelper registration

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            object navigationParameter;
            if (e.PageState != null && e.PageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = e.PageState["SelectedItem"];
            }

            // TODO: Assign a bindable group to this.DefaultViewModel["Group"]
            // TODO: Assign a collection of bindable items to this.DefaultViewModel["Items"]
            // TODO: Assign the selected item to this.flipView.SelectedItem
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }
        /// <summary>
        /// Tlacitko foceni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarButton_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            CameraCaptureUI dialog = new CameraCaptureUI();
            Size aspectRatio = new Size(16, 9);
            dialog.PhotoSettings.CroppedAspectRatio = aspectRatio;
            StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
            StorageFolder folder = KnownFolders.PicturesLibrary;
            if (file != null)
            {
                await file.MoveAsync(folder);


                BitmapImage bitmapImage = new BitmapImage();
                IRandomAccessStream rawStream = await file.OpenReadAsync();
                FileRandomAccessStream stream = (FileRandomAccessStream)rawStream;
                bitmapImage.SetSource(stream);
                PhotoFrame.Source = bitmapImage;
                PhotoFrame.Visibility = Visibility.Visible;

                var bytes = new byte[rawStream.Size];
                await rawStream.ReadAsync(bytes.AsBuffer(), (uint)rawStream.Size, Windows.Storage.Streams.InputStreamOptions.None);
                var reader = new DataReader(rawStream.GetInputStreamAt(0));
                await reader.LoadAsync((uint)rawStream.Size);
                reader.ReadBytes(bytes);
                photoStream = new MemoryStream(bytes);

                textPhotoSize.Text = String.Format("{0:0.00} MB", (float)photoStream.Length / 1000000);
            }


        }

        /// <summary>
        /// Tlacitko slozky
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.Desktop;
            filePicker.SettingsIdentifier = "FilePicker1";
            filePicker.CommitButtonText = _resourceLoader.GetString("OpenFile");
            var file = await filePicker.PickSingleFileAsync();
            //todo nahravani
            if (file != null)
            {
                BitmapImage bitmapImage = new BitmapImage();
                IRandomAccessStream rawStream = await file.OpenAsync(FileAccessMode.Read);
                FileRandomAccessStream stream = (FileRandomAccessStream)rawStream;
                bitmapImage.SetSource(stream);
                PhotoFrame.Source = bitmapImage;
                PhotoFrame.Visibility = Visibility.Visible;

                var bytes = new byte[rawStream.Size];
                await rawStream.ReadAsync(bytes.AsBuffer(), (uint)rawStream.Size, Windows.Storage.Streams.InputStreamOptions.None);
                var reader = new DataReader(rawStream.GetInputStreamAt(0));
                await reader.LoadAsync((uint)rawStream.Size);
                reader.ReadBytes(bytes);
                photoStream = new MemoryStream(bytes);

                textPhotoSize.Text = String.Format("{0:0.00} MB", (float)photoStream.Length / 1000000);
            }
        }

        #endregion


        private void iconSmile1_Tap(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            iconSmile1.Tag = "";
            iconSmile2.Tag = "";
            iconSmile3.Tag = "";
            iconSmile4.Tag = "";
            iconSmile5.Tag = "";

            (e.OriginalSource as Image).Tag = "OK";

            SetSmileIcons();

        }


        private void SetSmileIcons()
        {
            iconSmile1.Opacity = 0.4;
            iconSmile2.Opacity = 0.4;
            iconSmile3.Opacity = 0.4;
            iconSmile4.Opacity = 0.4;
            iconSmile5.Opacity = 0.4;

            if (iconSmile1.Tag as string == "OK") iconSmile1.Opacity = 1;
            if (iconSmile2.Tag as string == "OK") iconSmile2.Opacity = 1;
            if (iconSmile3.Tag as string == "OK") iconSmile3.Opacity = 1;
            if (iconSmile4.Tag as string == "OK") iconSmile4.Opacity = 1;
            if (iconSmile5.Tag as string == "OK") iconSmile5.Opacity = 1;

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (photoStream == null || isApplicable == false || App.ViewModel.IsGPS == false) return;

            //get smile
            string smile = "";
            if ((string)(iconSmile1.Tag) == "OK") smile = "1";
            if ((string)(iconSmile2.Tag) == "OK") smile = "2";
            if ((string)(iconSmile3.Tag) == "OK") smile = "3";
            if ((string)(iconSmile4.Tag) == "OK") smile = "4";
            if ((string)(iconSmile5.Tag) == "OK") smile = "5";

            if (smile == "")
            {
                MessageDialog msg = new MessageDialog(_resourceLoader.GetString("TextSendSmileSelect"), _resourceLoader.GetString("TextSendImageTitle"));
                msg.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler(Data.CommandHandlers)));
                await msg.ShowAsync();
                return;
            }


            string note = PhotoComment.Text;
            //zakomentováno povinné přidání komentu
            /* 
            if (note == "")
            {
                MessageDialog msg = new MessageDialog(_resourceLoader.GetString("TextSendWithoutNote"), _resourceLoader.GetString("TextSendImageTitle"));
                msg.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler(Data.CommandHandlers)));
                await msg.ShowAsync();
                return;
            }
            */

            /* progress indicator
            progress = new ProgressIndicator
            {
                IsVisible = true,
                IsIndeterminate = true
            };
            SystemTray.SetProgressIndicator(this, progress);
            */

            string l = App.ViewModel.NearestStation.Code;

            //create URL request
            string ut = DateTime.Now.Ticks.ToString();
            string ms = "0";
            string ids = App.ViewModel.NearestStation.Id.ToString();
            var h = App.ViewModel.MyPosition;
            string lo = App.ViewModel.MyPosition.Longitude.ToString().Replace(",", ".");
            string la = App.ViewModel.MyPosition.Latitude.ToString().Replace(",", ".");
            string ac = "";
            if (App.ViewModel.MyPosition.VerticalAccuracy > Int32.MaxValue)
            {
                ac = Int32.MaxValue.ToString();
            }
            else
            {
                ac = ((int)App.ViewModel.MyPosition.VerticalAccuracy).ToString();
            }



            string manuf = "PC";//Utils.WPVersions.GetManufacturer();
            string user = "PC";//Utils.WPVersions.GetWindowsLiveAnonymousID();

            long r = 158;

            for (int i = 0; i < l.Length; i++)
            {
                r = r + (int)(l[i]);
            }

            for (int i = 0; i < ms.Length; i++)
            {
                r = r + (int)(ms[i]);
            }
            for (int i = 0; i < ids.Length; i++)
            {
                r = r + (int)(ids[i]);
            }
            for (int i = 0; i < ut.Length; i++)
            {
                r = r + (int)(ut[i]);
            }

            for (int i = 0; i < lo.Length; i++)
            {
                r = r + (int)(lo[i]);
            }
            for (int i = 0; i < la.Length; i++)
            {
                r = r + (int)(la[i]);
            }
            for (int i = 0; i < ac.Length; i++)
            {
                r = r + (int)(ac[i]);
            }
            for (int i = 0; i < smile.Length; i++)
            {
                r = r + (int)(smile[i]);
            }
            /*for (int i = 0; i < note.Length; i++)
            {
                r = r + (int)(note[i]);
            }*/
            for (int i = 0; i < manuf.Length; i++)
            {
                r = r + (int)(manuf[i]);
            }
            for (int i = 0; i < user.Length; i++)
            {
                r = r + (int)(user[i]);
            }

            const int BLOCK_SIZE = 4096;
            HttpClient hc = new HttpClient();
            Stream writerResult = new MemoryStream();

            try
            {
                StreamContent streamContent;
                //vytvořit stream
                photoStream.Seek(0, SeekOrigin.Begin);  //nastaví pozici na začátek streamu                
                streamContent = new StreamContent(photoStream);

                HttpContent content = streamContent;                        
                MessageDialog msg = new MessageDialog(_resourceLoader.GetString("TextSendDone"), _resourceLoader.GetString("TextSendImageTitle"));
                msg.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler(Data.CommandHandlers)));
                await msg.ShowAsync();

                    //progress.IsVisible = false;

                ApplicationBarIcon_Reset_Click(this, null);


                // Write to the WebClient
                Uri uri = new Uri(URL + "?ns=" + l + "&ids=" + ids + "&ms=" + ms + "&t=" + ut + "&lo=" + lo + "&la=" + la + "&ac=" + ac + "&sm=" + smile + "&n0=" + note + "&m=" + manuf + "&u=" + user + "&key=" + r.ToString());
                //await hc.PostAsync(uri, content);
                photoStream = null;
            }
            catch (Exception)
            {
                MessageDialog msg = new MessageDialog(_resourceLoader.GetString("TextSendError"), _resourceLoader.GetString("TextSendImageTitle"));
                msg.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler(Data.CommandHandlers)));
                msg.ShowAsync();
            }
        }


        private void ApplicationBarIcon_Reset_Click(object sender, EventArgs e)
        {
            photoStream = null;
            PhotoFrame.Source = null;
            //PhotoUploadButton.IsEnabled = false;

            iconSmile1.Tag = "";
            iconSmile2.Tag = "";
            iconSmile3.Tag = "";
            iconSmile4.Tag = "";
            iconSmile5.Tag = "";

            SetSmileIcons();

            PhotoComment.Text = "";
            textPhotoSize.Text = "- MB";
            

        }

        private void CommentTextChanged(object sender, TextChangedEventArgs e) 
        {
            textCommentSize.Text = PhotoComment.Text.Length + "/" + PhotoComment.MaxLength;            
        }

    }
}
