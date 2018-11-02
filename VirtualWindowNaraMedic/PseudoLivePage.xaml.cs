using MjpegProcessor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Streaming.Adaptive;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using VLC;


namespace VirtualWindowUWP
{
    public sealed partial class PseudoLivePage : Page
    {
        private MjpegDecoder mjpegDecoder;

        public PseudoLivePage()
        {
            this.InitializeComponent();
            SetVideo();
        }

        public void SetVideo()
        {
            vlc_player.HardwareAcceleration = true;
            // vlc_player.Source = "rtsp://192.168.0.10/ONVIF/MediaInput?profile=3_def_profile1";
            vlc_player.Source = "rtsp://172.20.11.46:554/stream/profile1=r";
            vlc_player.Play();
        }

    }
}
