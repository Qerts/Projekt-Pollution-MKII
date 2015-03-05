using System;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using Pollution.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;

namespace Pollution
{
	public partial class PElementGraph : UserControl
	{
        private PElement element;
        private bool animated;
        private double vpp;
        
        public PElementGraph()
		{
			// Required to initialize variables
			InitializeComponent();

           // IndicatorAnimation.Completed += new EventHandler(IndicatorAnimation_Completed);
            IndicatorAnimation.Completed +=IndicatorAnimation_Completed;
            Refresh();
		}

        

        
        public void Refresh()
        {
            if(DataContext==null || !(DataContext is PElement)) return;
          
            PElement el = (PElement)DataContext;

            if (el.Equals(element))
            {
                return;
            }

            if (el.State > 6)
            {
                //filterDisabled.Visibility = Windows.UI.Xaml.Visibility.Visible;
                LayoutRoot.Opacity = 0.2;
            }
            else
            {
                //filterDisabled.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                LayoutRoot.Opacity = 1;
            }
                
            filterDisabled.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            int max = el.Graph.Limit56;
            if (el.Value > max) max = (int)el.Value;
            max = (int)(max * 1.1);

            vpp = max / Height;

            e1.Height = (int)(el.Graph.Limit12 / vpp);
            e2.Height = (int)((el.Graph.Limit23 - el.Graph.Limit12) / vpp);
            e3.Height = (int)((el.Graph.Limit34 - el.Graph.Limit23) / vpp);
            e4.Height = (int)((el.Graph.Limit45 - el.Graph.Limit34) / vpp);
            e5.Height = (int)((el.Graph.Limit56 - el.Graph.Limit45) / vpp);
            e6.Height = Height - e1.Height - e2.Height - e3.Height - e4.Height - e5.Height;
            //e6.Height = (int)((max - el.Graph.Limit56) / vpp);

            filter.Height = Height;
            filterDisabled.Height = Height+3;

            l12.Text = el.Graph.Limit12.ToString();
            l23.Text = el.Graph.Limit23.ToString();
            l34.Text = el.Graph.Limit34.ToString();
            l45.Text = el.Graph.Limit45.ToString();
            l56.Text = el.Graph.Limit56.ToString();

            int topOffset;
            if (Height < 200)
            {
                l12.FontSize = 7;
                l23.FontSize = 7;
                l34.FontSize = 7;
                l45.FontSize = 7;
                l56.FontSize = 7;
                topOffset = 3;
            }
            else
            {
                topOffset = 0;
            }




            Canvas.SetTop(l12, 3+e2.Height + e3.Height + e4.Height + e5.Height + e6.Height + topOffset);
            Canvas.SetTop(l23, e3.Height + e4.Height + e5.Height + e6.Height + topOffset);
            Canvas.SetTop(l34, e4.Height + e5.Height + e6.Height + topOffset);
            Canvas.SetTop(l45, e5.Height + e6.Height + topOffset);
            Canvas.SetTop(l56, e6.Height + topOffset);

            if (el.State > 6)
            {
                valueIndicator.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                valueIndicator.Visibility = Windows.UI.Xaml.Visibility.Visible;

                // Canvas.SetTop(valueIndicator, Height - (el.Value / vpp) - 10);


                
                Canvas.SetTop(valueIndicator, Height - 5);

                (IndicatorAnimation.Children[0] as DoubleAnimation).From = Height - 5;
                (IndicatorAnimation.Children[0] as DoubleAnimation).To = Height - (el.Value / vpp) - 5;

                IndicatorAnimation.BeginTime = new TimeSpan(1500);
                IndicatorAnimation.Begin();
                animated = true;
                
                
            }
            //this.LayoutRoot.Visibility = System.Windows.Visibility.Visible;
            element = el;

            
        }

        void IndicatorAnimation_Completed(object sender, object e)
        {
            animated = false;
            //Canvas.SetTop(valueIndicator, Height - (element.Value / vpp) - 10);
            
        }

        private void UserControl_LayoutUpdated(object sender, EventArgs e)
        {
            Refresh();
        }

        private void UserControl_LayoutUpdated(object sender, object e)
        {
            Refresh();
        }
	}
}