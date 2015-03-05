using Pollution.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Pollution
{
    public class DownloadService
    {
        //localsettings
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private const string URL = "http://data.garvis.cz/pollution/timeshot";
        private const string URLphotos = "http://data.garvis.cz/pollution/stationphotos";

        public Uri CreateURLforData() 
        {
            string l = "";
            object obj;
            localSettings.Containers["AppSettings"].Values.TryGetValue("lastNearestStation", out obj); //pokud nefunguje geolokace, tak natahne jen null
            l = obj.ToString();
            if (l == null) 
            { 
                l = "";
            }

            //Vytvoření url
            string ut = DateTime.Now.Ticks.ToString();
            string ms = "0";
            string manuf = getUserIdentity();
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

            return new Uri(URL + "?ns=" + l + "&ms=" + ms + "&t=" + ut + "&m=" + manuf + "&key=" + r.ToString());
        }
        public Uri CreateURLforPhotos() 
        {
            
            
            string ut = DateTime.Now.Ticks.ToString();
            string ms = "0";
            string manuf = getUserIdentity();
            long r = 158;
            string l = "G";

            //vytvoření url

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

            return new Uri(URLphotos + "?ns=" + l + "&ms=" + ms + "&t=" + ut + "&m=" + manuf + "&key=" + r.ToString());
        }
        public Uri CreateURLforStationData(string code) 
        {
            string l = code;          //vlozi do stringu pozadavku kod stanice oznacene jako detail

            //create URL request
            string ut = DateTime.Now.Ticks.ToString();
            string ms = "0";
            string manuf = getUserIdentity();
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

            return new Uri(URL + "?ns=" + l + "&ms=" + ms + "&t=" + ut + "&m=" + manuf + "&key=" + r.ToString());
        }
        public Uri CreateURLforStationPhotos(string code) 
        {
            string l = code;          //vlozi do stringu pozadavku kod stanice oznacene jako detail

            //create URL request
            string ut = DateTime.Now.Ticks.ToString();
            string ms = "0";
            string manuf = getUserIdentity();
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

            return new Uri(URLphotos + "?ns=" + l + "&ms=" + ms + "&t=" + ut + "&m=" + manuf + "&key=" + r.ToString());
        }

        public async Task<string> DownloadData() 
        {
            HttpClient wc = new HttpClient();
            var response = await wc.GetAsync(CreateURLforData());
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        public async Task<string> DownloadPhotos() 
        {
            HttpClient wc = new HttpClient();
            var response = await wc.GetAsync(CreateURLforPhotos());
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }




        /// <summary>
        /// Tato metoda zpracuje data přijatá ve stringu fotografií.
        /// </summary>
        /// <param name="content"></param>
        public void ProcessDonwloadedPhotos(string content)
        {
            //pravděpodobně preventivní vymazání předchozích fotek
            App.ViewModel.PhotosGlobal = null;

            //pokud je string nulový, tak fotky zůstanou prázdné
            if (content.Length == 0)
            {
                App.ViewModel.PhotosGlobal = null;
            }
            else
            {
                //vytvoření seznamu fotografií
                List<PPhoto> pPhotos = new List<PPhoto>();

                string contentString = content;
                string[] contentStringList;
                PPhoto pPhoto;

                //pokud je string neprázdný
                if (contentString != null && contentString.Length != 0)
                {
                    try
                    {
                        //zajištění, že se StringReader disposne ihned po ukončení bloku a zároveň čtení stringu
                        using (StringReader reader = new StringReader(contentString))
                        {
                            string line;
                            //čtení až do konce stringu
                            while ((line = reader.ReadLine()) != null)
                            {
                                //vynechání všech prázdných řádků
                                if (line.Length == 0)
                                {
                                    continue;
                                }
                                try
                                {
                                    //rozdělení řádků dle znaků
                                    contentStringList = line.Split('|');
                                    //přeskočení neúplných řádků
                                    if (contentStringList.Length != 5) continue;

                                    //naparsování dat na objekt fotky
                                    pPhoto = new PPhoto();
                                    pPhoto.Time = new DateTime(Int64.Parse(contentStringList[1]));
                                    pPhoto.URL = contentStringList[0];
                                    pPhoto.Smile = Int32.Parse(contentStringList[2]);
                                    pPhoto.Note = contentStringList[3];
                                    pPhoto.StationCode = contentStringList[4]; //unable to set Station, list is not ready - dřívější koment

                                    pPhotos.Add(pPhoto);

                                }
                                catch (Exception exp)
                                {
                                }

                            }
                        }
                        //vložení všech fot do view modelu
                        App.ViewModel.PhotosGlobal = pPhotos;

                    }
                    catch (Exception exp)
                    {
                    }

                }
                /*
                //je velice pravděpodobné, že následující if není v nové aplikaci použit, jelikož vypočítává nové fotky a jejich počet zobrazuje na nevyužitý setting
                //pokud byly vloženy nějaké fotky
                if (App.ViewModel.PhotosGlobal != null)
                {
                    //vytvoření data
                    DateTime d = DateTime.MinValue;

                    //pokus o získání posledního data
                    //IsolatedStorageSettings.ApplicationSettings.TryGetValue("lastPhotoTime", out d);
                    object obj;
                    localSettings.Containers["AppSettings"].Values.TryGetValue("lastPhotoTime", out obj);
                    d = DateTime.Parse(obj.ToString());

                    //sečtení všech fotek starších než datum poslední aktualizace??
                    int c = 0;
                    foreach (PPhoto photo in App.ViewModel.PhotosGlobal)
                    {
                        if (photo.Time > d) c++;
                    }

                    //pokud nejsou žádné starší fotky, načte se do titulku fotek něco??
                    if (c == 0)
                    {
                        //App.ViewModel.PivotPhotosHeader = AppResources.AppPhotos;
                        App.ViewModel.PivotPhotosHeader = _myResourceLoader.GetString("AppPhotos");
                    }
                    else
                    {
                        //pokud je čas poslední fotky větší než aktuální
                        if (App.ViewModel.PhotosGlobal[App.ViewModel.PhotosGlobal.Count - 1].Time > d)
                        {
                            //všechny načtené jsou novější
                            //App.ViewModel.PivotPhotosHeader = AppResources.AppPhotos + " (+" + App.ViewModel.PhotosGlobal.Count + ")";
                            App.ViewModel.PivotPhotosHeader = _myResourceLoader.GetString("AppPhotos") + " (+" + App.ViewModel.PhotosGlobal.Count + ")";
                        }
                        else
                        {
                            //App.ViewModel.PivotPhotosHeader = AppResources.AppPhotos + " (" + c + ")";
                            App.ViewModel.PivotPhotosHeader = _myResourceLoader.GetString("AppPhotos") + " (" + c + ")";
                        }
                    }
                }*/
            }


        }

        private string getUserIdentity() 
        {
            string identity = string.Empty;

            
            var getUserName = Task.Run(async delegate
            {
                identity = await Windows.System.UserProfile.UserInformation.GetDisplayNameAsync();
            });
            getUserName.Wait();


            return "PC-"+identity;
        }
    }
}
