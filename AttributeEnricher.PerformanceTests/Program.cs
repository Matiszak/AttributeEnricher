using BenchmarkDotNet.Running;

namespace AttributeEnricher.PerformanceTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<AttributeEnricherBenchmark>();
        }
    }
}
