using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace StringsAndCollections
{
    class StringErrorsFixer
    {
        private const string PhoneRegex = @"\((\d{3})\)\s?(\d{3})-(\d{2})-(\d{2})";
        private Dictionary<string, List<string>> _errorsDictionary;
        private string _workingDirectory { get; set; }
        private string _dictionaryPath { get; set; }

        public StringErrorsFixer()
        {
            _workingDirectory = Directory.GetCurrentDirectory();
            string dictionaryPath = _workingDirectory + "/dictionary.json";
            ChangeDictionary(dictionaryPath);
        }

        public StringErrorsFixer(string directory, string dictionary)
        {
            _workingDirectory = directory;
            ChangeDictionary(dictionary);
        }

        public static explicit operator string(StringErrorsFixer stringErrorsFixer)
        {
            return stringErrorsFixer.ToString();
        }

        public override string ToString()
        {
            StringBuilder errorFixerStringBuilder = new StringBuilder();
            errorFixerStringBuilder.Append("В данный момент вы работаете в папке: " + _workingDirectory);
            errorFixerStringBuilder.Append('\n');
            errorFixerStringBuilder.Append("Выбран словарь по адресу: " + _dictionaryPath);
            return errorFixerStringBuilder.ToString();
        }

        public void ChangeDirectory(string newDirectory)
        {
            _workingDirectory = newDirectory;
        }

        public void FixFile(string textFile)
        {
            StringBuilder resultFileText = new StringBuilder();
            using (StreamReader streamReader = new StreamReader(textFile))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    foreach (var word in _errorsDictionary)
                    {
                        foreach (var error in word.Value)
                        {
                            foreach (Match errorMatch in Regex.Matches(line, error))
                            {
                                line = line.Replace(error, word.Key);
                            }

                        }
                    }
                    line = Regex.Replace(line.ToString(), PhoneRegex, "+380 $1 $2 $3 $4");
                    resultFileText.AppendLine(line);
                }
            }
            using (StreamWriter streamWriter = new StreamWriter(textFile))
            {
                streamWriter.Write(resultFileText.ToString());
            }
        }

        public void StartFixingFiles()
        {
            foreach (string file in Directory.EnumerateFiles(_workingDirectory, "*.txt", SearchOption.TopDirectoryOnly))
            {
                Console.WriteLine(file);
                FixFile(file);
            }
        }

        public void ChangeDictionary(string dictionaryPath)
        {
            _dictionaryPath = dictionaryPath;
            using (StreamReader dictionaryStreamReader = new StreamReader(dictionaryPath))
            {
                string fullDictionaryText = dictionaryStreamReader.ReadToEnd();
                _errorsDictionary = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(fullDictionaryText);
            }
        }
    }

    class Menu
    {
        private static Menu _instance;
        private StringErrorsFixer _stringErrorsFixer = new StringErrorsFixer();
        public static Menu Instance
        {

            get
            {
                if (_instance == null) _instance = new Menu();
                return _instance;
            }
        }

        public void MenuRender()
        {
            Console.WriteLine(_stringErrorsFixer);
            MenuOperationChoice();
            Console.Clear();
        }

        public void MenuFixerStart()
        {
            _stringErrorsFixer.StartFixingFiles();
            Console.WriteLine("Текст файлов исправлен");
        }

        public void MenuChangeDirectory()
        {
            Console.Write("Введите новую директорию: ");
            string newDirectory = Console.ReadLine();
            _stringErrorsFixer.ChangeDirectory(newDirectory);
        }

        public void MenuChangeDictionary()
        {
            Console.Write("Введите новый словарь: ");
            string newDictionary = Console.ReadLine();
            _stringErrorsFixer.ChangeDictionary(newDictionary);
        }
        public void MenuOperationChoice()
        {
            Console.WriteLine("Выберите следующую операцию: ");
            Console.WriteLine("1. Запустить исправитель");
            Console.WriteLine("2. Изменить директорию");
            Console.WriteLine("3. Указать адрес до словаря");
            Console.Write("Введите номер операции: ");
            int operationChoice = Convert.ToInt16(Console.ReadLine());
            switch (operationChoice)
            {
                case 1:
                    MenuFixerStart();
                    break;
                case 2:
                    MenuChangeDirectory();
                    break;
                case 3:
                    MenuChangeDictionary();
                    break;
                default:
                    Console.WriteLine("Неопознанная команда! Попробуйте заново");
                    break;
            }
            Console.WriteLine("Нажмите любую кнопку чтобы продолжить...");
            Console.ReadKey();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Menu.Instance.MenuRender();
            }
        }
    }
}
