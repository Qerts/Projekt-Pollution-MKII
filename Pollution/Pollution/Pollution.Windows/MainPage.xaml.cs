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

            LayoutGrid.ColumnDefinitions.Clear();
            LayoutGrid.RowDefinitions.Clear();



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

        }


        public void setLandscapeChanges()
        {

            LayoutGrid.ColumnDefinitions.Clear();
            LayoutGrid.RowDefinitions.Clear();



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

        }


    }
}
