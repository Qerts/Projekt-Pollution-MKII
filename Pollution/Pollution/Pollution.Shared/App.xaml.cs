﻿using Pollution.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
#if WINDOWS_APP
using Pollution.Flyouts;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Pollution.Common;
using System.Threading.Tasks; 
#endif

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Pollution
{

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private static StationViewModel viewModel = null;
        private ResourceLoader _resourceLoader = new ResourceLoader();

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static StationViewModel ViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (viewModel == null)
                    viewModel = new StationViewModel();

                return viewModel;
            }
        }


#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {


        


#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
#if WINDOWS_APP
            //BackgroundService.RegisterTileTask();   //mělo by zavolat a spustit úlohu na pozadí
            //BackgroundService.RegisterTask();
#endif


            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }


#if WINDOWS_APP
            //přidání extended splash
            if (e.PreviousExecutionState != ApplicationExecutionState.Running)
            {
                bool loadState = (e.PreviousExecutionState == ApplicationExecutionState.Terminated);
                ExtendedSplash extendedSplash = new ExtendedSplash(e.SplashScreen, loadState);               
                Window.Current.Content = extendedSplash;
            }
#endif
#if WINDOWS_PHONE_APP


            // Ensure the current window is active
            Window.Current.Activate();
#endif
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

#if WINDOWS_APP
            // TODO: Save application state and stop any background activity
            Task saveState = Task.Run(async () =>
            {
                await SuspensionManager.SaveAsync();
            });
            
#endif
            deferral.Complete();
        }

#if WINDOWS_APP

        //vlastni kod
        public void ShowCustomSettingFlyout()
        {
            FlyoutAbout CustomSettingFlyout = new FlyoutAbout();
            CustomSettingFlyout.Show();

        }
        public void ShowCustomSettingFlyout2()
        {
            FlyoutSettings CustomSettingFlyout2 = new FlyoutSettings();
            CustomSettingFlyout2.Show();
        }
        public void ShowCustomSettingFlyout3()
        {
            FlyoutInformations CustomSettingFlyout3 = new FlyoutInformations();
            CustomSettingFlyout3.Show();
        }
        public void ShowCustomSettingFlyout4()
        {
            FlyoutTerms CustomSettingFlyout4 = new FlyoutTerms();
            CustomSettingFlyout4.Show();
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;

        }

        private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {

            args.Request.ApplicationCommands.Add(new SettingsCommand(
                "MenuAbout", _resourceLoader.GetString("MenuAbout"), (handler) => ShowCustomSettingFlyout()));
            args.Request.ApplicationCommands.Add(new SettingsCommand(
                "MenuSettings", _resourceLoader.GetString("MenuSettings/Text"), (handler) => ShowCustomSettingFlyout2()));
            args.Request.ApplicationCommands.Add(new SettingsCommand(
                "MenuInformations", _resourceLoader.GetString("MenuInfo"), (handler) => ShowCustomSettingFlyout3())); 
            args.Request.ApplicationCommands.Add(new SettingsCommand(
                 "MenuTerms", _resourceLoader.GetString("MenuTerms"), (handler) => ShowCustomSettingFlyout4()));
        } 


#endif
        
    }
}