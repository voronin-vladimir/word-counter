using System;
using word_counter;
using static System.Int32;

FolderPath:
Console.WriteLine("Enter folder path");
var folderPath = Console.ReadLine();

if (string.IsNullOrEmpty(folderPath))
{
    Console.WriteLine("Directory does not exist.");
    goto FolderPath;
}

WordLength:
Console.WriteLine("Enter min word length");
var isNumber = TryParse(Console.ReadLine(), out var minWordLength);

if (!isNumber || minWordLength < 1)
{
    Console.WriteLine("Invalid word length.");
    goto WordLength;
}

var topWords = WordCounter.Count(folderPath, minWordLength);

Console.WriteLine("\nTop 10 words:\n");

foreach (var (word, value) in topWords)
{
    Console.WriteLine($"{word} - {value}");
}