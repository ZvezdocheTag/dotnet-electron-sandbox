using System;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Class1
    {
        public async Task<object> GetAppDomainDirectory(dynamic input)
        {
            return await Task.Run(() => AppDomain.CurrentDomain.BaseDirectory);
        }

    }
}
