using System;
using System.Collections.Generic;
using System.Text;

namespace Pollution
{
    public class GPSService
    {
        private Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public void SetGeolocator() 
        {
            App.ViewModel.SetGeolocator();
        }

        internal void SetPosition()
        {
            App.ViewModel.CalculateDistances();
        }
    }
}
