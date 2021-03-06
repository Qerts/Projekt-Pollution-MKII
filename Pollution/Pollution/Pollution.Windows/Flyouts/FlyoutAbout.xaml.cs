﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace Pollution.Flyouts
{
    public sealed partial class FlyoutAbout : SettingsFlyout
    {
        private ResourceLoader _resourceLoader = new ResourceLoader();
        public FlyoutAbout()
        {
            this.InitializeComponent();


            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            
            versionTextValue.Text = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            publisherTextValue.Text = package.PublisherDisplayName;

            updateTime.Text = App.ViewModel.LastPositionTime.ToString();
            
            //var settings = IsolatedStorageSettings.ApplicationSettings;

            versionTextValue.Text = Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor + "." + Package.Current.Id.Version.Build + "." + Package.Current.Id.Version.Revision;

        }

        private async void Image_Tap(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://data.garvis.cz/garvis/billboard/link/pollution"));
        }


        /// <summary>
        /// Metoda pro přesměrování aplikace do IE s adresou autora
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.garvis.cz"));
        }

        /// <summary>
        /// Metoda pro přesměrování aplikace do IE s adresou autora
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonFB_Click(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.facebook.com/garviscz"));
        }

        /// <summary>
        /// Metoda pro přesměrování aplikace do IE s adresou autora
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.smogalarm.cz"));
        }

        /// <summary>
        /// Metoda pro přesměrování aplikace do Marketplace pro její hodnocení
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonReview_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=69cee4e3-c46f-47c7-80a6-bf69eba36662"));//ms-windows-store:REVIEW?PFN=9d25fef6-d64d-4963-92da-ee8d102f3b6b"));
        }
    }
}
