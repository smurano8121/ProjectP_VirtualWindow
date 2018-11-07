using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace VirtualWindowUWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
   

    // test

    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>

        public static Frame rootFrame;

        Socket socket;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(StartPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }

            var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;

            // Make titlebar as view
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            // Set active window colors
            titleBar.ForegroundColor = Windows.UI.Colors.White;
            titleBar.BackgroundColor = Windows.UI.Colors.Transparent;
            titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
            titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonHoverBackgroundColor = Windows.UI.Colors.LightGray;
            titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonPressedBackgroundColor = Windows.UI.Colors.DarkGray;

            // Enter to fullscreen mode
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();        

            // Manage key listener
            Window.Current.CoreWindow.KeyDown += KeyDownEventHandler;

            // Add mouse cursor pointer handler
            AutoHideCursor.Start();
            Window.Current.CoreWindow.PointerPressed += AutoHideCursor.AutoHider_PointerUsed;
            Window.Current.CoreWindow.PointerMoved += AutoHideCursor.AutoHider_PointerUsed;

            // Create socket datagram
            socket = new Socket();
            socket.CreateSocketListener();
            socket.setRootFrame(rootFrame);

            // Get file list of Video Page and Image page
            ImagePage.GetImageList();
            VideoPage.GetVideoList();

        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

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
            //TODO: Save application state and stop any background activity
            deferral.Complete();

            // release socket
            socket.CloseSocket();
        }

        private void KeyDownEventHandler(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            frame.ContentTransitions = new TransitionCollection();
            frame.ContentTransitions.Add(new NavigationThemeTransition());

            switch (e.VirtualKey)
            {
                // switch full screen mode
                case Windows.System.VirtualKey.F:
                    if (Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().IsFullScreenMode)
                    {
                        Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().ExitFullScreenMode();
                    }
                    else
                    {
                        Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                    }
                    break;
                // change cursor mode
                case Windows.System.VirtualKey.B:
                    AutoHideCursor.SwitchCursorMode();
                    break;
                // go to start page
                case Windows.System.VirtualKey.Escape:
                    // change mode to blank mode
                    frame.ContentTransitions = new TransitionCollection();
                    frame.ContentTransitions.Add(new NavigationThemeTransition());
                    frame.Navigate(typeof(StartPage));
                    frame.BackStack.Clear();
                    break;
                // for debug!
                case Windows.System.VirtualKey.D:
                    Debug.WriteLine(GetMode());
                    break;
            }
        }

        public static string GetMode()
        {
            return rootFrame.CurrentSourcePageType.ToString().Replace("VirtualWindowUWP.", "");
        }

        // Stretch window size to all working area
        public static void StretchWindow()
        {
            var x = 0;
            var y = 0;
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(x, y));
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryResizeView(new Size(x, y));
        }

        public static void NavigateTo(Type pageType)
        {
            rootFrame.ContentTransitions = new TransitionCollection();
            rootFrame.ContentTransitions.Add(new NavigationThemeTransition());
            rootFrame.Navigate(pageType);
        }
    }
}
