using System;
using System.Collections.Generic;
using System.Text;
using task19;

namespace Task25
{
    public class MyHashSet<E> where E : IComparable<E>
    {
        private MyTreeMap<E, object> map;
        private static readonly object Dummy = new object();
        
        public MyHashSet()
        {
            map = new MyTreeMap<E, object>();
        }
        
        public MyHashSet(E[] a)
        {
            if (a == null)
                throw new ArgumentNullException("a", "Массив не может быть null.");

            map = new MyTreeMap<E, object>();
            AddAll(a);
        }
        
        public MyHashSet(int initialCapacity, float loadFactor)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException("Начальная ёмкость должна быть положительным числом.");
            if (loadFactor <= 0 || loadFactor > 1)
                throw new ArgumentException("Коэффициент загрузки должен быть в диапазоне (0, 1].");

            map = new MyTreeMap<E, object>();
        }
        
        public MyHashSet(int initialCapacity)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException("Начальная ёмкость должна быть положительным числом.");

            map = new MyTreeMap<E, object>();
        }
        
        public bool Add(E e)
        {
            if (map.ContainsKey(e))
                return false;

            map.Put(e, Dummy);
            return true;
        }
        
        public void AddAll(E[] a)
        {
            if (a == null)
                throw new ArgumentNullException("a", "Массив не может быть null.");

            for (int i = 0; i < a.Length; i++)
                Add(a[i]);
        }
        
        public void Clear()
        {
            map.Clear();
        }

        // 8. Метод contains(object o) для проверки, находится ли указанный объект во множестве.
        public bool Contains(object o)
        {
            if (o == null)
                return false;
            if (!(o is E))
                return false;

            return map.ContainsKey((E)o);
        }

        // 9. Метод containsAll(E[] a) для проверки, содержатся ли указанные объекты во множестве.
        public bool ContainsAll(E[] a)
        {
            if (a == null)
                throw new ArgumentNullException("a", "Массив не может быть null.");

            for (int i = 0; i < a.Length; i++)
                if (!Contains(a[i]))
                    return false;

            return true;
        }

        // 10. Метод isEmpty() для проверки, является ли множество пустым.
        public bool IsEmpty()
        {
            return map.IsEmpty();
        }

        // 11. Метод remove(object o) для удаления указанного объекта из множества, если он есть там.
        public bool Remove(object o)
        {
            if (o == null)
                return false;
            if (!(o is E))
                return false;

            E key = (E)o;
            if (!map.ContainsKey(key))
                return false;

            map.Remove(key);
            return true;
        }

        // 12. Метод removeAll(E[] a) для удаления указанных объектов из множества.
        public void RemoveAll(E[] a)
        {
            if (a == null)
                throw new ArgumentNullException("a", "Массив не может быть null.");

            for (int i = 0; i < a.Length; i++)
                Remove(a[i]);
        }

        // 13. Метод retainAll(E[] a) для оставления во множестве только указанных объектов.
        public void RetainAll(E[] a)
        {
            if (a == null)
                throw new ArgumentNullException("a", "Массив не может быть null.");
            
            MyHashSet<E> toKeep = new MyHashSet<E>(a);
            
            List<E> currentKeys = map.KeySet();
            
            List<E> toRemove = new List<E>();
            for (int i = 0; i < currentKeys.Count; i++)
                if (!toKeep.Contains(currentKeys[i]))
                    toRemove.Add(currentKeys[i]);
            
            for (int i = 0; i < toRemove.Count; i++)
                map.Remove(toRemove[i]);
        }

        // 14. Метод size() для получения размера множества в элементах.
        public int Size()
        {
            return map.Size();
        }

        // 15. Метод toArray() для возвращения массива объектов, содержащего все элементы множества.
        public E[] ToArray()
        {
            List<E> keys = map.KeySet();
            E[] result = new E[keys.Count];
            for (int i = 0; i < keys.Count; i++)
                result[i] = keys[i];
            return result;
        }

        // 16. Метод toArray(E[] a) для возвращения массива объектов, содержащего все элементы множества.
        public E[] ToArray(E[] a)
        {
            E[] elements = ToArray();

            if (a == null)
                return elements;

            if (a.Length < elements.Length)
                return elements;

            for (int i = 0; i < elements.Length; i++)
                a[i] = elements[i];

            if (a.Length > elements.Length)
                a[elements.Length] = default(E);

            return a;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            List<E> keys = map.KeySet();
            for (int i = 0; i < keys.Count; i++)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append(keys[i]);
            }

            sb.Append("}");
            return sb.ToString();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Тест MyHashSet<int> ===\n");

            // Конструктор 1: пустое множество
            MyHashSet<int> set = new MyHashSet<int>();
            Console.WriteLine("Пусто? " + set.IsEmpty());     // True
            Console.WriteLine("Размер: " + set.Size());        // 0

            // Метод add()
            set.Add(10);
            set.Add(20);
            set.Add(30);
            set.Add(20); // дубликат — не добавится
            Console.WriteLine("\nПосле добавления 10, 20, 30, 20 (повтор):");
            Console.WriteLine("Размер: " + set.Size());        // 3
            Console.WriteLine("Пусто? " + set.IsEmpty());      // False
            Console.WriteLine("Множество: " + set);            // {10, 20, 30} — отсортировано

            // Метод contains()
            Console.WriteLine("\nContains(20): " + set.Contains(20));   // True
            Console.WriteLine("Contains(99): " + set.Contains(99));     // False

            // Метод addAll()
            int[] extra = { 40, 50, 10 }; // 10 уже есть
            set.AddAll(extra);
            Console.WriteLine("\nПосле AddAll({40, 50, 10}):");
            Console.WriteLine("Размер: " + set.Size());        // 5
            Console.WriteLine("Множество: " + set);            // {10, 20, 30, 40, 50}

            // Метод containsAll()
            int[] checkAll = { 10, 20, 30 };
            Console.WriteLine("\nContainsAll({10,20,30}): " + set.ContainsAll(checkAll)); // True
            int[] checkMiss = { 10, 99 };
            Console.WriteLine("ContainsAll({10,99}): " + set.ContainsAll(checkMiss));    // False

            // Метод toArray()
            int[] arr = set.ToArray();
            Console.Write("\nToArray(): ");
            for (int i = 0; i < arr.Length; i++)
                Console.Write(arr[i] + " ");
            Console.WriteLine();

            // Метод toArray(E[] a) — с существующим массивом
            int[] dest = new int[10];
            set.ToArray(dest);
            Console.Write("ToArray(dest[10]): ");
            for (int i = 0; i < set.Size(); i++)
                Console.Write(dest[i] + " ");
            Console.WriteLine();

            // Метод toArray(E[] a) — с null
            int[] fromNull = set.ToArray(null);
            Console.WriteLine("ToArray(null) длина: " + fromNull.Length); // 5

            // Метод remove()
            bool removed = set.Remove(20);
            Console.WriteLine("\nRemove(20): " + removed);               // True
            Console.WriteLine("Remove(99): " + set.Remove(99));          // False
            Console.WriteLine("Размер после удалений: " + set.Size());   // 4

            // Метод removeAll()
            int[] toRemove = { 10, 40 };
            set.RemoveAll(toRemove);
            Console.WriteLine("\nПосле RemoveAll({10, 40}):");
            Console.WriteLine("Размер: " + set.Size());        // 2
            Console.WriteLine("Множество: " + set);

            // Конструктор 2: из массива
            Console.WriteLine("\n=== Конструктор из массива ===");
            int[] initArr = { 5, 2, 8, 2, 5 };
            MyHashSet<int> setFromArr = new MyHashSet<int>(initArr);
            Console.WriteLine("Множество: " + setFromArr);     // {2, 5, 8} — отсортировано, без дублей
            Console.WriteLine("Размер: " + setFromArr.Size()); // 3

            // Конструктор 3: с ёмкостью и коэффициентом загрузки
            MyHashSet<string> strSet = new MyHashSet<string>(8, 0.5f);
            strSet.Add("яблоко");
            strSet.Add("банан");
            strSet.Add("вишня");
            Console.WriteLine("\n=== MyHashSet<string> (ёмкость=8, загрузка=0.5) ===");
            Console.WriteLine("Размер: " + strSet.Size());
            Console.WriteLine("Множество: " + strSet);

            // Конструктор 4: только с ёмкостью
            MyHashSet<string> strSet2 = new MyHashSet<string>(4);
            strSet2.Add("один");
            strSet2.Add("два");
            Console.WriteLine("\n=== MyHashSet<string> (ёмкость=4) ===");
            Console.WriteLine("Размер: " + strSet2.Size());
            Console.WriteLine("Множество: " + strSet2);

            // Метод retainAll()
            Console.WriteLine("\n=== Тест retainAll() ===");
            MyHashSet<int> retSet = new MyHashSet<int>(new int[] { 1, 2, 3, 4, 5 });
            Console.WriteLine("До retainAll: " + retSet);
            retSet.RetainAll(new int[] { 2, 4, 6 });
            Console.WriteLine("После retainAll({2,4,6}): " + retSet); // {2, 4}

            // Метод clear()
            retSet.Clear();
            Console.WriteLine("\nПосле Clear():");
            Console.WriteLine("Размер: " + retSet.Size());    // 0
            Console.WriteLine("Пусто? " + retSet.IsEmpty());  // True

            // Обработка ошибок
            Console.WriteLine("\n=== Тест обработки ошибок ===");

            try
            {
                MyHashSet<int> badSet = new MyHashSet<int>(-1);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Ошибка (ёмкость -1): " + ex.Message);
            }

            try
            {
                MyHashSet<int> badSet2 = new MyHashSet<int>(16, 1.5f);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Ошибка (загрузка 1.5): " + ex.Message);
            }

            try
            {
                MyHashSet<int> badSet3 = new MyHashSet<int>(null);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("Ошибка (массив null): " + ex.Message);
            }

            try
            {
                set.AddAll(null);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("Ошибка (AddAll null): " + ex.Message);
            }

            try
            {
                set.ContainsAll(null);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("Ошибка (ContainsAll null): " + ex.Message);
            }
        }
    }
}
