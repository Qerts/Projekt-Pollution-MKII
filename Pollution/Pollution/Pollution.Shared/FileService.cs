using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Pollution
{
    public class FileService
    {

        /// <summary>
        /// Voláním této funkce jsou zadaná data uložena do souboru raw dat.
        /// </summary>
        /// <param name="content"></param>
        public async Task SaveDataToFile(string content, string fileName)
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            try
            {
                await Windows.Storage.FileIO.WriteTextAsync(file, content);
            }
            catch (Exception)
            {

                Task waitTask = Task.Run(() => { Task.Delay(1000);});
                waitTask.Wait();
                SaveDataToFile(content, fileName);
            }
        }

        /// <summary>
        /// Voláním této funkce jsou načtena data ze souboru a uložena jako zdroj raw dat do view modelu.
        /// </summary>
        public async Task LoadDataFromFile(string fileName)
        {
            
#if WINDOWS_APP
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.TryGetItemAsync(fileName) as StorageFile;
            if (file != null)
            {
                App.ViewModel.RawData = await FileIO.ReadTextAsync(file);
            }
            else
            {
                await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            }            
#endif
        }
    }
}
