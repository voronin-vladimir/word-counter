using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace word_counter
{
    public static class WordCounter
    {
        public static IEnumerable<KeyValuePair<string, int>> Count(string folderPath, int minWordLength)
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
                    foreach (var word in ExtractWords(line, minWordLength))
                    {
                        wordCounts.AddOrUpdate(word, 1, (_, oldValue) => oldValue + 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {file}: {ex.Message}");
            }
        }

        private static IEnumerable<string> ExtractWords(string text, int minLength)
        {
            foreach (Match match in Regex.Matches(text, @"\b[\p{L}]+\b"))
            {
                var word = match.Value.ToLowerInvariant();
                if (word.Length > minLength)
                {
                    yield return word;
                }
            }
        }
    }
}