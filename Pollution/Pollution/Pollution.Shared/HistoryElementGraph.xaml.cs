using System;
using System.Windows;
using System.Windows.Input;
using Pollution.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;

namespace Pollution
{
    public partial class HistoryElementGraph : UserControl
    {

        double vpp, vpt;

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register("ElementType", typeof(string), typeof(HistoryElementGraph), null);


  
        public string ElementType
        {
            get { return (string)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }

        


        private EPElementType et;
        private PElement el;

        public HistoryElementGraph()
        {
            // Required to initialize variables
            InitializeComponent();

           
        }


        public void Refresh()
        {
            dataPath.Height = Height - 40;
            dataPath.Width = Width;

            if (ElementType == "SO2") et = EPElementType.SO2;
            if (ElementType == "NO2") et = EPElementType.NO2;
            if (ElementType == "CO") et = EPElementType.CO;
            if (ElementType == "O3") et = EPElementType.O3;
            if (ElementType == "PM10") et = EPElementType.PM10;
            if (ElementType == "PM1024") et = EPElementType.PM1024;

            el = new PElement(et);

            if (DataContext == null || !(DataContext is PHistory)) return;

            PHistory h = (PHistory)DataContext;

            double max = h.GetMaxElementValue(et) * 1.05;

            if (max == -1) return;

            if (max <= el.Graph.Limit12) max = el.Graph.Limit12 * 1.1;

            int koef = 1;
            if (max < 70) koef = 3; else koef = 10;

            vpp = max / (Height - 40);

            List<PElement> l = h.GetListValues(et);

            vpt = (double)l.Count / Width;

            PathGeometry geometry = new PathGeometry();
            PathFigure figure = null;

            bool line = false;
            bool move = true;
            int mc = 0;
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i] == null || l[i].Value == -1)
                {
                    mc++;
                    line = false;
                    move = true;
                    
                    continue;
                }

                if (line == false)
                {
                    if (mc <= 5 && mc > 0)
                    {
                        move = false;
                        line = true;
                    }
                    else
                    {
                        move = true;
                        line = false;
                        if (figure != null) { geometry.Figures.Add(figure); }
                        figure = null;
                    }
                    mc = 0;
                }

                if (move)
                {
                    figure = new PathFigure();
                    figure.StartPoint = new Point(i / vpt, dataPath.Height - (Math.Round(l[i].Value/koef)*koef / vpp));
                    move = false;
                    line = true;
                    continue;
                }
                if (line && figure != null)
                {
                    if (i % 2 == 0)
                    {
                        figure.Segments.Add(new LineSegment()
                        {
                            Point = new Point(i / vpt, dataPath.Height - (Math.Round(l[i].Value / koef) * koef / vpp))
                        });
                    }

                }

            }
            if (figure != null) { geometry.Figures.Add(figure); }


            dataPath.Data = geometry;

            #region YLabels

            Canvas.SetTop(line11, Height - 40);
            line11.Width = Width;

            if (el.Graph.Limit12 <= max)
            {
                Canvas.SetTop(line12, (Height - 40) - ((int)(el.Graph.Limit12 / vpp)));
                Canvas.SetTop(label12, Canvas.GetTop(line12) - 13);
                label12.Text = el.Graph.Limit12 + " µg/m³";
                line12.Width = Width;
                Canvas.SetLeft(label12, Width - 70);

                line12.Visibility = Visibility.Visible;
                label12.Visibility = Visibility.Visible;
            }
            else
            {
                line12.Visibility = Visibility.Collapsed;
                label12.Visibility = Visibility.Collapsed;
            }

            if (el.Graph.Limit23 <= max)
            {
                Canvas.SetTop(line23, (Height - 40) - ((int)(el.Graph.Limit23 / vpp)));
                Canvas.SetTop(label23, Canvas.GetTop(line23) - 13);
                label23.Text = el.Graph.Limit23 + " µg/m³";
                line23.Width = Width;
                Canvas.SetLeft(label23, Width - 70);

                line23.Visibility = Visibility.Visible;
                label23.Visibility = Visibility.Visible;
            }
            else
            {
                line23.Visibility = Visibility.Collapsed;
                label23.Visibility = Visibility.Collapsed;
            }

            if (el.Graph.Limit34 <= max)
            {
                Canvas.SetTop(line34, (Height - 40) - ((int)(el.Graph.Limit34 / vpp)));
                Canvas.SetTop(label34, Canvas.GetTop(line34) - 13);
                label34.Text = el.Graph.Limit34 + " µg/m³";
                line34.Width = Width;
                Canvas.SetLeft(label34, Width - 70);

                line34.Visibility = Visibility.Visible;
                label34.Visibility = Visibility.Visible;
            }
            else
            {
                line34.Visibility = Visibility.Collapsed;
                label34.Visibility = Visibility.Collapsed;
            }

            if (el.Graph.Limit45 <= max)
            {
                Canvas.SetTop(line45, (Height - 40) - ((int)(el.Graph.Limit45 / vpp)));
                Canvas.SetTop(label45, Canvas.GetTop(line45) - 13);
                label45.Text = el.Graph.Limit45 + " µg/m³";
                line45.Width = Width;
                Canvas.SetLeft(label45, Width - 70);

                line45.Visibility = Visibility.Visible;
                label45.Visibility = Visibility.Visible;
            }
            else
            {
                line45.Visibility = Visibility.Collapsed;
                label45.Visibility = Visibility.Collapsed;
            }

            if (el.Graph.Limit56 <= max)
            {
                Canvas.SetTop(line56, (Height - 40) - ((int)(el.Graph.Limit56 / vpp)));
                Canvas.SetTop(label56, Canvas.GetTop(line56) - 13);
                label56.Text = el.Graph.Limit56 + " µg/m³";
                line56.Width = Width;
                Canvas.SetLeft(label56, Width - 70);

                line56.Visibility = Visibility.Visible;
                label56.Visibility = Visibility.Visible;
            }
            else
            {
                line56.Visibility = Visibility.Collapsed;
                label56.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region Xlabels

            int day = DateTime.Now.Hour;

            Canvas.SetLeft(subline1, (l.Count-day)/vpt);
            Canvas.SetLeft(sublabel1, Canvas.GetLeft(subline1) - 20);            
            sublabel1.Text = DateTime.Now.AddHours(-1 * day).ToString("d");
            subline1.Height = Height - 40;
            sublabel1.Width = Width;
            Canvas.SetTop(sublabel1, subline1.Height + 4);

            //interval
            int day1 = ((int)(l.Count / 24) - 1) / 4 * 24;

            Canvas.SetLeft(subline2, (l.Count - day1 - day) / vpt);
            Canvas.SetLeft(sublabel2, Canvas.GetLeft(subline2) - 20);
            sublabel2.Text = DateTime.Now.AddHours(-day1 - day).ToString("d");
            subline2.Height = Height - 40;
            Canvas.SetTop(sublabel2, subline2.Height + 4); 

            Canvas.SetLeft(subline3, (l.Count - (day1 * 2) - day) / vpt);
            Canvas.SetLeft(sublabel3, Canvas.GetLeft(subline3) - 20);
            sublabel3.Text = DateTime.Now.AddHours((-2 * day1) - day).ToString("d");
            subline3.Height = Height - 40;
            Canvas.SetTop(sublabel3, subline3.Height + 4); 

            Canvas.SetLeft(subline4, (l.Count - (day1 * 3) - day) / vpt);
            Canvas.SetLeft(sublabel4, Canvas.GetLeft(subline4) - 20);
            sublabel4.Text = DateTime.Now.AddHours((-3 * day1) - day).ToString("d");
            subline4.Height = Height - 40;
            Canvas.SetTop(sublabel4, subline4.Height + 4); 

            Canvas.SetLeft(subline5, (l.Count - (day1 * 4) - day) / vpt);
            Canvas.SetLeft(sublabel5, Canvas.GetLeft(subline5) - 20);
            sublabel5.Text = DateTime.Now.AddHours((-4 * day1) - day).ToString("d");
            subline5.Height = Height - 40;
            Canvas.SetTop(sublabel5, subline5.Height + 4); 

            #endregion

        }



        private void UserControl_LayoutUpdated(object sender, EventArgs e)
        {
            //Refresh();
        }
        private void UserControl_LayoutUpdated(object sender, object e)
        {
            //Refresh();
        }

    }
}