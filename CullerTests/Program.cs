using BenchmarkDotNet.Running;

namespace MathTests
{
    class Program
    {
        static void Main(string[] args)
        {
            // Fire up the benchmarking engine
            var summary = BenchmarkRunner.Run<CullingBenchmark>();
        }
    }
}