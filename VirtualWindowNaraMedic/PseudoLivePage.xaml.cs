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
using Windows.Storage;
using Windows.System;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using System.Diagnostics;
using Windows.Web.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace VirtualWindowUWP
{
    public sealed partial class PseudoLivePage : Page
    {
        // To get video library, we have to declare the function in app manifest.
        private static StorageFolder videoLibrary = KnownFolders.VideosLibrary;
        // Media element static object
        private static MediaElement videoObject;
        // 天気取得の関数を呼び出す時間トリガー
        private DispatcherTimer darksky_request_manage;
        private static string hh;

        public PseudoLivePage()
        {
            this.InitializeComponent();
            // set MediaElement into static variable
            videoObject = PseudoLivePlayer;

            this.Loaded += UpdateVideo;

            // 天気取得関数の時限起動設定
            this.darksky_request_manage = new DispatcherTimer();
            //this.darksky_request_manage.Interval = TimeSpan.FromSeconds(30);
            this.darksky_request_manage.Interval = TimeSpan.FromMinutes(5);
            this.darksky_request_manage.Tick += UpdateVideo;
            this.darksky_request_manage.Start();
        }

        private static async void ReadVideo2(string weather)
        {
            int hh = 0;
            int mm = 0;
            
            DateTime now = DateTime.Now;

            //例)8,9,10,11,12→10 | 13.14,15,16,17→15 | 18,19,20,21,22→20
            int real_minute = int.Parse(now.ToString("mm"));
            int judge = real_minute % 10;
            if (judge == 1) { mm = real_minute - 1; }
            else if (judge == 2) { mm = real_minute - 2; }
            else if (judge == 3) { mm = real_minute + 2; }
            else if (judge == 4) { mm = real_minute + 1; }
            else if (judge == 6) { mm = real_minute - 1; }
            else if (judge == 7) { mm = real_minute - 2; }
            else if (judge == 8) { mm = real_minute + 2; }
            else if (judge == 9) { mm = real_minute + 1; }
            else { mm = real_minute;  }

            if (mm == 60) { hh = 1 + int.Parse(now.ToString("HH")); }
            else { hh = int.Parse(now.ToString("HH")); }
            string fileName = now.ToString("MM") + "_" + weather + "_" + hh + "_" + mm + ".mp4";
            Debug.WriteLine(fileName);
            StorageFolder targetDir = await StorageFolder.GetFolderFromPathAsync("C:\\Users\\smura\\Videos\\" + now.ToString("MM") + "\\" + weather);
            StorageFile targetVideo = await targetDir.GetFileAsync(fileName);

            var stream = await targetVideo.OpenAsync(Windows.Storage.FileAccessMode.Read);

            videoObject.SetSource(stream, targetVideo.ContentType);
        }

        private async void UpdateVideo(object sender, object e)
        {
            string currentWeather = await TestSync();
            ReadVideo2(currentWeather);
        }

        private static async Task<string> TestSync()
        {
            string endpoint = string.Format("https://api.darksky.net/forecast/579137f0816593c1d256911bc1c62f0f/34.8031949,135.7787311");

            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(new Uri(endpoint));
            string json = await result.Content.ReadAsStringAsync();

            //jsonを解析して結果を取得する
            JObject root = JObject.Parse(json);
            string weather = root["currently"]["icon"].ToString();

            var icon = "";

            //sunny
            if (weather == "clear-day" || weather == "clear-night" || weather == "partly-cloudy-day" || weather == "partly-cloudy-night")
            {
                icon = "sunny";

            }
            //cloudy
            else if (weather == "cloudy" || weather == "wind")
            {
                icon = "cloudy";
            }
            //rainny
            else if (weather == "rain")
            {
                icon = "rainny";
            }
            //snow
            else if (weather == "snow" || weather == "sleet")
            {
                icon = "sonwly";
            }
            //foggy
            else if (weather == "fog")
            {
                icon = "foggy";
            }

            return icon;
        }

    }
}
