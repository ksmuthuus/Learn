using System;
using System.Collections.Generic;
using System.Text;

namespace TryIt.ChronologicalTime
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var setVal = DateTime.MaxValue.Ticks - 2518622207999999999;
            Console.WriteLine(new DateTime(setVal));

            var curVal = DateTime.MaxValue.Ticks - DateTime.UtcNow.Date.Ticks;
            Console.WriteLine(new DateTime(curVal));


        }
    }
}
