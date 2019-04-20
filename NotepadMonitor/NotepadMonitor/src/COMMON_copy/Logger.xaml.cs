using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace CommonFunctions
{
    /// <summary>
    /// Interaction logic for Log.xaml
    /// </summary>
    public partial class Logger : UserControl
    {
        public Logger()
        {
            InitializeComponent();
        }

        public void DoEvents()
        {
            //Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
        }

        public void Log(string s)
        {
            //TalkToMainThread(() =>
            //{
            TbLog.AppendText("\n" + DateTime.Now + "  :  " + s);
            TbLog.ScrollToEnd();
            FileInfo f = new FileInfo(ExePath() + ".log");
            StreamWriter sw = f.AppendText();
            sw.WriteLine(s);
            sw.Close();

            DoEvents();
            //});
        }


        public static string ExePath()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

    }
}
