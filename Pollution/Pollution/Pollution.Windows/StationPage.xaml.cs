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
using Pollution.ViewModels;
using System.Net;
using System.Globalization;
using Windows.UI.Core;
using Windows.Storage.Pickers;
using Windows.Media.Capture;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Devices.Enumeration;
using Windows.UI.Popups;
using Windows.Media.MediaProperties;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace Pollution
{
    
    public sealed partial class StationPage : Page
    {

 

        private const string URL = "http://data.garvis.cz/pollution/stationhistory";
        private const string URLphotos = "http://data.garvis.cz/pollution/stationphotos";

        bool historyOK = false;

        private ResourceLoader _myResourceLoader = new ResourceLoader();
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        /*
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }
        */
        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public StationPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.Loaded += StationPage_Loaded;
            this.Unloaded += StationPage_Unloaded;

            contentGraphsGrid.DataContext = App.ViewModel;

            Station tmp = App.ViewModel.DetailsStation; // Get Station to display
            //vynulování fotografií z VM, aby se nezobrazovaly u stanic, které nemají vlastní fotografie
            App.ViewModel.PhotosDetailsStation = null;           
        }


        


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           
             Station myElement = e.Parameter as Station;
             this.DataContext = myElement;
             App.ViewModel.DetailsStation = myElement;
             navigationHelper.OnNavigatedTo(e);
             


             historyQuality.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
             historyNO2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
             historyCO.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
             historyO3.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
             historyPM10.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
             


            /*
             * //je treba dodela progressy
             StatusBarProgressIndicator progress = new ProgressIndicator
             {
                 IsVisible = true,
                 IsIndeterminate = true
             };
            
             SystemTray.SetProgressIndicator(this, progress);
            */
            
            
             string l = App.ViewModel.DetailsStation.Code;          //vlozi do stringu pozadavku kod stanice oznacene jako detail
            

             //create URL request
             string ut = DateTime.Now.Ticks.ToString();
             string ms = "0";
             string manuf = "Nokia";
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

             var request = HttpWebRequest.Create(URL + "?ns=" + l + "&ms=" + ms + "&t=" + ut + "&m=" + manuf + "&key=" + r.ToString());
             var result = (IAsyncResult)request.BeginGetResponse(ResponseCallback, request);

             //Request for Photos

             var request1 = HttpWebRequest.Create(URLphotos + "?ns=" + l + "&ms=" + ms + "&t=" + ut + "&m=" + manuf + "&key=" + r.ToString());
             var result1 = (IAsyncResult)request1.BeginGetResponse(ResponseCallbackPhotos, request1);   
        }

        private async void ResponseCallback(IAsyncResult result)
        {
            var request = (HttpWebRequest)result.AsyncState;
            var response = request.EndGetResponse(result);



            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var contents = reader.ReadToEnd();
                //Dispatcher.BeginInvoke((Action)(() => { ProcessResult(contents, response.ContentLength); }));
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(new CoreDispatcherPriority(), (() => { ProcessResult(contents, response.ContentLength); }));
            }
        }

        private async void ResponseCallbackPhotos(IAsyncResult result)
        {
            var request = (HttpWebRequest)result.AsyncState;
            var response = request.EndGetResponse(result);

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var contents = reader.ReadToEnd();
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(new CoreDispatcherPriority(), (() => { ProcessResultPhotos(contents, response.ContentLength); }));
            }
        }

        void ProcessResultPhotos(string content, long length)
        {

            if (length <= 0)
            {
                return;
            }

            List<PPhoto> pp = new List<PPhoto>();

            //process history data
            string s = content;
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
                                if (sl.Length != 4) continue;


                                p = new PPhoto();
                                p.Time = new DateTime(Int64.Parse(sl[1]));
                                p.URL = sl[0];
                                p.Smile = Int32.Parse(sl[2]);
                                p.Note = sl[3];
                                p.Station = null;

                                pp.Add(p);

                            }
                            catch (Exception exp)
                            {
                            }

                        }
                    }

                    App.ViewModel.PhotosDetailsStation = pp;
                }
                catch (Exception exp)
                {
                }
            }
        }

        void ProcessResult(string content, long length)
        {
            PHistory ph = null;

            int daysQuality = 0, daysHistory = 0;

            if (length <= 0)
            {
                //MessageBox.Show(AppResources.MsgDownloadError, AppResources.Error, MessageBoxButton.OK);
                MessageDialog md = new MessageDialog(_myResourceLoader.GetString("MsgDownloadError"));
                md.ShowAsync();
                historyOK = false;
                //dodelat progres stahování
                /*
                if (SystemTray.ProgressIndicator != null)
                    SystemTray.ProgressIndicator.IsVisible = false;
                */
            }
            else
            {

                App.ViewModel.HistoryDetailsStation = new PHistory();
                ph = App.ViewModel.HistoryDetailsStation;

                historyOK = true;

                //process history data
                string s = content;
                string[] sl;

                int q;
                double v;
                PElement pe;


                if (s != null && s.Length != 0)
                {
                    try
                    {
                        using (StringReader reader = new StringReader(s))
                        {
                            string line;

                            //first line dateticks|qualitydays|historydays

                            line = reader.ReadLine();
                            sl = line.Split('|');
                            daysQuality = Int32.Parse(sl[1]);
                            daysHistory = Int32.Parse(sl[2]);



                            //second line
                            line = reader.ReadLine();
                            if (line.Length <= 5)
                            {
                                historyOK = false;
                                return;
                            }

                            sl = line.Split('|');

                            for (int i = 0; i < sl.Length; i++)
                            {
                                ph.SetQualityIndex(i, Int32.Parse(sl[i]));
                            }


                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.Length == 0)
                                {
                                    ph.AddQualityValue(-1);
                                    ph.AddListValue(EPElementType.SO2, null);
                                    ph.AddListValue(EPElementType.NO2, null);
                                    ph.AddListValue(EPElementType.CO, null);
                                    ph.AddListValue(EPElementType.O3, null);
                                    ph.AddListValue(EPElementType.PM10, null);
                                    ph.AddListValue(EPElementType.PM1024, null);
                                    continue;
                                }
                                try
                                {
                                    sl = line.Split('|');

                                    if (sl[0] != "") ph.AddQualityValue(Int32.Parse(sl[0])); else ph.AddQualityValue(-1);

                                    //SO2
                                    pe = new PElement(EPElementType.SO2);
                                    if (sl[1] != "")
                                    {
                                        pe.Value = Double.Parse(sl[1], CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        pe.Value = -1;
                                    }

                                    if (sl[2] != "")
                                    {
                                        pe.State = Int32.Parse(sl[2]);
                                    }
                                    else
                                    {
                                        pe.State = 0;
                                    }
                                    ph.AddListValue(EPElementType.SO2, pe);

                                    //NO2
                                    pe = new PElement(EPElementType.NO2);
                                    if (sl[3] != "")
                                    {
                                        pe.Value = Double.Parse(sl[3], CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        pe.Value = -1;
                                    }

                                    if (sl[4] != "")
                                    {
                                        pe.State = Int32.Parse(sl[4]);
                                    }
                                    else
                                    {
                                        pe.State = 0;
                                    }
                                    ph.AddListValue(EPElementType.NO2, pe);

                                    //CO
                                    pe = new PElement(EPElementType.CO);
                                    if (sl[5] != "")
                                    {
                                        pe.Value = Double.Parse(sl[5], CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        pe.Value = -1;
                                    }

                                    if (sl[6] != "")
                                    {
                                        pe.State = Int32.Parse(sl[6]);
                                    }
                                    else
                                    {
                                        pe.State = 0;
                                    }
                                    ph.AddListValue(EPElementType.CO, pe);

                                    //O3
                                    pe = new PElement(EPElementType.O3);
                                    if (sl[7] != "")
                                    {
                                        pe.Value = Double.Parse(sl[7], CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        pe.Value = -1;
                                    }

                                    if (sl[8] != "")
                                    {
                                        pe.State = Int32.Parse(sl[8]);
                                    }
                                    else
                                    {
                                        pe.State = 0;
                                    }
                                    ph.AddListValue(EPElementType.O3, pe);

                                    //PM10
                                    pe = new PElement(EPElementType.PM10);
                                    if (sl[9] != "")
                                    {
                                        pe.Value = Double.Parse(sl[9], CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        pe.Value = -1;
                                    }

                                    if (sl[10] != "")
                                    {
                                        pe.State = Int32.Parse(sl[10]);
                                    }
                                    else
                                    {
                                        pe.State = 0;
                                    }
                                    ph.AddListValue(EPElementType.PM10, pe);

                                    //PM24
                                    pe = new PElement(EPElementType.PM1024);
                                    if (sl[11] != "")
                                    {
                                        pe.Value = Double.Parse(sl[11], CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        pe.Value = -1;
                                    }

                                    if (sl[12] != "")
                                    {
                                        pe.State = Int32.Parse(sl[12]);
                                    }
                                    else
                                    {
                                        pe.State = 0;
                                    }
                                    ph.AddListValue(EPElementType.PM1024, pe);

                                }
                                catch (Exception exp)
                                {
                                }

                            }

                        }
                    }
                    catch (Exception exp)
                    {
                        historyOK = false;
                    }


                }
                //dodělat porgresindikátor
                /*
                if (SystemTray.ProgressIndicator != null)
                    SystemTray.ProgressIndicator.IsVisible = false;
                */


            }

            if (historyOK)
            {
                historyQuality.DataContext = App.ViewModel.HistoryDetailsStation;
                historyQuality.Refresh();
                historyQuality.Visibility = Windows.UI.Xaml.Visibility.Visible;
                

                if (ph != null && ph.GetMaxElementValue(EPElementType.SO2) != -1)
                {
                    historySO2.DataContext = App.ViewModel.HistoryDetailsStation;
                    historySO2.Refresh();
                    historySO2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else 
                {
                    //contentGraphsGrid.Items.Remove(soGraphStck); 
                    soGraphStck.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                

                if (ph != null && ph.GetMaxElementValue(EPElementType.NO2) != -1)
                {
                    historyNO2.DataContext = App.ViewModel.HistoryDetailsStation;
                    historyNO2.Refresh();
                    historyNO2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else 
                { 
                    //contentGraphsGrid.Items.Remove(noGraphStck);
                    noGraphStck.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }


                if (ph != null && ph.GetMaxElementValue(EPElementType.CO) != -1)
                {
                    historyCO.DataContext = App.ViewModel.HistoryDetailsStation;
                    historyCO.Refresh();
                    historyCO.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else 
                { 
                    //contentGraphsGrid.Items.Remove(coGraphStck);
                    coGraphStck.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }


                if (ph != null && ph.GetMaxElementValue(EPElementType.O3) != -1)
                {
                    historyO3.DataContext = App.ViewModel.HistoryDetailsStation;
                    historyO3.Refresh();
                    historyO3.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else 
                { 
                    //contentGraphsGrid.Items.Remove(oGraphStck);
                    oGraphStck.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }

                if (ph != null && ph.GetMaxElementValue(EPElementType.PM10) != -1)
                {
                    historyPM10.DataContext = App.ViewModel.HistoryDetailsStation;
                    historyPM10.Refresh();
                    historyPM10.Visibility = Windows.UI.Xaml.Visibility.Visible;

                }
                else 
                { 
                    //contentGraphsGrid.Items.Remove(pmGraphStck); 
                    pmGraphStck.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                

            }
            else
            {
                //errorLabel.Visibility = System.Windows.Visibility.Visible;
                
            }

            try
            {
                //Přidání fot do stackpanelu      
                if (App.ViewModel.PhotosDetailsStation != null)
                {
                    foreach (var item in App.ViewModel.PhotosDetailsStation)
                    {                        
                        Grid itemGrid = new Grid();
                        itemGrid.Margin = new Thickness(0,0,0,10);
                        itemGrid.Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
                        //itemGrid.Height = (ListPhotos.Height%152) * 152;
                        RowDefinition row1 = new RowDefinition();
                        row1.Height = new GridLength(10, GridUnitType.Star);
                        itemGrid.RowDefinitions.Add(row1);
                        RowDefinition row2 = new RowDefinition();
                        row2.Height = new GridLength(2, GridUnitType.Star);
                        row2.MinHeight = 50;
                        itemGrid.RowDefinitions.Add(row2);


                        Image img1 = new Image();
                        img1.Stretch = Stretch.UniformToFill;
                        img1.Source = new BitmapImage(new Uri(item.FullURL));
                        img1.Margin = new Thickness(5,10,5,0);
                        itemGrid.Children.Add(img1);

                        StackPanel stck1 = new StackPanel();
                        Grid.SetRow(stck1,1);
                        stck1.MinHeight = 200;
                        stck1.Margin = new Thickness(10,0,10,2);
                        itemGrid.Children.Add(stck1);            
                        


                        TextBlock txtblock = new TextBlock();
                        txtblock.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                        txtblock.TextWrapping = TextWrapping.NoWrap;
                        txtblock.Text = item.TimeText;
                        txtblock.Height = 30;
                        txtblock.Margin = new Thickness(0,1,4,0);
                        txtblock.FontSize = 12;
                        txtblock.Foreground = new SolidColorBrush(Colors.White);
                        txtblock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                        stck1.Children.Add(txtblock);
                        
                        Image img2 = new Image();
                        img2.Stretch = Stretch.UniformToFill;
                        img2.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                        img2.Height = 50;
                        img2.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
                        img2.Margin = new Thickness(0, -10, 0, 0);
                        img2.Width = 50;
                        img2.Source = item.IconSmile;
                        stck1.Children.Add(img2);
                        
                        TextBlock txtblock2 = new TextBlock();
                        txtblock2.TextWrapping = TextWrapping.Wrap;
                        txtblock2.Text = item.NoteText;
                        txtblock2.FontSize = 14;
                        txtblock2.Foreground = new SolidColorBrush(Colors.White);
                        txtblock2.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                        txtblock2.TextAlignment = TextAlignment.Right;
                        txtblock2.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                        txtblock2.Margin = new Thickness(55, -50, 0, 0);
                        stck1.Children.Add(txtblock2);

                      
                         
                        ListPhotos.Children.Add(itemGrid);
                    } 
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /*
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }
        */


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        private void homeButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void folderButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

            Frame.Navigate(typeof(PhotoPage));
        }


        private void photo_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) 
        {
            //TODO
        }
        #endregion

        #region VISUAL STATES

        //aktuální orientace
        private string currentState;

        /// <summary>
        /// Funkce rozlisujici vizualni stavy. Tedy jestli je aplikace vertikalne nebo horizontalne. 
        /// Pro horizontalnim stavu jeste rozlisuje, jestli je aplikace v rezimu na celou obrazovku, 
        /// pripnuta na leve nebo na prave strane. 
        /// </summary>
        private void DetermineVisualState()
        {
            var state = string.Empty;
            var applicationView = ApplicationView.GetForCurrentView();
            var size = Window.Current.Bounds;

            if (applicationView.IsFullScreen)
            {
                if (applicationView.Orientation == ApplicationViewOrientation.Landscape)
                {
                    state = "FullScreenLandscape";
                    currentState = "FullScreenLandscape";
                    setLandscapeChanges();
                }
                else
                {
                    state = "FullScreenPortrait";
                    currentState = "FullScreenPortrait";
                    setPortraitChanges();
                }
            }
            else
            {
                if (size.Width == 320)
                {
                    state = "Snapped";
                    currentState = "FullScreenLandscape";
                    setLandscapeChanges();
                }
                else if (size.Width <= (700))//puvodne 500
                {
                    state = "Narrow";
                    currentState = "FullScreenLandscape";
                    setLandscapeChanges();
                }
                else
                {
                    state = "Filled";
                    currentState = "FullScreenLandscape";
                    setLandscapeChanges();
                }

            }

            System.Diagnostics.Debug.WriteLine("Width: {0}, New VisulState: {1}", size.Width, state);

            VisualStateManager.GoToState(this, state, true);
        }

        private void setPortraitChanges()
        {
            //ScrollBar
            contentScrollViewer.VerticalScrollMode = ScrollMode.Enabled;
            contentScrollViewer.HorizontalScrollMode = ScrollMode.Disabled;
            contentScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            contentScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;

            //InnerContentGrid
            contentInnerRegion.ColumnDefinitions.Clear();
            contentInnerRegion.RowDefinitions.Clear();
            contentInnerRegion.RowDefinitions.Add(new RowDefinition());
            contentInnerRegion.RowDefinitions.Add(new RowDefinition());
            contentInnerRegion.RowDefinitions.Add(new RowDefinition());

            //Statuses
            Grid.SetRow(contentStatusesGrid, 0);

            //Graphs
            Grid.SetRow(contentGraphsGrid, 1);

            //Graphs grid
            contentGraphsGrid.RowDefinitions.Add(new RowDefinition());
            contentGraphsGrid.RowDefinitions.Add(new RowDefinition());
            contentGraphsGrid.RowDefinitions.Add(new RowDefinition());
            contentGraphsGrid.RowDefinitions.Add(new RowDefinition());
            contentGraphsGrid.RowDefinitions.Add(new RowDefinition());
            contentGraphsGrid.RowDefinitions.Add(new RowDefinition());
            contentGraphsGrid.ColumnDefinitions.Clear();

            //Graphs gridation
            Grid.SetRow(historyStck, 0);
            Grid.SetColumn(historyStck, 0);
            Grid.SetRow(coGraphStck, 1);
            Grid.SetColumn(coGraphStck, 0);
            Grid.SetRow(noGraphStck, 2);
            Grid.SetColumn(noGraphStck, 0);
            Grid.SetRow(oGraphStck, 3);
            Grid.SetColumn(oGraphStck, 0);
            Grid.SetRow(pmGraphStck, 4);
            Grid.SetColumn(pmGraphStck, 0);
            Grid.SetRow(soGraphStck, 5);
            Grid.SetColumn(soGraphStck, 0);

            //Photos
            Grid.SetRow(contentPhotoGridView, 2);
            ListPhotos.Orientation = Orientation.Vertical;
            ListPhotos.Margin = new Thickness(0, 0, 0, 50);

            //SingleGraphs
            historyQuality.Height = (Window.Current.Bounds.Width / 10) * 6;
            historyQuality.Width = (Window.Current.Bounds.Width / 10) * 8;
            historyPM10.Height = (Window.Current.Bounds.Width / 10) * 6;
            historyPM10.Width = (Window.Current.Bounds.Width / 10) * 8;
            historyO3.Height = (Window.Current.Bounds.Width / 10) * 6;
            historyO3.Width = (Window.Current.Bounds.Width / 10) * 8;
            historyNO2.Height = (Window.Current.Bounds.Width / 10) * 6;
            historyNO2.Width = (Window.Current.Bounds.Width / 10) * 8;
            historyCO.Height = (Window.Current.Bounds.Width / 10) * 6;
            historyCO.Width = (Window.Current.Bounds.Width / 10) * 8;
            historySO2.Height = (Window.Current.Bounds.Width / 10) * 6;
            historySO2.Width = (Window.Current.Bounds.Width / 10) * 8;
        }

        private void setLandscapeChanges()
        {
            //ScrollBar
            contentScrollViewer.VerticalScrollMode = ScrollMode.Disabled;
            contentScrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
            contentScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            contentScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            //InnerContentGrid,
            contentInnerRegion.ColumnDefinitions.Clear();
            contentInnerRegion.RowDefinitions.Clear();
            contentInnerRegion.ColumnDefinitions.Add(new ColumnDefinition());
            contentInnerRegion.ColumnDefinitions.Add(new ColumnDefinition());
            contentInnerRegion.ColumnDefinitions.Add(new ColumnDefinition());

            //Statuses
            Grid.SetColumn(contentStatusesGrid, 0);

            //Graphs
            Grid.SetColumn(contentGraphsGrid, 1);

            //Graphs grid            
            contentGraphsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGraphsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGraphsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGraphsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGraphsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGraphsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGraphsGrid.RowDefinitions.Clear();

            //Graphs gridation
            Grid.SetRow(historyStck, 0);
            Grid.SetColumn(historyStck, 0);
            Grid.SetRow(coGraphStck, 0);
            Grid.SetColumn(coGraphStck, 1);
            Grid.SetRow(noGraphStck, 0);
            Grid.SetColumn(noGraphStck, 2);
            Grid.SetRow(oGraphStck, 0);
            Grid.SetColumn(oGraphStck, 3);
            Grid.SetRow(pmGraphStck, 0);
            Grid.SetColumn(pmGraphStck, 4);
            Grid.SetRow(soGraphStck, 0);
            Grid.SetColumn(soGraphStck, 5);

            //Photos
            Grid.SetColumn(contentPhotoGridView, 2);
            ListPhotos.Orientation = Orientation.Horizontal;
            ListPhotos.Margin = new Thickness(0, 0, 50, 0);

            //SingleGraphs
            historyQuality.Height = (Window.Current.Bounds.Height / 10) * 6;
            historyQuality.Width = (Window.Current.Bounds.Height / 10) * 6;
            historyPM10.Height = (Window.Current.Bounds.Height / 10) * 6;
            historyPM10.Width = (Window.Current.Bounds.Height / 10) * 6;
            historyO3.Height = (Window.Current.Bounds.Height / 10) * 6;
            historyO3.Width = (Window.Current.Bounds.Height / 10) * 6;
            historyNO2.Height = (Window.Current.Bounds.Height / 10) * 6;
            historyNO2.Width = (Window.Current.Bounds.Height / 10) * 6;
            historyCO.Height = (Window.Current.Bounds.Height / 10) * 6;
            historyCO.Width = (Window.Current.Bounds.Height / 10) * 6;
            historySO2.Height = (Window.Current.Bounds.Height / 10) * 6;
            historySO2.Width = (Window.Current.Bounds.Height / 10) * 6;
        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            DetermineVisualState();
        }

        void StationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        void StationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
            DetermineVisualState();
        }
        #endregion

    }
}
