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
        public static IEnumerable<KeyValuePair<string, int>> Count(string folderPath, int minWordLength)
        {
            var wordCounts = new ConcurrentDictionary<string, int>(
                StringComparer.OrdinalIgnoreCase);

            var files = Directory.EnumerateFiles(folderPath, "*.txt",
                SearchOption.TopDirectoryOnly);

            Parallel.ForEach(files, file =>
            {
                var localDict = new Dictionary<string, int>();
                ProcessFile(file, minWordLength, localDict);
                MergeDictionaries(wordCounts, localDict);
            });

            return wordCounts
                .OrderByDescending(kvp => kvp.Value)
                .Take(10);
        }

        private static void ProcessFile(string file, int minWordLength,
            Dictionary<string, int> wordCounts)
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

        private static void ProcessLine(string line, int minLength, Dictionary<string, int> wordCounts)
        {
            var start = -1;

            for (var i = 0; i < line.Length; i++)
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
            Dictionary<string, int> wordCounts)
        {
            if (length < minLength)
                return;

            wordCounts.TryGetValue(word, out var count);
            wordCounts[word] = count + 1;
        }

        private static void MergeDictionaries(ConcurrentDictionary<string, int> result,
            Dictionary<string, int> local)
        {
            foreach (var (word, count) in local)
            {
                result.AddOrUpdate(
                    word,
                    count,
                    (_, old) => old + count);
            }
        }
    }
}