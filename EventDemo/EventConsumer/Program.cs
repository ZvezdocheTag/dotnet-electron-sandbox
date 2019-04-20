using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventGeneratorLib;

namespace EventConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Создаем экземпляр класса
            EventGenerator eg = new EventGenerator();

            //Подписываемся на его событие. Реагировать на событие будем в методе Eg_DemoEvent
            eg.DemoEvent += Eg_DemoEvent;

            //Чтобы программа тут же не закрылась, ждем нажатия клавиши. А пока ждем, таймер тикает
            Console.WriteLine("Таймер будет тикать, пока ты не нажмешь Эникей");
            Console.ReadKey();

        }


        //обработчик
        private static void Eg_DemoEvent(object sender, DemoEventAgrs e)
        {
            Console.WriteLine("В Демогенераторе произошло Демособытие уже в " + e.Ticks + "-й раз");
        }
    }
}
