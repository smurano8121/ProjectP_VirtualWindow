using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Animation;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.System;
using Windows.Networking.Connectivity;
using Windows.Networking;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VirtualWindowUWP
{
    public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();

            // Add KeyDown event handler into CoreWindow
            // Have to remove this handler when this page is unloaded.
            Window.Current.CoreWindow.KeyDown += KeyDownHandle;
            this.Unloaded += (sender, e) =>
            {
                Window.Current.CoreWindow.KeyDown -= KeyDownHandle;
            };

            // Load information view properties
            LoadInformation();
        }

        public void NavigateButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Type pageType = null;
            switch (button.Name)
            {
                case "liveButton":
                    pageType = typeof(PseudoLivePage); break;
                case "blankButton":
                    pageType = typeof(BlankPage); break;
                case "imageButton":
                    pageType = typeof(ImagePage); break;
                case "videoButton":
                    pageType = typeof(VideoPage); break;
            }
            App.NavigateTo(pageType);
        }

        private void LoadInformation()
        {
            TextBlock ip = ip_text;
            TextBlock pt = port_text;
            TextBlock vr = version_text;

            // Load local ip address
            var profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile != null && profile.NetworkAdapter != null)
            {
                foreach (var info in NetworkInformation.GetHostNames())
                {
                    if (info.Type == HostNameType.Ipv4 &&
                        profile.NetworkAdapter.NetworkAdapterId == info.IPInformation.NetworkAdapter.NetworkAdapterId)
                    {
                        // IPv4 かつ 現在接続中のアダプターID
                        ip.Text = info.CanonicalName;
                    }
                }
            }

            // Load socket port address
            pt.Text = Socket.GetPort();

            // Load app version
            var versionInfo = Windows.ApplicationModel.Package.Current.Id.Version;
            string version = string.Format(
                   "{0}.{1}.{2}",
                   versionInfo.Major, versionInfo.Minor,
                   versionInfo.Build);
            vr.Text = version;
        }

        private void KeyDownHandle(object send, Windows.UI.Core.KeyEventArgs e)
        {
            switch (e.VirtualKey)
            {
                case VirtualKey.K:
                    Grid gr = information_view;
                    switch (gr.Visibility)
                    {
                        case Visibility.Collapsed:
                            gr.Visibility = Visibility.Visible;
                            break;
                        default:
                            gr.Visibility = Visibility.Collapsed;
                            break;
                    }
                    break;
            }
        }
    }
}
