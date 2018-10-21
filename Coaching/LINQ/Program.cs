using Coaching.LINQ;
using System;

namespace Coaching
{
    class Program
    {
        static void Main(string[] args)
        {
            Manufacturer m = new Manufacturer();
            var result = m.ProcessManufacturers("manufacturers.csv");

            foreach(var data in result)
            {
                Console.WriteLine(data.HeadQuarters);
            }
        }
    }
}
