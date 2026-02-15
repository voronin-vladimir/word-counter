using BenchmarkDotNet.Attributes;

namespace word_counter
{
    [MemoryDiagnoser]
    public class WordCounterBenchmark
    {
        private WordCounter _counter;
        private string _folderPath;

        [Params(5, 10)] public int MinWordLength;

        [GlobalSetup]
        public void Setup()
        {
            _counter = new WordCounter();
            _folderPath = @"C:\Users\ironr\Downloads\Telegram Desktop";
        }

        [Benchmark]
        public void Sequential()
        {
            _counter.Count(_folderPath, MinWordLength);
        }
    }
}