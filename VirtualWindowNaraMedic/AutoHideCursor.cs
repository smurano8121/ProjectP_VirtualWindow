using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace VirtualWindowUWP
{
    class AutoHideCursor
    {
        private static DispatcherTimer timer;


        public static void Start()
        {
            // Set up timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += HideCursor;

            if (Window.Current.CoreWindow.PointerCursor != null)
            {
                timer.Start();
            }
        }

        public static void SwitchCursorMode()
        {
            if (Window.Current.CoreWindow.PointerCursor != null)
            {
                Window.Current.CoreWindow.PointerCursor = null;
            }
            else
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
            }
        }

        public static void AutoHider_PointerUsed(Windows.UI.Core.CoreWindow core, Windows.UI.Core.PointerEventArgs e)
        {
            if (Window.Current.CoreWindow.PointerCursor == null)
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
            }
            timer.Stop();
            timer.Start();
        }

        private static void HideCursor(object sender, object e)
        {
            if (Window.Current.CoreWindow.PointerCursor != null)
            {
                Window.Current.CoreWindow.PointerCursor = null;
            }
            timer.Stop();
        }
    }
}
