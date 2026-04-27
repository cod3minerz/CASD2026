using System;
using System.Collections.Generic;
using System.IO;
using Task25;

namespace Task26
{
    public class Line : IComparable<Line>
    {
        public string Original;     
        public List<string> Words;  

        public Line(string original)
        {
            Original = original;
            Words = new List<string>();
            
            string[] parts = original.Split(' ');
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                    Words.Add(parts[i]);
            }
            
            Words.Sort(delegate(string a, string b)
            {
                return a.Length.CompareTo(b.Length);
            });
        }
        
        public int CompareTo(Line other)
        {
            if (other == null)
                return 1;

            int count = Math.Min(Words.Count, other.Words.Count);

            for (int i = 0; i < count; i++)
            {
                int cmp = Words[i].Length.CompareTo(other.Words[i].Length);
                if (cmp != 0)
                    return cmp;
            }
            
            return Words.Count.CompareTo(other.Words.Count);
        }

        public override string ToString()
        {
            return Original;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            string inputFile = Path.Combine(Directory.GetCurrentDirectory(), "input.txt");

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Ошибка: файл input.txt не найден по пути: " + inputFile);
                return;
            }
            
            MyHashSet<Line> set = new MyHashSet<Line>();

            string[] fileLines = File.ReadAllLines(inputFile);

            Console.WriteLine("=== Чтение из файла ===");
            for (int i = 0; i < fileLines.Length; i++)
            {
                string rawLine = fileLines[i];
                if (rawLine.Trim().Length == 0)
                    continue;

                Line line = new Line(rawLine);
                bool added = set.Add(line);

                Console.Write("  \"" + rawLine + "\"");
                if (!added)
                    Console.Write("  <-- дубликат, не добавлена");
                Console.WriteLine();
            }

            Console.WriteLine("\nВсего уникальных строк в множестве: " + set.Size());
            
            Console.WriteLine("\n=== Строки в порядке сравнения (от меньшей к большей) ===");
            Line[] sorted = set.ToArray();
            for (int i = 0; i < sorted.Length; i++)
            {
                Line l = sorted[i];
                Console.Write((i + 1) + ". \"" + l.Original + "\"");
                Console.Write("  |  слова по длине: [");
                for (int j = 0; j < l.Words.Count; j++)
                {
                    if (j > 0)
                        Console.Write(", ");
                    Console.Write(l.Words[j] + "(" + l.Words[j].Length + ")");
                }
                Console.WriteLine("]");
            }
        }
    }
}
