using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace StringsAndCollections
{
    class StringErrorsFixer
    {
        private const string PhoneRegex = @"\((\d{3})\)\s?(\d{3})-(\d{2})-(\d{2})";
        private Dictionary<string, List<string>> _errorsDictionary;
        private string _workingDirectory { get; set; }


        public StringErrorsFixer(string directory, string dictionary)
        {
            _workingDirectory = directory;
            ChangeDictionary(dictionary);
        }



        public void ChangeDirectory(string newDirectory)
        {
            _workingDirectory = newDirectory;
        }

        public void FixFile(string textFile)
        {
            using (StreamReader streamReader = new StreamReader(textFile))
            {
                string line;
                while((line = streamReader.ReadLine()) != null)
                {
                    foreach (var word in _errorsDictionary)
                    {
                        foreach (var error in word.Value)
                        {
                            line.Replace(error, word.Key);
                        }
                    }
                    Regex.Replace(line, PhoneRegex, "+380 $1 $2 $3 $4");
                }
            }
        }

        public void StartFixingFiles()
        {
            foreach (string file in Directory.EnumerateFiles(_workingDirectory, "*.txt", SearchOption.TopDirectoryOnly))
            {
                FixFile(file);
            }
            Console.WriteLine("Текст файлов исправлен");
        }

        public void ChangeDictionary(string dictionaryPath)
        {
            using (StreamReader dictionaryStreamReader = new StreamReader(dictionaryPath))
            {
                string fullDictionaryText = dictionaryStreamReader.ReadToEnd();
                _errorsDictionary = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(fullDictionaryText);
            }
        }


    }

    internal class Program
    {
        static void Main(string[] args)
        {

        }
    }
}
