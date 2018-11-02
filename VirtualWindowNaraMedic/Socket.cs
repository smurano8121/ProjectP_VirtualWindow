using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace VirtualWindowUWP
{
    class Socket
    {
        private static String portNum = "50005";
        private Windows.Networking.Sockets.StreamSocketListener socketListener;
        private Frame rootFrame;
        Stream inStream;
        Stream outStream;
        StreamReader streamReader;
        StreamWriter streamWriter;

        public async void CreateSocketListener()
        {
            try
            {
                // Create a StreamSocketListener to start listening for TCP connections.
                socketListener = new Windows.Networking.Sockets.StreamSocketListener();
                
                // Hook up an event handler to call when connections are received.
                socketListener.ConnectionReceived += SocketListener_ConnectionReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that' s not currently in use.
                await socketListener.BindServiceNameAsync(portNum);

            }
            catch (Exception e)
            {
                Debug.WriteLine("CreateSocketListener() fault...");
                // Handle exception.
            }
        }

        public static String GetPort()
        {
            return portNum;
        }
         
        public void setRootFrame(Frame frame)
        {
            rootFrame = frame;
        }

        private async void SocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender,
        Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            

            // Read line from the remote client.
            inStream = args.Socket.InputStream.AsStreamForRead();
            streamReader = new StreamReader(inStream);
            // Add timeout function (ms)
            // streamReader.BaseStream.ReadTimeout = 3000;

            string request = await streamReader.ReadLineAsync();

            try
            {
                await CheckCommand(request, args);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Command Execution Fault...");
                Debug.WriteLine(e);
            }
        }

        // http://garicchi.com/?p=5891
        // string CheckCommand(String msg)
        private async Task CheckCommand(String msg, Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            await Task.Run(async () =>
            {
                string result = "";
                outStream = args.Socket.OutputStream.AsStreamForWrite();
                streamWriter = new StreamWriter(outStream);

                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    try
                    {
                        Debug.WriteLine(msg);
                        switch (msg)
                        {
                            case "ECHO":
                                result = await streamReader.ReadLineAsync();
                                break;
                            case "IMAGE":
                                // change mode to image mode and set the specified picture
                                rootFrame.ContentTransitions = new TransitionCollection();
                                rootFrame.ContentTransitions.Add(new NavigationThemeTransition());
                                rootFrame.Navigate(typeof(ImagePage));
                                result = "OK";
                                break;
                            case "VIDEO":
                                // change mode to video mode and set the specified video
                                rootFrame.ContentTransitions = new TransitionCollection();
                                rootFrame.ContentTransitions.Add(new NavigationThemeTransition());
                                rootFrame.Navigate(typeof(VideoPage));
                                result = "OK";
                                break;
                            case "LIVE":
                                // change mode to live mode
                                rootFrame.ContentTransitions = new TransitionCollection();
                                rootFrame.ContentTransitions.Add(new NavigationThemeTransition());
                                rootFrame.Navigate(typeof(LivePage));
                                result = "OK";
                                break;
                            case "PseudoLIVE":
                                // change mode to live mode
                                rootFrame.ContentTransitions = new TransitionCollection();
                                rootFrame.ContentTransitions.Add(new NavigationThemeTransition());
                                rootFrame.Navigate(typeof(PseudoLivePage));
                                result = "OK";
                                break;
                            case "BLANK":
                                // change mode to blank mode
                                rootFrame.ContentTransitions = new TransitionCollection();
                                rootFrame.ContentTransitions.Add(new NavigationThemeTransition());
                                rootFrame.Navigate(typeof(BlankPage));
                                result = "OK";
                                break;
                            case "HOME":
                                // change mode to blank mode
                                rootFrame.ContentTransitions = new TransitionCollection();
                                rootFrame.ContentTransitions.Add(new NavigationThemeTransition());
                                rootFrame.Navigate(typeof(StartPage));
                                rootFrame.BackStack.Clear();
                                result = "OK";
                                break;
                            case "NEXT":
                                Debug.WriteLine(App.GetMode());
                                switch (App.GetMode())
                                {
                                    case "ImagePage":
                                        ImagePage.NextImage(); break;
                                    case "VideoPage":
                                        VideoPage.NextVideo(); break;
                                    default:
                                        break;
                                }
                                result = "OK";
                                break;
                            case "PREVIOUS":
                                switch (App.GetMode())
                                {
                                    case "ImagePage":
                                        ImagePage.PreviousImage(); break;
                                    case "VideoPage":
                                        VideoPage.PreviousVideo(); break;
                                    default:
                                        break;
                                }
                                result = "OK";
                                break;
                            case "SET_IMAGE_BY_ID":
                                {
                                    string id = await streamReader.ReadLineAsync();
                                    ImagePage.SetImageIndex(int.Parse(id));
                                    result = "OK";
                                    break;
                                }
                            case "SET_VIDEO_BY_ID":
                                {
                                    string id = await streamReader.ReadLineAsync();
                                    VideoPage.SetVideoIndex(int.Parse(id));
                                    result = "OK";
                                    break;
                                }
                            case "TOGGLE_FULLSCREEN":
                                {
                                    if (Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().IsFullScreenMode)
                                    {
                                        Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().ExitFullScreenMode();
                                    }
                                    else
                                    {
                                        Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                                    }
                                    break;
                                }
                            case "GET_MODE":
                                result = App.GetMode();
                                break;
                            case "GET_IMAGE_THUMBS":
                                result = await SendImageThumbsAsync();
                                break;
                            case "GET_VIDEO_THUMBS":
                                result = await SendVideoThumbsAsync();
                                break;
                            default:
                                result = "Invalid command.";
                                break;
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Failed to send command.");
                        result = "Failed to send command.";
                    }
                });

                // send back result strings
                //await writer.WriteLineAsync(result);
                //await writer.FlushAsync();
                streamWriter.WriteLine(result);
                streamWriter.Flush();

                Debug.WriteLine(result);
            });

        }

        // ref: https://social.msdn.microsoft.com/Forums/en-US/ec977327-291e-4409-8ef7-2cefeca7698c/problem-using-bitmapimagesetsourceasync?forum=winappswithcsharp
        private async Task<string> SendImageThumbsAsync()
        {
            List<StorageItemThumbnail> thumbs = ImagePage.GetThumbnailList();
            
            // First, send the number of thumbnail (images)
            streamWriter.WriteLine(thumbs.Count);
            streamWriter.Flush();

            // Second, send bitmap with Base64.
            for (int i = 0; i < thumbs.Count; i++)
            {
                
                BitmapImage img = new BitmapImage();
                var dr = new DataReader(thumbs[i].GetInputStreamAt(0));
                byte[] bytes = new byte[thumbs[i].Size];
                await dr.LoadAsync((uint)thumbs[i].Size);
                dr.ReadBytes(bytes);

                // Send base64
                streamWriter.WriteLine(System.Convert.ToBase64String(bytes));
                streamWriter.Flush();
            }
            
            // Finally, send status code.
            return "OK";
        }

        private async Task<string> SendVideoThumbsAsync()
        {
            List<StorageItemThumbnail> thumbs = VideoPage.GetThumbnailList();

            // First, send the number of thumbnail (images)
            streamWriter.WriteLine(thumbs.Count);
            streamWriter.Flush();

            // Second, send bitmap with Base64.
            for (int i = 0; i < thumbs.Count; i++)
            {

                BitmapImage img = new BitmapImage();
                var dr = new DataReader(thumbs[i].GetInputStreamAt(0));
                byte[] bytes = new byte[thumbs[i].Size];
                await dr.LoadAsync((uint)thumbs[i].Size);
                dr.ReadBytes(bytes);

                // Send base64
                streamWriter.WriteLine(System.Convert.ToBase64String(bytes));
                streamWriter.Flush();
            }

            // Finally, send status code.
            return "OK";
        }

        async Task<byte[]> Convert(IRandomAccessStream s)
        {
            var dr = new DataReader(s.GetInputStreamAt(0));
            var bytes = new byte[s.Size];
            await dr.LoadAsync((uint)s.Size);
            dr.ReadBytes(bytes);
            return bytes;
        }

        public void CloseSocket()
        {
            socketListener.Dispose();
        }
    }
}
