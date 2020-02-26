/*
Thread safe implementation of a random number generator
*/
using System;
using System.Runtime.CompilerServices;
using System.Threading;

#if DEBUG
[assembly: InternalsVisibleTo("ShortcodeService.Tests")]
#endif
namespace ShortcodeService
{
    internal class RandomService : IRandomService
    {
        static int seed = Environment.TickCount;

        private static readonly ThreadLocal<string> uniqueSeed =
            new ThreadLocal<string>(() =>
            {
                var uniqueMachine = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")
                ?? Environment.MachineName;

                var process = Thread.CurrentThread.ManagedThreadId;

                var threadSafeSeed = Interlocked.Increment(ref seed);

                return $"{uniqueMachine}{process}{threadSafeSeed}";
            });

        private static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(uniqueSeed.Value.GetHashCode()));

        public double NextDouble()
        {
            return random.Value.NextDouble();
        }

        public int Next()
        {
            return random.Value.Next();
        }

        public int Next(int minValue, int maxValue)
        {
            return random.Value.Next(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            return random.Value.Next(maxValue);
        }

        public void NextBytes(byte[] buffer)
        {
            random.Value.NextBytes(buffer);
        }

        public void NextBytes(Span<byte> buffer)
        {
            random.Value.NextBytes(buffer);
        }
    }
}