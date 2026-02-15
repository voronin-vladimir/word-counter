using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace word_counter
{
    public class WordCounter
    {
        public IEnumerable<KeyValuePair<string, int>> Count(string folderPath, int minWordLength)
        {
            var wordCounts = new ConcurrentDictionary<string, int>(
                StringComparer.OrdinalIgnoreCase);

            var files = Directory.EnumerateFiles(folderPath, "*.txt",
                SearchOption.TopDirectoryOnly);

            Parallel.ForEach(files, file => { ProcessFile(file, wordCounts, minWordLength); });

            return wordCounts
                .OrderByDescending(kvp => kvp.Value)
                .Take(10);
        }

        private static void ProcessFile(string file, ConcurrentDictionary<string, int> wordCounts,
            int minWordLength)
        {
            try
            {
                foreach (var line in File.ReadLines(file))
                {
                    ProcessLine(line, minWordLength, wordCounts);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {file}: {ex.Message}");
            }
        }

        private static void ProcessLine(string line, int minLength,
            ConcurrentDictionary<string, int> wordCounts)
        {
            int start = -1;

            for (int i = 0; i < line.Length; i++)
            {
                if (char.IsLetter(line[i]))
                {
                    if (start < 0)
                        start = i;
                }
                else
                {
                    if (start >= 0)
                    {
                        var word = line.Substring(start, i - start);
                        var length = i - start;
                        CountWord(word, length, minLength, wordCounts);
                        start = -1;
                    }
                }
            }

            if (start >= 0)
            {
                var word = line.Substring(start, line.Length - start);
                var length = line.Length - start;
                CountWord(word, length, minLength, wordCounts);
            }
        }

        private static void CountWord(string word, int length, int minLength,
            ConcurrentDictionary<string, int> wordCounts)
        {
            if (length < minLength)
                return;

            wordCounts.AddOrUpdate(word, 1, (_, old) => old + 1);
        }
    }
}