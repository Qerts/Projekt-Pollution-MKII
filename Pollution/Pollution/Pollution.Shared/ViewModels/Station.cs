using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
//using Resources;

namespace Pollution.ViewModels
{

    public enum EStationClassification
    {
        NONE,
        TRAFFIC,
        INTERCITY,
        AROUNDCITY,
        VILLAGE,
        INDUSTRY
    }

    public class Station: INotifyPropertyChanged
    {
        private int id;
        private string code;
        private string name;
        private string owner;
        private int quality;
        //private GeoCoordinate position;
        private PElement so2;
        private PElement no2;
        private PElement co;
        private PElement o3;
        private PElement pm10;
        private PElement pm24;
        private double distance;
        private EStationClassification classification;
        private DateTime? lastPhoto;
        
        public Station()
        {
            id = 0;
            code = "";
            name = "";
            owner = "";
            quality = 0;
            //position = new GeoCoordinate();
            so2 = new PElement(EPElementType.SO2);
            no2 = new PElement(EPElementType.NO2);
            co = new PElement(EPElementType.CO);
            o3 = new PElement(EPElementType.O3);
            pm10 = new PElement(EPElementType.PM10);
            pm24 = new PElement(EPElementType.PM1024);
            distance = -1.0F;
            classification = EStationClassification.NONE;
            lastPhoto = null;
        }

        public Station(int _id, string _code, string _name, string _owner, EStationClassification _class, int _quality,/* GeoCoordinate _position,*/ PElement _so2, PElement _no2, PElement _co, PElement _o3, PElement _pm10, PElement _pm24, DateTime? lp = null)
        {
            id = _id;
            code = _code;
            name = _name;
            owner = _owner;
            classification = _class;
            quality = _quality;
            //position = _position;
            so2 = _so2;
            no2 = _no2;
            co = _co;
            o3  = _o3;
            pm10 = _pm10;
            pm24 = _pm24;
            distance = 0.0F;
        }

        public int Id
        {
            get { return id; }
            set { if (id != value) { id = value; NotifyPropertyChanged("Id"); } }
        }

        public string Code
        {
            get { return code; }
            set { if (code != value) { code = value; NotifyPropertyChanged("Code"); } }
        }

        public string Name
        {
            get { return name; }
            set { if (name != value) { name = value; NotifyPropertyChanged("Name"); } }
        }

        public string Owner
        {
            get { return owner; }
            set { if (owner != value) { owner = value; NotifyPropertyChanged("Owner"); } }
        }

        public EStationClassification Classification
        {
            get { return classification; }
            set { if (classification != value) { classification = value; NotifyPropertyChanged("Classification"); } }
        }

        public int Quality
        {
            get { return quality; }
            set { if (quality != value) { quality = value; NotifyPropertyChanged("Quality"); } }
        }

        public string QualityNumber
        {
            get
            {
                if (quality >= 7) return "?";
                return quality.ToString();

            }
        }
        /*
        public GeoCoordinate Position
        {
            get { return position; }
            set { if (position != value) { position = value; NotifyPropertyChanged("Position"); } }
        }
        */
        public PElement So2
        {
            get { return so2; }
            set { if (so2 != value) { so2 = value; NotifyPropertyChanged("So2"); } }
        }

        public PElement No2
        {
            get { return no2; }
            set { if (no2 != value) { no2 = value; NotifyPropertyChanged("No2"); } }
        }

        public PElement Co
        {
            get { return co; }
            set { if (co != value) { co = value; NotifyPropertyChanged("Co"); } }
        }

        public PElement O3
        {
            get { return o3; }
            set { if (o3 != value) { o3 = value; NotifyPropertyChanged("O3"); } }
        }

        public PElement Pm10
        {
            get { return pm10; }
            set { if (pm10 != value) { pm10 = value; NotifyPropertyChanged("Pm10"); } }
        }

        public PElement Pm24
        {
            get { return pm24; }
            set { if (pm24 != value) { pm24 = value; NotifyPropertyChanged("Pm24"); } }
        }

        public DateTime? LastPhoto
        {
            get { return lastPhoto; }
            set { if (lastPhoto != value) { lastPhoto = value; NotifyPropertyChanged("LastPhoto"); } }
        }
        /*
        public string LastPhotoString
        {
            get 
            { 
                if (lastPhoto != null) return ((DateTime)LastPhoto).ToShortDateString();
                return "-";
            }
        }
        */ 
        /*
        public string DistanceLastPhotoString
        {
            get
            {
                if (lastPhoto == null) return DistanceString;
                return DistanceString + " | " + AppResources.TextLastPhoto+" "+LastPhotoString;
            }
        }
        */
        public double Distance
        {
            get { return distance; }
            set { if (distance != value) { distance = value; NotifyPropertyChanged("Distance"); NotifyPropertyChanged("DistanceString"); NotifyPropertyChanged("Location"); } }
        }

        public string DistanceString
        {
            get 
            {
                if (distance < 0) return "? km";
                if (distance < 1000) return (int)distance + " m";                
                return string.Format("{0:0} km", distance/1000);
            }
        }
        /*
        public string Region
        {
            get
            {
                switch(code[0])
                {
                    case 'A': return AppResources.RegionA;
                    case 'S': return AppResources.RegionS;
                    case 'C': return AppResources.RegionC;
                    case 'P': return AppResources.RegionP;
                    case 'K': return AppResources.RegionK;
                    case 'U': return AppResources.RegionU;
                    case 'L': return AppResources.RegionL;
                    case 'H': return AppResources.RegionH;
                    case 'E': return AppResources.RegionE;
                    case 'J': return AppResources.RegionJ;
                    case 'B': return AppResources.RegionB;
                    case 'M': return AppResources.RegionM;
                    case 'Z': return AppResources.RegionZ;
                    case 'T': return AppResources.RegionT;
                }
                return "";
            }
        }
        */
        /*
        public string Description
        {
            get
            {
                //string s = Region + " | " + Code;
                string s = Region;
                if (Classification == EStationClassification.NONE) return s;

                switch(Classification)
                {
                    case EStationClassification.TRAFFIC: s = s + " | " + AppResources.ClassTraffic; break;
                    case EStationClassification.INTERCITY: s = s + " | " + AppResources.ClassIntercity; break;
                    case EStationClassification.AROUNDCITY: s = s + " | " + AppResources.ClassAroundcity; break;
                    case EStationClassification.VILLAGE: s = s + " | " + AppResources.ClassVillage; break;
                    case EStationClassification.INDUSTRY: s = s + " | " + AppResources.ClassIndustry; break;
                }

                return s;
            }
        }
        */
        /*
        public string Location
        {
            get
            {
                GpsAngle la = GpsAngle.FromDouble(Position.Latitude);
                GpsAngle lo = GpsAngle.FromDouble(Position.Longitude);

                return la.ToString("ns")+", "+lo.ToString("we")+" (+"+DistanceString+")";
            }
        }
        */
        public void UpdateStatesBasedOnValues()
        {
            PGraph pg;
            if (Co.State == 0 && Co.Value != 0)
            {
                pg = new PGraph(EPElementType.CO);
                Co.State = pg.GetState(Co.Value);
            }

            if (No2.State == 0 && No2.Value != 0)
            {
                pg = new PGraph(EPElementType.NO2);
                No2.State = pg.GetState(No2.Value);
            }

            if (O3.State == 0 && O3.Value != 0)
            {
                pg = new PGraph(EPElementType.O3);
                O3.State = pg.GetState(O3.Value);
            }

            if (Pm10.State == 0 && Pm10.Value != 0)
            {
                pg = new PGraph(EPElementType.PM10);
                Pm10.State = pg.GetState(Pm10.Value);
            }

            if (So2.State == 0 && So2.Value != 0)
            {
                pg = new PGraph(EPElementType.SO2);
                So2.State = pg.GetState(So2.Value);
            }

        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Station ShallowCopy()
        {
            return (Station)this.MemberwiseClone();
        }
    }
}
