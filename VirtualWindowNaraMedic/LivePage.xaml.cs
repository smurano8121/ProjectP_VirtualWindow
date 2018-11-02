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
    public sealed partial class LivePage : Page
    {
        private MjpegDecoder mjpegDecoder;

        //public LivePage()
        //{
        //    this.InitializeComponent();
        //    mjpegDecoder = new MjpegDecoder();
        //    mjpegDecoder.FrameReady += mjpeg_FrameReady;
        //}

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    mjpegDecoder.ParseStream(new Uri("http://192.168.10.13/cgi-bin/mjpeg?framerate=15&resolution=640x480"));
        //    // Project N camera url
        //    // http://192.168.0.10/cgi-bin/mjpeg?framerate=15&resolution=640x480
        //    // http://192.168.10.13/cgi-bin/mjpeg
        //}

        //private async void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
        //{
        //    using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
        //    {
        //        await ms.WriteAsync(e.FrameBuffer);
        //        ms.Seek(0);

        //        var bmp = new BitmapImage();
        //        await bmp.SetSourceAsync(ms);

        //        //image is the Image control in XAML
        //        image.Source = bmp;
        //    }
        //}

        public LivePage()
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
