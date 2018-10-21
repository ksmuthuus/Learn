using System;

namespace TryIt
{
    public class SlidingWindow
    {
        public string Identity { get; set; }
        public TimeSpan Window { get; set; }
        public int MaxAttempts { get; set; }
        public bool Block { get; set; }
        public TimeSpan MaxBlockTime { get; set; }
        public bool AbortIfCantBlock { get; set; }

        public AttemptQueue<DateTime> Attempts { get; set; }

        private readonly object _syncRoot = new object();

        public bool OverrideAttemptLimitationExceptions { get; set; }

        public override string ToString()
        {
            return $"Count: {Attempts.Count()}, Window: {Window}, MaxAttempts: {MaxAttempts}, Block: {Block}, MaxBlockTime: {MaxBlockTime}, AbortIfCantBlock: {AbortIfCantBlock}";
        }
        public bool Attempt(string activityId, bool logAttempt)
        {
            var attemptLimited = false;
            Console.ReadLine();
            var time = DateTime.UtcNow;
            var blockWindowStart = time - MaxBlockTime;  //Used for Queue Cleanup
            var windowStart = time - Window;      //Used for max block time

            // delete those outside of window

            var timeLeft = TimeSpan.MaxValue;
            bool isBlocking;
            lock (_syncRoot)
            {
                if (logAttempt)
                {
                    //var time = DateTime.UtcNow;
                    Console.WriteLine($"Attempt logged at {time} details: {this}");
                    Attempts.Enqueue(time);
                }
                //Attempts.RemoveAll(t => t < windowStart);
                //isBlocking = Attempts.Count() == MaxAttempts || timeLeft <= MaxBlockTime;
                timeLeft = Attempts.Max() - blockWindowStart;
                isBlocking = Attempts.Count() == MaxAttempts;
                if (isBlocking)
                {

                    if (Attempts.Count() > 0)
                    {
                        timeLeft = Attempts.Max() - blockWindowStart;
                    }
                    else
                    {
                        isBlocking = false;
                    }
                }
            }

            if (isBlocking)
            {
                // get last attempts
                if (Block && timeLeft <= MaxBlockTime)
                {
                    Console.WriteLine($"Attempt is blocked for {timeLeft} details: {this}");
                    //Thread.Sleep(timeLeft);
                    //throw new TooManyAttemptsException(ToString());
                }
                else if (AbortIfCantBlock)
                {
                    if (OverrideAttemptLimitationExceptions)
                    {
                        Console.WriteLine($"Attempt TooManyAttemptsException suppressed by setting OverrideAttemptLimitationExceptions in service configuration details: {this}");
                    }
                    else
                    {
                        Console.WriteLine($"Attempt throws TooManyAttemptsException details: {this}");
                        //throw new TooManyAttemptsException(ToString());
                    }
                }
                else
                {
                    Console.WriteLine($"Attempt was limited but neither exception or blocking has taken place details: {this}");
                }

                attemptLimited = true;
            }
           

            return attemptLimited;
        }
    }
}
