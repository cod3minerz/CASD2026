using System;
using System.Collections.Generic;
using System.Text;

namespace Task21
{
    public class Entry<K, V>
    {
        public K Key;
        public V Value;
        public Entry<K, V> Next; 
    
        public Entry(K key, V value)
        {
            Key = key;
            Value = value;
            Next = null;
        }
    }
    
    public class MyHashMap<K, V>
    {
        private Entry<K, V>[] table; 
        private int size;           
        private float loadFactor;   
        private int resizeThreshold;
        
        public MyHashMap()
        {
            table = new Entry<K, V>[16];
            size = 0;
            loadFactor = 0.75f;
            RecalculateResizeThreshold();
        }
        
        public MyHashMap(int initialCapacity)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException("Начальная ёмкость должна быть положительным числом.");
    
            table = new Entry<K, V>[initialCapacity];
            size = 0;
            loadFactor = 0.75f;
            RecalculateResizeThreshold();
        }
        
        public MyHashMap(int initialCapacity, float loadFactor)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException("Начальная ёмкость должна быть положительным числом.");
            if (loadFactor <= 0 || loadFactor > 1)
                throw new ArgumentException("Коэффициент загрузки должен быть в диапазоне (0, 1].");
    
            table = new Entry<K, V>[initialCapacity];
            size = 0;
            this.loadFactor = loadFactor;
            RecalculateResizeThreshold();
        }

        private void RecalculateResizeThreshold()
        {
            resizeThreshold = (int)(table.Length * loadFactor);
            if (resizeThreshold <= 0)
                resizeThreshold = 1;
        }
        
        private int GetBucketIndex(K key)
        {
            int hashCode = key == null ? 0 : key.GetHashCode();
            return (hashCode & 0x7FFFFFFF) % table.Length;
        }
        
        private void Resize()
        {
            int newCapacity = table.Length * 2;
            Entry<K, V>[] newTable = new Entry<K, V>[newCapacity];
            
            for (int i = 0; i < table.Length; i++)
            {
                Entry<K, V> current = table[i];
                while (current != null)
                {
                    Entry<K, V> next = current.Next;
                    
                    int hashCode = current.Key == null ? 0 : current.Key.GetHashCode();
                    int newIndex = (hashCode & 0x7FFFFFFF) % newCapacity;
                    
                    current.Next = newTable[newIndex];
                    newTable[newIndex] = current;
    
                    current = next;
                }
            }

            table = newTable;
            RecalculateResizeThreshold();
        }
        
        public void Put(K key, V value)
        {
            if (size >= resizeThreshold)
                Resize();
    
            int index = GetBucketIndex(key);
            Entry<K, V> current = table[index];
            
            while (current != null)
            {
                bool sameHash = (key == null && current.Key == null)
                             || (key != null && current.Key != null && key.GetHashCode() == current.Key.GetHashCode());
    
                if (sameHash)
                {
                    bool keysEqual = (key == null && current.Key == null)
                                  || (key != null && key.Equals(current.Key));
    
                    if (keysEqual)
                    {
                        current.Value = value;
                        return;
                    }
                }
    
                current = current.Next;
            }
            
            Entry<K, V> newEntry = new Entry<K, V>(key, value);
            newEntry.Next = table[index];
            table[index] = newEntry;

            size++;
        }
        
        public V Get(K key)
        {
            int index = GetBucketIndex(key);
            Entry<K, V> current = table[index];

            while (current != null)
            {
                bool keysEqual = (key == null && current.Key == null)
                              || (key != null && key.Equals(current.Key));

                if (keysEqual)
                    return current.Value;

                current = current.Next;
            }

            return default(V);
        }

        public V Get(object key)
        {
            K typedKey;
            try
            {
                typedKey = (K)key;
            }
            catch
            {
                return default(V);
            }

            return Get(typedKey);
        }
        
        public bool Remove(K key)
        {
            int index = GetBucketIndex(key);
            Entry<K, V> current = table[index];
            Entry<K, V> previous = null;

            while (current != null)
            {
                bool keysEqual = (key == null && current.Key == null)
                              || (key != null && key.Equals(current.Key));

                if (keysEqual)
                {
                    if (previous == null)
                        table[index] = current.Next; 
                    else
                        previous.Next = current.Next;

                    size--;
                    return true;
                }

                previous = current;
                current = current.Next;
            }

            return false;
        }

        public bool Remove(object key)
        {
            K typedKey;
            try
            {
                typedKey = (K)key;
            }
            catch
            {
                return false;
            }

            return Remove(typedKey);
        }
        
        public bool ContainsKey(K key)
        {
            int index = GetBucketIndex(key);
            Entry<K, V> current = table[index];

            while (current != null)
            {
                bool keysEqual = (key == null && current.Key == null)
                              || (key != null && key.Equals(current.Key));

                if (keysEqual)
                    return true;

                current = current.Next;
            }

            return false;
        }

        public bool ContainsKey(object key)
        {
            K typedKey;
            try
            {
                typedKey = (K)key;
            }
            catch
            {
                return false;
            }

            return ContainsKey(typedKey);
        }
        
        public bool ContainsValue(object value)
        {
            for (int i = 0; i < table.Length; i++)
            {
                Entry<K, V> current = table[i];
                while (current != null)
                {
                    bool valuesEqual = (value == null && current.Value == null)
                                    || (value != null && value.Equals(current.Value));
    
                    if (valuesEqual)
                        return true;
    
                    current = current.Next;
                }
            }
    
            return false;
        }
        
        public void Clear()
        {
            for (int i = 0; i < table.Length; i++)
                table[i] = null;
    
            size = 0;
        }
        
        public bool IsEmpty()
        {
            return size == 0;
        }
        
        public int Size()
        {
            return size;
        }
        
        public HashSet<K> KeySet()
        {
            HashSet<K> keys = new HashSet<K>();
    
            for (int i = 0; i < table.Length; i++)
            {
                Entry<K, V> current = table[i];
                while (current != null)
                {
                    keys.Add(current.Key);
                    current = current.Next;
                }
            }
    
            return keys;
        }
        
        public HashSet<Entry<K, V>> EntrySet()
        {
            HashSet<Entry<K, V>> entries = new HashSet<Entry<K, V>>();
    
            for (int i = 0; i < table.Length; i++)
            {
                Entry<K, V> current = table[i];
                while (current != null)
                {
                    entries.Add(current);
                    current = current.Next;
                }
            }
    
            return entries;
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
    
            bool first = true;
            for (int i = 0; i < table.Length; i++)
            {
                Entry<K, V> current = table[i];
                while (current != null)
                {
                    if (!first)
                        sb.Append(", ");
    
                    sb.Append(current.Key);
                    sb.Append("=");
                    sb.Append(current.Value);
    
                    first = false;
                    current = current.Next;
                }
            }
    
            sb.Append("}");
            return sb.ToString();
        }
    }
    
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Тест MyHashMap<string, int> ===\n");
    
            
            MyHashMap<string, int> map = new MyHashMap<string, int>();
            
            Console.WriteLine("Пусто? " + map.IsEmpty());      // True
            Console.WriteLine("Размер: " + map.Size());         // 0
            
            map.Put("яблоко", 1);
            map.Put("банан", 2);
            map.Put("вишня", 3);
            map.Put("дыня", 4);
    
            Console.WriteLine("\nПосле добавления 4 элементов:");
            Console.WriteLine("Размер: " + map.Size());         // 4
            Console.WriteLine("Пусто? " + map.IsEmpty());       // False
            Console.WriteLine("Отображение: " + map);
            
            Console.WriteLine("\nGet(\"банан\"): " + map.Get("банан")); // 2
            Console.WriteLine("Get(\"груша\"): " + map.Get("груша"));   // 0 (default)
            
            map.Put("банан", 99);
            Console.WriteLine("\nПосле перезаписи банан=99:");
            Console.WriteLine("Get(\"банан\"): " + map.Get("банан")); // 99
            Console.WriteLine("Размер: " + map.Size());               // 4 (не увеличился)
            
            Console.WriteLine("\nContainsKey(\"яблоко\"): " + map.ContainsKey("яблоко")); // True
            Console.WriteLine("ContainsKey(\"груша\"): " + map.ContainsKey("груша"));     // False
            Console.WriteLine("ContainsValue(3): " + map.ContainsValue(3));               // True
            Console.WriteLine("ContainsValue(999): " + map.ContainsValue(999));           // False
            
            Console.WriteLine("\nМножество ключей:");
            foreach (string key in map.KeySet())
                Console.WriteLine("  " + key);
            
            Console.WriteLine("\nМножество пар:");
            foreach (Entry<string, int> entry in map.EntrySet())
                Console.WriteLine("  " + entry.Key + " -> " + entry.Value);
            
            bool removed = map.Remove("вишня");
            Console.WriteLine("\nRemove(\"вишня\"): " + removed);       // True
            Console.WriteLine("Размер после удаления: " + map.Size()); // 3
            Console.WriteLine("ContainsKey(\"вишня\"): " + map.ContainsKey("вишня")); // False
            
            Console.WriteLine("\n=== Тест расширения таблицы ===");
            MyHashMap<int, string> bigMap = new MyHashMap<int, string>(4, 0.75f);
            for (int i = 0; i < 20; i++)
                bigMap.Put(i, "val" + i);
    
            Console.WriteLine("Добавлено 20 элементов, размер: " + bigMap.Size());
            Console.WriteLine("Get(15): " + bigMap.Get(15));
            
            map.Clear();
            Console.WriteLine("\nПосле Clear():");
            Console.WriteLine("Размер: " + map.Size());    // 0
            Console.WriteLine("Пусто? " + map.IsEmpty());  // True
            
            Console.WriteLine("\n=== Тест обработки ошибок ===");
            try
            {
                MyHashMap<string, int> badMap = new MyHashMap<string, int>(-1);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
    
            try
            {
                MyHashMap<string, int> badMap2 = new MyHashMap<string, int>(16, 1.5f);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
    }
}
