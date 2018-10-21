using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TryIt
{
    public class Program
    {
        public static void SlidingWindowMain(string[] args)
        {
            SlidingWindow p1 = new SlidingWindow();
            p1.Window = TimeSpan.FromMinutes(1);
            p1.MaxAttempts = 6;
            p1.MaxBlockTime = TimeSpan.FromMinutes(2);
            p1.AbortIfCantBlock = true;
            p1.Block = true;
            p1.Attempts = new AttemptQueue<DateTime>(p1.MaxAttempts);
            Console.WriteLine("Hit!");
            do
            {
                p1.Attempt(Guid.NewGuid().ToString(), true);
            } while (true) ;

        }
    }
}
