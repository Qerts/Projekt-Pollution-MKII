using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Pollution;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Data;
using Pollution.ViewModels;
using Utils;
using Windows.ApplicationModel.Resources;
using Pollution.Utils;
using Windows.UI.Text;
using Windows.ApplicationModel;

namespace Pollution 
{
    public class Tile
    {
        Random random = new Random();
        private ColorQualityConverter _colorConverter = new ColorQualityConverter();
        private ResourceLoader _resourceLoader = new ResourceLoader();
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
            try
            {
                Binding colorBinding = new Binding();
                colorBinding.Source = App.ViewModel;
                colorBinding.Path = new PropertyPath("CurrentStation.So2.State");
                colorBinding.Converter = new ColorQualityConverter();
                tile.SetBinding(Grid.BackgroundProperty, colorBinding);
            }
            catch (Exception)            {            }

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

            //nabindování
            TextBlock ratingString = new TextBlock();
            ratingString.Text = "SO2";
            Binding valueBinding = new Binding();
            valueBinding.Source = App.ViewModel;
            valueBinding.Path = new PropertyPath("CurrentStation.So2.Value");
            valueBinding.Converter = new NegativeValueConverter();
            ratingString.SetBinding(TextBlock.TextProperty, valueBinding);
            //ratingString.FontWeight = FontWeights.Bold;          

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
            try
            {
                Binding statusBinding = new Binding();
                statusBinding.Source = App.ViewModel;
                statusBinding.Path = new PropertyPath("CurrentStation.So2.State");
                statusBinding.Converter = new ColorQualityConverter();
                stationString.SetBinding(TextBlock.TextProperty, statusBinding);
            }
            catch (Exception)
            {
            } stationString.Foreground = new SolidColorBrush(Colors.White);
            stationString.FontSize = Data.getFontSize_SmallText();
            stationString.TextAlignment = TextAlignment.Center;
            stationString.HorizontalAlignment = HorizontalAlignment.Center;
            stationString.VerticalAlignment = VerticalAlignment.Center;
            stationString.FontWeight = FontWeights.Bold;
            Grid.SetColumnSpan(stationString, 5);
            Grid.SetRow(stationString, 2);
            tile.Children.Add(stationString);

            TextBlock nameString = new TextBlock();
            nameString.Text = "SO\x2082";
            nameString.Foreground = new SolidColorBrush(Colors.White);
            nameString.FontSize = Data.getFontSize_CommonText();
            nameString.TextAlignment = TextAlignment.Center;
            nameString.HorizontalAlignment = HorizontalAlignment.Center;
            nameString.FontWeight = FontWeights.Bold;
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


            try
            {
                Binding colorBinding = new Binding();
                colorBinding.Source = App.ViewModel;
                colorBinding.Path = new PropertyPath("CurrentStation.O3.State");
                colorBinding.Converter = new ColorQualityConverter();
                tile.SetBinding(Grid.BackgroundProperty, colorBinding);
            }
            catch (Exception)
            {
                
                
            }

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
            Binding valueBinding = new Binding();
            valueBinding.Source = App.ViewModel;
            valueBinding.Path = new PropertyPath("CurrentStation.O3.Value");
            valueBinding.Converter = new NegativeValueConverter();
            ratingString.SetBinding(TextBlock.TextProperty, valueBinding);
            //ratingString.FontWeight = FontWeights.Bold;


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
            try
            {
                Binding statusBinding = new Binding();
                statusBinding.Source = App.ViewModel;
                statusBinding.Path = new PropertyPath("CurrentStation.O3.State");
                statusBinding.Converter = new ColorQualityConverter();
                stationString.SetBinding(TextBlock.TextProperty, statusBinding);
            }
            catch (Exception)
            {
                
                
            } stationString.Foreground = new SolidColorBrush(Colors.White);
            stationString.FontSize = Data.getFontSize_SmallText();
            stationString.TextAlignment = TextAlignment.Center;
            stationString.HorizontalAlignment = HorizontalAlignment.Center;
            stationString.VerticalAlignment = VerticalAlignment.Center;
            stationString.FontWeight = FontWeights.Bold;
            Grid.SetColumnSpan(stationString, 5);
            Grid.SetRow(stationString, 2);
            tile.Children.Add(stationString);

            TextBlock nameString = new TextBlock();
            nameString.Text = "O\x2083";
            nameString.Foreground = new SolidColorBrush(Colors.White);
            nameString.FontSize = Data.getFontSize_CommonText();
            nameString.TextAlignment = TextAlignment.Center;
            nameString.FontWeight = FontWeights.Bold;
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

            try
            {
                Binding colorBinding = new Binding();
                colorBinding.Source = App.ViewModel;
                colorBinding.Path = new PropertyPath("CurrentStation.Co.State");
                colorBinding.Converter = new ColorQualityConverter();
                tile.SetBinding(Grid.BackgroundProperty, colorBinding);
            }
            catch (Exception)
            {
                
            }

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
            Binding valueBinding = new Binding();
            valueBinding.Source = App.ViewModel;
            valueBinding.Path = new PropertyPath("CurrentStation.Co.Value");
            valueBinding.Converter = new NegativeValueConverter();
            ratingString.SetBinding(TextBlock.TextProperty, valueBinding);
            //ratingString.FontWeight = FontWeights.Bold;



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
            try
            {
                Binding statusBinding = new Binding();
                statusBinding.Source = App.ViewModel;
                statusBinding.Path = new PropertyPath("CurrentStation.Co.State");
                statusBinding.Converter = new ColorQualityConverter();
                stationString.SetBinding(TextBlock.TextProperty, statusBinding);
            }
            catch (Exception)
            {
                
            } stationString.Foreground = new SolidColorBrush(Colors.White);
            stationString.FontSize = Data.getFontSize_SmallText();
            stationString.TextAlignment = TextAlignment.Center;
            stationString.HorizontalAlignment = HorizontalAlignment.Center;
            stationString.VerticalAlignment = VerticalAlignment.Center;
            stationString.FontWeight = FontWeights.Bold;
            Grid.SetColumnSpan(stationString, 5);
            Grid.SetRow(stationString, 2);
            tile.Children.Add(stationString);

            TextBlock nameString = new TextBlock();
            nameString.Text = "CO";
            nameString.Foreground = new SolidColorBrush(Colors.White);
            nameString.FontSize = Data.getFontSize_CommonText();
            nameString.TextAlignment = TextAlignment.Center;
            nameString.HorizontalAlignment = HorizontalAlignment.Center;
            nameString.FontWeight = FontWeights.Bold;
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

            try
            {
                Binding colorBinding = new Binding();
                colorBinding.Source = App.ViewModel;
                colorBinding.Path = new PropertyPath("CurrentStation.Pm10.State");
                colorBinding.Converter = new ColorQualityConverter();
                tile.SetBinding(Grid.BackgroundProperty, colorBinding);

            }
            catch (Exception)
            {
            }
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
            ratingString.Text = "PM10";
            Binding valueBinding = new Binding();
            valueBinding.Source = App.ViewModel;
            valueBinding.Path = new PropertyPath("CurrentStation.Pm10.Value");
            valueBinding.Converter = new NegativeValueConverter();
            ratingString.SetBinding(TextBlock.TextProperty, valueBinding);
            //ratingString.FontWeight = FontWeights.Bold;

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
            try
            {
                Binding statusBinding = new Binding();
                statusBinding.Source = App.ViewModel;
                statusBinding.Path = new PropertyPath("CurrentStation.Pm10.State");
                statusBinding.Converter = new ColorQualityConverter();
                stationString.SetBinding(TextBlock.TextProperty, statusBinding);
            }
            catch (Exception)
            {
            } stationString.Foreground = new SolidColorBrush(Colors.White);
            stationString.FontSize = Data.getFontSize_SmallText();
            stationString.TextAlignment = TextAlignment.Center;
            stationString.HorizontalAlignment = HorizontalAlignment.Center;
            stationString.VerticalAlignment = VerticalAlignment.Center;
            stationString.FontWeight = FontWeights.Bold;
            Grid.SetColumnSpan(stationString, 5);
            Grid.SetRow(stationString, 2);
            tile.Children.Add(stationString);

            TextBlock nameString = new TextBlock();
            nameString.Text = Data.getString_PM10();
            nameString.Foreground = new SolidColorBrush(Colors.White);
            nameString.FontSize = Data.getFontSize_CommonText();
            nameString.TextAlignment = TextAlignment.Center;
            nameString.HorizontalAlignment = HorizontalAlignment.Center;
            nameString.FontWeight = FontWeights.Bold;
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
            try
            {
                Binding colorBinding = new Binding();
                colorBinding.Source = App.ViewModel;
                colorBinding.Path = new PropertyPath("CurrentStation.No2.State");
                colorBinding.Converter = new ColorQualityConverter();
                tile.SetBinding(Grid.BackgroundProperty, colorBinding);

            }
            catch (Exception)
            {
            }            
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
            Binding valueBinding = new Binding();
            valueBinding.Source = App.ViewModel;
            valueBinding.Path = new PropertyPath("CurrentStation.No2.Value");
            valueBinding.Converter = new NegativeValueConverter();
            ratingString.SetBinding(TextBlock.TextProperty, valueBinding);
            //ratingString.FontWeight = FontWeights.Bold;
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
            try
            {
                Binding statusBinding = new Binding();
                statusBinding.Source = App.ViewModel;
                statusBinding.Path = new PropertyPath("CurrentStation.No2.State");
                statusBinding.Converter = new ColorQualityConverter();
                stationString.SetBinding(TextBlock.TextProperty, statusBinding);

            }
            catch (Exception)
            {
            } stationString.Foreground = new SolidColorBrush(Colors.White);
            stationString.FontSize = Data.getFontSize_SmallText();
            stationString.TextAlignment = TextAlignment.Center;
            stationString.HorizontalAlignment = HorizontalAlignment.Center;
            stationString.VerticalAlignment = VerticalAlignment.Center;
            stationString.FontWeight = FontWeights.Bold;
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
            nameString.FontWeight = FontWeights.Bold;
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
            //bude třeba nabindovat pro změnu jazyka
            title.Text = _resourceLoader.GetString("MapPanelButton");
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
            //bude třeba nabindovat prop změnu jazyka
            title.Text = _resourceLoader.GetString("DataPanelButton");
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
                //bude třeba nabindovat por změnu jazyka
                Text = _resourceLoader.GetString("MenuPanelButton"),
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White)
            };
            menuPanelButton.Children.Add(title);

            menuPanelButton.Tapped += mainPage.menuPanelService;

            return menuPanelButton;
        }


        /// <summary>
        /// Hlavní dlaždice pro zobrazení jména stanice a smajlíka.
        /// </summary>
        /// <param name="rowNum"></param>
        /// <param name="colNum"></param>
        /// <returns></returns>
        public Grid mainStatusTile(int rowNum, int colNum)
        {
            Grid tile = new Grid();
            tile.Opacity = 1;
            try
            {
                Binding colorBinding = new Binding();
                colorBinding.Source = App.ViewModel;
                colorBinding.Path = new PropertyPath("CurrentStation.Quality");
                colorBinding.Converter = new ColorQualityConverter();
                tile.SetBinding(Grid.BackgroundProperty, colorBinding);
            }
            catch (Exception)
            {
            } Grid.SetRow(tile, rowNum);
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

            /*
            TextBlock statusTitle = new TextBlock();
            //bude pravděpodobně třeba změnit pro plynulé přepínání jazyka
            statusTitle.Text = _resourceLoader.GetString("StatusTitle");
            statusTitle.FontSize = Data.getFontSize_LargeText();
            statusTitle.VerticalAlignment = VerticalAlignment.Center;
            statusTitle.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumnSpan(statusTitle, 2);
            tile.Children.Add(statusTitle);
            */

            Image img = new Image();
            //bude třeba změnit por plynulou změnu obrázku
            Binding imageBinding = new Binding();
            imageBinding.Source = App.ViewModel;
            imageBinding.Path = new PropertyPath("CurrentStation.Quality");
            imageBinding.Converter = new ImageQualityConverter();
            img.SetBinding(Image.SourceProperty, imageBinding);
            Grid.SetRow(img, 1);
            Grid.SetColumn(img, 1);
            Grid.SetColumnSpan(img, 3);
            img.Stretch = Stretch.Uniform;
            tile.Children.Add(img);
        
            TextBlock stationName = new TextBlock();
            Binding b = new Binding();
            try
            {
                Binding nameBinding = new Binding();
                nameBinding.Source = App.ViewModel;
                nameBinding.Path = new PropertyPath("CurrentStation.Name");
                stationName.SetBinding(TextBlock.TextProperty, nameBinding);

            }
            catch (Exception)
            {
            } 
            
            stationName.FontSize = Data.getFontSize_LargeText();
            stationName.TextAlignment = TextAlignment.Center;
            stationName.VerticalAlignment = VerticalAlignment.Center;
            stationName.FontWeight = FontWeights.Bold;
            Grid.SetColumnSpan(stationName, 5);
            Grid.SetRow(stationName, 2);
            tile.Children.Add(stationName);

            tile.Name = "mainStatusTile";

            return tile;
        }
        /// <summary>
        /// Prázdná dlaždice. Je jimivydlážděna plocha
        /// </summary>
        /// <param name="rowNum"></param>
        /// <param name="colNum"></param>
        /// <returns></returns>
        public Grid RandomizedBlankTile(int rowNum, int colNum) 
        {
            
            Grid tile = new Grid();
            tile.Background = new SolidColorBrush(Colors.Black);
            try
            {
                Binding colorBinding = new Binding();
                colorBinding.Source = App.ViewModel;
                colorBinding.Path = new PropertyPath("CurrentStation.Quality");
                colorBinding.Converter = new ColorQualityConverter();
                tile.SetBinding(Grid.BackgroundProperty, colorBinding);
            }
            catch (Exception)
            {
                
               
            }
            tile.Name = "BlankTile";
            tile.Children.Clear();
            //tile.Opacity = ((double)(random.Next(50, 100))) / 100;
            tile.Opacity = ((double)(random.Next(40, 70))) / 100;//pro fotky na pozadí
            Grid.SetRow(tile, rowNum);
            Grid.SetColumn(tile, colNum);
            return tile;
            
        }

        public Grid InfoTile(int rowNum, int colNum)
        {

            Grid tile = new Grid();
            tile.Background = new SolidColorBrush(Color.FromArgb(255,50,50,50));
            tile.Name = "InfoTile";
            tile.Children.Clear();
            Grid.SetRow(tile, rowNum);
            Grid.SetColumn(tile, colNum);

            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(1, GridUnitType.Auto);
            tile.RowDefinitions.Add(row1);
            RowDefinition row2 = new RowDefinition();
            row2.Height = new GridLength(1, GridUnitType.Auto);
            tile.RowDefinitions.Add(row2);
            RowDefinition row3 = new RowDefinition();
            row3.Height = new GridLength(1, GridUnitType.Auto);
            tile.RowDefinitions.Add(row3);


            StackPanel versionStackPanel = new StackPanel();
            versionStackPanel.Margin = new Thickness(5, 5, 5, 0);
            Grid.SetRow(versionStackPanel, 1);
            tile.Children.Add(versionStackPanel);

            TextBlock versionTitle = new TextBlock();
            versionTitle.Text = _resourceLoader.GetString("TextVersion/Text") + ": ";
            versionTitle.FontSize = Data.getFontSize_SmallText();
            versionTitle.FontWeight = FontWeights.Bold;
            versionTitle.Padding = new Thickness(5, 5, 5, 0);
            versionStackPanel.Children.Add(versionTitle);
            TextBlock versionValue = new TextBlock();
            versionValue.Text = Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor + "." + Package.Current.Id.Version.Build + "." + Package.Current.Id.Version.Revision;
            versionValue.FontSize = Data.getFontSize_SmallText();
            versionValue.FontWeight = FontWeights.Bold;
            versionValue.Padding = new Thickness(5, 5, 5, 0);
            versionStackPanel.Children.Add(versionValue);


            StackPanel lastUpdateStackPanel = new StackPanel();
            lastUpdateStackPanel.Margin = new Thickness(5, 5, 5, 0);
            Grid.SetRow(lastUpdateStackPanel, 0);
            tile.Children.Add(lastUpdateStackPanel);

            TextBlock lastLocationTitle = new TextBlock();
            lastLocationTitle.Text = _resourceLoader.GetString("TextUpdateTime");
            lastLocationTitle.FontSize = Data.getFontSize_SmallText();
            lastLocationTitle.FontWeight = FontWeights.Bold;
            lastLocationTitle.TextWrapping = TextWrapping.WrapWholeWords;
            lastLocationTitle.Padding = new Thickness(5, 5, 5, 0);
            lastUpdateStackPanel.Children.Add(lastLocationTitle);
            TextBlock lastLocationUpdate = new TextBlock();
            lastLocationUpdate.FontSize = Data.getFontSize_SmallText();
            lastLocationUpdate.FontWeight = FontWeights.Bold;
            lastLocationUpdate.TextWrapping = TextWrapping.WrapWholeWords;
            lastLocationUpdate.Padding = new Thickness(5, 5, 5, 0);
            Binding locationBinding = new Binding();
            locationBinding.Source = App.ViewModel;
            locationBinding.Path = new PropertyPath("LastPositionTime");
            lastLocationUpdate.SetBinding(TextBlock.TextProperty, locationBinding);
            lastUpdateStackPanel.Children.Add(lastLocationUpdate);




            return tile;

        }
    }
}
