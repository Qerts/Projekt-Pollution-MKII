using System;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace Pollution.ViewModels
{
    public class PGraph
    {
        public int State1 { get { return 1; } }
        public int State2 { get { return 2; } }
        public int State3 { get { return 3; } }
        public int State4 { get { return 4; } }
        public int State5 { get { return 5; } }
        public int State6 { get { return 6; } }
        public int State7 { get { return 7; } }
        public int State8 { get { return 8; } }

        public int Limit12 { get; set; }
        public int Limit23 { get; set; }
        public int Limit34 { get; set; }
        public int Limit45 { get; set; }
        public int Limit56 { get; set; }


        private EPElementType type;

        public PGraph()
        {
        }

        public PGraph(EPElementType type)
        {
            this.type = type;

            switch (type)
            {
                case EPElementType.SO2:
                    Limit12 = 25;
                    Limit23 = 50;
                    Limit34 = 120;
                    Limit45 = 350;
                    Limit56 = 500;
                    break;
                case EPElementType.NO2:
                    Limit12 = 25;
                    Limit23 = 50;
                    Limit34 = 100;
                    Limit45 = 200;
                    Limit56 = 400;
                    break;
                case EPElementType.CO:
                    Limit12 = 1000;
                    Limit23 = 2000;
                    Limit34 = 4000;
                    Limit45 = 10000;
                    Limit56 = 30000;
                    break;
                case EPElementType.O3:
                    Limit12 = 33;
                    Limit23 = 65;
                    Limit34 = 120;
                    Limit45 = 180;
                    Limit56 = 240;
                    break;
                case EPElementType.PM10:
                    Limit12 = 20;
                    Limit23 = 40;
                    Limit34 = 70;
                    Limit45 = 90;
                    Limit56 = 180;
                    break;




            }

        }

        public int GetState(double v)
        {
            if (v < Limit12) return 1;
            if (v < Limit23) return 2;
            if (v < Limit34) return 3;
            if (v < Limit45) return 4;
            if (v < Limit56) return 5;
            return 6;

        }

    }
}
