using System;
using System.Threading.Tasks;
using ExternalLibrary;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace QuickStart.Core
{
    class ExternalMethods
    {
        private readonly Library _library = new Library();

        public async Task<object> GetPersonInfo(dynamic input)
        {
            return await Task.Run(() => JsonConvert.SerializeObject(_library.GetPerson(), Formatting.Indented));
        }

        public async Task<object> GetInput(dynamic input)
        {
            return await Task.Run(() =>
            {

                if (input == "notepad")
                {
                    string b = "notepad.exe";
                    var prc = Process.Start("notepad.exe", @"c:\Users\fronty\Desktop\test.txt");
                    prc.WaitForInputIdle();
                    bool ok = MoveWindow(prc.MainWindowHandle, 0, 0, 300, 600, false);

                    return ok;
                }
                else
                {
                    var prcD = Process.Start(@"C:\Program Files (x86)\Simple EMR\Simple EMR.exe");
                    prcD.WaitForInputIdle();
                    bool okd = MoveWindow(prcD.MainWindowHandle, 0, 300, 300, 200, false);
                    return okd;

                }

            });
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);
    }
}
