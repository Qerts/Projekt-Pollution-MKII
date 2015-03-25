using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications;

namespace BackgroundTask
{
    public sealed class TileTask : IBackgroundTask
    {
        static string textElementName = "text";
        private int number = 1;

        private static MyGeocoordinate MyPosition { get; set; }
        CancellationTokenSource source;


        async void IBackgroundTask.Run(IBackgroundTaskInstance taskInstance)
        {
            
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            
            //Geolokace
            try
            {
                if (source == null)
                {
                    source = new CancellationTokenSource();
                }
                CancellationToken token = source.Token;

                Geolocator locator = new Geolocator();
                Geoposition position = await locator.GetGeopositionAsync().AsTask(token);
                MyGeocoordinate coord = new MyGeocoordinate(position.Coordinate.Latitude, position.Coordinate.Longitude);
                MyPosition = coord;
            }
            catch (Exception)
            {

                throw;
            }
            finally 
            {
                source = null;
            }
            //endGeolokace


                // Update the live tile with the feed items.
            UpdateTile();

            

            // Inform the system that the task is finished.
            deferral.Complete();
            

        }

        void locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Geoposition position = null;

                var task = Task.Run(async () =>
                {
                    position = await sender.GetGeopositionAsync(new TimeSpan(0, 0, 0, 0, 200), new TimeSpan(0, 0, 3));
                });
                task.Wait();

            //Definice MyPosition vlastní náhradní třídou pro geopozici a uložení šířky a délky do jejích property.
            MyGeocoordinate coord = new MyGeocoordinate();
            coord.Latitude = position.Coordinate.Point.Position.Latitude;
            coord.Longitude = position.Coordinate.Point.Position.Longitude;
            if (coord != null)
            {
                MyPosition = coord;
            }
        }

        private static void UpdateTile()
        {
            //localsettings
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            //načíst proměnnou useGPS, nearestStation, nearestWithoutStation
            object tmpObject = null;
            var nearestStation = true;
            localSettings.Containers["AppSettings"].Values.TryGetValue("nearestStation", out tmpObject);
            bool? x = tmpObject as bool?;
            if (x != null)
            {
                nearestStation = x.Value;
            }

            tmpObject = null;
            bool nearestWithoutStation = false;
            localSettings.Containers["AppSettings"].Values.TryGetValue("nearestWithoutStation", out tmpObject);
            x = tmpObject as bool?;
            if (x != null)
            {
                nearestWithoutStation = x.Value;
            }

            tmpObject = null;
            bool useGPS = true;
            //IsolatedStorageSettings.ApplicationSettings.TryGetValue("useGPS", out useGPS);
            localSettings.Containers["AppSettings"].Values.TryGetValue("useGPS", out tmpObject);
            x = tmpObject as bool?;
            if (x != null)
            {
                useGPS = x.Value;
            }

            tmpObject = null;
            bool useLiveTile = true;
            //IsolatedStorageSettings.ApplicationSettings.TryGetValue("useLiveTile", out useLiveTile);
            localSettings.Containers["AppSettings"].Values.TryGetValue("useLiveTile", out tmpObject);
            x = tmpObject as bool?;
            if (x != null)
            {
                useLiveTile = x.Value;
            }

            tmpObject = null;
            localSettings.Containers["AppSettings"].Values.TryGetValue("infoStation", out tmpObject);
            string infoStation = tmpObject.ToString();


            //ověřit, jestli bude načtena nejbližší stanice, nejbližší stanice bez stavu nebo bude zobrazen šedý stav
            
            //je použit live tile
            if (!useLiveTile)
            {
                return;
            }
            //je zvolena stanice
            if (infoStation != null && nearestStation == false)
            {
                var gsTask = Task.Run(async () =>
                {
                    await GetStation();
                });
                gsTask.Wait();
                return;
            }
            //nefunguje gps
            if (!useGPS)
            {
                GetNoGPS();
                return;
            }
            //je zvolena stanice bez stavu
            if (nearestWithoutStation)
            {
                var gwsTask = Task.Run(async () =>
                {
                    await GetWithoutStation();
                });
                gwsTask.Wait();
                
                return;
            }
            //je zvolena stanice se stavem
            if (nearestStation)
            {
                var gnsTask = Task.Run(async () =>
                {
                    await GetNearestStation();
                });
                gnsTask.Wait();
                
                return;
            }


            /*
            //naaktualizovat dlaždice
            i = DateTime.Today;

            XmlDocument contentSmall = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text02);
            contentSmall.GetElementsByTagName(textElementName)[0].InnerText =  i.ToString();
            TileNotification notifSmall = new TileNotification(contentSmall);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notifSmall);

            XmlDocument contentWide = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text01);
            contentWide.GetElementsByTagName(textElementName)[0].InnerText =  i.ToString();
            TileNotification notifWide = new TileNotification(contentWide);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notifWide);

            XmlDocument contentBig = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare310x310Text07);
            contentBig.GetElementsByTagName(textElementName)[0].InnerText = i.ToString();
            TileNotification notifBig = new TileNotification(contentBig);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notifBig);
            */
        }

        private static async Task GetNearestStation()
        {
            ResourceLoader _resourceLoader = new ResourceLoader();

            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("rawdata.txt");
            var content = await FileIO.ReadTextAsync(file);
            var parsedContent = content.Split('|');
            int nearestStation = 0;
            double tmpDistance = 0;
            for (int i = 0; i < parsedContent.Length; i++)
            {
                if (i % 20 == 1)
                {
                    if ((int.Parse(parsedContent[ i + 6 ]) <= 6))
                    {
                        MyGeocoordinate coord = new MyGeocoordinate(Double.Parse(parsedContent[i + 5], System.Globalization.CultureInfo.InvariantCulture), Double.Parse(parsedContent[i + 4], System.Globalization.CultureInfo.InvariantCulture));
                        var result = coord.GetDistanceTo(MyPosition);

                        if (tmpDistance == 0 || tmpDistance > result)
                        {
                            tmpDistance = result;
                            nearestStation = i;
                        } 
                    }
                }
            }
            //naplnit hodnoty
            int stationID = nearestStation;
            var stationName = parsedContent[stationID + 1];
            var stationValue = parsedContent[stationID + 6];
            var updateTime = _resourceLoader.GetString("UpdateTime");
            string stationStatus;
            switch (int.Parse(stationValue))
            {
                case 1:
                    stationStatus = _resourceLoader.GetString("QualityState1/Text");
                    break;
                case 2:
                    stationStatus = _resourceLoader.GetString("QualityState2/Text");
                    break;
                case 3:
                    stationStatus = _resourceLoader.GetString("QualityState3/Text");
                    break;
                case 4:
                    stationStatus = _resourceLoader.GetString("QualityState4/Text");
                    break;
                case 5:
                    stationStatus = _resourceLoader.GetString("QualityState5/Text"); ;
                    break;
                case 6:
                    stationStatus = _resourceLoader.GetString("QualityState6/Text");
                    break;
                case 7:
                    stationStatus = _resourceLoader.GetString("QualityState7/Text");
                    break;
                case 8:
                    stationStatus = _resourceLoader.GetString("QualityState8/Text");
                    break;
            }
            string stationRegion;
            switch (parsedContent[nearestStation].ToCharArray()[0])
            {
                case 'A': stationRegion = _resourceLoader.GetString("RegionA");
                    break;
                case 'S': stationRegion = _resourceLoader.GetString("RegionS");
                    break;
                case 'C': stationRegion = _resourceLoader.GetString("RegionC");
                    break;
                case 'P': stationRegion = _resourceLoader.GetString("RegionP");
                    break;
                case 'K': stationRegion = _resourceLoader.GetString("RegionK");
                    break;
                case 'U': stationRegion = _resourceLoader.GetString("RegionU");
                    break;
                case 'L': stationRegion = _resourceLoader.GetString("RegionL");
                    break;
                case 'H': stationRegion = _resourceLoader.GetString("RegionH");
                    break;
                case 'E': stationRegion = _resourceLoader.GetString("RegionE");
                    break;
                case 'J': stationRegion = _resourceLoader.GetString("RegionJ");
                    break;
                case 'B': stationRegion = _resourceLoader.GetString("RegionB");
                    break;
                case 'M': stationRegion = _resourceLoader.GetString("RegionM");
                    break;
                case 'Z': stationRegion = _resourceLoader.GetString("RegionZ");
                    break;
                case 'T': stationRegion = _resourceLoader.GetString("RegionT");
                    break;
                default:
                    stationRegion = "error";
                    break;
            }
            string stationImage = string.Empty;
            switch (int.Parse(stationValue))
            {
                case 1:
                    stationImage = @"ms-appx:///SharedAssets/Smiley7.png";
                    break;
                case 2:
                    stationImage = @"ms-appx:///SharedAssets/Smiley6.png";
                    break;
                case 3:
                    stationImage = @"ms-appx:///SharedAssets/Smiley5.png";
                    break;
                case 4:
                    stationImage = @"ms-appx:///SharedAssets/Smiley4.png";
                    break;
                case 5:
                    stationImage = @"ms-appx:///SharedAssets/Smiley3.png";
                    break;
                case 6:
                    stationImage = @"ms-appx:///SharedAssets/Smiley2.png";
                    break;
                case 7:
                    stationImage = @"ms-appx:///SharedAssets/Smiley0.png";
                    break;
                case 8:
                    stationImage = @"ms-appx:///SharedAssets/Smiley0.png";
                    break;
            }

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            #region MEDIUM TILE
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare150x150(true);
            XmlDocument contentSmall2 = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText04);
            contentSmall2.GetElementsByTagName("text")[0].InnerText = stationName + "\n" + stationRegion;
            //contentSmall2.GetElementsByTagName("text")[1].InnerText = stationRegion;
            contentSmall2.GetElementsByTagName("image")[0].Attributes[1].InnerText = stationImage;
            TileNotification notifSmall2 = new TileNotification(contentSmall2);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notifSmall2);
            #endregion
            #region WIDE TILE
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForWide310x150(true);
            XmlDocument contentWide = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150SmallImageAndText03);
            contentWide.GetElementsByTagName("text")[0].InnerText = stationName + "\n" + stationRegion;
            contentWide.GetElementsByTagName("image")[0].Attributes[1].InnerText = stationImage;
            TileNotification notifWide = new TileNotification(contentWide);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notifWide);
            #endregion
            #region LARGE
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare310x310(true);
            XmlDocument contentBig = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare310x310ImageAndText01);
            contentBig.GetElementsByTagName("text")[0].InnerText = stationName + "\n" + stationRegion;
            //contentBig.GetElementsByTagName("text")[1].InnerText = stationRegion;
            contentBig.GetElementsByTagName("image")[0].Attributes[1].InnerText = stationImage;
            TileNotification notifBig = new TileNotification(contentBig);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notifBig);
            #endregion
        }

        private static async Task GetWithoutStation()
        {
            ResourceLoader _resourceLoader = new ResourceLoader();           

            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("rawdata.txt");
            var content = await FileIO.ReadTextAsync(file);
            var parsedContent = content.Split('|');
            int nearestStation = 0;
            double tmpDistance = 0;
            for (int i = 0; i < parsedContent.Length; i++)
            {
                if (i%20 == 1)
                {
                    MyGeocoordinate coord = new MyGeocoordinate(Double.Parse(parsedContent[i + 5], System.Globalization.CultureInfo.InvariantCulture), Double.Parse(parsedContent[i + 4], System.Globalization.CultureInfo.InvariantCulture));
                    var result = coord.GetDistanceTo(MyPosition);

                    if (tmpDistance == 0 || tmpDistance > result)
                    {
                        tmpDistance = result;
                        nearestStation = i;
                    }
                }
            }
            //naplnit hodnoty
            int stationID = nearestStation;
            var stationName = parsedContent[stationID + 1];
            var stationValue = parsedContent[stationID + 6];
            var updateTime = _resourceLoader.GetString("UpdateTime");
            string stationStatus;
            switch (int.Parse(stationValue))
            {
                case 1:
                    stationStatus = _resourceLoader.GetString("QualityState1/Text");
                    break;
                case 2:
                    stationStatus = _resourceLoader.GetString("QualityState2/Text");
                    break;
                case 3:
                    stationStatus = _resourceLoader.GetString("QualityState3/Text");
                    break;
                case 4:
                    stationStatus = _resourceLoader.GetString("QualityState4/Text");
                    break;
                case 5:
                    stationStatus = _resourceLoader.GetString("QualityState5/Text");
                    break;
                case 6:
                    stationStatus = _resourceLoader.GetString("QualityState6/Text");
                    break;
                case 7:
                    stationStatus = _resourceLoader.GetString("QualityState7/Text");
                    break;
                case 8:
                    stationStatus = _resourceLoader.GetString("QualityState8/Text");
                    break;
            }
            string stationRegion;
            switch (parsedContent[nearestStation].ToCharArray()[0])
            {
                case 'A': stationRegion = _resourceLoader.GetString("RegionA");
                    break;
                case 'S': stationRegion = _resourceLoader.GetString("RegionS");
                    break;
                case 'C': stationRegion = _resourceLoader.GetString("RegionC");
                    break;
                case 'P': stationRegion = _resourceLoader.GetString("RegionP");
                    break;
                case 'K': stationRegion = _resourceLoader.GetString("RegionK");
                    break;
                case 'U': stationRegion = _resourceLoader.GetString("RegionU");
                    break;
                case 'L': stationRegion = _resourceLoader.GetString("RegionL");
                    break;
                case 'H': stationRegion = _resourceLoader.GetString("RegionH");
                    break;
                case 'E': stationRegion = _resourceLoader.GetString("RegionE");
                    break;
                case 'J': stationRegion = _resourceLoader.GetString("RegionJ");
                    break;
                case 'B': stationRegion = _resourceLoader.GetString("RegionB");
                    break;
                case 'M': stationRegion = _resourceLoader.GetString("RegionM");
                    break;
                case 'Z': stationRegion = _resourceLoader.GetString("RegionZ");
                    break;
                case 'T': stationRegion = _resourceLoader.GetString("RegionT");
                    break;
                default:
                    stationRegion = "error";
                    break;
            }
            string stationImage = string.Empty;
            switch (int.Parse(stationValue))
            {
                case 1:
                    stationImage = @"ms-appx:///SharedAssets/Smiley7.png";
                    break;
                case 2:
                    stationImage = @"ms-appx:///SharedAssets/Smiley6.png";
                    break;
                case 3:
                    stationImage = @"ms-appx:///SharedAssets/Smiley5.png";
                    break;
                case 4:
                    stationImage = @"ms-appx:///SharedAssets/Smiley4.png";
                    break;
                case 5:
                    stationImage = @"ms-appx:///SharedAssets/Smiley3.png";
                    break;
                case 6:
                    stationImage = @"ms-appx:///SharedAssets/Smiley2.png";
                    break;
                case 7:
                    stationImage = @"ms-appx:///SharedAssets/Smiley0.png";
                    break;
                case 8:
                    stationImage = @"ms-appx:///SharedAssets/Smiley0.png";
                    break;
            }

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            #region MEDIUM TILE            
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare150x150(true);
            XmlDocument contentSmall2 = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText04);
            contentSmall2.GetElementsByTagName("text")[0].InnerText = stationName + "\n" + stationRegion;
            //contentSmall2.GetElementsByTagName("text")[1].InnerText = stationRegion;
            contentSmall2.GetElementsByTagName("image")[0].Attributes[1].InnerText = stationImage;
            TileNotification notifSmall2 = new TileNotification(contentSmall2);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notifSmall2);
            #endregion
            #region WIDE TILE
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForWide310x150(true);
            XmlDocument contentWide = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150SmallImageAndText03);
            contentWide.GetElementsByTagName("text")[0].InnerText = stationName + "\n" + stationRegion;
            contentWide.GetElementsByTagName("image")[0].Attributes[1].InnerText = stationImage;
            TileNotification notifWide = new TileNotification(contentWide);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notifWide);
            #endregion 
            #region LARGE
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare310x310(true);
            XmlDocument contentBig = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare310x310ImageAndText01);
            contentBig.GetElementsByTagName("text")[0].InnerText = stationName + "\n" + stationRegion;
            //contentBig.GetElementsByTagName("text")[1].InnerText = stationRegion;
            contentBig.GetElementsByTagName("image")[0].Attributes[1].InnerText = stationImage;
            TileNotification notifBig = new TileNotification(contentBig);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notifBig);
            #endregion
        }

        private static void GetNoGPS()
        {
            //todo případně, v tomto stavu při nezískání gps zs
        }

        private static async Task GetStation()
        {
            //localsettings
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            object tmpObject = null;
            localSettings.Containers["AppSettings"].Values.TryGetValue("infoStation", out tmpObject);
            string infoStation = tmpObject.ToString();

            ResourceLoader _resourceLoader = new ResourceLoader();
            //otevřít soubor
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("rawdata.txt");
            //načíst soubor
            var content = await FileIO.ReadTextAsync(file);
            //najít stanici
            string[] parsedContent = content.Split('|');
            //načíst hodnoty
            string mainStatus = string.Empty;
            int stationID = 0;
            for (int i = 0; i < parsedContent.Length; i++)
            {
                if (infoStation == parsedContent[i])
                {
                    mainStatus = parsedContent[i + 6];
                    stationID = i;
                }
            }

            //naplnit hodnoty
            var stationName = parsedContent[stationID + 1];
            var stationValue = parsedContent[stationID + 6];
            var updateTime = _resourceLoader.GetString("UpdateTime");
            string stationStatus;
            switch (int.Parse(stationValue))
            {
                case 1:
                    stationStatus = _resourceLoader.GetString("QualityState1/Text");
                    break;
                case 2:
                    stationStatus = _resourceLoader.GetString("QualityState2/Text");
                    break;
                case 3:
                    stationStatus = _resourceLoader.GetString("QualityState3/Text");
                    break;
                case 4:
                    stationStatus = _resourceLoader.GetString("QualityState4/Text");
                    break;
                case 5:
                    stationStatus = _resourceLoader.GetString("QualityState5/Text"); ;
                    break;
                case 6:
                    stationStatus = _resourceLoader.GetString("QualityState6/Text");
                    break;
                case 7:
                    stationStatus = _resourceLoader.GetString("QualityState7/Text");
                    break;
                case 8:
                    stationStatus = _resourceLoader.GetString("QualityState8/Text");
                    break;
            }
            string stationRegion;
            switch (parsedContent[stationID].ToCharArray()[0])
            {
                case 'A': stationRegion = _resourceLoader.GetString("RegionA");
                    break;
                case 'S': stationRegion = _resourceLoader.GetString("RegionS");
                    break;
                case 'C': stationRegion = _resourceLoader.GetString("RegionC");
                    break;
                case 'P': stationRegion = _resourceLoader.GetString("RegionP");
                    break;
                case 'K': stationRegion = _resourceLoader.GetString("RegionK");
                    break;
                case 'U': stationRegion = _resourceLoader.GetString("RegionU");
                    break;
                case 'L': stationRegion = _resourceLoader.GetString("RegionL");
                    break;
                case 'H': stationRegion = _resourceLoader.GetString("RegionH");
                    break;
                case 'E': stationRegion = _resourceLoader.GetString("RegionE");
                    break;
                case 'J': stationRegion = _resourceLoader.GetString("RegionJ");
                    break;
                case 'B': stationRegion = _resourceLoader.GetString("RegionB");
                    break;
                case 'M': stationRegion = _resourceLoader.GetString("RegionM");
                    break;
                case 'Z': stationRegion = _resourceLoader.GetString("RegionZ");
                    break;
                case 'T': stationRegion = _resourceLoader.GetString("RegionT");
                    break;
                default:
                    stationRegion = "error";
                    break;
            }
            string stationImage = string.Empty;
            switch (int.Parse(stationValue))
            {
                case 1:
                    stationImage = @"ms-appx:///SharedAssets/Smiley7.png";
                    break;
                case 2:
                    stationImage = @"ms-appx:///SharedAssets/Smiley6.png";
                    break;
                case 3:
                    stationImage = @"ms-appx:///SharedAssets/Smiley5.png";
                    break;
                case 4:
                    stationImage = @"ms-appx:///SharedAssets/Smiley4.png";
                    break;
                case 5:
                    stationImage = @"ms-appx:///SharedAssets/Smiley3.png";
                    break;
                case 6:
                    stationImage = @"ms-appx:///SharedAssets/Smiley2.png";
                    break;
                case 7:
                    stationImage = @"ms-appx:///SharedAssets/Smiley0.png";
                    break;
                case 8:
                    stationImage = @"ms-appx:///SharedAssets/Smiley0.png";
                    break;
            }
            //aktualizovat dlaždice
            #region MEDIUM TILE
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare150x150(true);
            XmlDocument contentSmall2 = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText04);
            contentSmall2.GetElementsByTagName("text")[0].InnerText = stationName + "\n" + stationRegion;
            //contentSmall2.GetElementsByTagName("text")[1].InnerText = stationRegion;
            contentSmall2.GetElementsByTagName("image")[0].Attributes[1].InnerText = stationImage;
            TileNotification notifSmall2 = new TileNotification(contentSmall2);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notifSmall2);
            #endregion
            #region WIDE TILE
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForWide310x150(true);
            XmlDocument contentWide = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150SmallImageAndText03);
            contentWide.GetElementsByTagName("text")[0].InnerText = stationName + "\n" + stationRegion;
            contentWide.GetElementsByTagName("image")[0].Attributes[1].InnerText = stationImage;
            TileNotification notifWide = new TileNotification(contentWide);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notifWide);
            #endregion
            #region LARGE
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare310x310(true);
            XmlDocument contentBig = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare310x310ImageAndText01);
            contentBig.GetElementsByTagName("text")[0].InnerText = stationName + "\n" + stationRegion;
            //contentBig.GetElementsByTagName("text")[1].InnerText = stationRegion;
            contentBig.GetElementsByTagName("image")[0].Attributes[1].InnerText = stationImage;
            TileNotification notifBig = new TileNotification(contentBig);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notifBig);
            #endregion

        }

        
        

        

        
        
    
    }
}
