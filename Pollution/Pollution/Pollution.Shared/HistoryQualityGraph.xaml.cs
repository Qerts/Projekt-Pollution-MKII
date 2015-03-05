using System;
using System.Windows;
using System.Windows.Input;
using Pollution.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Pollution
{
	public partial class HistoryQualityGraph : UserControl
	{
        
        double vpp;

        public HistoryQualityGraph()
		{
			// Required to initialize variables
			InitializeComponent();
        
		}

        
        public void Refresh()
        {
            if(DataContext==null || !(DataContext is PHistory)) return;
          

            PHistory h = (PHistory)DataContext;
            vpp = h.GetMaxQaulityCount() / (Height - 40);


            bar1.Height = (int)(h.GetQualityIndex(1) / vpp);
            bar2.Height = (int)(h.GetQualityIndex(2) / vpp);
            bar3.Height = (int)(h.GetQualityIndex(3) / vpp);
            bar4.Height = (int)(h.GetQualityIndex(4) / vpp);
            bar5.Height = (int)(h.GetQualityIndex(5) / vpp);
            bar6.Height = (int)(h.GetQualityIndex(6) / vpp);
            bar7.Height = (int)(h.GetQualityIndex(7) / vpp);
            bar8.Height = (int)(h.GetQualityIndex(8) / vpp);

            Canvas.SetTop(bar1, (Height - 40) - bar1.Height);
            Canvas.SetTop(bar2, (Height - 40) - bar2.Height);
            Canvas.SetTop(bar3, (Height - 40) - bar3.Height);
            Canvas.SetTop(bar4, (Height - 40) - bar4.Height);
            Canvas.SetTop(bar5, (Height - 40) - bar5.Height);
            Canvas.SetTop(bar6, (Height - 40) - bar6.Height);
            Canvas.SetTop(bar7, (Height - 40) - bar7.Height);
            Canvas.SetTop(bar8, (Height - 40) - bar8.Height);


            Canvas.SetTop(line11, Height - 40);
            Canvas.SetTop(line13, (Height - 40) - ((int)(h.GetMaxQaulityCount() / 3 / vpp)));
            Canvas.SetTop(line23, (Height - 40) - ((int)(h.GetMaxQaulityCount() / 3 * 2 / vpp)));

            Canvas.SetTop(label13, Canvas.GetTop(line13) - 13);
            Canvas.SetTop(label23, Canvas.GetTop(line23) - 13);

            label13.Text = ""+(int)(h.GetMaxQaulityCount() / 3);
            label23.Text = ""+(int)(h.GetMaxQaulityCount() / 3 * 2);

            line11.Width = Width;
            line13.Width = Width;
            line23.Width = Width;

            bar1.Width = (Width/400)*30;
            bar2.Width = (Width/400)*30;
            bar3.Width = (Width/400)*30;
            bar4.Width = (Width/400)*30;
            bar5.Width = (Width/400)*30;
            bar6.Width = (Width/400)*30;
            bar7.Width = (Width/400)*30;
            bar8.Width = (Width/400)*30;

            Canvas.SetLeft(bar1, (Width / 400) * 30);
            Canvas.SetLeft(bar2, (Width / 400) * 77);
            Canvas.SetLeft(bar3, (Width / 400) * 124);
            Canvas.SetLeft(bar4, (Width / 400) * 171);
            Canvas.SetLeft(bar5, (Width / 400) * 218);
            Canvas.SetLeft(bar6, (Width / 400) * 265);
            Canvas.SetLeft(bar7, (Width / 400) * 312);
            Canvas.SetLeft(bar8, (Width / 400) * 359);
            
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