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
        private static MediaElement videoObject1;
        private static MediaElement videoObject2;
        // 天気取得の関数を呼び出す時間トリガー
        private DispatcherTimer darksky_request_manage;
        //画像透過の関数を呼び出す時間トリガー
        private DispatcherTimer transmission_image;
        //透過speedの調整用
        private int transmission_speed;
        //
        private int count;

        public PseudoLivePage()
        {
            this.InitializeComponent();
            // set MediaElement into static variable
            videoObject1 = PseudoLivePlayer1;
            videoObject2 = PseudoLivePlayer2;
            this.Loaded += UpdateVideo;
    
            // 天気取得関数の時限起動設定
            this.darksky_request_manage = new DispatcherTimer();
            this.darksky_request_manage.Interval = TimeSpan.FromSeconds(20);
            this.darksky_request_manage.Tick += UpdateVideo;
            this.darksky_request_manage.Start();

            // 画面の透過率を変える設定
            this.transmission_image = new DispatcherTimer();
            this.transmission_image.Interval = TimeSpan.FromSeconds(1);
            this.transmission_image.Tick += TransmissionImage;
            this.transmission_image.Start();
        }

        private static async void ReadVideo2(string weather, int count)
        {
            int hh = 0;
            int mm = 0;
            string fileName = "";

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

            if ( mm <= 2 || 58 <= mm )
            {
                hh = 1 + int.Parse(now.ToString("HH"));
                Debug.WriteLine(mm);
                mm = 0;
                fileName = now.ToString("MM") + "_" + hh + "0" + mm + "_" + weather + ".mp4";
            }
            else if (mm == 5) {
                hh = int.Parse(now.ToString("HH"));
                fileName = now.ToString("MM") + "_" + hh + "0" + mm + "_" + weather + ".mp4";
            }
            else
            {
                hh = int.Parse(now.ToString("HH"));
                fileName = now.ToString("MM") + "_" + hh + "" + mm + "_" + weather + ".mp4";
            }
            
            Debug.WriteLine(fileName);
            StorageFolder targetDir = await StorageFolder.GetFolderFromPathAsync("C:\\Users\\smura\\Videos\\stockvideo\\" + now.ToString("MM") + "\\" + weather);
            StorageFile targetVideo = await targetDir.GetFileAsync(fileName);

            var stream = await targetVideo.OpenAsync(Windows.Storage.FileAccessMode.Read);

            if (count == 0)
            {
                videoObject1.SetSource(stream, targetVideo.ContentType);
            }
            Debug.WriteLine(count);
            videoObject2.SetSource(stream, targetVideo.ContentType);
        }

        private async void UpdateVideo(object sender, object e)
        {
            string currentWeather = await TestSync();
            ReadVideo2(currentWeather, count);
            count++;
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
                icon = "snowy";
            }
            //foggy
            else if (weather == "fog")
            {
                icon = "foggy";
            }
            return icon;
        }

        private async void TransmissionImage(object sender, object e)
        {
            transmission_speed += 1;
            if (transmission_speed == 1) {
                videoObject1.Opacity = 0.1;
                videoObject2.Opacity = 0.1;
            }
            else if (transmission_speed == 10) {
                videoObject1.Opacity = 1;
                videoObject2.Opacity = 1;
            }
        }

    }
}
