using System;
using System.IO;
using Task25;

namespace Task27
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string inputFile = Path.Combine(Directory.GetCurrentDirectory(), "input27.txt");

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Ошибка: файл input.txt не найден по пути: " + inputFile);
                return;
            }

            MyHashSet<string> set = new MyHashSet<string>();

            string[] lines = File.ReadAllLines(inputFile);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                int j = 0;

                while (j < line.Length)
                {
                    if ((line[j] >= 'a' && line[j] <= 'z') || (line[j] >= 'A' && line[j] <= 'Z'))
                    {
                        int start = j;
                        while (j < line.Length && ((line[j] >= 'a' && line[j] <= 'z') || (line[j] >= 'A' && line[j] <= 'Z')))
                            j++;

                        string word = line.Substring(start, j - start).ToLower();
                        set.Add(word);
                    }
                    else
                    {
                        j++;
                    }
                }
            }

            Console.WriteLine("Уникальные слова (" + set.Size() + " шт.):");
            Console.WriteLine(set);
        }
    }
}
