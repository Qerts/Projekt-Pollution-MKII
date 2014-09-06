using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Pollution
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            for (int a = 0; a < 66; a++)
            {
                LayoutGrid.Children.Add(Tile.RandomizedBlankTile(a / 11, a % 11));
            }

            LayoutGrid.Children.Add(Tile.imageTile(1, 3, "Assets/Logo.scale-100.png"));
            LayoutGrid.Children.Add(Tile.statusTIle_O3(1, 4));
            LayoutGrid.Children.Add(Tile.statusTile_CO(1, 5));
            LayoutGrid.Children.Add(Tile.statusTile_SO2(1, 6));
            LayoutGrid.Children.Add(Tile.statusTile_PM10(1, 7));
            LayoutGrid.Children.Add(Tile.statusTile_NO2(1, 8));
            LayoutGrid.Children.Add(Tile.mainStatusTile(2, 3));
        }



    }
}
