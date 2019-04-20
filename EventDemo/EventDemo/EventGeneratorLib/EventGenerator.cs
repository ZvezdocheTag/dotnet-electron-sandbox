using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace EventGeneratorLib
{
    public class EventGenerator
    {
        private readonly Timer _timer;

        //Локальное поле, в котором мы будем хранить счетчик тиков
        private int _ticks;


        //Событие, которое на которое могут подписываться другие классы
        public event EventHandler<DemoEventAgrs> DemoEvent;
        
        //Конструктор класса, исполняется в момент создания экземпляра класса
        public EventGenerator()
        {
            //Создаем экземпляр таймера с интервалом 1000 мс и подписываемся на его событие Elapsed
            _timer = new Timer { Interval = 1000 };
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _ticks++;
            //Всякий раз, когда таймер тикает, мы генерим свое событие, но не простое, а несущее информацию о количестве тиков (а могло бы нести и более сложные данные)
            DemoEvent?.Invoke(this, new DemoEventAgrs { Ticks = _ticks });
        }
    }
    
    //Класс-хранилка данных, передаваемых с ивентом
    public class DemoEventAgrs : EventArgs
    {
        public int Ticks;
    }
}
