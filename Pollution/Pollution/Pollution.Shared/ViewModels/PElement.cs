using System;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace Pollution.ViewModels
{

    public enum EPElementType
    {
        SO2,
        NO2,
        CO,
        O3,
        PM10,
        PM1024
    }

    public class PElement
    {
        public double Value { set; get; }
        public int State { set; get; }
        public EPElementType Type { get; set; }
        public PGraph Graph { get; set; }

        public PElement(EPElementType type)
        {
            Type = type;
            Value = 0.0F;
            State = 0;
            Graph = new PGraph(Type);

        }

        public PElement(float value, int state, EPElementType type)
        {
            Value = value;
            State = state;
            Type = type;
            Graph = new PGraph(Type);
        }

        public override string ToString()
        {            
            if (Value < 0) return "";
            return string.Format("{0:0.0}", Value);
        }
    }
}
