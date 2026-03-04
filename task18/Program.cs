namespace task18
{
    using System;
    using System.Collections.Generic;

    public class DefaultComparator<K> : IComparer<K> where K : IComparable<K>
    {
        public int Compare(K? x, K? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.CompareTo(y);
        }
    }

    public class MyTreeMap<K, V> where K : IComparable<K>
    {
        private Node? root;
        private int size;
        private IComparer<K> comparator;

        private class Node
        {
            public K Key;
            public V Value;
            public Node? Left;
            public Node? Right;

            public Node(K key, V value)
            {
                Key = key;
                Value = value;
                Left = Right = null;
            }
        }

        public MyTreeMap()
        {
            comparator = new DefaultComparator<K>();
            root = null;
            size = 0;
        }

        public MyTreeMap(IComparer<K> compar)
        {
            comparator = compar;
            root = null;
            size = 0;
        }

        public void Clear()
        {
            root = null;
            size = 0;
        }

        public bool ContainsKey(K key)
        {
            return ContainsKey(root, key);
        }

        private bool ContainsKey(Node? node, K key)
        {
            if (node == null) return false;
            int comparison = comparator.Compare(key, node.Key);
            if (comparison < 0) return ContainsKey(node.Left, key);
            if (comparison > 0) return ContainsKey(node.Right, key);
            return true;
        }

        public bool ContainsValue(V value)
        {
            return ContainsValue(root, value);
        }

        private bool ContainsValue(Node? node, V value)
        {
            if (node == null) return false;
            if (EqualityComparer<V>.Default.Equals(node.Value, value))
                return true;
            return ContainsValue(node.Left, value) || ContainsValue(node.Right, value);
        }

        public List<KeyValuePair<K, V>> EntrySet()
        {
            List<KeyValuePair<K, V>> entries = new List<KeyValuePair<K, V>>();
            InOrderTraversal(root, entries);
            return entries;
        }

        private void InOrderTraversal(Node? node, List<KeyValuePair<K, V>> entries)
        {
            if (node != null)
            {
                InOrderTraversal(node.Left, entries);
                entries.Add(new KeyValuePair<K, V>(node.Key, node.Value));
                InOrderTraversal(node.Right, entries);
            }
        }

        public V Get(K key)
        {
            Node? node = Get(root, key);
            if (node == null) throw new KeyNotFoundException("Ключ не найден.");
            return node.Value;
        }

        private Node? Get(Node? node, K key)
        {
            if (node == null) return null;
            int comparison = comparator.Compare(key, node.Key);
            if (comparison < 0) return Get(node.Left, key);
            if (comparison > 0) return Get(node.Right, key);
            return node;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public List<K> KeySet()
        {
            List<K> keys = new List<K>();
            InOrderKeyTraversal(root, keys);
            return keys;
        }

        private void InOrderKeyTraversal(Node? node, List<K> keys)
        {
            if (node != null)
            {
                InOrderKeyTraversal(node.Left, keys);
                keys.Add(node.Key);
                InOrderKeyTraversal(node.Right, keys);
            }
        }

        public void Put(K key, V value)
        {
            bool added;
            root = Put(root, key, value, out added);
            if (added) size++;
        }

        private Node? Put(Node? node, K key, V value, out bool added)
        {
            if (node == null)
            {
                added = true;
                return new Node(key, value);
            }
            int comparison = comparator.Compare(key, node.Key);
            if (comparison < 0)
                node.Left = Put(node.Left, key, value, out added);
            else if (comparison > 0)
                node.Right = Put(node.Right, key, value, out added);
            else
            {
                node.Value = value;
                added = false;
            }
            return node;
        }

        public void Remove(K key)
        {
            bool removed;
            root = Remove(root, key, out removed);
            if (removed) size--;
        }

        private Node? Remove(Node? node, K key, out bool removed)
        {
            if (node == null)
            {
                removed = false;
                return null;
            }
            int comparison = comparator.Compare(key, node.Key);
            if (comparison < 0)
            {
                node.Left = Remove(node.Left, key, out removed);
                return node;
            }
            if (comparison > 0)
            {
                node.Right = Remove(node.Right, key, out removed);
                return node;
            }
            removed = true;
            if (node.Left == null) return node.Right;
            if (node.Right == null) return node.Left;
            Node minNode = GetMin(node.Right)!;
            node.Key = minNode.Key;
            node.Value = minNode.Value;
            node.Right = Remove(node.Right, minNode.Key, out _);
            return node;
        }

        private Node? GetMin(Node? node)
        {
            if (node == null) return null;
            while (node.Left != null) node = node.Left;
            return node;
        }

        public int Size()
        {
            return size;
        }

        public K FirstKey()
        {
            if (root == null) throw new InvalidOperationException("Дерево пусто.");
            return GetMin(root)!.Key;
        }

        public K LastKey()
        {
            if (root == null) throw new InvalidOperationException("Дерево пусто.");
            Node node = root;
            while (node.Right != null) node = node.Right;
            return node.Key;
        }

        public List<KeyValuePair<K, V>> HeadMap(K end)
        {
            List<KeyValuePair<K, V>> result = new List<KeyValuePair<K, V>>();
            HeadMap(root, end, result);
            return result;
        }

        private void HeadMap(Node? node, K end, List<KeyValuePair<K, V>> result)
        {
            if (node == null) return;
            int comparison = comparator.Compare(node.Key, end);
            if (comparison < 0)
            {
                result.Add(new KeyValuePair<K, V>(node.Key, node.Value));
                HeadMap(node.Left, end, result);
                HeadMap(node.Right, end, result);
            }
            else
            {
                HeadMap(node.Left, end, result);
            }
        }

        public List<KeyValuePair<K, V>> SubMap(K start, K end)
        {
            List<KeyValuePair<K, V>> result = new List<KeyValuePair<K, V>>();
            SubMap(root, start, end, result);
            return result;
        }

        private void SubMap(Node? node, K start, K end, List<KeyValuePair<K, V>> result)
        {
            if (node == null) return;
            int cmpStart = comparator.Compare(node.Key, start);
            int cmpEnd = comparator.Compare(node.Key, end);
            if (cmpStart >= 0 && cmpEnd < 0)
                result.Add(new KeyValuePair<K, V>(node.Key, node.Value));
            if (cmpStart > 0) SubMap(node.Left, start, end, result);
            if (cmpEnd < 0) SubMap(node.Right, start, end, result);
        }

        public List<KeyValuePair<K, V>> TailMap(K start)
        {
            List<KeyValuePair<K, V>> result = new List<KeyValuePair<K, V>>();
            TailMap(root, start, result);
            return result;
        }

        private void TailMap(Node? node, K start, List<KeyValuePair<K, V>> result)
        {
            if (node == null) return;
            int cmp = comparator.Compare(node.Key, start);
            if (cmp >= 0)
            {
                result.Add(new KeyValuePair<K, V>(node.Key, node.Value));
                TailMap(node.Right, start, result);
            }
            else
            {
                TailMap(node.Left, start, result);
            }
        }

        public KeyValuePair<K, V> LowerEntry(K key)
        {
            Node? node = LowerEntry(root, key);
            if (node == null) throw new KeyNotFoundException("Нет элемента, меньшего указанного ключа.");
            return new KeyValuePair<K, V>(node.Key, node.Value);
        }

        private Node? LowerEntry(Node? node, K key)
        {
            if (node == null) return null;
            int cmp = comparator.Compare(node.Key, key);
            if (cmp < 0)
            {
                Node? right = LowerEntry(node.Right, key);
                return right != null ? right : node;
            }
            return LowerEntry(node.Left, key);
        }

        public KeyValuePair<K, V> FloorEntry(K key)
        {
            Node? node = FloorEntry(root, key);
            if (node == null) throw new KeyNotFoundException("Нет элемента, меньшего или равного указанному ключу.");
            return new KeyValuePair<K, V>(node.Key, node.Value);
        }

        private Node? FloorEntry(Node? node, K key)
        {
            if (node == null) return null;
            int cmp = comparator.Compare(node.Key, key);
            if (cmp == 0) return node;
            if (cmp < 0)
            {
                Node? right = FloorEntry(node.Right, key);
                return right != null ? right : node;
            }
            return FloorEntry(node.Left, key);
        }

        public KeyValuePair<K, V> HigherEntry(K key)
        {
            Node? node = HigherEntry(root, key);
            if (node == null) throw new KeyNotFoundException("Нет элемента, большего указанного ключа.");
            return new KeyValuePair<K, V>(node.Key, node.Value);
        }

        private Node? HigherEntry(Node? node, K key)
        {
            if (node == null) return null;
            int cmp = comparator.Compare(node.Key, key);
            if (cmp > 0)
            {
                Node? left = HigherEntry(node.Left, key);
                return left != null ? left : node;
            }
            return HigherEntry(node.Right, key);
        }

        public KeyValuePair<K, V> CeilingEntry(K key)
        {
            Node? node = CeilingEntry(root, key);
            if (node == null) throw new KeyNotFoundException("Нет элемента, большего или равного указанному ключу.");
            return new KeyValuePair<K, V>(node.Key, node.Value);
        }

        private Node? CeilingEntry(Node? node, K key)
        {
            if (node == null) return null;
            int cmp = comparator.Compare(node.Key, key);
            if (cmp == 0) return node;
            if (cmp > 0)
            {
                Node? left = CeilingEntry(node.Left, key);
                return left != null ? left : node;
            }
            return CeilingEntry(node.Right, key);
        }

        public K LowerKey(K key)
        {
            Node? node = LowerEntry(root, key);
            if (node == null) throw new KeyNotFoundException("Нет элемента, меньшего указанного ключа.");
            return node.Key;
        }

        public K FloorKey(K key)
        {
            Node? node = FloorEntry(root, key);
            if (node == null) throw new KeyNotFoundException("Нет элемента, меньшего или равного указанному ключу.");
            return node.Key;
        }

        public K HigherKey(K key)
        {
            Node? node = HigherEntry(root, key);
            if (node == null) throw new KeyNotFoundException("Нет элемента, большего указанного ключа.");
            return node.Key;
        }

        public K CeilingKey(K key)
        {
            Node? node = CeilingEntry(root, key);
            if (node == null) throw new KeyNotFoundException("Нет элемента, большего или равного указанному ключу.");
            return node.Key;
        }

        public KeyValuePair<K, V> PollFirstEntry()
        {
            if (root == null) throw new InvalidOperationException("Дерево пусто.");
            Node minNode = GetMin(root)!;
            bool removed;
            root = Remove(root, minNode.Key, out removed);
            if (removed) size--;
            return new KeyValuePair<K, V>(minNode.Key, minNode.Value);
        }

        public KeyValuePair<K, V> PollLastEntry()
        {
            if (root == null) throw new InvalidOperationException("Дерево пусто.");
            Node maxNode = GetMax(root)!;
            bool removed;
            root = Remove(root, maxNode.Key, out removed);
            if (removed) size--;
            return new KeyValuePair<K, V>(maxNode.Key, maxNode.Value);
        }

        private Node? GetMax(Node? node)
        {
            if (node == null) return null;
            while (node.Right != null) node = node.Right;
            return node;
        }

        public KeyValuePair<K, V> FirstEntry()
        {
            if (root == null) throw new InvalidOperationException("Дерево пусто.");
            Node minNode = GetMin(root)!;
            return new KeyValuePair<K, V>(minNode.Key, minNode.Value);
        }

        public KeyValuePair<K, V> LastEntry()
        {
            if (root == null) throw new InvalidOperationException("Дерево пусто.");
            Node maxNode = GetMax(root)!;
            return new KeyValuePair<K, V>(maxNode.Key, maxNode.Value);
        }
    }

    public class Program
    {
        public static void Main()
        {
            MyTreeMap<int, string> map = new MyTreeMap<int, string>();
            map.Put(10, "apple");
            map.Put(5, "banana");
            map.Put(15, "cherry");
            map.Put(12, "grape");

            var entries = map.EntrySet();
            Console.WriteLine("All entries:");
            foreach (var entry in entries)
                Console.WriteLine($"{entry.Key}: {entry.Value}");

            Console.WriteLine("\nGet value for key 10: " + map.Get(10));
            Console.WriteLine("\nContains key 5: " + map.ContainsKey(5));
            Console.WriteLine("\nContains value 'cherry': " + map.ContainsValue("cherry"));
            Console.WriteLine("\nFirst key: " + map.FirstKey());
            Console.WriteLine("Last key: " + map.LastKey());

            var headMap = map.HeadMap(12);
            Console.WriteLine("\nHeadMap (keys < 12):");
            foreach (var entry in headMap)
                Console.WriteLine($"{entry.Key}: {entry.Value}");

            var subMap = map.SubMap(5, 12);
            Console.WriteLine("\nSubMap (keys >= 5 and < 12):");
            foreach (var entry in subMap)
                Console.WriteLine($"{entry.Key}: {entry.Value}");

            map.Remove(10);
            Console.WriteLine("\nAfter removing key 10:");
            entries = map.EntrySet();
            foreach (var entry in entries)
                Console.WriteLine($"{entry.Key}: {entry.Value}");

            Console.WriteLine("\nFirst key: " + map.FirstKey());
            Console.WriteLine("Last key: " + map.LastKey());
            Console.WriteLine("\nSize of map: " + map.Size());
        }
    }
}
