using System;
using System.Threading.Tasks;

namespace QuickStart.Core
{
    class Rectangle
    {
        // member variables
        String str = "Tutorials Point";
        double length;
        double width;

        public void Acceptdetails()
        {
            length = 4.5;
            width = 3.5;
        }

        public double GetArea()
        {
            return length * width;
        }

        public void Display()
        {
            Console.WriteLine("Length: {0}", length);
            Console.WriteLine("Width: {0, 1}", width);
            Console.WriteLine("Area: {0}", GetArea());
        }
    }


    public class LocalMethods
    {
        public async Task<object> GetAppDomainDirectory(dynamic input)
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public async Task<object> GetCurrentTime(dynamic input)
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public async Task<object> UseDynamicInput(dynamic input)
        {
            return $".NET Core welcomes {input}";
        }

        public async Task<object> ReturnMessage(dynamic input)
        {
            Rectangle r = new Rectangle();
            r.Acceptdetails();
            r.Display();

            return $".NET Standard welcomes HAPPY CODING BITCHES";
        }
    }


}
