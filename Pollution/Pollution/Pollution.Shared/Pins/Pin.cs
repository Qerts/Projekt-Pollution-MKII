using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Globalization;
using Pollution.ViewModels;
using Windows.UI.Xaml.Data;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI;

namespace Pollution
{
    public enum EPinType
    {
        NONE = 0,
        MAN,
        STATION
    }

    /// <summary>
    /// Třída realizující konverzi z EPinType do PintTypeStyle.
    /// Slouží pro nastabení stylu tlačítka v XAML v závislosti na enumerátoru PinType.
    /// </summary>
    public class PinTypeToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            var s = App.Current.Resources;
            switch ((EPinType)value)
            {
                case EPinType.MAN:
                    return App.Current.Resources["PushpinStyleMan"];

                case EPinType.STATION:
                    return null;//App.Current.Resources["PushpinStyleStation"];
                default:
                    return App.Current.Resources["PushpinStyleMan"];    //Libovolný Pushpin styl, jelikož špendlík nebude stejně viděn
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    } 

    /// <summary>
    /// Třída reprezentující špendlík (Ikonku) na mapě
    /// </summary>
    public class Pin : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propChanged)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propChanged));
        }

        #region Fields
        [XmlElement]
        private MyGeocoordinate location;

        [XmlElement]
        private EPinType pinType;

        private Station station;
        private bool isSelected;
        private bool isVisible;
        #endregion

        #region Properties

        /// <summary>
        /// Umístění špendlíku
        /// </summary>
        public MyGeocoordinate Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
                OnPropertyChanged("Location");
                OnPropertyChanged("Text");
                OnPropertyChanged("Visibility");
            }
        }

        /// <summary>
        /// Typ špendlíku
        /// </summary>
        public EPinType PinType
        {
            get
            {
                return pinType;
            }
            set
            {
                pinType = value;
                OnPropertyChanged("PinType");
            }
        }

        public Station Station
        {
            get
            {
                return station;
            }
            set
            {
                station = value;
                OnPropertyChanged("Station");
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
                OnPropertyChanged("SelectedBrush");
            }
        }

        public SolidColorBrush SelectedBrush
        {
            get
            {
                
                return new SolidColorBrush(Color.FromArgb(255,244, 244, 245)); 
            }
        }

        /// <summary>
        /// Text, který se bude zobrazovat v hlavičce aplikace.
        /// </summary>
        public string Text
        {
            get 
            { 
                return (location == null) ? "pozice není známa" : location.Latitude + " | " + location.Longitude;
            }
        }

        /// <summary>
        /// V případě, že špendlík nebude mít určenou lokaci, zajistí tato Property, že špendlík neuvidíme na mapě.
        /// </summary>
        public Visibility Visibility
        {
            get { return (Location == null) ? Visibility.Collapsed : (isVisible ? Visibility.Visible : Visibility.Collapsed); }
            set
            {
                if (value == Visibility.Visible) isVisible = true;
                else
                    isVisible = false;
                OnPropertyChanged("Visibility");
            }
        }


        #endregion

        #region Constructors
        /// <summary>
        /// Konstruktor. Vytvoří instanci třídy.
        /// </summary>
        public Pin()
        {
            Location = null;
            PinType = EPinType.NONE;
            isVisible = true;
        }

        /// <summary>
        /// Konstruktor. Vytvoří instanci třídy.
        /// </summary>
        public Pin(EPinType pinType)
        {
            Location = null;
            PinType = pinType;
            isVisible = true;
        }

        #endregion


    }



}
