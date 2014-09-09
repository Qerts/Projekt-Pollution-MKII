using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public List<UIElement> blankTileCollection = new List<UIElement>();
        public List<UIElement> mainStatusCollection = new List<UIElement>();
        public List<UIElement> statusesCollection = new List<UIElement>();
        public List<UIElement> imagesCollection = new List<UIElement>();


        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;

            
        }
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
            DetermineVisualState();
        }


        void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }
        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            DetermineVisualState();
        }

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
                    setLandscapeChanges();
                }
                else
                {
                    state = "FullScreenPortrait";
                    setPortraitChanges();
                }
            }
            else 
            {
                if (size.Width == 320) 
                {
                    state = "Snapped";
                }
                else if (size.Width <= 500)
                {
                    state = "Narrow";
                }
                else 
                {
                    state = "Filled";
                }

            }

            System.Diagnostics.Debug.WriteLine("Width: {0}, New VisulState: {1}", size.Width, state);

            VisualStateManager.GoToState(this, state, true);
        }



        
 
       

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            for (int a = 0; a < 66; a++)
            {
                var tile = Tile.RandomizedBlankTile(a / 11, a % 11);
                LayoutGrid.Children.Add(tile);
                blankTileCollection.Add(tile);
            }


            

            //OBRAZKY
            var imgtile = Tile.imageTile(1, 3, "Assets/Logo.scale-100.png");
            LayoutGrid.Children.Add(imgtile);
            imagesCollection.Add(imgtile);
            //MALE STATUSY
            Random random = new Random();
            var o3status = Tile.statusTIle_O3(random.Next(1, 4), 5);
            LayoutGrid.Children.Add(o3status);
            statusesCollection.Add(o3status);
            var costatus = Tile.statusTile_CO(random.Next(1, 4), 6);
            LayoutGrid.Children.Add(costatus);
            statusesCollection.Add(costatus);
            var so2status = Tile.statusTile_SO2(random.Next(1, 4), 7);
            LayoutGrid.Children.Add(so2status);
            statusesCollection.Add(so2status);
            var pm10status = Tile.statusTile_PM10(random.Next(1, 4), 8);
            LayoutGrid.Children.Add(pm10status);
            statusesCollection.Add(pm10status);
            var no2status = Tile.statusTile_NO2(random.Next(1, 4), 9);
            LayoutGrid.Children.Add(no2status);
            statusesCollection.Add(no2status);
            //HLAVNI STATUS
            var mainstatus = Tile.mainStatusTile(2, 2);
            LayoutGrid.Children.Add(mainstatus);
            mainStatusCollection.Add(mainstatus);
            
        }


        public void setPortraitChanges() 
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

            foreach (FrameworkElement element in statusesCollection)
            {

                Grid.SetColumn(element, random.Next(1, 4));
                Grid.SetRow(element, random.Next(5,9));
            }
            /*
            foreach (FrameworkElement element in mainStatusCollection)
            {

                Grid.SetColumn(element, 2);
                Grid.SetRow(element, blankTileCollection.IndexOf(element) % 11);
            }
            */
            
            foreach (FrameworkElement element in imagesCollection)
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
            coldef32.Width = new GridLength(4, GridUnitType.Star);
            MenuPanel.ColumnDefinitions.Add(coldef32);

            Grid.SetColumn(MenuPanel, 1);
            Grid.SetColumnSpan(MenuPanel, 5);
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
            ColumnDefinition coldef52 = new ColumnDefinition();
            coldef52.Width = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(coldef52);
            ColumnDefinition coldef53 = new ColumnDefinition();
            coldef53.Width = new GridLength(2, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(coldef53);
            ColumnDefinition coldef54 = new ColumnDefinition();
            coldef54.Width = new GridLength(1, GridUnitType.Star);
            menuPanelButtonGrid.ColumnDefinitions.Add(coldef54);

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
            Grid.SetColumn(menuPanelButton4, 1);
            Grid.SetRow(menuPanelButton4, 2);
            Grid.SetColumn(menuPanelButton5, 2);
            Grid.SetRow(menuPanelButton5, 2);

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
            

        }


        public void setLandscapeChanges()
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

            foreach (FrameworkElement element in statusesCollection)
            {

                Grid.SetColumn(element, random.Next(5, 9));
                Grid.SetRow(element, random.Next(1, 4));
            }
            /*
            foreach (FrameworkElement element in mainStatusCollection)
            {

                Grid.SetColumn(element, 2);
                Grid.SetRow(element, blankTileCollection.IndexOf(element) % 11);
            }
            */

            foreach (FrameworkElement element in imagesCollection)
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
            Grid.SetRowSpan(MenuPanel, 5);

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
            Grid.SetColumn(menuPanelButton3, 3);
            Grid.SetRow(menuPanelButton3, 1);
            Grid.SetColumn(menuPanelButton4, 4);
            Grid.SetRow(menuPanelButton4, 1);
            Grid.SetColumn(menuPanelButton5, 5);
            Grid.SetRow(menuPanelButton5, 1);

            Grid.SetColumn(menuPanelInfoGrid, 1);
            Grid.SetRow(menuPanelInfoGrid, 0);

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

        }


        public void mapPanelService(object sender, RoutedEventArgs e) 
        {
            //MapPanel.Margin = new Thickness(0,0,0,0);
        }

        public void LayoutRoot_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            // TODO: Add event handler implementation here.
        }

        private void LayoutRoot_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }


    }
}
