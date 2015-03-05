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
            await Windows.Storage.FileIO.WriteTextAsync(file, content);
        }

        /// <summary>
        /// Voláním této funkce jsou načtena data ze souboru a uložena jako zdroj raw dat do view modelu.
        /// </summary>
        public async Task LoadDataFromFile(string fileName)
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync(fileName);
            App.ViewModel.RawData = await FileIO.ReadTextAsync(file);
        }

    }
}
