using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIt
{
    class CacheProgram
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = "mksdevredis.redis.cache.windows.net:6379,password=3iKpreWrdWqslLoJjHD0+OTElHYGv+N8yEd0X1kxOPc=,ssl=False,abortConnect=False";
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
        static void CacheMain(string[] args)
        {
            // Connection refers to a property that returns a ConnectionMultiplexer
            // as shown in the previous example.
            IDatabase cache = Connection.GetDatabase();

            // Perform cache operations using the cache object...

            // Simple PING command
            string cacheCommand = "PING";
            //Console.WriteLine("\nCache command  : " + cacheCommand);
            //Console.WriteLine("Cache response : " + cache.Execute(cacheCommand).ToString());

            //// Simple get and put of integral data types into the cache
            //cacheCommand = "GET Message";
            //Console.WriteLine("\nCache command  : " + cacheCommand + " or StringGet()");
            //Console.WriteLine("Cache response : " + cache.StringGet("Message").ToString());

            cacheCommand = "SET Message \"Hello! The cache is working from a .NET console app!\"";
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringSet()");
            Console.WriteLine("Cache response : " + cache.StringSet("Message", "Hello! The cache is working from a .NET console app!").ToString());

            // Demostrate "SET Message" executed as expected...
            cacheCommand = "GET Message";
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringGet()");
            Console.WriteLine("Cache response : " + cache.StringGet("Message").ToString());

            // Get the client list, useful to see if connection list is growing...
            cacheCommand = "CLIENT LIST";
            Console.WriteLine("\nCache command  : " + cacheCommand);
            Console.WriteLine("Cache response : \n" + cache.Execute("CLIENT", "LIST").ToString().Replace("id=", "id="));

            lazyConnection.Value.Dispose();
        }
    }
}
