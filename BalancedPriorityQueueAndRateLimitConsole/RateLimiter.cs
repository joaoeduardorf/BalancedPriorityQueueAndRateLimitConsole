using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalancedPriorityQueueAndRateLimitConsole
{
    public class RateLimiter
    {
        private readonly int _requestsPerSecond;
        private readonly SemaphoreSlim _semaphore;
        private DateTime _lastRequestTime;

        public RateLimiter(int requestsPerSecond)
        {
            _requestsPerSecond = requestsPerSecond;
            _semaphore = new SemaphoreSlim(_requestsPerSecond);
            _lastRequestTime = DateTime.MinValue;
        }

        public void MakeRequest(Action action)
        {
            _semaphore.Wait();

            try
            {
                var now = DateTime.Now;
                var timeElapsedSinceLastRequest = now - _lastRequestTime;
                var minTimeBetweenRequests = TimeSpan.FromSeconds(1.0 / _requestsPerSecond);

                if (timeElapsedSinceLastRequest < minTimeBetweenRequests)
                {
                    var delay = minTimeBetweenRequests - timeElapsedSinceLastRequest;
                    Thread.Sleep(delay);
                }

                action();
                _lastRequestTime = DateTime.Now;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
