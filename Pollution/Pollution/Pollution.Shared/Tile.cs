using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Pollution;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace Pollution 
{
    public class Tile : UserControl
    {
        

        public static Grid testTile(int rowNum, int colNum)
        {
            return new Grid();
        }
        public static Grid imageTile(int rowNum, int colNum)
        {
            return new Grid();
        }
        public static Grid stringTile(int rowNum, int colNum)
        {
            return new Grid();
        }
        public static Grid statusTile_SO2(int rowNum, int colNum)
        {
            return new Grid();
        }
        public static Grid statusTIle_O3(int rowNum, int colNum)
        {
            return new Grid();
        }
        public static Grid statusTile_CO(int rowNum, int colNum)
        {
            return new Grid();
        }
        public static Grid statusTile_PM10(int rowNum, int colNum)
        {
            return new Grid();
        }
        public static Grid statusTile_NO2(int rowNum, int colNum)
        {
            return new Grid();
        }
        public static Grid buttonTile_MenuPanel(int rowNum, int colNum)
        {
            return new Grid();
        }
        public static Grid buttonTile_DataPanel(int rowNum, int colNum)
        {
            return new Grid();
        }
        public static Grid buttonTile_MapPanel(int rowNum, int colNum)
        {
            return new Grid();
        }

        public static Grid mainStatusTile(int rowNum, int colNum)
        {
            Grid tile = new Grid();
            tile.Opacity = 1;
            tile.Background = Data.getColorAndStatus(Data.getMainMood()).Item1;
            Grid.SetRow(tile, rowNum);
            Grid.SetColumn(tile, colNum);
            Grid.SetRowSpan(tile, 2);
            Grid.SetColumnSpan(tile, 2);


            RowDefinition rowdef1 = new RowDefinition();
            rowdef1.Height = new GridLength(1, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef1);
            RowDefinition rowdef2 = new RowDefinition();
            rowdef2.Height = new GridLength(2, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef2);
            RowDefinition rowdef3 = new RowDefinition();
            rowdef3.Height = new GridLength(1, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef3);

            ColumnDefinition coldef1 = new ColumnDefinition();
            coldef1.Width = new GridLength(1, GridUnitType.Star);
            tile.ColumnDefinitions.Add(coldef1);
            ColumnDefinition coldef2 = new ColumnDefinition();
            coldef2.Width = new GridLength(1, GridUnitType.Star);
            tile.ColumnDefinitions.Add(coldef2);
            ColumnDefinition coldef3 = new ColumnDefinition();
            coldef3.Width = new GridLength(1, GridUnitType.Star);
            tile.ColumnDefinitions.Add(coldef3);
            ColumnDefinition coldef4 = new ColumnDefinition();
            coldef4.Width = new GridLength(1, GridUnitType.Star);
            tile.ColumnDefinitions.Add(coldef4);
            ColumnDefinition coldef5 = new ColumnDefinition();
            coldef5.Width = new GridLength(1, GridUnitType.Star);
            tile.ColumnDefinitions.Add(coldef5);

            
            TextBlock statusTitle = new TextBlock();
            statusTitle.Text = "Stav:";
            statusTitle.FontSize = Window.Current.Bounds.Height / 24;//predelat na dynamiku
            statusTitle.VerticalAlignment = VerticalAlignment.Center;
            statusTitle.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumnSpan(statusTitle, 2);
            tile.Children.Add(statusTitle);

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(Data.getColorAndStatus(Data.getMainMood()).Item3));
            Grid.SetRow(img, 1);
            Grid.SetColumn(img, 1);
            Grid.SetColumnSpan(img, 3);
            img.Stretch = Stretch.Fill;
            tile.Children.Add(img);
        
            TextBlock stationName = new TextBlock();
            stationName.Text = Data.StationName;
            stationName.FontSize = Window.Current.Bounds.Height / 40;
            stationName.TextAlignment = TextAlignment.Center;
            stationName.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumnSpan(stationName, 5);
            Grid.SetRow(stationName, 2);
            tile.Children.Add(stationName);

            tile.Name = "mainStatusTile";

            return tile;
        }


        public static Grid RandomizedBlankTile(int rowNum, int colNum) 
        {
            Grid tile = new Grid();
            Random random = new Random();
            tile.Background = Data.getMainColor();
            tile.Name = "BlankTile";
            tile.Children.Clear();
            tile.Opacity = ((double)(random.Next(50, 100))) / 100;
            Grid.SetRow(tile, rowNum);
            Grid.SetColumn(tile, colNum);
            return tile;
            
        }

















































        /*
   public Tile(TileType type, int rowNum, int colNum)
   {



       switch (type) 
       {
           case TileType.RandomizedBlankTile :
               tohle = RandomizedBlankTile(rowNum, colNum);
               break;
           case TileType.ButtonTile_DataPanel :
               break;
           case TileType.ButtonTile_MapPanel :
               break;
           case TileType.ButtonTile_MenuPanel :
               break;
           case TileType.ImageTile :
               break;
           case TileType.MainStatusTile :
               break;
           case TileType.StatusTile_CO :
               break;
           case TileType.StatusTile_NO2 :
               break;
           case TileType.StatusTile_O3 :
               break;
           case TileType.StatusTile_PM10 :
               break;
           case TileType.StatusTile_SO2 :
               break;
           case TileType.StringTile :
               break;
           case TileType.TestTile :
               break;
       }



   }

   public enum TileType 
   {
       TestTile,
       RandomizedBlankTile,
       ImageTile,
       StringTile,
       StatusTile_SO2,
       StatusTile_O3,
       StatusTile_CO,
       StatusTile_PM10,
       StatusTile_NO2,
       MainStatusTile,
       ButtonTile_MenuPanel,
       ButtonTile_DataPanel,
       ButtonTile_MapPanel
   }*/
    }
}
