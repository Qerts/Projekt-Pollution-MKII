using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;

namespace Pollution.ViewModels
{
    public class PHistory
    {

        private int[] qualityIndexes;
        public List<PElement> SO2;
        public List<PElement> NO2;
        public List<PElement> CO;
        public List<PElement> O3;
        public List<PElement> PM10;
        public List<PElement> PM1024;
        public List<int> Quality;        

        private int maxQualityCount;

        
        private PGraph refer = new PGraph(EPElementType.SO2);
                
        public PGraph Referenced { get { return refer; } }

        private double maxSO2 = -1;
        private double maxNO2 = -1;
        private double maxCO = -1;
        private double maxO3 = -1;
        private double maxPM10 = -1;
        private double maxPM1024 = -1;


        public PHistory()
        {
            qualityIndexes = new int[9];
            SO2 = new List<PElement>();
            NO2 = new List<PElement>();
            CO = new List<PElement>();
            O3 = new List<PElement>();
            PM10 = new List<PElement>();
            PM1024 = new List<PElement>();
            Quality = new List<int>();

            maxQualityCount = 0;
        }

        public void SetQualityIndex(int i, int count)
        {
            if(i < qualityIndexes.Length) qualityIndexes[i] = count;
            maxQualityCount = Math.Max(maxQualityCount, count);
        }

        public int GetQualityIndex(int i)
        {
            if(i < qualityIndexes.Length) return qualityIndexes[i];
            return 0;
        }

        public int GetMaxQaulityCount()
        {
            return maxQualityCount;
        }

        public void AddQualityValue(int q)
        {
            Quality.Add(q);
        }

        public void AddListValue(EPElementType t, PElement p)
        {
            switch (t)
            {
                case EPElementType.SO2: SO2.Add(p); if(p!=null) maxSO2 = Math.Max(maxSO2, p.Value); break;
                case EPElementType.NO2: NO2.Add(p); if (p != null) maxNO2 = Math.Max(maxNO2, p.Value); break;
                case EPElementType.CO: CO.Add(p); if (p != null) maxCO = Math.Max(maxCO, p.Value); break;
                case EPElementType.O3: O3.Add(p); if (p != null) maxO3 = Math.Max(maxO3, p.Value); break;
                case EPElementType.PM10: PM10.Add(p); if (p != null) maxPM10 = Math.Max(maxPM10, p.Value); break;
                case EPElementType.PM1024: PM1024.Add(p); if (p != null) maxPM1024 = Math.Max(maxPM1024, p.Value); break;

            }
        }

        public double GetMaxElementValue(EPElementType t)
        {
            
            switch (t)
            {
                case EPElementType.SO2: return maxSO2;
                case EPElementType.NO2: return maxNO2;
                case EPElementType.CO: return maxCO;
                case EPElementType.O3: return maxO3;
                case EPElementType.PM10: return maxPM10;
                case EPElementType.PM1024: return maxPM1024;

            }
            return -1;

        }

        public List<PElement> GetListValues(EPElementType t)
        {
            List<PElement> l = null;
            switch (t)
            {
                case EPElementType.SO2: l = SO2; break;
                case EPElementType.NO2: l = NO2; break;
                case EPElementType.CO: l = CO; break;
                case EPElementType.O3: l = O3; break;
                case EPElementType.PM10: l = PM10; break;
                case EPElementType.PM1024: l = PM1024; break;

            }

            return l;

        }
        


    }
}
