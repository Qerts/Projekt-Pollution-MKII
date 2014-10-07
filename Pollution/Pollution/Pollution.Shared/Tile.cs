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
    public class Tile
    {
        Random random = new Random();
        //static Random random = new Random();
        private MainPage mainPage;

        public Tile(MainPage value) 
        {
            this.mainPage = value;
        }

        public Grid testTile(int rowNum, int colNum, string mytxt)
        {
            Grid tile = new Grid();
            TextBlock txt = new TextBlock();
            txt.Text = mytxt;
            tile.Children.Add(txt);


            Grid.SetRow(tile, rowNum);
            Grid.SetColumn(tile, colNum);
            return tile;
        }
        public Grid imageTile(int rowNum, int colNum, string uri)
        {
            Grid tile = new Grid();
            Grid.SetRow(tile, rowNum);
            Grid.SetColumn(tile, colNum);
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:" + uri));
            imageBrush.Stretch = Stretch.UniformToFill;
            tile.Background = imageBrush;
            

            
            tile.Name = "imageTile";

            return tile;
        }
        public Grid statusTile_SO2(int rowNum, int colNum)
        {
            //pro pripad nemoznosti nacist stav
            string status = "error";

            Grid tile = new Grid();
            Grid.SetRow(tile, rowNum);
            Grid.SetColumn(tile, colNum);

            tile.Background = Data.getColorAndStatus(Data.getStatus_SO2()).Item1;

            RowDefinition rowdef1 = new RowDefinition();
            rowdef1.Height = new GridLength(1, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef1);
            RowDefinition rowdef2 = new RowDefinition();
            rowdef2.Height = new GridLength(2, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef2);
            RowDefinition rowdef3 = new RowDefinition();
            rowdef3.Height = new GridLength(1, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef3);
            RowDefinition rowdef4 = new RowDefinition();
            rowdef4.Height = new GridLength(2, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef4);

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

            TextBlock ratingString = new TextBlock();
            ratingString.Text = Data.getValue_SO2().ToString();
            ratingString.Foreground = new SolidColorBrush(Colors.White);
            ratingString.FontSize = Data.getFontSize_StatuValue();
            ratingString.TextAlignment = TextAlignment.Center;
            ratingString.HorizontalAlignment = HorizontalAlignment.Center;
            ratingString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(ratingString, 0);
            Grid.SetColumnSpan(ratingString, 5);
            Grid.SetRow(ratingString, 1);
            tile.Children.Add(ratingString);

            TextBlock stationString = new TextBlock();
            status = Data.GetSO2ColorAndStatus().Item2;
            stationString.Text = status;
            stationString.Foreground = new SolidColorBrush(Colors.White);
            stationString.FontSize = Data.getFontSize_SmallText();
            stationString.TextAlignment = TextAlignment.Center;
            stationString.HorizontalAlignment = HorizontalAlignment.Center;
            stationString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumnSpan(stationString, 5);
            Grid.SetRow(stationString, 2);
            tile.Children.Add(stationString);

            TextBlock nameString = new TextBlock();
            nameString.Text = "SO\x2082";
            nameString.Foreground = new SolidColorBrush(Colors.White);
            nameString.FontSize = Data.getFontSize_CommonText();
            nameString.TextAlignment = TextAlignment.Center;
            nameString.HorizontalAlignment = HorizontalAlignment.Center;
            nameString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumnSpan(nameString, 2);
            Grid.SetRow(nameString, 3);
            tile.Children.Add(nameString);


            tile.Name = "statusTile";

            return tile;
        }
        public Grid statusTIle_O3(int rowNum, int colNum)
        {
            //pro pripad nemoznosti nacist stav
            string status = "error";

            Grid tile = new Grid();
            Grid.SetRow(tile, rowNum);
            Grid.SetColumn(tile, colNum);

            tile.Background = Data.getColorAndStatus(Data.getStatus_O3()).Item1;

            RowDefinition rowdef1 = new RowDefinition();
            rowdef1.Height = new GridLength(1, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef1);
            RowDefinition rowdef2 = new RowDefinition();
            rowdef2.Height = new GridLength(2, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef2);
            RowDefinition rowdef3 = new RowDefinition();
            rowdef3.Height = new GridLength(1, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef3);
            RowDefinition rowdef4 = new RowDefinition();
            rowdef4.Height = new GridLength(2, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef4);

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

            TextBlock ratingString = new TextBlock();
            ratingString.Text = Data.getValue_O3().ToString();
            ratingString.Foreground = new SolidColorBrush(Colors.White);
            ratingString.FontSize = Data.getFontSize_StatuValue();
            ratingString.TextAlignment = TextAlignment.Center;
            ratingString.HorizontalAlignment = HorizontalAlignment.Center;
            ratingString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(ratingString, 0);
            Grid.SetColumnSpan(ratingString, 5);
            Grid.SetRow(ratingString, 1);
            tile.Children.Add(ratingString);

            TextBlock stationString = new TextBlock();
            status = Data.GetO3ColorAndStatus().Item2;
            stationString.Text = status;
            stationString.Foreground = new SolidColorBrush(Colors.White);
            stationString.FontSize = Data.getFontSize_SmallText();
            stationString.TextAlignment = TextAlignment.Center;
            stationString.HorizontalAlignment = HorizontalAlignment.Center;
            stationString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumnSpan(stationString, 5);
            Grid.SetRow(stationString, 2);
            tile.Children.Add(stationString);

            TextBlock nameString = new TextBlock();
            nameString.Text = "O\x2083";
            nameString.Foreground = new SolidColorBrush(Colors.White);
            nameString.FontSize = Data.getFontSize_CommonText();
            nameString.TextAlignment = TextAlignment.Center;
            nameString.HorizontalAlignment = HorizontalAlignment.Center;
            nameString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumnSpan(nameString, 2);
            Grid.SetRow(nameString, 3);
            tile.Children.Add(nameString);


            tile.Name = "statusTile";

            return tile;
        }
        public Grid statusTile_CO(int rowNum, int colNum)
        {
            //pro pripad nemoznosti nacist stav
            string status = "error";

            Grid tile = new Grid();
            Grid.SetRow(tile, rowNum);
            Grid.SetColumn(tile, colNum);

            tile.Background = Data.getColorAndStatus(Data.getStatus_CO()).Item1;

            RowDefinition rowdef1 = new RowDefinition();
            rowdef1.Height = new GridLength(1, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef1);
            RowDefinition rowdef2 = new RowDefinition();
            rowdef2.Height = new GridLength(2, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef2);
            RowDefinition rowdef3 = new RowDefinition();
            rowdef3.Height = new GridLength(1, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef3);
            RowDefinition rowdef4 = new RowDefinition();
            rowdef4.Height = new GridLength(2, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef4);

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

            TextBlock ratingString = new TextBlock();
            ratingString.Text = Data.getValue_CO().ToString();
            ratingString.Foreground = new SolidColorBrush(Colors.White);
            ratingString.FontSize = Data.getFontSize_StatuValue();
            ratingString.TextAlignment = TextAlignment.Center;
            ratingString.HorizontalAlignment = HorizontalAlignment.Center;
            ratingString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(ratingString, 0);
            Grid.SetColumnSpan(ratingString, 5);
            Grid.SetRow(ratingString, 1);
            tile.Children.Add(ratingString);

            TextBlock stationString = new TextBlock();
            status = Data.GetCOColorAndStatus().Item2;
            stationString.Text = status;
            stationString.Foreground = new SolidColorBrush(Colors.White);
            stationString.FontSize = Data.getFontSize_SmallText();
            stationString.TextAlignment = TextAlignment.Center;
            stationString.HorizontalAlignment = HorizontalAlignment.Center;
            stationString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumnSpan(stationString, 5);
            Grid.SetRow(stationString, 2);
            tile.Children.Add(stationString);

            TextBlock nameString = new TextBlock();
            nameString.Text = "CO";
            nameString.Foreground = new SolidColorBrush(Colors.White);
            nameString.FontSize = Data.getFontSize_CommonText();
            nameString.TextAlignment = TextAlignment.Center;
            nameString.HorizontalAlignment = HorizontalAlignment.Center;
            nameString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumnSpan(nameString, 2);
            Grid.SetRow(nameString, 3);
            tile.Children.Add(nameString);


            tile.Name = "statusTile";

            return tile;
        }
        public Grid statusTile_PM10(int rowNum, int colNum)
        {
            //pro pripad nemoznosti nacist stav
            string status = "error";

            Grid tile = new Grid();
            Grid.SetRow(tile, rowNum);
            Grid.SetColumn(tile, colNum);

            tile.Background = Data.getColorAndStatus(Data.getStatus_PM10()).Item1;

            RowDefinition rowdef1 = new RowDefinition();
            rowdef1.Height = new GridLength(1, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef1);
            RowDefinition rowdef2 = new RowDefinition();
            rowdef2.Height = new GridLength(2, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef2);
            RowDefinition rowdef3 = new RowDefinition();
            rowdef3.Height = new GridLength(1, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef3);
            RowDefinition rowdef4 = new RowDefinition();
            rowdef4.Height = new GridLength(2, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef4);

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

            TextBlock ratingString = new TextBlock();
            ratingString.Text = Data.getValue_PM10().ToString();
            ratingString.Foreground = new SolidColorBrush(Colors.White);
            ratingString.FontSize = Data.getFontSize_StatuValue();
            ratingString.TextAlignment = TextAlignment.Center;
            ratingString.HorizontalAlignment = HorizontalAlignment.Center;
            ratingString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(ratingString, 0);
            Grid.SetColumnSpan(ratingString, 5);
            Grid.SetRow(ratingString, 1);
            tile.Children.Add(ratingString);

            TextBlock stationString = new TextBlock();
            status = Data.GetPM10ColorAndStatus().Item2;
            stationString.Text = status;
            stationString.Foreground = new SolidColorBrush(Colors.White);
            stationString.FontSize = Data.getFontSize_SmallText();
            stationString.TextAlignment = TextAlignment.Center;
            stationString.HorizontalAlignment = HorizontalAlignment.Center;
            stationString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumnSpan(stationString, 5);
            Grid.SetRow(stationString, 2);
            tile.Children.Add(stationString);

            TextBlock nameString = new TextBlock();
            nameString.Text = Data.getString_PM10();
            nameString.Foreground = new SolidColorBrush(Colors.White);
            nameString.FontSize = Data.getFontSize_CommonText();
            nameString.TextAlignment = TextAlignment.Center;
            nameString.HorizontalAlignment = HorizontalAlignment.Center;
            nameString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumnSpan(nameString, 2);
            Grid.SetRow(nameString, 3);
            tile.Children.Add(nameString);


            tile.Name = "statusTile";

            return tile;
        }
        public Grid statusTile_NO2(int rowNum, int colNum)
        {
            //pro pripad nemoznosti nacist stav
            string status = "error";

            Grid tile = new Grid();
            Grid.SetRow(tile, rowNum);
            Grid.SetColumn(tile, colNum);
            
            tile.Background = Data.getColorAndStatus(Data.getStatus_NO2()).Item1;
            
            RowDefinition rowdef1 = new RowDefinition();
            rowdef1.Height = new GridLength(1, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef1);
            RowDefinition rowdef2 = new RowDefinition();
            rowdef2.Height = new GridLength(2, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef2);
            RowDefinition rowdef3 = new RowDefinition();
            rowdef3.Height = new GridLength(1, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef3);
            RowDefinition rowdef4 = new RowDefinition();
            rowdef4.Height = new GridLength(2, GridUnitType.Star);
            tile.RowDefinitions.Add(rowdef4);

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

            TextBlock ratingString = new TextBlock();
            ratingString.Text = Data.getValue_NO2().ToString();
            ratingString.Foreground = new SolidColorBrush(Colors.White);
            ratingString.FontSize = Data.getFontSize_StatuValue();
            ratingString.TextAlignment = TextAlignment.Center;
            ratingString.HorizontalAlignment = HorizontalAlignment.Center;
            ratingString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(ratingString, 0);
            Grid.SetColumnSpan(ratingString, 5);
            Grid.SetRow(ratingString, 1);
            tile.Children.Add(ratingString);

            TextBlock stationString = new TextBlock();
            status = Data.GetNO2ColorAndStatus().Item2;
            stationString.Text = status;
            stationString.Foreground = new SolidColorBrush(Colors.White);
            stationString.FontSize = Data.getFontSize_SmallText();
            stationString.TextAlignment = TextAlignment.Center;
            stationString.HorizontalAlignment = HorizontalAlignment.Center;
            stationString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumnSpan(stationString, 5);
            Grid.SetRow(stationString, 2);
            tile.Children.Add(stationString);

            TextBlock nameString = new TextBlock();
            nameString.Text = "NO\x2082";
            nameString.Foreground = new SolidColorBrush(Colors.White);
            nameString.FontSize = Data.getFontSize_CommonText();
            nameString.TextAlignment = TextAlignment.Center;
            nameString.HorizontalAlignment = HorizontalAlignment.Center;
            nameString.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumnSpan(nameString, 2);
            Grid.SetRow(nameString, 3);
            tile.Children.Add(nameString);

            
            tile.Name = "statusTile";
            
            return tile;
        }
        public Grid buttonTile_MapPanel(int rowNum, int colNum)
        {
            Grid mapPanelButton = new Grid();
            Grid.SetRow(mapPanelButton, rowNum);
            Grid.SetColumn(mapPanelButton, colNum);
            mapPanelButton.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
            TextBlock title = new TextBlock();
            title.FontSize = Data.getFontSize_LargeText();
            title.TextWrapping = TextWrapping.WrapWholeWords;
            title.Text = "Mapa";
            title.TextAlignment = TextAlignment.Center;
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.VerticalAlignment = VerticalAlignment.Center;
            title.Foreground = new SolidColorBrush(Colors.White);
            mapPanelButton.Children.Add(title);

            mapPanelButton.Tapped += mainPage.mapPanelService;

            return mapPanelButton;
        }

        public Grid buttonTile_DataPanel(int rowNum, int colNum)
        {
            Grid dataPanelButton = new Grid();
            Grid.SetRow(dataPanelButton, rowNum);
            Grid.SetColumn(dataPanelButton, colNum);
            dataPanelButton.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
            TextBlock title = new TextBlock();
            title.TextWrapping = TextWrapping.WrapWholeWords;
            title.FontSize = Data.getFontSize_LargeText();
            title.Text = "Další informace";
            title.TextAlignment = TextAlignment.Center;
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.VerticalAlignment = VerticalAlignment.Center;
            title.Foreground = new SolidColorBrush(Colors.White);
            dataPanelButton.Children.Add(title);

            dataPanelButton.Tapped += mainPage.dataPanelService;

            return dataPanelButton;
        }

        public Grid buttonTile_MenuPanel(int rowNum, int colNum)
        {
            Grid menuPanelButton = new Grid();
            Grid.SetRow(menuPanelButton, rowNum);
            Grid.SetColumn(menuPanelButton, colNum);
            menuPanelButton.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
            TextBlock title = new TextBlock() 
            {
                TextWrapping = TextWrapping.WrapWholeWords,
                FontSize = Data.getFontSize_LargeText(),
                Text = "Další\nfunkce",
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White)
            };
            menuPanelButton.Children.Add(title);

            menuPanelButton.Tapped += mainPage.menuPanelService;

            return menuPanelButton;
        }



        public Grid mainStatusTile(int rowNum, int colNum)
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
            statusTitle.FontSize = Data.getFontSize_Title();
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
            stationName.FontSize = Data.getFontSize_LargeText();
            stationName.TextAlignment = TextAlignment.Center;
            stationName.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumnSpan(stationName, 5);
            Grid.SetRow(stationName, 2);
            tile.Children.Add(stationName);

            tile.Name = "mainStatusTile";

            return tile;
        }
        public Grid RandomizedBlankTile(int rowNum, int colNum) 
        {
            
            Grid tile = new Grid();
            tile.Background = Data.getColorAndStatus(Data.getMainMood()).Item1;
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
