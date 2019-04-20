using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Gma.System.MouseKeyHook;
using NotepadMonitor.Classes;
using System.Windows.Forms;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace NotepadMonitor
{                
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //todo mvvm
        private void ShowLabel()
        {
            if (IzActive())
            {
                tbOn.Visibility = Visibility.Visible;
                tbOff.Visibility = Visibility.Hidden;
            }
            else
            {
                tbOn.Visibility = Visibility.Hidden;
                tbOff.Visibility = Visibility.Visible;
            }
        }

        private bool InNotepad
        {
            get => _inNotepad;
            set
            {
                _inNotepad = value; 
                ShowLabel();
            }
        }

        private bool HotkeyPressed
        {
            get => _hotkeyPressed;
            set
            {
                _hotkeyPressed = value; 
                ShowLabel();
            }
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientRect(IntPtr hwnd, ref Rect lpRect);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        //public struct LongPoint
        //{
        //    public long X { get; set; }
        //    public long Y { get; set; }
        //}


        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref System.Drawing.Point lpPoint);

        //todo viewmodel

        private bool _inNotepad = false;
        private bool _hotkeyPressed = false;
        private IntPtr _notepadHandle;
        private bool IzActive()  //"IsActive" name is occupied
        {
            return InNotepad && HotkeyPressed;
        }


        private void StartKbdHook()
        {
            
            IKeyboardMouseEvents events = Hook.GlobalEvents();
            events.KeyDown += Events_KeyDown; ;
            events.KeyPress += Events_KeyPress;
            //events.KeyUp += Events_KeyUp;
        }

        //todo handle arrows, del etc etc or read text directly from notepad
        private void Events_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!IzActive()) return;
            var c = e.KeyChar;
            //Logger.Log(((int)c).ToString());
            if (_capturedText.Length > 0 && c == 8)
            {
                _capturedText = _capturedText.Remove(_capturedText.Length - 1);
            }
            else _capturedText += e.KeyChar;
        }

        private string _capturedText = "";

        private void CallApi(string text)
        {

            //text =
            //    "Patient reports no frequent nosebleeds, no nose problems, and no sinus problems: congestion. She reports dry mouth but reports no sore throat, no bleeding gums, no snoring, no mouth ulcers, and no teeth problems. She reports arthralgia/joint pain (right knee) but reports no muscle aches, no muscle weakness, no back pain, and no swelling in the extremities.";


            if (string.IsNullOrEmpty(text))
            {
                Logger.Log("Nothing to send");
                return;
            }
            //const string uri = "https://sparrow.droicelabs.com/api/v1/icds";
            const string uri = "https://sparrow-dev.droicelabs.com/api/v1/icds";
            var potsData = "{" + "\"text\": \"" + text + "\"" + "}";
            Logger.Log("Calling API with text:" + text);
            using (var wc = new WebClient())
            {
                try
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    wc.Headers[HttpRequestHeader.Authorization] = "2B209811-574D-4CE0-9D41-0D31B0B43B9D";
                    var result = wc.UploadString(uri, potsData);
                    //Logger.Log(wc.ResponseHeaders.ToString());
                    Logger.Log("API response: " + result);
                }
                catch (Exception e)
                {
                    Logger.Log("ERROR: " + e.Message);
                }
        
            }
        }


        private void Events_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Shift | Keys.Control | Keys.Alt | Keys.F1))
            {
                Logger.Log(e.KeyData + " pressed. Keyboard tracking enabled (when in notepad)");
                HotkeyPressed = true;
            }
            if (e.KeyData == (Keys.Shift | Keys.Control | Keys.Alt | Keys.F2))
            {
                Logger.Log(e.KeyData + " pressed. Keyboard tracking disabled");
                HotkeyPressed = false;
            }
            if (e.KeyData == (Keys.Shift | Keys.Control | Keys.Alt | Keys.F3))
            {
                CallApi(_capturedText);
            }
            //Logger.Log(e.KeyData.ToString());
        }


        public void StopKbdHook()
        {
            IKeyboardMouseEvents events = Hook.GlobalEvents();
            events.KeyDown -= Events_KeyDown;
            events.KeyPress -= Events_KeyPress;
            //events.KeyUp -= Events_KeyUp;
        }


        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            WindowFocusDisabler.DisableFocus(this);

            Left = SystemParameters.WorkArea.Width - ActualWidth;
            Top = SystemParameters.WorkArea.Height - ActualHeight;

            ForegroundAppMonitor.ForegroundAppChanged += (s1, e1) =>
            {
                WindowInfo wi = new WindowInfo();
                Logger.Log("Active window changed to    [" + wi.ProcessID + "] " + wi.Process.ProcessName);
                InNotepad = wi.Process.ProcessName.ToLower() == "notepad";
                _notepadHandle = wi.Process.MainWindowHandle;

            };

            ForegroundAppMonitor.StartMonitoring();

            DispatcherTimer dt = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 50) };
            dt.Tick += (s2, e2) =>   //todo polling is evil
            {
                if (InNotepad)
                {
                    



                    if (RbInside.IsChecked == true) //inside, top right
                    {

                        var clientRect = new Rect();
                        GetClientRect(_notepadHandle, ref clientRect);
                        //Logger.Log(
                        //    clientRect.Left + ", " +
                        //    clientRect.Top + ", " +
                        //    clientRect.Right + ", " +
                        //    clientRect.Bottom
                        //);

                        System.Drawing.Point topleft = new System.Drawing.Point { X = clientRect.Left, Y = clientRect.Top };
                        System.Drawing.Point bottomRight = new System.Drawing.Point { X = clientRect.Right, Y = clientRect.Bottom };

                        ClientToScreen(_notepadHandle, ref topleft);
                        ClientToScreen(_notepadHandle, ref bottomRight);

                        //Logger.Log(topleft.X + "-" + topleft.Y);

                        var insidePos = (new Point(bottomRight.X, topleft.Y)).PixelsToWPFUnits();

                        insidePos.X -= ActualWidth;
                        Left = insidePos.X;
                        Top = insidePos.Y;
                    }
                    else
                    {
                        var rect = new Rect();
                        GetWindowRect(_notepadHandle, ref rect);
                        var outsidePos = (new Point(rect.Right, rect.Top)).PixelsToWPFUnits();//notepad's top right corner pos
                        Left = outsidePos.X;
                        Top = outsidePos.Y;
                    }

                }
            };

            dt.Start();

            StartKbdHook();
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
            }
        }
    }

    //todo move somewhere
    public static class ExtMethods
    {
        public static Point PixelsToWPFUnits(this Point @this)
        {
            return PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformFromDevice.Transform(@this);
        }

        public static Point WPFUnitsToPixels(this Point @this)
        {
            return PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice.Transform(@this);
        }
    }

}
