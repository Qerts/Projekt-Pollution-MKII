using Bing.Maps;
using Pollution.Flyouts;
using Pollution.Utils;
using Pollution.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;





// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Pollution
{
    public sealed partial class MainPage : Page
    {
        private List<UIElement> blankTileCollection = new List<UIElement>();
        private List<FrameworkElement> statusCollection = new List<FrameworkElement>();
        private List<FrameworkElement> imageCollection = new List<FrameworkElement>();

  

        
        private Tile tileGenerator;

        //"ploše" řešená tlačítka panelů
        Grid mapbutton, databutton, menubutton, mainstatus, infoTile;

        //aktuální orientace
        private string currentState;
        

        //globalni promenne pro animace jednotlivych orientaci
        Storyboard dataPanelShowSB;
        Storyboard dataPanelHideSB;
        Storyboard menuPanelShowSB;
        Storyboard menuPanelHideSB;
        Storyboard dataPanelShowSBP;
        Storyboard dataPanelHideSBP;
        Storyboard menuPanelShowSBP;
        Storyboard menuPanelHideSBP;
        Storyboard mapPanelShowSB;
        Storyboard mapPanelShowButtonSB;
        Storyboard mapPanelHideSB;
        Storyboard mapPanelHideButtonSB;
        Storyboard mapPanelShowSBP;
        Storyboard mapPanelHideSBP;
        DoubleAnimation doubleAnim;
        DoubleAnimation doubleAnim2;
        DoubleAnimation doubleAnim3;
        DoubleAnimation doubleAnim4;

        //globální proměnné pro piny, zatím trochu zmatečné
        Pin oldSelected;
        Pin newSelected;
        
        /// <summary>
        /// Krom inicializace komponent jsou zde volány funkce pro načtení dat.
        /// </summary>
        public MainPage()
        {

            
            this.InitializeComponent();
            //spuštění tasku


            //localsettings definition
            setUpLocalSettings();

            
            //Přednačtení dat
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DataPreLoad();
            });
            

            //nastavení datacontextu
            rootPage.DataContext = App.ViewModel;
            
            //5 s pauza
            var initialStartPause = Task.Run(async delegate
            {
                await Task.Delay(5000);
            });
            initialStartPause.Wait();
            
            //stažení dat
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                LoadData();
            });
            
            //stažení dat každých 60s
            var afterStartLoad = Task.Run(async delegate
            {
                while (true)
                {                                        
                    await Task.Delay(600000);
                    this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        LoadData();
                    });
                }
            });
            
            
            //prirazeni udalosti 
            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;

            tileGenerator = new Tile(this);

            //inicializace a umístění tlačítek
            mapbutton = tileGenerator.buttonTile_MapPanel(1, 1);
            MapPanel.Children.Add(mapbutton);
            menubutton = tileGenerator.buttonTile_MenuPanel(1, 0);
            MenuPanel.Children.Add(menubutton);
            databutton = tileGenerator.buttonTile_DataPanel(1, 1);
            DataPanel.Children.Add(databutton);

            //Přiřazení události, která nastane, pokud se změní property ViewModelu.
            App.ViewModel.PropertyChanged += new PropertyChangedEventHandler(ViewModel_PropertyChanged);
        }

        /// <summary>
        /// LocalSettings nahrazuje ve WinRT LocalStorage.
        /// </summary>
        private void setUpLocalSettings()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings; 
            ApplicationDataContainer container;
            container = localSettings.CreateContainer("AppSettings", Windows.Storage.ApplicationDataCreateDisposition.Always);
            
     
        }

        /// <summary>
        /// Při načtení indexu je zjištěn state a zajištěno, aby se reagovalo na změnu stavu v průběhu zobrazení stránky.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
            DetermineVisualState();
        }
        /// <summary>
        /// Pravděpodobně pro zamezení výpočtů při změně stavu zařízení, přičemž hlavní strana není obrazena
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            DetermineVisualState();
        }

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
                    setSnappedChanges();
                }
                else if (size.Width <= (700))//puvodne 500
                {
                    state = "Narrow";
                    currentState = "FullScreenLandscape";
                    setNarrowChanges();
                }
                else 
                {
                    state = "Filled";
                    currentState = "FullScreenLandscape";
                    setFilledChanges();
                }

            }

            System.Diagnostics.Debug.WriteLine("Width: {0}, New VisulState: {1}", size.Width, state);

            VisualStateManager.GoToState(this, state, true);
        }
        /// <summary>
        /// V teto funkci jsou deklarovany a inicializovány prvky hlavniho gridu, tedy dlazdice. 
        /// Prvky jsou reinicializovány nebo premistovany ve funkcich pro jednotlive stavy.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            for (int a = 0; a < 66; a++)
            {
                var tile = tileGenerator.RandomizedBlankTile(a / 11, a % 11);
                LayoutGrid.Children.Add(tile);
                blankTileCollection.Add(tile);
            }

            //male statusy
            var o3smallstatus = tileGenerator.statusTIle_O3(0,0);
            LayoutGrid.Children.Add(o3smallstatus);
            statusCollection.Add(o3smallstatus);
            
            var cosmallstatus = tileGenerator.statusTile_CO(0,0);
            LayoutGrid.Children.Add(cosmallstatus);
            statusCollection.Add(cosmallstatus);

            var so2smallstatus = tileGenerator.statusTile_SO2(0, 0);
            LayoutGrid.Children.Add(so2smallstatus);
            statusCollection.Add(so2smallstatus);
            var pm10smallstatus = tileGenerator.statusTile_PM10(0, 0);
            LayoutGrid.Children.Add(pm10smallstatus);
            statusCollection.Add(pm10smallstatus);
            var no2smallstatus = tileGenerator.statusTile_NO2(0, 0);
            LayoutGrid.Children.Add(no2smallstatus);
            statusCollection.Add(no2smallstatus);
            
            /*
            //OBRAZKY
            var imgtile = tileGenerator.imageTile(1, 3, "Assets/Logo.scale-100.png");
            LayoutGrid.Children.Add(imgtile);
            imageCollection.Add(imgtile);
            
            var imgtile2 = tileGenerator.imageTile(1, 3, "Assets/Logo.scale-100.png");
            LayoutGrid.Children.Add(imgtile2);
            imageCollection.Add(imgtile2);
            var imgtile3 = tileGenerator.imageTile(1, 3, "Assets/Logo.scale-100.png");
            LayoutGrid.Children.Add(imgtile3);
            imageCollection.Add(imgtile3);
            var imgtile4 = tileGenerator.imageTile(1, 3, "Assets/Logo.scale-100.png");
            LayoutGrid.Children.Add(imgtile4);
            imageCollection.Add(imgtile4);
            var imgtile5 = tileGenerator.imageTile(1, 3, "Assets/Logo.scale-100.png");
            LayoutGrid.Children.Add(imgtile5);
            imageCollection.Add(imgtile5);
            var imgtile6 = tileGenerator.imageTile(1, 3, "Assets/Logo.scale-100.png");
            LayoutGrid.Children.Add(imgtile6);
            imageCollection.Add(imgtile6);
            */

            //HLAVNI STATUS
            mainstatus = tileGenerator.mainStatusTile(2, 2);
            mainstatus.Tapped += mainstatus_Tapped;
            LayoutGrid.Children.Add(mainstatus);
                       
            //VELIKOSTI PISMA ODVOZENE OD VELIKOSTI ZOBRAZENI
            setFontSizes();  
     
            //informační panel
            infoTile = tileGenerator.InfoTile(1, 1);
            LayoutGrid.Children.Add(infoTile);

            //nabindování obrázku na pozadí
            Binding layoutGridBinding = new Binding();
            layoutGridBinding.Source = App.ViewModel;
            layoutGridBinding.Path = new PropertyPath("CurrentStation.Quality");
            layoutGridBinding.Converter = new BackGroundImageConverter();
            LayoutGrid.SetBinding(Grid.BackgroundProperty, layoutGridBinding);

            //BackgroundService.RegisterBackgroundTask("BackgroundTask.TileTask", "TileTask", new TimeTrigger(15, false), null);
            BackgroundService.RegisterTileTask();
        }

        void mainstatus_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.ViewModel.CurrentStation != null)
            {
                this.Frame.Navigate(typeof(StationPage), App.ViewModel.CurrentStation);
            }
        }



        #region CHANGES
        /// <summary>
        /// Funkce obsluhujici zmeny v zobrazeni pri vertikalnim stavu. Jedna se hlavne o zmeny vlastnosti gridu, panelu
        /// a podobne.
        /// </summary>
        private void setPortraitChanges() 
        {
            //layoutroot zmeny 

            LayoutRoot.ColumnDefinitions.Clear();
            LayoutRoot.RowDefinitions.Clear();



            RowDefinition rowdef21 = new RowDefinition();
            rowdef21.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(rowdef21);
            RowDefinition rowdef22 = new RowDefinition();
            rowdef22.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(rowdef22);
            RowDefinition rowdef23 = new RowDefinition();
            rowdef23.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(rowdef23);
            RowDefinition rowdef24 = new RowDefinition();
            rowdef24.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(rowdef24);
            RowDefinition rowdef25 = new RowDefinition();
            rowdef25.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(rowdef25);
            RowDefinition rowdef26 = new RowDefinition();
            rowdef26.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(rowdef26);
            RowDefinition rowdef27 = new RowDefinition();
            rowdef27.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(rowdef27);
            RowDefinition rowdef28 = new RowDefinition();
            rowdef28.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(rowdef28);
            RowDefinition rowdef29 = new RowDefinition();
            rowdef29.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(rowdef29);
            RowDefinition rowdef210 = new RowDefinition();
            rowdef210.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(rowdef210);
            RowDefinition rowdef211 = new RowDefinition();
            rowdef211.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(rowdef211);



            ColumnDefinition coldef21 = new ColumnDefinition();
            coldef21.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(coldef21);
            ColumnDefinition coldef22 = new ColumnDefinition();
            coldef22.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(coldef22);
            ColumnDefinition coldef23 = new ColumnDefinition();
            coldef23.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(coldef23);
            ColumnDefinition coldef24 = new ColumnDefinition();
            coldef24.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(coldef24);
            ColumnDefinition coldef25 = new ColumnDefinition();
            coldef25.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(coldef25);
            ColumnDefinition coldef26 = new ColumnDefinition();
            coldef26.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(coldef26);

            
            
            //layoutgrid zmeny
            LayoutGrid.ColumnDefinitions.Clear();
            LayoutGrid.RowDefinitions.Clear();

            Grid.SetRowSpan(LayoutGrid, 11);
            Grid.SetColumnSpan(LayoutGrid, 6);

            RowDefinition rowdef1 = new RowDefinition();
            rowdef1.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef1);
            RowDefinition rowdef2 = new RowDefinition();
            rowdef2.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef2);
            RowDefinition rowdef3 = new RowDefinition();
            rowdef3.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef3);
            RowDefinition rowdef4 = new RowDefinition();
            rowdef4.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef4);
            RowDefinition rowdef5 = new RowDefinition();
            rowdef5.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef5);
            RowDefinition rowdef6 = new RowDefinition();
            rowdef6.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef6);
            RowDefinition rowdef7 = new RowDefinition();
            rowdef7.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef7);
            RowDefinition rowdef8 = new RowDefinition();
            rowdef8.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef8);
            RowDefinition rowdef9 = new RowDefinition();
            rowdef9.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef9);
            RowDefinition rowdef10 = new RowDefinition();
            rowdef10.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef10);
            RowDefinition rowdef11 = new RowDefinition();
            rowdef11.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef11);



            ColumnDefinition coldef1 = new ColumnDefinition();
            coldef1.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef1);
            ColumnDefinition coldef2 = new ColumnDefinition();
            coldef2.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef2);
            ColumnDefinition coldef3 = new ColumnDefinition();
            coldef3.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef3);
            ColumnDefinition coldef4 = new ColumnDefinition();
            coldef4.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef4);
            ColumnDefinition coldef5 = new ColumnDefinition();
            coldef5.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef5);
            ColumnDefinition coldef6 = new ColumnDefinition();
            coldef6.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef6);
            


            Random random = new Random();

            foreach (FrameworkElement element in blankTileCollection)
            {

                Grid.SetColumn(element, blankTileCollection.IndexOf(element) / 11);
                Grid.SetRow(element, blankTileCollection.IndexOf(element) % 11);
            }

            foreach (FrameworkElement element in statusCollection)
            {

                Grid.SetColumn(element, random.Next(1, 4));
                Grid.SetRow(element, random.Next(5,9));
            }

            Grid.SetRow(mainstatus, 2);
            Grid.SetColumn(mainstatus, 2);
            
            foreach (FrameworkElement element in imageCollection)
            {

                Grid.SetColumn(element, random.Next(1,4));
                Grid.SetRow(element, random.Next(1, 9));
            }

            //zmeny na datapanelu
            Grid.SetColumn(DataPanel, 0);
            Grid.SetColumnSpan(DataPanel, 5);
            Grid.SetRow(DataPanel, 0);
            Grid.SetRowSpan(DataPanel, 11);

            DataPanel.RowDefinitions.Clear();
            DataPanel.ColumnDefinitions.Clear();

            RowDefinition rowdef12 = new RowDefinition();
            rowdef12.Height = new GridLength(1, GridUnitType.Star);
            DataPanel.RowDefinitions.Add(rowdef12);
            RowDefinition rowdef13 = new RowDefinition();
            rowdef13.Height = new GridLength(1, GridUnitType.Star);
            DataPanel.RowDefinitions.Add(rowdef13);
            RowDefinition rowdef14 = new RowDefinition();
            rowdef14.Height = new GridLength(9, GridUnitType.Star);
            DataPanel.RowDefinitions.Add(rowdef14);

            ColumnDefinition coldef11 = new ColumnDefinition();
            coldef11.Width = new GridLength(4, GridUnitType.Star);
            DataPanel.ColumnDefinitions.Add(coldef11);
            ColumnDefinition coldef12 = new ColumnDefinition();
            coldef12.Width = new GridLength(1, GridUnitType.Star);
            DataPanel.ColumnDefinitions.Add(coldef12);            


            Grid.SetColumn(dataPanelScrl, 0);
            Grid.SetColumnSpan(dataPanelScrl, 1);
            Grid.SetRow(dataPanelScrl, 0);
            Grid.SetRowSpan(dataPanelScrl, 3);

            dataPanelStck.Orientation = Orientation.Vertical;

            Grid.SetColumn(dataPanelButtonBackgroundPanel, 0);
            Grid.SetColumnSpan(dataPanelButtonBackgroundPanel, 2);
            Grid.SetRow(dataPanelButtonBackgroundPanel, 1);
            Grid.SetRowSpan(dataPanelButtonBackgroundPanel, 1);

            //menupanel zemny

            MenuPanel.ColumnDefinitions.Clear();
            MenuPanel.RowDefinitions.Clear();

            RowDefinition rowdef32 = new RowDefinition();
            rowdef32.Height = new GridLength(9, GridUnitType.Star);
            MenuPanel.RowDefinitions.Add(rowdef32);
            RowDefinition rowdef33 = new RowDefinition();
            rowdef33.Height = new GridLength(1, GridUnitType.Star);
            MenuPanel.RowDefinitions.Add(rowdef33);
            RowDefinition rowdef34 = new RowDefinition();
            rowdef34.Height = new GridLength(1, GridUnitType.Star);
            MenuPanel.RowDefinitions.Add(rowdef34);

            ColumnDefinition coldef31 = new ColumnDefinition();
            coldef31.Width = new GridLength(1, GridUnitType.Star);
            MenuPanel.ColumnDefinitions.Add(coldef31);
            ColumnDefinition coldef32 = new ColumnDefinition();
            coldef32.Width = new GridLength(2, GridUnitType.Star);
            MenuPanel.ColumnDefinitions.Add(coldef32);

            Grid.SetColumn(MenuPanel, 1);
            Grid.SetColumnSpan(MenuPanel, 3);
            Grid.SetRow(MenuPanel, 0);
            Grid.SetRowSpan(MenuPanel, 11);

            MenuPanelSubGrid.ColumnDefinitions.Clear();
            MenuPanelSubGrid.RowDefinitions.Clear();

            RowDefinition rowdef42 = new RowDefinition();
            rowdef42.Height = new GridLength(6, GridUnitType.Star);
            MenuPanelSubGrid.RowDefinitions.Add(rowdef42);
            RowDefinition rowdef43 = new RowDefinition();
            rowdef43.Height = new GridLength(5, GridUnitType.Star);
            MenuPanelSubGrid.RowDefinitions.Add(rowdef43);

            Grid.SetColumn(MenuPanelSubGrid, 1);
            Grid.SetColumnSpan(MenuPanelSubGrid, 1);
            Grid.SetRow(MenuPanelSubGrid, 0);
            Grid.SetRowSpan(MenuPanelSubGrid, 3);

            menuPanelButtonGrid.ColumnDefinitions.Clear();
            menuPanelButtonGrid.RowDefinitions.Clear();

            RowDefinition rowdef52 = new RowDefinition();
            rowdef52.Height = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(rowdef52);
            RowDefinition rowdef53 = new RowDefinition();
            rowdef53.Height = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(rowdef53);
            RowDefinition rowdef54 = new RowDefinition();
            rowdef54.Height = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(rowdef54);
            RowDefinition rowdef55 = new RowDefinition();
            rowdef55.Height = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(rowdef55);
            RowDefinition rowdef56 = new RowDefinition();
            rowdef56.Height = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(rowdef56);
            RowDefinition rowdef57 = new RowDefinition();
            rowdef57.Height = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(rowdef57);
            RowDefinition rowdef58 = new RowDefinition();
            rowdef58.Height = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(rowdef58);

            ColumnDefinition coldef50 = new ColumnDefinition();
            coldef50.Width = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(coldef50);
            ColumnDefinition coldef51 = new ColumnDefinition();
            coldef51.Width = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(coldef51);            
            ColumnDefinition coldef54 = new ColumnDefinition();
            coldef54.Width = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(coldef54);

            Grid.SetColumn(menuPanelButtonGrid, 0);
            Grid.SetColumnSpan(menuPanelButtonGrid, 1);
            Grid.SetRow(menuPanelButtonGrid, 0);
            Grid.SetRowSpan(menuPanelButtonGrid, 1);


            Grid.SetColumn(menuPanelButton1, 1);
            Grid.SetRow(menuPanelButton1, 1);
            Grid.SetColumn(menuPanelButton2, 1);
            Grid.SetRow(menuPanelButton2, 2);
            Grid.SetColumn(menuPanelButton3, 1);
            Grid.SetRow(menuPanelButton3, 3);

            Grid.SetColumn(menuPanelInfoGrid, 0);
            Grid.SetRow(menuPanelInfoGrid, 1);
            
            
            //naaktualizovani hodnot
            statusCOTxt.Text = Data.getString_CO();
            statusCOValue.Text = Data.getValue_CO().ToString();
            statusCOClr.Background = Data.getColorAndStatus(Data.getStatus_CO()).Item1;
            statusSOTxt.Text = Data.getString_SO2();
            statusSOValueTxt.Text = Data.getValue_SO2().ToString();
            statusSOClr.Background = Data.getColorAndStatus(Data.getStatus_SO2()).Item1;
            statusPOTxt.Text = Data.getString_PM10();
            statusPOValue.Text = Data.getValue_PM10().ToString();
            statusPOClr.Background = Data.getColorAndStatus(Data.getStatus_PM10()).Item1;
            statusOTxt.Text = Data.getString_O3();
            statusOValue.Text = Data.getValue_O3().ToString();
            statusOClr.Background = Data.getColorAndStatus(Data.getStatus_O3()).Item1;
            statusNOTxt.Text = Data.getString_NO2();
            statusNOValue.Text = Data.getValue_NO2().ToString();
            statusNOClr.Background = Data.getColorAndStatus(Data.getStatus_NO2()).Item1;

            mainStatusLbl.Background = Data.getMainColor();

            legendLabelB.Background = Data.getColorAndStatus(Data.StatusName.Bad).Item1;
            legendLabelG.Background = Data.getColorAndStatus(Data.StatusName.Good).Item1;
            legendLabelND.Background = Data.getColorAndStatus(Data.StatusName.NoData).Item1;
            legendLabelNM.Background = Data.getColorAndStatus(Data.StatusName.NoMeasurement).Item1;
            legendLabelSa.Background = Data.getColorAndStatus(Data.StatusName.Satisfying).Item1;
            legendLabelSu.Background = Data.getColorAndStatus(Data.StatusName.Suitable).Item1;
            legendLabelVB.Background = Data.getColorAndStatus(Data.StatusName.VeryBad).Item1;
            legendLabelVG.Background = Data.getColorAndStatus(Data.StatusName.VeryGood).Item1;

            menuPanelButton1.Background = Data.getColorAndStatus(Data.getMainMood()).Item1;
            menuPanelButton2.Background = Data.getColorAndStatus(Data.getMainMood()).Item1;
            menuPanelButton3.Background = Data.getColorAndStatus(Data.getMainMood()).Item1;
            menuPanelButton4.Background = Data.getColorAndStatus(Data.getMainMood()).Item1;
            menuPanelButton5.Background = Data.getColorAndStatus(Data.getMainMood()).Item1;

            //POSUNUTI PANELU
            MapPanel.Margin = new Thickness(0, -((Window.Current.Bounds.Height / 11) * 8), 0, ((Window.Current.Bounds.Height / 11) * 8));
            DataPanel.Margin = new Thickness(-((Window.Current.Bounds.Width / 6) * 4), 0, ((Window.Current.Bounds.Width / 6) * 4), 0);
            MenuPanel.Margin = new Thickness(((Window.Current.Bounds.Width / 6) * 4), 0, -((Window.Current.Bounds.Width / 6) * 4), 0);


            //UMISTENI BUTTONU
            Grid.SetColumn(menubutton, 0);
            Grid.SetRow(menubutton, 1);
            Grid.SetColumn(databutton, 1);
            Grid.SetRow(databutton, 1);
            Grid.SetColumn(mapbutton, 0);
            Grid.SetRow(mapbutton, 0);

            //ZAROVNANI ELEMENTU DATAPANELU
            statuses.Height = dataPanelStck.Width;
            statuses.Width = dataPanelStck.Width;
            bars.Height = dataPanelStck.Width;
            bars.Width = dataPanelStck.Width;
            legend.Height = dataPanelStck.Width;
            legend.Width = dataPanelStck.Width;

            //MALE STATUSY a obrazky
            setStatusesPortrait();
            setImagesPortrait();

            //animace pro portrait
            setAnimationsPortrait();

            //infopanel
            Grid.SetRow(infoTile, 10);
            Grid.SetColumn(infoTile, 0);

        }
        /// <summary>
        /// Funkce obsluhujici zmeny v zobrazeni pri horizontalnim stavu. Jedna se hlavne o zmeny vlastnosti gridu, panelu
        /// a podobne.
        /// </summary>
        private void setLandscapeChanges()
        {
            

            //layoutroot zmeny 

            LayoutRoot.ColumnDefinitions.Clear();
            LayoutRoot.RowDefinitions.Clear();



            ColumnDefinition rowdef21 = new ColumnDefinition();
            rowdef21.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef21);
            ColumnDefinition rowdef22 = new ColumnDefinition();
            rowdef22.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef22);
            ColumnDefinition rowdef23 = new ColumnDefinition();
            rowdef23.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef23);
            ColumnDefinition rowdef24 = new ColumnDefinition();
            rowdef24.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef24);
            ColumnDefinition rowdef25 = new ColumnDefinition();
            rowdef25.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef25);
            ColumnDefinition rowdef26 = new ColumnDefinition();
            rowdef26.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef26);
            ColumnDefinition rowdef27 = new ColumnDefinition();
            rowdef27.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef27);
            ColumnDefinition rowdef28 = new ColumnDefinition();
            rowdef28.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef28);
            ColumnDefinition rowdef29 = new ColumnDefinition();
            rowdef29.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef29);
            ColumnDefinition rowdef210 = new ColumnDefinition();
            rowdef210.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef210);
            ColumnDefinition rowdef211 = new ColumnDefinition();
            rowdef211.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef211);



            RowDefinition coldef21 = new RowDefinition();
            coldef21.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef21);
            RowDefinition coldef22 = new RowDefinition();
            coldef22.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef22);
            RowDefinition coldef23 = new RowDefinition();
            coldef23.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef23);
            RowDefinition coldef24 = new RowDefinition();
            coldef24.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef24);
            RowDefinition coldef25 = new RowDefinition();
            coldef25.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef25);
            RowDefinition coldef26 = new RowDefinition();
            coldef26.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef26);

            //layoutgrid zmeny

            LayoutGrid.ColumnDefinitions.Clear();
            LayoutGrid.RowDefinitions.Clear();

            Grid.SetRowSpan(LayoutGrid, 6);
            Grid.SetColumnSpan(LayoutGrid, 11);

            RowDefinition rowdef1 = new RowDefinition();
            rowdef1.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef1);
            RowDefinition rowdef2 = new RowDefinition();
            rowdef2.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef2);
            RowDefinition rowdef3 = new RowDefinition();
            rowdef3.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef3);
            RowDefinition rowdef4 = new RowDefinition();
            rowdef4.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef4);
            RowDefinition rowdef5 = new RowDefinition();
            rowdef5.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef5);
            RowDefinition rowdef6 = new RowDefinition();
            rowdef6.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef6);
            



            ColumnDefinition coldef1 = new ColumnDefinition();
            coldef1.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef1);
            ColumnDefinition coldef2 = new ColumnDefinition();
            coldef2.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef2);
            ColumnDefinition coldef3 = new ColumnDefinition();
            coldef3.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef3);
            ColumnDefinition coldef4 = new ColumnDefinition();
            coldef4.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef4);
            ColumnDefinition coldef5 = new ColumnDefinition();
            coldef5.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef5);
            ColumnDefinition coldef6 = new ColumnDefinition();
            coldef6.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef6);
            ColumnDefinition rowdef7 = new ColumnDefinition();
            rowdef7.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(rowdef7);
            ColumnDefinition rowdef8 = new ColumnDefinition();
            rowdef8.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(rowdef8);
            ColumnDefinition rowdef9 = new ColumnDefinition();
            rowdef9.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(rowdef9);
            ColumnDefinition rowdef10 = new ColumnDefinition();
            rowdef10.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(rowdef10);
            ColumnDefinition rowdef11 = new ColumnDefinition();
            rowdef11.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(rowdef11);

            Random random = new Random();

            foreach (FrameworkElement element in blankTileCollection)
            {

                Grid.SetColumn(element, blankTileCollection.IndexOf(element) % 11);
                Grid.SetRow(element, blankTileCollection.IndexOf(element) / 11);
            }

            foreach (FrameworkElement element in statusCollection)
            {

                Grid.SetColumn(element, random.Next(5, 9));
                Grid.SetRow(element, random.Next(1, 4));
            }
            
            Grid.SetRow(mainstatus, 2);
            Grid.SetColumn(mainstatus, 2);


            foreach (FrameworkElement element in imageCollection)
            {

                Grid.SetColumn(element, random.Next(1, 9));
                Grid.SetRow(element, random.Next(1, 4));
            }

            //zmeny na datapanelu

            Grid.SetColumn(DataPanel, 0);
            Grid.SetColumnSpan(DataPanel, 11);
            Grid.SetRow(DataPanel, 1);
            Grid.SetRowSpan(DataPanel, 5);

            DataPanel.RowDefinitions.Clear();
            DataPanel.ColumnDefinitions.Clear();

            ColumnDefinition rowdef12 = new ColumnDefinition();
            rowdef12.Width = new GridLength(1, GridUnitType.Star);
            DataPanel.ColumnDefinitions.Add(rowdef12);
            ColumnDefinition rowdef13 = new ColumnDefinition();
            rowdef13.Width = new GridLength(1, GridUnitType.Star);
            DataPanel.ColumnDefinitions.Add(rowdef13);
            ColumnDefinition rowdef14 = new ColumnDefinition();
            rowdef14.Width = new GridLength(9, GridUnitType.Star);
            DataPanel.ColumnDefinitions.Add(rowdef14);

            RowDefinition coldef11 = new RowDefinition();
            coldef11.Height = new GridLength(1, GridUnitType.Star);
            DataPanel.RowDefinitions.Add(coldef11);
            RowDefinition coldef12 = new RowDefinition();
            coldef12.Height = new GridLength(4, GridUnitType.Star);
            DataPanel.RowDefinitions.Add(coldef12);



            Grid.SetColumn(dataPanelScrl, 0);
            Grid.SetColumnSpan(dataPanelScrl, 3);
            Grid.SetRow(dataPanelScrl, 1);
            Grid.SetRowSpan(dataPanelScrl, 1);

            dataPanelStck.Orientation = Orientation.Horizontal;

            Grid.SetColumn(dataPanelButtonBackgroundPanel, 1);
            Grid.SetColumnSpan(dataPanelButtonBackgroundPanel, 1);
            Grid.SetRow(dataPanelButtonBackgroundPanel, 0);
            Grid.SetRowSpan(dataPanelButtonBackgroundPanel, 2);

            //menupanel zemny

            MenuPanel.ColumnDefinitions.Clear();
            MenuPanel.RowDefinitions.Clear();

            RowDefinition rowdef32 = new RowDefinition();
            rowdef32.Height = new GridLength(2, GridUnitType.Star);
            MenuPanel.RowDefinitions.Add(rowdef32);
            RowDefinition rowdef33 = new RowDefinition();
            rowdef33.Height = new GridLength(1, GridUnitType.Star);
            MenuPanel.RowDefinitions.Add(rowdef33);
            

            ColumnDefinition coldef31 = new ColumnDefinition();
            coldef31.Width = new GridLength(9, GridUnitType.Star);
            MenuPanel.ColumnDefinitions.Add(coldef31);
            ColumnDefinition coldef32 = new ColumnDefinition();
            coldef32.Width = new GridLength(1, GridUnitType.Star);
            MenuPanel.ColumnDefinitions.Add(coldef32);
            ColumnDefinition coldef33 = new ColumnDefinition();
            coldef33.Width = new GridLength(1, GridUnitType.Star);
            MenuPanel.ColumnDefinitions.Add(coldef33);

            Grid.SetColumn(MenuPanel, 0);
            Grid.SetColumnSpan(MenuPanel, 11);
            Grid.SetRow(MenuPanel, 0);
            Grid.SetRowSpan(MenuPanel, 3);

            MenuPanelSubGrid.ColumnDefinitions.Clear();
            MenuPanelSubGrid.RowDefinitions.Clear();

            ColumnDefinition coldef42 = new ColumnDefinition();
            coldef42.Width = new GridLength(6, GridUnitType.Star);
            MenuPanelSubGrid.ColumnDefinitions.Add(coldef42);
            ColumnDefinition coldef43 = new ColumnDefinition();
            coldef43.Width = new GridLength(5, GridUnitType.Star);
            MenuPanelSubGrid.ColumnDefinitions.Add(coldef43);

            Grid.SetColumn(MenuPanelSubGrid, 0);
            Grid.SetColumnSpan(MenuPanelSubGrid, 3);
            Grid.SetRow(MenuPanelSubGrid, 0);
            Grid.SetRowSpan(MenuPanelSubGrid, 1);

            menuPanelButtonGrid.ColumnDefinitions.Clear();
            menuPanelButtonGrid.RowDefinitions.Clear();

            ColumnDefinition rowdef52 = new ColumnDefinition();
            rowdef52.Width = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef52);
            ColumnDefinition rowdef53 = new ColumnDefinition();
            rowdef53.Width = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef53);
            ColumnDefinition rowdef54 = new ColumnDefinition();
            rowdef54.Width = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef54);
            ColumnDefinition rowdef55 = new ColumnDefinition();
            rowdef55.Width = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef55);
            ColumnDefinition rowdef56 = new ColumnDefinition();
            rowdef56.Width = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef56);
            ColumnDefinition rowdef57 = new ColumnDefinition();
            rowdef57.Width = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef57);
            ColumnDefinition rowdef58 = new ColumnDefinition();
            rowdef58.Width = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef58);

            RowDefinition coldef50 = new RowDefinition();
            coldef50.Height = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef50);
            RowDefinition coldef51 = new RowDefinition();
            coldef51.Height = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef51);
            RowDefinition coldef52 = new RowDefinition();
            RowDefinition coldef54 = new RowDefinition();
            coldef54.Height = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef54);

            Grid.SetColumn(menuPanelButtonGrid, 0);
            Grid.SetColumnSpan(menuPanelButtonGrid, 1);
            Grid.SetRow(menuPanelButtonGrid, 0);
            Grid.SetRowSpan(menuPanelButtonGrid, 1);

            Grid.SetColumn(menuPanelButton1, 1);
            Grid.SetRow(menuPanelButton1, 1);
            Grid.SetColumn(menuPanelButton2, 2);
            Grid.SetRow(menuPanelButton2, 1);
            Grid.SetColumn(menuPanelButton3, 3);
            Grid.SetRow(menuPanelButton3, 1);
            Grid.SetColumn(menuPanelButton4, 4);
            Grid.SetRow(menuPanelButton4, 1);
            Grid.SetColumn(menuPanelButton5, 5);
            Grid.SetRow(menuPanelButton5, 1);

            Grid.SetColumn(menuPanelInfoGrid, 1);
            Grid.SetRow(menuPanelInfoGrid, 0);

            //mappanel zmeny

            Grid.SetColumn(MapPanel, 0);
            Grid.SetColumnSpan(MapPanel, 11);
            Grid.SetRow(MapPanel, 0);
            Grid.SetRowSpan(MapPanel, 6);

            MapPanel.RowDefinitions.Clear();
            MapPanel.ColumnDefinitions.Clear();

            RowDefinition rowdef62 = new RowDefinition();
            rowdef62.Height = new GridLength(1, GridUnitType.Star);
            MapPanel.RowDefinitions.Add(rowdef62);
            RowDefinition rowdef63 = new RowDefinition();
            rowdef63.Height = new GridLength(1, GridUnitType.Star);
            MapPanel.RowDefinitions.Add(rowdef63);
            RowDefinition rowdef64 = new RowDefinition();
            rowdef64.Height = new GridLength(4, GridUnitType.Star);
            MapPanel.RowDefinitions.Add(rowdef64);

            ColumnDefinition coldef61 = new ColumnDefinition();
            coldef61.Width = new GridLength(10, GridUnitType.Star);
            MapPanel.ColumnDefinitions.Add(coldef61);
            ColumnDefinition coldef62 = new ColumnDefinition();
            coldef62.Width = new GridLength(1, GridUnitType.Star);
            MapPanel.ColumnDefinitions.Add(coldef62);


            Grid.SetColumn(mapPanelSubGrid, 0);
            Grid.SetColumnSpan(mapPanelSubGrid, 1);
            Grid.SetRow(mapPanelSubGrid, 0);
            Grid.SetRowSpan(mapPanelSubGrid, 3);



            /////mappanel konec

            statusCOTxt.Text = Data.getString_CO();
            statusSOTxt.Text = Data.getString_SO2();
            statusPOTxt.Text = Data.getString_PM10();
            statusOTxt.Text = Data.getString_O3();
            statusNOTxt.Text = Data.getString_NO2();
            

            //POSUNUTI PANELU
            MapPanel.Margin = new Thickness(-((Window.Current.Bounds.Width / 11) * 10), 0, ((Window.Current.Bounds.Width / 11) * 10), 0);
            DataPanel.Margin = new Thickness(0, ((Window.Current.Bounds.Height / 6) * 4), 0, -((Window.Current.Bounds.Height / 6) * 4));
            MenuPanel.Margin = new Thickness(0, -((Window.Current.Bounds.Height / 6) * 2), 0, ((Window.Current.Bounds.Height / 6) * 2));

            //UMISTENI BUTTONU
            Grid.SetColumn(menubutton, 1);
            Grid.SetRow(menubutton, 1);
            Grid.SetColumn(databutton, 1);
            Grid.SetRow(databutton, 0);
            Grid.SetColumn(mapbutton, 1);
            Grid.SetRow(mapbutton, 1);


            //ZAROVNANI ELEMENTU DATAPANELU
            statuses.Height = dataPanelStck.Height;
            statuses.Width = dataPanelStck.Height;
            bars.Height = dataPanelStck.Height;
            bars.Width = dataPanelStck.Height;
            legend.Height = dataPanelStck.Height;
            legend.Width = dataPanelStck.Height;

            //MALE STATUSY a obrazky
            setStatusesLandscape();
            setImagesLandscape();

            //animace pro landscape
            setAnimationsLandscape();

            //infopanel
            Grid.SetRow(infoTile, 5);
            Grid.SetColumn(infoTile, 10);
        }
        /// <summary>
        /// Funkce obsluhujici zmeny v zobrazeni pri horizontalnim stavu. Jedna se hlavne o zmeny vlastnosti gridu, panelu
        /// a podobne.
        /// </summary>
        private void setSnappedChanges() 
        {
            //docasne zmeny
            MapPanel.Visibility = Visibility.Collapsed;
            //layoutroot zmeny 

            LayoutRoot.ColumnDefinitions.Clear();
            LayoutRoot.RowDefinitions.Clear();


            ColumnDefinition rowdef21 = new ColumnDefinition();
            rowdef21.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef21);
            ColumnDefinition rowdef22 = new ColumnDefinition();
            rowdef22.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef22);
            ColumnDefinition rowdef23 = new ColumnDefinition();
            rowdef23.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef23);
            ColumnDefinition rowdef24 = new ColumnDefinition();
            rowdef24.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef24);
            ColumnDefinition rowdef25 = new ColumnDefinition();
            rowdef25.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef25);
            
            



            RowDefinition coldef21 = new RowDefinition();
            coldef21.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef21);
            RowDefinition coldef22 = new RowDefinition();
            coldef22.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef22);
            RowDefinition coldef23 = new RowDefinition();
            coldef23.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef23);
            RowDefinition coldef24 = new RowDefinition();
            coldef24.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef24);
            RowDefinition coldef25 = new RowDefinition();
            coldef25.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef25);
            RowDefinition coldef26 = new RowDefinition();
            coldef26.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef26);

            //layoutgrid zmeny

            LayoutGrid.ColumnDefinitions.Clear();
            LayoutGrid.RowDefinitions.Clear();
            

            Grid.SetRowSpan(LayoutGrid, 6);
            Grid.SetColumnSpan(LayoutGrid, 6);

            RowDefinition rowdef1 = new RowDefinition();
            rowdef1.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef1);
            RowDefinition rowdef2 = new RowDefinition();
            rowdef2.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef2);
            RowDefinition rowdef3 = new RowDefinition();
            rowdef3.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef3);
            RowDefinition rowdef4 = new RowDefinition();
            rowdef4.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef4);
            RowDefinition rowdef5 = new RowDefinition();
            rowdef5.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef5);
            RowDefinition rowdef6 = new RowDefinition();
            rowdef6.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef6);




            ColumnDefinition coldef1 = new ColumnDefinition();
            coldef1.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef1);
            ColumnDefinition coldef2 = new ColumnDefinition();
            coldef2.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef2);
            ColumnDefinition coldef3 = new ColumnDefinition();
            coldef3.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef3);
            ColumnDefinition coldef4 = new ColumnDefinition();
            coldef4.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef4);
            ColumnDefinition coldef5 = new ColumnDefinition();
            coldef5.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef5);
            
            
            
            Random random = new Random();
            
            foreach (FrameworkElement element in blankTileCollection)
            {

                Grid.SetColumn(element, blankTileCollection.IndexOf(element) % 6);
                Grid.SetRow(element, blankTileCollection.IndexOf(element) / 6);
            }

            Grid.SetColumn(mainstatus, 1);
            Grid.SetRow(mainstatus, 1);



            Grid.SetColumn(statusCollection.ElementAt(0), 1);
            Grid.SetRow(statusCollection.ElementAt(0), 3);
            Grid.SetColumn(statusCollection.ElementAt(1), 2);
            Grid.SetRow(statusCollection.ElementAt(1), 3);
            Grid.SetColumn(statusCollection.ElementAt(2), 3);
            Grid.SetRow(statusCollection.ElementAt(2), 3);
            Grid.SetColumn(statusCollection.ElementAt(3), 3);
            Grid.SetRow(statusCollection.ElementAt(3), 2);
            Grid.SetColumn(statusCollection.ElementAt(4), 3);
            Grid.SetRow(statusCollection.ElementAt(4), 1);

            
            
            foreach (FrameworkElement element in imageCollection)
            {

                Grid.SetColumn(element, random.Next(1, 3));
                Grid.SetRow(element, 4);
            }
            
            //zmeny na datapanelu

            Grid.SetColumn(DataPanel, 0);
            Grid.SetColumnSpan(DataPanel, 5);
            Grid.SetRow(DataPanel, 1);
            Grid.SetRowSpan(DataPanel, 5);

            DataPanel.RowDefinitions.Clear();
            DataPanel.ColumnDefinitions.Clear();

            ColumnDefinition rowdef12 = new ColumnDefinition();
            rowdef12.Width = new GridLength(1, GridUnitType.Star);
            DataPanel.ColumnDefinitions.Add(rowdef12);
            ColumnDefinition rowdef13 = new ColumnDefinition();
            rowdef13.Width = new GridLength(1, GridUnitType.Star);
            DataPanel.ColumnDefinitions.Add(rowdef13);
            ColumnDefinition rowdef14 = new ColumnDefinition();
            rowdef14.Width = new GridLength(3, GridUnitType.Star);
            DataPanel.ColumnDefinitions.Add(rowdef14);

            RowDefinition coldef11 = new RowDefinition();
            coldef11.Height = new GridLength(1, GridUnitType.Star);
            DataPanel.RowDefinitions.Add(coldef11);
            RowDefinition coldef12 = new RowDefinition();
            coldef12.Height = new GridLength(4, GridUnitType.Star);
            DataPanel.RowDefinitions.Add(coldef12);



            Grid.SetColumn(dataPanelScrl, 0);
            Grid.SetColumnSpan(dataPanelScrl, 3);
            Grid.SetRow(dataPanelScrl, 1);
            Grid.SetRowSpan(dataPanelScrl, 1);

            dataPanelStck.Orientation = Orientation.Horizontal;


            //menupanel zemny

            MenuPanel.ColumnDefinitions.Clear();
            MenuPanel.RowDefinitions.Clear();

            RowDefinition rowdef32 = new RowDefinition();
            rowdef32.Height = new GridLength(4, GridUnitType.Star);
            MenuPanel.RowDefinitions.Add(rowdef32);
            RowDefinition rowdef33 = new RowDefinition();
            rowdef33.Height = new GridLength(1, GridUnitType.Star);
            MenuPanel.RowDefinitions.Add(rowdef33);


            ColumnDefinition coldef31 = new ColumnDefinition();
            coldef31.Width = new GridLength(3, GridUnitType.Star);
            MenuPanel.ColumnDefinitions.Add(coldef31);
            ColumnDefinition coldef32 = new ColumnDefinition();
            coldef32.Width = new GridLength(1, GridUnitType.Star);
            MenuPanel.ColumnDefinitions.Add(coldef32);
            ColumnDefinition coldef33 = new ColumnDefinition();
            coldef33.Width = new GridLength(1, GridUnitType.Star);
            MenuPanel.ColumnDefinitions.Add(coldef33);

            Grid.SetColumn(MenuPanel, 0);
            Grid.SetColumnSpan(MenuPanel, 5);
            Grid.SetRow(MenuPanel, 0);
            Grid.SetRowSpan(MenuPanel, 5);

            MenuPanelSubGrid.ColumnDefinitions.Clear();
            MenuPanelSubGrid.RowDefinitions.Clear();

            ColumnDefinition coldef42 = new ColumnDefinition();
            coldef42.Width = new GridLength(3, GridUnitType.Star);
            MenuPanelSubGrid.ColumnDefinitions.Add(coldef42);
            ColumnDefinition coldef43 = new ColumnDefinition();
            coldef43.Width = new GridLength(2, GridUnitType.Star);
            MenuPanelSubGrid.ColumnDefinitions.Add(coldef43);

            Grid.SetColumn(MenuPanelSubGrid, 0);
            Grid.SetColumnSpan(MenuPanelSubGrid, 3);
            Grid.SetRow(MenuPanelSubGrid, 0);
            Grid.SetRowSpan(MenuPanelSubGrid, 1);

            menuPanelButtonGrid.ColumnDefinitions.Clear();
            menuPanelButtonGrid.RowDefinitions.Clear();

            ColumnDefinition rowdef52 = new ColumnDefinition();
            rowdef52.Width = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef52);
            ColumnDefinition rowdef53 = new ColumnDefinition();
            rowdef53.Width = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef53);
            ColumnDefinition rowdef54 = new ColumnDefinition();
            rowdef54.Width = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef54);
            ColumnDefinition rowdef58 = new ColumnDefinition();
            rowdef58.Width = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef58);

            RowDefinition coldef50 = new RowDefinition();
            coldef50.Height = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef50);
            RowDefinition coldef51 = new RowDefinition();
            coldef51.Height = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef51);
            RowDefinition coldef52 = new RowDefinition();
            coldef52.Height = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef52);
            RowDefinition coldef53 = new RowDefinition();
            coldef53.Height = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef53);
            RowDefinition coldef54 = new RowDefinition();
            coldef54.Height = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef54);

            Grid.SetColumn(menuPanelButtonGrid, 0);
            Grid.SetColumnSpan(menuPanelButtonGrid, 1);
            Grid.SetRow(menuPanelButtonGrid, 0);
            Grid.SetRowSpan(menuPanelButtonGrid, 1);

            Grid.SetColumn(menuPanelButton1, 1);
            Grid.SetRow(menuPanelButton1, 1);
            Grid.SetColumn(menuPanelButton2, 2);
            Grid.SetRow(menuPanelButton2, 1);
            Grid.SetColumn(menuPanelButton3, 1);
            Grid.SetRow(menuPanelButton3, 2);
            Grid.SetColumn(menuPanelButton4, 2);
            Grid.SetRow(menuPanelButton4, 2);
            Grid.SetColumn(menuPanelButton5, 1);
            Grid.SetRow(menuPanelButton5, 3);

            Grid.SetColumn(menuPanelInfoGrid, 1);
            Grid.SetRow(menuPanelInfoGrid, 0);
            statusCOTxt.Text = Data.getString_CO();
            statusSOTxt.Text = Data.getString_SO2();
            statusPOTxt.Text = Data.getString_PM10();
            statusOTxt.Text = Data.getString_O3();
            statusNOTxt.Text = Data.getString_NO2();
            

            //POSUNUTI PANELU
            MapPanel.Margin = new Thickness(-((Window.Current.Bounds.Width / 5) * 8), 0, ((Window.Current.Bounds.Width / 5) * 8), 0);
            DataPanel.Margin = new Thickness(0, ((Window.Current.Bounds.Height / 6) * 4), 0, -((Window.Current.Bounds.Height / 6) * 4));
            MenuPanel.Margin = new Thickness(0, -((Window.Current.Bounds.Height / 6) * 4), 0, ((Window.Current.Bounds.Height / 6) * 4));

            //UMISTENI BUTTONU
            Grid.SetColumn(menubutton, 1);
            Grid.SetRow(menubutton, 1);
            Grid.SetColumn(databutton, 1);
            Grid.SetRow(databutton, 0);
            Grid.SetColumn(mapbutton, 0);
            Grid.SetRow(mapbutton, 0);


            //ZAROVNANI ELEMENTU DATAPANELU
            statuses.Height = dataPanelStck.Height;
            statuses.Width = dataPanelStck.Height;
            bars.Height = dataPanelStck.Height;
            bars.Width = dataPanelStck.Height;
            legend.Height = dataPanelStck.Height;
            legend.Width = dataPanelStck.Height;

            //MALE STATUSY a obrazky
            setImagesLandscape();

            //animace pro landscape
            setAnimationsLandscape();

            //infopanel
            Grid.SetRow(infoTile, 5);
            Grid.SetColumn(infoTile, 4);
             
        }
        private void setNarrowChanges() 
        {
            //docasne zmeny
            MapPanel.Visibility = Visibility.Collapsed;
            //layoutroot zmeny 

            LayoutRoot.ColumnDefinitions.Clear();
            LayoutRoot.RowDefinitions.Clear();


            ColumnDefinition rowdef21 = new ColumnDefinition();
            rowdef21.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef21);
            ColumnDefinition rowdef22 = new ColumnDefinition();
            rowdef22.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef22);
            ColumnDefinition rowdef23 = new ColumnDefinition();
            rowdef23.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef23);
            ColumnDefinition rowdef24 = new ColumnDefinition();
            rowdef24.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef24);
            ColumnDefinition rowdef25 = new ColumnDefinition();
            rowdef25.Width = new GridLength(1, GridUnitType.Star);
            LayoutRoot.ColumnDefinitions.Add(rowdef25);





            RowDefinition coldef21 = new RowDefinition();
            coldef21.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef21);
            RowDefinition coldef22 = new RowDefinition();
            coldef22.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef22);
            RowDefinition coldef23 = new RowDefinition();
            coldef23.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef23);
            RowDefinition coldef24 = new RowDefinition();
            coldef24.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef24);
            RowDefinition coldef25 = new RowDefinition();
            coldef25.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef25);
            RowDefinition coldef26 = new RowDefinition();
            coldef26.Height = new GridLength(1, GridUnitType.Star);
            LayoutRoot.RowDefinitions.Add(coldef26);

            //layoutgrid zmeny

            LayoutGrid.ColumnDefinitions.Clear();
            LayoutGrid.RowDefinitions.Clear();


            Grid.SetRowSpan(LayoutGrid, 6);
            Grid.SetColumnSpan(LayoutGrid, 6);

            RowDefinition rowdef1 = new RowDefinition();
            rowdef1.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef1);
            RowDefinition rowdef2 = new RowDefinition();
            rowdef2.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef2);
            RowDefinition rowdef3 = new RowDefinition();
            rowdef3.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef3);
            RowDefinition rowdef4 = new RowDefinition();
            rowdef4.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef4);
            RowDefinition rowdef5 = new RowDefinition();
            rowdef5.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef5);
            RowDefinition rowdef6 = new RowDefinition();
            rowdef6.Height = new GridLength(1, GridUnitType.Star);
            LayoutGrid.RowDefinitions.Add(rowdef6);




            ColumnDefinition coldef1 = new ColumnDefinition();
            coldef1.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef1);
            ColumnDefinition coldef2 = new ColumnDefinition();
            coldef2.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef2);
            ColumnDefinition coldef3 = new ColumnDefinition();
            coldef3.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef3);
            ColumnDefinition coldef4 = new ColumnDefinition();
            coldef4.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef4);
            ColumnDefinition coldef5 = new ColumnDefinition();
            coldef5.Width = new GridLength(1, GridUnitType.Star);
            LayoutGrid.ColumnDefinitions.Add(coldef5);



            Random random = new Random();

            foreach (FrameworkElement element in blankTileCollection)
            {

                Grid.SetColumn(element, blankTileCollection.IndexOf(element) % 6);
                Grid.SetRow(element, blankTileCollection.IndexOf(element) / 6);
            }

            Grid.SetColumn(mainstatus, 1);
            Grid.SetRow(mainstatus, 1);



            Grid.SetColumn(statusCollection.ElementAt(0), 1);
            Grid.SetRow(statusCollection.ElementAt(0), 3);
            Grid.SetColumn(statusCollection.ElementAt(1), 2);
            Grid.SetRow(statusCollection.ElementAt(1), 3);
            Grid.SetColumn(statusCollection.ElementAt(2), 3);
            Grid.SetRow(statusCollection.ElementAt(2), 3);
            Grid.SetColumn(statusCollection.ElementAt(3), 3);
            Grid.SetRow(statusCollection.ElementAt(3), 2);
            Grid.SetColumn(statusCollection.ElementAt(4), 3);
            Grid.SetRow(statusCollection.ElementAt(4), 1);



            foreach (FrameworkElement element in imageCollection)
            {

                Grid.SetColumn(element, random.Next(1, 3));
                Grid.SetRow(element, 4);
            }

            //zmeny na datapanelu

            Grid.SetColumn(DataPanel, 0);
            Grid.SetColumnSpan(DataPanel, 5);
            Grid.SetRow(DataPanel, 1);
            Grid.SetRowSpan(DataPanel, 5);

            DataPanel.RowDefinitions.Clear();
            DataPanel.ColumnDefinitions.Clear();

            ColumnDefinition rowdef12 = new ColumnDefinition();
            rowdef12.Width = new GridLength(1, GridUnitType.Star);
            DataPanel.ColumnDefinitions.Add(rowdef12);
            ColumnDefinition rowdef13 = new ColumnDefinition();
            rowdef13.Width = new GridLength(1, GridUnitType.Star);
            DataPanel.ColumnDefinitions.Add(rowdef13);
            ColumnDefinition rowdef14 = new ColumnDefinition();
            rowdef14.Width = new GridLength(3, GridUnitType.Star);
            DataPanel.ColumnDefinitions.Add(rowdef14);

            RowDefinition coldef11 = new RowDefinition();
            coldef11.Height = new GridLength(1, GridUnitType.Star);
            DataPanel.RowDefinitions.Add(coldef11);
            RowDefinition coldef12 = new RowDefinition();
            coldef12.Height = new GridLength(4, GridUnitType.Star);
            DataPanel.RowDefinitions.Add(coldef12);



            Grid.SetColumn(dataPanelScrl, 0);
            Grid.SetColumnSpan(dataPanelScrl, 3);
            Grid.SetRow(dataPanelScrl, 1);
            Grid.SetRowSpan(dataPanelScrl, 1);

            dataPanelStck.Orientation = Orientation.Horizontal;


            //menupanel zemny

            MenuPanel.ColumnDefinitions.Clear();
            MenuPanel.RowDefinitions.Clear();

            RowDefinition rowdef32 = new RowDefinition();
            rowdef32.Height = new GridLength(4, GridUnitType.Star);
            MenuPanel.RowDefinitions.Add(rowdef32);
            RowDefinition rowdef33 = new RowDefinition();
            rowdef33.Height = new GridLength(1, GridUnitType.Star);
            MenuPanel.RowDefinitions.Add(rowdef33);


            ColumnDefinition coldef31 = new ColumnDefinition();
            coldef31.Width = new GridLength(3, GridUnitType.Star);
            MenuPanel.ColumnDefinitions.Add(coldef31);
            ColumnDefinition coldef32 = new ColumnDefinition();
            coldef32.Width = new GridLength(1, GridUnitType.Star);
            MenuPanel.ColumnDefinitions.Add(coldef32);
            ColumnDefinition coldef33 = new ColumnDefinition();
            coldef33.Width = new GridLength(1, GridUnitType.Star);
            MenuPanel.ColumnDefinitions.Add(coldef33);

            Grid.SetColumn(MenuPanel, 0);
            Grid.SetColumnSpan(MenuPanel, 5);
            Grid.SetRow(MenuPanel, 0);
            Grid.SetRowSpan(MenuPanel, 5);

            MenuPanelSubGrid.ColumnDefinitions.Clear();
            MenuPanelSubGrid.RowDefinitions.Clear();

            ColumnDefinition coldef42 = new ColumnDefinition();
            coldef42.Width = new GridLength(3, GridUnitType.Star);
            MenuPanelSubGrid.ColumnDefinitions.Add(coldef42);
            ColumnDefinition coldef43 = new ColumnDefinition();
            coldef43.Width = new GridLength(2, GridUnitType.Star);
            MenuPanelSubGrid.ColumnDefinitions.Add(coldef43);

            Grid.SetColumn(MenuPanelSubGrid, 0);
            Grid.SetColumnSpan(MenuPanelSubGrid, 3);
            Grid.SetRow(MenuPanelSubGrid, 0);
            Grid.SetRowSpan(MenuPanelSubGrid, 1);

            menuPanelButtonGrid.ColumnDefinitions.Clear();
            menuPanelButtonGrid.RowDefinitions.Clear();

            ColumnDefinition rowdef52 = new ColumnDefinition();
            rowdef52.Width = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef52);
            ColumnDefinition rowdef53 = new ColumnDefinition();
            rowdef53.Width = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef53);
            ColumnDefinition rowdef54 = new ColumnDefinition();
            rowdef54.Width = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef54);
            ColumnDefinition rowdef58 = new ColumnDefinition();
            rowdef58.Width = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(rowdef58);

            RowDefinition coldef50 = new RowDefinition();
            coldef50.Height = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef50);
            RowDefinition coldef51 = new RowDefinition();
            coldef51.Height = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef51);
            RowDefinition coldef52 = new RowDefinition();
            coldef52.Height = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef52);
            RowDefinition coldef53 = new RowDefinition();
            coldef53.Height = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef53);
            RowDefinition coldef54 = new RowDefinition();
            coldef54.Height = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.RowDefinitions.Add(coldef54);

            Grid.SetColumn(menuPanelButtonGrid, 0);
            Grid.SetColumnSpan(menuPanelButtonGrid, 1);
            Grid.SetRow(menuPanelButtonGrid, 0);
            Grid.SetRowSpan(menuPanelButtonGrid, 1);

            Grid.SetColumn(menuPanelButton1, 1);
            Grid.SetRow(menuPanelButton1, 1);
            Grid.SetColumn(menuPanelButton2, 2);
            Grid.SetRow(menuPanelButton2, 1);
            Grid.SetColumn(menuPanelButton3, 1);
            Grid.SetRow(menuPanelButton3, 2);
            Grid.SetColumn(menuPanelButton4, 2);
            Grid.SetRow(menuPanelButton4, 2);
            Grid.SetColumn(menuPanelButton5, 1);
            Grid.SetRow(menuPanelButton5, 3);

            Grid.SetColumn(menuPanelInfoGrid, 1);
            Grid.SetRow(menuPanelInfoGrid, 0);
            statusCOTxt.Text = Data.getString_CO();
            statusSOTxt.Text = Data.getString_SO2();
            statusPOTxt.Text = Data.getString_PM10();
            statusOTxt.Text = Data.getString_O3();
            statusNOTxt.Text = Data.getString_NO2();
            

            //POSUNUTI PANELU
            MapPanel.Margin = new Thickness(-((Window.Current.Bounds.Width / 5) * 8), 0, ((Window.Current.Bounds.Width / 5) * 8), 0);
            DataPanel.Margin = new Thickness(0, ((Window.Current.Bounds.Height / 6) * 4), 0, -((Window.Current.Bounds.Height / 6) * 4));
            MenuPanel.Margin = new Thickness(0, -((Window.Current.Bounds.Height / 6) * 4), 0, ((Window.Current.Bounds.Height / 6) * 4));

            //UMISTENI BUTTONU
            Grid.SetColumn(menubutton, 1);
            Grid.SetRow(menubutton, 1);
            Grid.SetColumn(databutton, 1);
            Grid.SetRow(databutton, 0);
            Grid.SetColumn(mapbutton, 0);
            Grid.SetRow(mapbutton, 0);


            //ZAROVNANI ELEMENTU DATAPANELU
            statuses.Height = dataPanelStck.Height;
            statuses.Width = dataPanelStck.Height;
            bars.Height = dataPanelStck.Height;
            bars.Width = dataPanelStck.Height;
            legend.Height = dataPanelStck.Height;
            legend.Width = dataPanelStck.Height;

            //MALE STATUSY a obrazky
            setImagesLandscape();

            //animace pro landscape
            setAnimationsLandscape();

            //infopanel
            Grid.SetRow(infoTile, 5);
            Grid.SetColumn(infoTile, 4);
        }
        private void setFilledChanges() { }
        #endregion
        #region PANEL SERVICES
        /// <summary>
        /// Funkce obsluhujici map panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void mapPanelService(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            
            if (currentState == "FullScreenLandscape")
            {
                //MapPanel.Margin = new Thickness(-((Window.Current.Bounds.Width / 11) * 9), 0, ((Window.Current.Bounds.Width / 11) * 9), 0);
                if (((Storyboard)Resources["menuPanelShowSB"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["menuPanelShowSB"]).Stop();
                    ((Storyboard)Resources["menuPanelHideSB"]).Begin();
                }
                if (((Storyboard)Resources["dataPanelShowSB"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["dataPanelShowSB"]).Stop();
                    ((Storyboard)Resources["dataPanelHideSB"]).Begin();
                }


                if ((((Storyboard)Resources["mapPanelShowSB"]).GetCurrentState() == ClockState.Stopped) && (((Storyboard)Resources["mapPanelHideSB"]).GetCurrentState() == ClockState.Stopped))
                {

                    ((Storyboard)Resources["mapPanelShowSB"]).Begin();
                }
                else if (((Storyboard)Resources["mapPanelShowSB"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["mapPanelShowSB"]).Stop();
                    ((Storyboard)Resources["mapPanelHideSB"]).Begin();
                }
                else
                {
                    ((Storyboard)Resources["mapPanelHideSB"]).Stop();
                    ((Storyboard)Resources["mapPanelShowSB"]).Begin();
                }


            }
            else//portrait
            {
                MapPanel.Margin = new Thickness(0, -((Window.Current.Bounds.Height / 11) * 9), 0, ((Window.Current.Bounds.Height / 11) * 9));
                if (((Storyboard)Resources["menuPanelShowSBP"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["menuPanelShowSBP"]).Stop();
                    ((Storyboard)Resources["menuPanelHideSBP"]).Begin();
                }


                if ((((Storyboard)Resources["dataPanelShowSBP"]).GetCurrentState() == ClockState.Stopped) && (((Storyboard)Resources["dataPanelHideSBP"]).GetCurrentState() == ClockState.Stopped))
                {

                    ((Storyboard)Resources["dataPanelShowSBP"]).Begin();
                }
                else if (((Storyboard)Resources["dataPanelShowSBP"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["dataPanelShowSBP"]).Stop();
                    ((Storyboard)Resources["dataPanelHideSBP"]).Begin();
                }
                else
                {
                    ((Storyboard)Resources["dataPanelHideSBP"]).Stop();
                    ((Storyboard)Resources["dataPanelShowSBP"]).Begin();
                }



            }
            
        }
        /// <summary>
        /// Funkce obsluhujici data panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void dataPanelService(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {         
            if (currentState == "FullScreenLandscape") 
            {
                if (((Storyboard)Resources["mapPanelShowSB"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["mapPanelShowSB"]).Stop();
                    ((Storyboard)Resources["mapPanelHideSB"]).Begin();
                    
                } 
                if (((Storyboard)Resources["menuPanelShowSB"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["menuPanelShowSB"]).Stop();
                    ((Storyboard)Resources["menuPanelHideSB"]).Begin();
                }
                

                if ((((Storyboard)Resources["dataPanelShowSB"]).GetCurrentState() == ClockState.Stopped) && (((Storyboard)Resources["dataPanelHideSB"]).GetCurrentState() == ClockState.Stopped))
                {

                    ((Storyboard)Resources["dataPanelShowSB"]).Begin();
                    ((Storyboard)Resources["mapPanelShowButtonSB"]).Stop();
                    ((Storyboard)Resources["mapPanelHideButtonSB"]).Begin();
                }
                else if (((Storyboard)Resources["dataPanelShowSB"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["dataPanelShowSB"]).Stop();
                    ((Storyboard)Resources["dataPanelHideSB"]).Begin();
                    ((Storyboard)Resources["mapPanelHideButtonSB"]).Stop();
                    ((Storyboard)Resources["mapPanelShowButtonSB"]).Begin();
                }
                else
                {
                    ((Storyboard)Resources["dataPanelHideSB"]).Stop();
                    ((Storyboard)Resources["dataPanelShowSB"]).Begin();

                    ((Storyboard)Resources["mapPanelShowButtonSB"]).Stop();
                    ((Storyboard)Resources["mapPanelHideButtonSB"]).Begin();
                }


            }else
            {
                MapPanel.Margin = new Thickness(0, -((Window.Current.Bounds.Height / 11) * 8), 0, ((Window.Current.Bounds.Height / 11) * 8));
                if (((Storyboard)Resources["menuPanelShowSBP"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["menuPanelShowSBP"]).Stop();
                    ((Storyboard)Resources["menuPanelHideSBP"]).Begin();
                }


                if ((((Storyboard)Resources["dataPanelShowSBP"]).GetCurrentState() == ClockState.Stopped) && (((Storyboard)Resources["dataPanelHideSBP"]).GetCurrentState() == ClockState.Stopped)) 
                {

                    ((Storyboard)Resources["dataPanelShowSBP"]).Begin();
                }else if (((Storyboard)Resources["dataPanelShowSBP"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["dataPanelShowSBP"]).Stop();
                    ((Storyboard)Resources["dataPanelHideSBP"]).Begin();
                }
                else 
                {
                    ((Storyboard)Resources["dataPanelHideSBP"]).Stop();
                    ((Storyboard)Resources["dataPanelShowSBP"]).Begin();
                }



            }
            
        }
        /// <summary>
        /// Funkce obsluhujici menu panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void menuPanelService(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            

            if (currentState == "FullScreenLandscape")
            {
                if (((Storyboard)Resources["mapPanelShowSB"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["mapPanelShowSB"]).Stop();
                    ((Storyboard)Resources["mapPanelHideSB"]).Begin();
                } 
                if (((Storyboard)Resources["dataPanelShowSB"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["dataPanelShowSB"]).Stop();
                    ((Storyboard)Resources["dataPanelHideSB"]).Begin();
                }


                if ((((Storyboard)Resources["menuPanelShowSB"]).GetCurrentState() == ClockState.Stopped) && (((Storyboard)Resources["menuPanelHideSB"]).GetCurrentState() == ClockState.Stopped))
                {

                    ((Storyboard)Resources["menuPanelShowSB"]).Begin();
                    ((Storyboard)Resources["mapPanelShowButtonSB"]).Stop();
                    ((Storyboard)Resources["mapPanelHideButtonSB"]).Begin();
                }
                else if (((Storyboard)Resources["menuPanelShowSB"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["menuPanelShowSB"]).Stop();
                    ((Storyboard)Resources["menuPanelHideSB"]).Begin();
                    ((Storyboard)Resources["mapPanelHideButtonSB"]).Stop();
                    ((Storyboard)Resources["mapPanelShowButtonSB"]).Begin();
                }
                else
                {
                    ((Storyboard)Resources["menuPanelHideSB"]).Stop();
                    ((Storyboard)Resources["menuPanelShowSB"]).Begin();
                    ((Storyboard)Resources["mapPanelShowButtonSB"]).Stop();
                    ((Storyboard)Resources["mapPanelHideButtonSB"]).Begin();
                }
            }
            else 
            {
                //MapPanel.Margin = new Thickness(0, -((Window.Current.Bounds.Height / 11) * 4), 0, ((Window.Current.Bounds.Height / 11) * 4));
                //((Storyboard)Resources["dataPanelHideSBP"]).Begin();
                if (((Storyboard)Resources["dataPanelShowSBP"]).GetCurrentState() == ClockState.Filling) 
                {
                    ((Storyboard)Resources["dataPanelShowSBP"]).Stop();
                    ((Storyboard)Resources["dataPanelHideSBP"]).Begin();
                }


                if ((((Storyboard)Resources["menuPanelShowSBP"]).GetCurrentState() == ClockState.Stopped) && (((Storyboard)Resources["menuPanelHideSBP"]).GetCurrentState() == ClockState.Stopped))
                {

                    ((Storyboard)Resources["menuPanelShowSBP"]).Begin();
                }
                else if (((Storyboard)Resources["menuPanelShowSBP"]).GetCurrentState() == ClockState.Filling)
                {
                    ((Storyboard)Resources["menuPanelShowSBP"]).Stop();
                    ((Storyboard)Resources["menuPanelHideSBP"]).Begin();
                }
                else
                {
                    ((Storyboard)Resources["menuPanelHideSBP"]).Stop();
                    ((Storyboard)Resources["menuPanelShowSBP"]).Begin();
                }
            }
            
        }
        #endregion
        /// <summary>
        /// Funkce nastavujici velikost pisma dle velikosti obrazovky.
        /// </summary>
        private void setFontSizes() 
        {
            stationName.FontSize = Data.getFontSize_Title();
            stationRegion.FontSize = Data.getFontSize_LargeText();
            statusSOTxt.FontSize = Data.getFontSize_CommonText();
            statusCOTxt.FontSize = Data.getFontSize_CommonText();
            statusPOTxt.FontSize = Data.getFontSize_CommonText();
            statusOTxt.FontSize = Data.getFontSize_CommonText();
            statusNOTxt.FontSize = Data.getFontSize_CommonText();
            statusOValue.FontSize = Data.getFontSize_Title();
            statusSOValueTxt.FontSize = Data.getFontSize_Title();
            statusCOValue.FontSize = Data.getFontSize_Title();
            statusPOValue.FontSize = Data.getFontSize_Title();
            statusNOValue.FontSize = Data.getFontSize_Title();
            legendTxt1.FontSize = Data.getFontSize_SmallText();
            legendTxt2.FontSize = Data.getFontSize_SmallText();
            legendTxt3.FontSize = Data.getFontSize_SmallText();
            legendTxt4.FontSize = Data.getFontSize_SmallText();
            legendTxt5.FontSize = Data.getFontSize_SmallText();
            legendTxt6.FontSize = Data.getFontSize_SmallText();
            legendTxt7.FontSize = Data.getFontSize_SmallText();
            legendTxt8.FontSize = Data.getFontSize_SmallText();
            menuPanelButton1Txt.FontSize = Data.getFontSize_LargeText();
            menuPanelButton2Txt.FontSize = Data.getFontSize_LargeText();
            menuPanelButton3Txt.FontSize = Data.getFontSize_LargeText();
            menuPanelButton4Txt.FontSize = Data.getFontSize_LargeText();
            menuPanelButton5Txt.FontSize = Data.getFontSize_LargeText();
            menuPanelInfoTitleTxt.FontSize = Data.getFontSize_CommonText();
            menuPanelInfoValueTxt.FontSize = Data.getFontSize_CommonText();
            
        }
        #region STATUSES POSITION
        /// <summary>
        /// Funkce pro rozmisteni dlazdic statusu v horizontalnim filled zobrazeni.
        /// </summary>
        private void setStatusesLandscape()
        {
            Random random = new Random();

            List<Tuple<int, int>> mylist = new List<Tuple<int, int>>();
            Tuple<int, int> mytuple;

            for (int i = 0; i < 20; i++)
            {
                mytuple = Tuple.Create((i / 5) + 1, (i % 5) + 5);
                mylist.Add(mytuple);
            }

            int index;

            for (int i = 0; i < statusCollection.Count(); i++)
            {
                index = random.Next(0, 19 - i);
                Grid.SetRow(statusCollection.ElementAt(i), mylist.ElementAt(index).Item1);
                Grid.SetColumn(statusCollection.ElementAt(i), mylist.ElementAt(index).Item2);
                mylist.RemoveAt(index);
            }



        }
        /// <summary>
        /// Funkce pro rozmisteni dlazdic statusu ve vertikalnim zobrazeni.
        /// </summary>
        private void setStatusesPortrait()
        {
            Random random = new Random();

            //vytvor seznam tuplu
            List<Tuple<int, int>> mylist = new List<Tuple<int, int>>();
            Tuple<int, int> mytuple;

            //projdi dvacetkrat a v kazdem cyklu vloz jeden tuple
            for (int i = 0; i < 20; i++)
            {
                //do tuplu vloz souradnice 5,1 6,1 7,1 8,1 9,1 5,2 ....
                mytuple = Tuple.Create((i % 5) + 5, (i / 5) + 1);
                //vloz souradnice do seznamu
                mylist.Add(mytuple);
            }

            int index;//vytvor index



            //projdi seznam
            for (int i = 0; i < statusCollection.Count(); i++)
            {
                //vloz do indexu nahodne cislo od 0 do 19 minus pocet probehlych cyklu
                index = random.Next(0, 19 - i);
                //teststring += mylist.ElementAt(index).Item1.ToString() + mylist.ElementAt(index).Item2.ToString();
                //nastav row a col
                Grid.SetRow(statusCollection.ElementAt(i), mylist.ElementAt(index).Item1);
                Grid.SetColumn(statusCollection.ElementAt(i), mylist.ElementAt(index).Item2);
                //vyjmi z listu element na indexu
                mylist.RemoveAt(index);
            }


        }
        private void setStatusesSnapped() 
        {
            Random random = new Random();

            //vytvor seznam tuplu
            List<Tuple<int, int>> mylist = new List<Tuple<int, int>>();
            Tuple<int, int> mytuple;

            //projdi dvacetkrat a v kazdem cyklu vloz jeden tuple
            for (int i = 0; i < 20; i++)
            {
                //do tuplu vloz souradnice 5,1 6,1 7,1 8,1 9,1 5,2 ....
                mytuple = Tuple.Create((i % 5) + 5, (i / 5) + 1);
                //vloz souradnice do seznamu
                mylist.Add(mytuple);
            }

            int index;//vytvor index



            //projdi seznam
            for (int i = 0; i < statusCollection.Count(); i++)
            {
                //vloz do indexu nahodne cislo od 0 do 19 minus pocet probehlych cyklu
                index = random.Next(0, 19 - i);
                //teststring += mylist.ElementAt(index).Item1.ToString() + mylist.ElementAt(index).Item2.ToString();
                //nastav row a col
                Grid.SetRow(statusCollection.ElementAt(i), mylist.ElementAt(index).Item1);
                Grid.SetColumn(statusCollection.ElementAt(i), mylist.ElementAt(index).Item2);
                //vyjmi z listu element na indexu
                mylist.RemoveAt(index);
            }


        }
        private void setStatusesNarrow() 
        {
            Random random = new Random();

            //vytvor seznam tuplu
            List<Tuple<int, int>> mylist = new List<Tuple<int, int>>();
            Tuple<int, int> mytuple;

            //projdi dvacetkrat a v kazdem cyklu vloz jeden tuple
            for (int i = 0; i < 20; i++)
            {
                //do tuplu vloz souradnice 5,1 6,1 7,1 8,1 9,1 5,2 ....
                mytuple = Tuple.Create((i % 5) + 5, (i / 5) + 1);
                //vloz souradnice do seznamu
                mylist.Add(mytuple);
            }

            int index;//vytvor index



            //projdi seznam
            for (int i = 0; i < statusCollection.Count(); i++)
            {
                //vloz do indexu nahodne cislo od 0 do 19 minus pocet probehlych cyklu
                index = random.Next(0, 19 - i);
                //teststring += mylist.ElementAt(index).Item1.ToString() + mylist.ElementAt(index).Item2.ToString();
                //nastav row a col
                Grid.SetRow(statusCollection.ElementAt(i), mylist.ElementAt(index).Item1);
                Grid.SetColumn(statusCollection.ElementAt(i), mylist.ElementAt(index).Item2);
                //vyjmi z listu element na indexu
                mylist.RemoveAt(index);
            }


        }
        private void setStatusesFilled() { }
        #endregion
        #region IMAGES POSITION
        /// <summary>
        /// Funkce pro rozmisteni obrazku v horizontalnim filled zobrazeni.
        /// </summary>
        private void setImagesLandscape()
        {
            Random random = new Random();

            List<Tuple<int, int>> mylist = new List<Tuple<int, int>>();
            Tuple<int, int> mytuple;


            for (int i = 0; i < 37; i++)
            {
                mytuple = Tuple.Create((i / 9) + 1, (i % 9)+1);
                mylist.Add(mytuple);
            }
            
            mytuple = Tuple.Create(Grid.GetRow(mainstatus), Grid.GetColumn(mainstatus));
            for (int i = 0; i < mylist.Count(); i++)
            {
                if ((mylist.ElementAt(i).Equals(mytuple)) || (mylist.ElementAt(i).Equals(Tuple.Create(mytuple.Item1 + 1, mytuple.Item2))) || (mylist.ElementAt(i).Equals( Tuple.Create(mytuple.Item1, mytuple.Item2 + 1))) || (mylist.ElementAt(i).Equals( Tuple.Create(mytuple.Item1 + 1, mytuple.Item2 + 1))))
                {
                    mylist.RemoveAt(i);
                }
            }
            
            for (int i = 0; i < mylist.Count(); i++) 
            {
                for (int j = 0; j < statusCollection.Count(); j++)
                {
                    mytuple = Tuple.Create(Grid.GetRow(statusCollection.ElementAt(j)), Grid.GetColumn(statusCollection.ElementAt(j)));
                    if (mylist.ElementAt(i).Equals( mytuple))
                    {
                        mylist.Remove(mytuple);
                    }
                }
            }


            int index;

            for (int i = 0; i < imageCollection.Count(); i++)
            {
                index = random.Next(0, mylist.Count() - i - 1);
                Grid.SetRow(imageCollection.ElementAt(i), mylist.ElementAt(index).Item1);
                Grid.SetColumn(imageCollection.ElementAt(i), mylist.ElementAt(index).Item2);
                mylist.RemoveAt(index);
            }



        }
        /// <summary>
        /// Funkce pro rozmisteni obrazku ve vertikalnim zobrazeni.
        /// </summary>
        private void setImagesPortrait()
        {
            Random random = new Random();

            List<Tuple<int, int>> mylist = new List<Tuple<int, int>>();
            Tuple<int, int> mytuple;


            for (int i = 0; i < 37; i++)
            {
                mytuple = Tuple.Create((i % 9) + 1, (i / 9) + 1);
                mylist.Add(mytuple);
            }

            mytuple = Tuple.Create(Grid.GetRow(mainstatus), Grid.GetColumn(mainstatus));
            for (int i = 0; i < mylist.Count(); i++)
            {
                if ((mylist.ElementAt(i).Equals(mytuple)) || (mylist.ElementAt(i).Equals(Tuple.Create(mytuple.Item1 + 1, mytuple.Item2))) || (mylist.ElementAt(i).Equals(Tuple.Create(mytuple.Item1, mytuple.Item2 + 1))) || (mylist.ElementAt(i).Equals(Tuple.Create(mytuple.Item1 + 1, mytuple.Item2 + 1))))
                {
                    mylist.RemoveAt(i);
                }
            }

            for (int i = 0; i < mylist.Count(); i++)
            {
                for (int j = 0; j < statusCollection.Count(); j++)
                {
                    mytuple = Tuple.Create(Grid.GetRow(statusCollection.ElementAt(j)), Grid.GetColumn(statusCollection.ElementAt(j)));
                    if (mylist.ElementAt(i).Equals(mytuple))
                    {
                        mylist.Remove(mytuple);
                    }
                }
            }


            int index;

            for (int i = 0; i < imageCollection.Count(); i++)
            {
                index = random.Next(0, mylist.Count() - i - 1);
                Grid.SetRow(imageCollection.ElementAt(i), mylist.ElementAt(index).Item1);
                Grid.SetColumn(imageCollection.ElementAt(i), mylist.ElementAt(index).Item2);
                mylist.RemoveAt(index);
            }



        }
        private void setImagesSnapped() 
        {
            Random random = new Random();

            List<Tuple<int, int>> mylist = new List<Tuple<int, int>>();
            Tuple<int, int> mytuple;


            for (int i = 0; i < 37; i++)
            {
                mytuple = Tuple.Create(4, (i / 3) + 1);
                mylist.Add(mytuple);
            }

            mytuple = Tuple.Create(Grid.GetRow(mainstatus), Grid.GetColumn(mainstatus));
            for (int i = 0; i < mylist.Count(); i++)
            {
                if ((mylist.ElementAt(i).Equals(mytuple)) || (mylist.ElementAt(i).Equals(Tuple.Create(mytuple.Item1 + 1, mytuple.Item2))) || (mylist.ElementAt(i).Equals(Tuple.Create(mytuple.Item1, mytuple.Item2 + 1))) || (mylist.ElementAt(i).Equals(Tuple.Create(mytuple.Item1 + 1, mytuple.Item2 + 1))))
                {
                    mylist.RemoveAt(i);
                }
            }

            for (int i = 0; i < mylist.Count(); i++)
            {
                for (int j = 0; j < statusCollection.Count(); j++)
                {
                    mytuple = Tuple.Create(Grid.GetRow(statusCollection.ElementAt(j)), Grid.GetColumn(statusCollection.ElementAt(j)));
                    if (mylist.ElementAt(i).Equals(mytuple))
                    {
                        mylist.Remove(mytuple);
                    }
                }
            }


            int index;

            for (int i = 0; i < imageCollection.Count(); i++)
            {
                index = random.Next(0, mylist.Count() - i - 1);
                Grid.SetRow(imageCollection.ElementAt(i), mylist.ElementAt(index).Item1);
                Grid.SetColumn(imageCollection.ElementAt(i), mylist.ElementAt(index).Item2);
                mylist.RemoveAt(index);
            }
        }
        private void setImagesNarrow() 
        {
            Random random = new Random();

            List<Tuple<int, int>> mylist = new List<Tuple<int, int>>();
            Tuple<int, int> mytuple;


            for (int i = 0; i < 37; i++)
            {
                mytuple = Tuple.Create((i % 9) + 1, (i / 9) + 1);
                mylist.Add(mytuple);
            }

            mytuple = Tuple.Create(Grid.GetRow(mainstatus), Grid.GetColumn(mainstatus));
            for (int i = 0; i < mylist.Count(); i++)
            {
                if ((mylist.ElementAt(i).Equals(mytuple)) || (mylist.ElementAt(i).Equals(Tuple.Create(mytuple.Item1 + 1, mytuple.Item2))) || (mylist.ElementAt(i).Equals(Tuple.Create(mytuple.Item1, mytuple.Item2 + 1))) || (mylist.ElementAt(i).Equals(Tuple.Create(mytuple.Item1 + 1, mytuple.Item2 + 1))))
                {
                    mylist.RemoveAt(i);
                }
            }

            for (int i = 0; i < mylist.Count(); i++)
            {
                for (int j = 0; j < statusCollection.Count(); j++)
                {
                    mytuple = Tuple.Create(Grid.GetRow(statusCollection.ElementAt(j)), Grid.GetColumn(statusCollection.ElementAt(j)));
                    if (mylist.ElementAt(i).Equals(mytuple))
                    {
                        mylist.Remove(mytuple);
                    }
                }
            }


            int index;

            for (int i = 0; i < imageCollection.Count(); i++)
            {
                index = random.Next(0, mylist.Count() - i - 1);
                Grid.SetRow(imageCollection.ElementAt(i), mylist.ElementAt(index).Item1);
                Grid.SetColumn(imageCollection.ElementAt(i), mylist.ElementAt(index).Item2);
                mylist.RemoveAt(index);
            }
        }
        private void setImagesFilled() { }
        #endregion
        #region ANIMATIONS
        /// <summary>
        /// Nastaveni animaci pro horizontal filled.
        /// </summary>
        private void setAnimationsLandscape() 
        {
            Resources.Clear();

            //VYSUNUTI DATA PANELU
            dataPanelShowSB = new Storyboard();
            doubleAnim = new DoubleAnimation();

            DataPanel.RenderTransform = new TranslateTransform();

            doubleAnim.From = 0;
            doubleAnim.To = -(Window.Current.Bounds.Height / 6) * 4;
            doubleAnim.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            Storyboard.SetTarget(doubleAnim, DataPanel);
            Storyboard.SetTargetProperty(doubleAnim, "(FrameworkElement.RenderTransform).(TranslateTransform.Y)");

            dataPanelShowSB.Children.Add(doubleAnim);

            Resources.Add("dataPanelShowSB", dataPanelShowSB);

            //ZASUNUTI DATA PANELU

            dataPanelHideSB = new Storyboard();
            doubleAnim2 = new DoubleAnimation();

            DataPanel.RenderTransform = new TranslateTransform();

            doubleAnim2.From = -(Window.Current.Bounds.Height / 6) * 4; ;
            doubleAnim2.To = 0;
            doubleAnim2.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnim2.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            Storyboard.SetTarget(doubleAnim2, DataPanel);
            Storyboard.SetTargetProperty(doubleAnim2, "(FrameworkElement.RenderTransform).(TranslateTransform.Y)");

            dataPanelHideSB.Children.Add(doubleAnim2);
            
            Resources.Add("dataPanelHideSB", dataPanelHideSB);



            
            //VYSUNUTI MENU PANELU
            menuPanelShowSB = new Storyboard();
            doubleAnim3 = new DoubleAnimation();

            MenuPanel.RenderTransform = new TranslateTransform();

            doubleAnim3.From = 0;
            doubleAnim3.To = (Window.Current.Bounds.Height / 6) * 2;
            doubleAnim3.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnim3.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            Storyboard.SetTarget(doubleAnim3, MenuPanel);
            Storyboard.SetTargetProperty(doubleAnim3, "(FrameworkElement.RenderTransform).(TranslateTransform.Y)");

            menuPanelShowSB.Children.Add(doubleAnim3);

            Resources.Add("menuPanelShowSB", menuPanelShowSB);
            
            //ZASUNUTI MENU PANELU
            menuPanelHideSB = new Storyboard();
            doubleAnim4 = new DoubleAnimation();

            MenuPanel.RenderTransform = new TranslateTransform();

            doubleAnim4.From = (Window.Current.Bounds.Height / 6) * 2;
            doubleAnim4.To = 0;
            doubleAnim4.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnim4.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            Storyboard.SetTarget(doubleAnim4, MenuPanel);
            Storyboard.SetTargetProperty(doubleAnim4, "(FrameworkElement.RenderTransform).(TranslateTransform.Y)");

            menuPanelHideSB.Children.Add(doubleAnim4);

            Resources.Add("menuPanelHideSB", menuPanelHideSB);

            //VYSUNUTI MAP PANELU
            mapPanelShowSB = new Storyboard();
            doubleAnim = new DoubleAnimation();

            MapPanel.RenderTransform = new TranslateTransform();

            doubleAnim.From = 0;
            doubleAnim.To = (Window.Current.Bounds.Width / 11) * 10;
            doubleAnim.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            Storyboard.SetTarget(doubleAnim, MapPanel);
            Storyboard.SetTargetProperty(doubleAnim, "(FrameworkElement.RenderTransform).(TranslateTransform.X)");

            mapPanelShowSB.Children.Add(doubleAnim);

            Resources.Add("mapPanelShowSB", mapPanelShowSB);

            //ZASUNUTI MAP PANELU

            mapPanelHideSB = new Storyboard();
            doubleAnim2 = new DoubleAnimation();

            MapPanel.RenderTransform = new TranslateTransform();

            doubleAnim2.From = (Window.Current.Bounds.Width / 11) * 10; ;
            doubleAnim2.To = 0;
            doubleAnim2.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnim2.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            Storyboard.SetTarget(doubleAnim2, MapPanel);
            Storyboard.SetTargetProperty(doubleAnim2, "(FrameworkElement.RenderTransform).(TranslateTransform.X)");

            mapPanelHideSB.Children.Add(doubleAnim2);

            Resources.Add("mapPanelHideSB", mapPanelHideSB);
            
            //VYSUNUTI TLACITKA MAPPANELU
            mapPanelShowButtonSB = new Storyboard();
            doubleAnim = new DoubleAnimation();

            MapPanel.RenderTransform = new TranslateTransform();

            doubleAnim.From = -(Window.Current.Bounds.Width / 11) * 1;
            doubleAnim.To = 0;
            doubleAnim.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            Storyboard.SetTarget(doubleAnim, MapPanel);
            Storyboard.SetTargetProperty(doubleAnim, "(FrameworkElement.RenderTransform).(TranslateTransform.X)");

            mapPanelShowButtonSB.Children.Add(doubleAnim);

            Resources.Add("mapPanelShowButtonSB", mapPanelShowButtonSB);

            //ZASUNUTI TLACITKA MAPPANELU
            mapPanelHideButtonSB = new Storyboard();
            doubleAnim2 = new DoubleAnimation();

            MapPanel.RenderTransform = new TranslateTransform();

            doubleAnim2.From = 0;
            doubleAnim2.To = -(Window.Current.Bounds.Width / 11) * 1;
            doubleAnim2.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnim2.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            Storyboard.SetTarget(doubleAnim2, MapPanel);
            Storyboard.SetTargetProperty(doubleAnim2, "(FrameworkElement.RenderTransform).(TranslateTransform.X)");

            mapPanelHideButtonSB.Children.Add(doubleAnim2);

            Resources.Add("mapPanelHideButtonSB", mapPanelHideButtonSB);
        }
        /// <summary>
        /// Nastaveni animaci pro vertical filled.
        /// </summary>
        private void setAnimationsPortrait()
        {
            Resources.Clear();

            //VYSUNUTI DATA PANELU
            dataPanelShowSBP = new Storyboard();
            doubleAnim = new DoubleAnimation();

            DataPanel.RenderTransform = new TranslateTransform();

            doubleAnim.From = 0;
            doubleAnim.To = (Window.Current.Bounds.Width / 6) * 4;
            doubleAnim.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            Storyboard.SetTarget(doubleAnim, DataPanel);
            Storyboard.SetTargetProperty(doubleAnim, "(FrameworkElement.RenderTransform).(TranslateTransform.X)");

            dataPanelShowSBP.Children.Add(doubleAnim);

            Resources.Add("dataPanelShowSBP", dataPanelShowSBP);

            //ZASUNUTI DATA PANELU

            dataPanelHideSBP = new Storyboard();
            doubleAnim2 = new DoubleAnimation();

            DataPanel.RenderTransform = new TranslateTransform();

            doubleAnim2.From = (Window.Current.Bounds.Width / 6) * 4;
            doubleAnim2.To = 0;
            doubleAnim2.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnim2.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            Storyboard.SetTarget(doubleAnim2, DataPanel);
            Storyboard.SetTargetProperty(doubleAnim2, "(FrameworkElement.RenderTransform).(TranslateTransform.X)");

            dataPanelHideSBP.Children.Add(doubleAnim2);

            Resources.Add("dataPanelHideSBP", dataPanelHideSBP);





            //VYSUNUTI MENU PANELU
            menuPanelShowSBP = new Storyboard();
            doubleAnim3 = new DoubleAnimation();

            MenuPanel.RenderTransform = new TranslateTransform();

            doubleAnim3.From = 0;
            doubleAnim3.To = -(Window.Current.Bounds.Width / 6) * 2;
            doubleAnim3.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnim3.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            Storyboard.SetTarget(doubleAnim3, MenuPanel);
            Storyboard.SetTargetProperty(doubleAnim3, "(FrameworkElement.RenderTransform).(TranslateTransform.X)");

            menuPanelShowSBP.Children.Add(doubleAnim3);

            Resources.Add("menuPanelShowSBP", menuPanelShowSBP);



            //ZASUNUTI MENU PANELU
            Storyboard menuPanelHideSBP = new Storyboard();
            DoubleAnimation doubleAnim4 = new DoubleAnimation();

            MenuPanel.RenderTransform = new TranslateTransform();

            doubleAnim4.From = -(Window.Current.Bounds.Width / 6) * 2;
            doubleAnim4.To = 0;
            doubleAnim4.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnim4.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            Storyboard.SetTarget(doubleAnim4, MenuPanel);
            Storyboard.SetTargetProperty(doubleAnim4, "(FrameworkElement.RenderTransform).(TranslateTransform.X)");

            menuPanelHideSBP.Children.Add(doubleAnim4);

            Resources.Add("menuPanelHideSBP", menuPanelHideSBP);


        }
        private void setAnimationSnapped() 
        {
            //bude nutne udelat akorat mappanel
        }
        private void setAnimationNarrow() 
        {
            //bude nutne udelat akorat mappanel
        }
        private void setAnimationFIlled() 
        {
            //bude nutne udelat akorat mappanel
        }
        #endregion
        #region BUTTON EVENTS
        private void menuPanelButton1_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (App.ViewModel.CurrentStation != null)
            {
                this.Frame.Navigate(typeof(StationList));
            }
        }

        private void menuPanelButton2_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (App.ViewModel.CurrentStation != null)
            {
                this.Frame.Navigate(typeof(StationPage), App.ViewModel.CurrentStation); 
            }
        }

        private void menuPanelButton3_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            
            var fl = new FlyoutSettings();
            fl.Show();
        }

        bool cardOpen = false;
        private void flipMapButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Resources.Remove("FlipOpen");
            Resources.Remove("FlipClose");
            Resources.Add("FlipOpen", FlipOpen);
            Resources.Add("FlipClose", FlipClose);
            
            if (cardOpen == false)
            {
                ((Storyboard)Resources["FlipOpen"]).Begin();
                flipMapButton.Label = _myResourceLoader.GetString("MapCardOpen");
                cardOpen = true;
            }
            else
            {
                ((Storyboard)Resources["FlipClose"]).Begin();
                flipMapButton.Label = _myResourceLoader.GetString("MapCardClose");
                cardOpen = false;
            }
        }

        private void stationListButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StationList));
        }

        private void Pushpin_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Pin p = (sender as Pushpin).DataContext as Pin;
            if (p.PinType != EPinType.STATION || p.Station == null) return;

            App.ViewModel.DetailsStation = p.Station;
            this.Frame.Navigate(typeof(StationPage), App.ViewModel.DetailsStation);
        }

        #endregion


    }
}
