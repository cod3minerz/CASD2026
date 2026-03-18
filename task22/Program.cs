using System;
using System.IO;
using System.Text.RegularExpressions;
using Task21;

namespace Task22
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string filePath = "input.txt";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }
            
            MyHashMap<string, int> tagCount = new MyHashMap<string, int>();

            Regex tagRegex = new Regex(@"</?([a-zA-Z][a-zA-Z0-9]*)>");

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (trimmed.Length == 0)
                    continue;

                MatchCollection matches = tagRegex.Matches(trimmed);

                foreach (Match match in matches)
                {
                    string tagName = match.Groups[1].Value;
                    
                    string normalizedTag = tagName.ToLower();
                    
                    int currentCount = tagCount.Get(normalizedTag);
                    tagCount.Put(normalizedTag, currentCount + 1);
                }
            }
            
            if (tagCount.IsEmpty())
            {
                Console.WriteLine("Теги не найдены.");
                return;
            }

            Console.WriteLine("Тег -> Количество вхождений:");
            Console.WriteLine(new string('-', 35));

            foreach (Entry<string, int> entry in tagCount.EntrySet())
            {
                Console.WriteLine("<" + entry.Key + ">  ->  " + entry.Value);
            }

            Console.WriteLine(new string('-', 35));
            Console.WriteLine("Всего уникальных тегов: " + tagCount.Size());
        }
    }
}