using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Task21;

namespace Task23
{
    public enum VarType
    {
        Int,
        Float,
        Double
    }

    public class VariableInfo
    {
        public VarType Type;
        public string Value;

        public VariableInfo(VarType type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    public class Program
    {
        private static bool TryParseVarType(string typeText, out VarType varType)
        {
            string t = typeText.Trim();
            if (string.Equals(t, "int"))
            {
                varType = VarType.Int;
                return true;
            }

            if (string.Equals(t, "float"))
            {
                varType = VarType.Float;
                return true;
            }

            if (string.Equals(t, "double"))
            {
                varType = VarType.Double;
                return true;
            }

            varType = default;
            return false;
        }

        private static string VarTypeToText(VarType t)
        {
            if (t == VarType.Int) return "int";
            if (t == VarType.Float) return "float";
            return "double";
        }

        public static void Main(string[] args)
        {
            string inputPath = "input.txt";
            string outputPath = "output.txt";

            if (!File.Exists(inputPath))
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }

            string text = File.ReadAllText(inputPath);

            Regex defRegex = new Regex(
                @"^\s*(?<type>[A-Za-z_][A-Za-z0-9_]*)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*=\s*(?<value>[0-9]+)\s*;\s*$",
                RegexOptions.Compiled | RegexOptions.Singleline
            );

            string[] rawParts = Regex.Split(text, ";");

            MyHashMap<string, VariableInfo> table = new MyHashMap<string, VariableInfo>();
            StringBuilder report = new StringBuilder();

            int definitionIndex = 0;
            for (int i = 0; i < rawParts.Length; i++)
            {
                string part = rawParts[i];
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                definitionIndex++;

                string candidate = part + ";";
                Match m = defRegex.Match(candidate);

                if (!m.Success)
                {
                    report.AppendLine("Некорректное определение #" + definitionIndex + ": " + candidate.Trim());
                    continue;
                }

                string typeText = m.Groups["type"].Value;
                string name = m.Groups["name"].Value;
                string valueText = m.Groups["value"].Value;

                if (!TryParseVarType(typeText, out VarType vt))
                {
                    report.AppendLine("Некорректный тип в определении #" + definitionIndex + ": " + candidate.Trim());
                    continue;
                }

                if (table.ContainsKey(name))
                {
                    report.AppendLine("Переопределение переменной: " + name + " (оставлено первое определение)");
                    continue;
                }

                if (!ulong.TryParse(valueText, out _))
                {
                    report.AppendLine("Некорректное значение в определении #" + definitionIndex + ": " + candidate.Trim());
                    continue;
                }

                table.Put(name, new VariableInfo(vt, valueText));
            }

            StringBuilder output = new StringBuilder();

            if (report.Length > 0)
            {
                output.AppendLine("Сообщения:");
                output.Append(report);
                output.AppendLine();
            }
            else
            {
                output.AppendLine("Сообщения: отсутствуют");
                output.AppendLine();
            }

            output.AppendLine("Результат:");
            foreach (Entry<string, VariableInfo> entry in table.EntrySet())
            {
                string typeText = VarTypeToText(entry.Value.Type);
                output.AppendLine(typeText + " →> " + entry.Key + "(" + entry.Value.Value + ")");
            }

            string outputText = output.ToString();
            File.WriteAllText(outputPath, outputText);
            Console.Write(outputText);
        }
    }
}

