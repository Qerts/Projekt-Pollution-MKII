using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using Windows.UI.Popups;

namespace Pollution
{
    public static class Connection
    {
        public static void connect()
        {
            DateTime lastDownload;
            bool newDownload = true;

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                //var dialog = new MessageDialog(AppResources.MsgDownloadError + AppResources.Error);
                //var res = await dialog.ShowAsync();
                //MessageBox.Show(AppResources.MsgDownloadError, AppResources.Error, MessageBoxButton.OK);
                //newDownload = false;
            }
        }

    }
}
