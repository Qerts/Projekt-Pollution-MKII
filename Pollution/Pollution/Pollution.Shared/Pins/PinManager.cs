using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace Pollution
{
    /// <summary>
    /// Třída, která řídí kolekci špendlíků (ikon na mapě)-
    /// </summary>
    public class PinManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propChanged)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propChanged));
        }

        #region Fields
        public const double defaultZoomLevel = 20.0;
        private const string fileName = "PinManagerStorage.xml";

        private ObservableCollection<Pin> pins = new ObservableCollection<Pin>();
        #endregion

        #region Properties
        /// <summary>
        /// Základní nastavení úrovně přiblížení na mapě
        /// </summary>
        public double DefaultZoomLevel
        {
            get { return defaultZoomLevel; }
        }

        /// <summary>
        /// Kolekce špendlíků
        /// </summary>
        public ObservableCollection<Pin> Pins
        {
            get
            {
                return pins;
            }
            set
            {
                pins = value;
                OnPropertyChanged("Pins");
            }
        }
        #endregion

       
    }
}
