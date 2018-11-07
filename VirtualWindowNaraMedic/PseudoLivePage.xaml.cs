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
        // The list which contains stored videos in video library.
        private static IReadOnlyList<StorageFile> storedVideo;
        // File number index of stored video which is shown in Media Element.
        private static int videoIndex = 0;
        // Media element static object
        private static MediaElement videoObject;
        // Thumbnail object
        private static List<StorageItemThumbnail> thumbnailList;
        // Storage File Querry for change detecting
        private static StorageFileQueryResult queryResult;
        // 天気取得の関数を呼び出す時間トリガー
        private DispatcherTimer tikeda;


        public PseudoLivePage()
        {
            this.InitializeComponent();

            // set MediaElement into static variable
            videoObject = PseudoLivePlayer;

            // Show first image file stored in picture library.
            // Note: "first image" means the top file when files are sorted by Name.
            ReadVideo();

            // 天気取得関数の時限起動設定
            this.tikeda = new DispatcherTimer();
            this.tikeda.Interval = TimeSpan.FromSeconds(30);
            this.tikeda.Tick += UpdateVideo;
            this.tikeda.Start();
        }

        public static async void GetVideoList()
        {
            // Create file query for change detection
            var options = new Windows.Storage.Search.QueryOptions
            {
                FolderDepth = Windows.Storage.Search.FolderDepth.Deep
            };

            // Add change detection event listener
            queryResult = videoLibrary.CreateFileQueryWithOptions(options);
            queryResult.ContentsChanged += QueryContentsChanged;

            // Read videos
            storedVideo = await queryResult.GetFilesAsync();

            // // get tumbnails
            UpdateThumbs();
        }

        private static async void ReadVideo()
        {
            StorageFolder targetDir = await StorageFolder.GetFolderFromPathAsync("C:\\Users\\smura\\Videos\\virtualWindow");
            StorageFile targetVideo = await targetDir.GetFileAsync("aa.mp4");
            var stream = await targetVideo.OpenAsync(Windows.Storage.FileAccessMode.Read);
            videoObject.SetSource(stream, targetVideo.ContentType);
        }

        private static async void ReadVideo2(string weather)
        {
            DateTime now = DateTime.Now;
            int month = now.Month;
            int hour = now.Hour;
            int minute = now.Minute;
            string fileName = month.ToString() + "_" + weather + "_" + now.ToString("HH") + ".mp4";
            Debug.WriteLine(fileName);
            StorageFolder targetDir = await StorageFolder.GetFolderFromPathAsync("C:\\Users\\smura\\Videos\\" + month + "\\" + weather);
            StorageFile targetVideo = await targetDir.GetFileAsync(fileName);

            var stream = await targetVideo.OpenAsync(Windows.Storage.FileAccessMode.Read);

            videoObject.SetSource(stream, targetVideo.ContentType);
        }

        public static async void UpdateThumbs()
        {
            thumbnailList = new List<StorageItemThumbnail>();
            foreach (StorageFile file in storedVideo)
            {
                // Get thumbnail
                const uint requestedSize = 350;
                const ThumbnailMode thumbnailMode = ThumbnailMode.VideosView;
                const ThumbnailOptions thumbnailOptions = ThumbnailOptions.UseCurrentScale;
                var tmp = await file.GetThumbnailAsync(thumbnailMode, requestedSize, thumbnailOptions);
                thumbnailList.Add(tmp);
            }
        }

        private static async void QueryContentsChanged(Windows.Storage.Search.IStorageQueryResultBase sender, object args)
        {
            Debug.WriteLine("Change detected");

            // Reset index
            videoIndex = 0;

            // Read videos
            storedVideo = null;
            storedVideo = await queryResult.GetFilesAsync();

            // get tumbnails
            UpdateThumbs();
        }

        public static void SetVideoIndex(int i)
        {
            videoIndex = i;
            ReadVideo();
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
