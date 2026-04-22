namespace task19
{
    using System;
    using System.Collections;
    using System.Collections.Generic;


    public interface TreeMapComparator<E>
    {
        int Compare(E a, E b);
    }

    internal class DefaultTreeMapComparator<E> : TreeMapComparator<E> where E : IComparable<E>
    {
        public int Compare(E a, E b)
        {
            if (a == null && b == null) return 0;
            if (a == null) return -1;
            if (b == null) return 1;
            return a.CompareTo(b);
        }
    }

    public class MyTreeMap<K, V> where K : IComparable<K>
    {
        private class Node
        {
            public K Key;
            public V Value;
            public Node Left;
            public Node Right;
            public Node Parent;
            public bool isRed; 

            public Node(K key, V value, bool red)
            {
                Key = key;
                Value = value;
                Left = null;
                Right = null;
                Parent = null;
                isRed = red;
            }
        }

        private Node root;
        private int size;
        private TreeMapComparator<K> comparator;

        private int Cmp(K a, K b)
        {
            return comparator.Compare(a, b);
        }

        public MyTreeMap()
        {
            comparator = new DefaultTreeMapComparator<K>();
            root = null;
            size = 0;
        }

        public MyTreeMap(TreeMapComparator<K> comp)
        {
            comparator = comp != null ? comp : new DefaultTreeMapComparator<K>();
            root = null;
            size = 0;
        }

        public TreeMapComparator<K> GetComparator()
        {
            return comparator;
        }

        private void RotateLeft(Node node)
        {
            Node r = node.Right;
            if (r == null) return;

            node.Right = r.Left;
            if (r.Left != null)
                r.Left.Parent = node;

            r.Parent = node.Parent;
            if (node.Parent == null)
                root = r;
            else if (node == node.Parent.Left)
                node.Parent.Left = r;
            else
                node.Parent.Right = r;

            r.Left = node;
            node.Parent = r;
        }

        private void RotateRight(Node node)
        {
            Node L = node.Left;
            if (L == null) return;

            node.Left = L.Right;
            if (L.Right != null)
                L.Right.Parent = node;

            L.Parent = node.Parent;
            if (node.Parent == null)
                root = L;
            else if (node == node.Parent.Right)
                node.Parent.Right = L;
            else
                node.Parent.Left = L;

            L.Right = node;
            node.Parent = L;
        }

        // Восстановление свойств
        private void InsertFixup(Node z)
        {
            while (z.Parent != null && z.Parent.isRed)
            {
                if (z.Parent == z.Parent.Parent.Left)
                {
                    Node y = z.Parent.Parent.Right;
                    if (y != null && y.isRed)
                    {
                        z.Parent.isRed = false;
                        y.isRed = false;
                        z.Parent.Parent.isRed = true;
                        z = z.Parent.Parent;
                    }
                    else
                    {
                        if (z == z.Parent.Right)
                        {
                            z = z.Parent;
                            RotateLeft(z);
                        }
                        z.Parent.isRed = false;
                        z.Parent.Parent.isRed = true;
                        RotateRight(z.Parent.Parent);
                        break;
                    }
                }
                else
                {
                    // симметричный случай
                    Node y = z.Parent.Parent.Left;
                    if (y != null && y.isRed)
                    {
                        z.Parent.isRed = false;
                        y.isRed = false;
                        z.Parent.Parent.isRed = true;
                        z = z.Parent.Parent;
                    }
                    else
                    {
                        if (z == z.Parent.Left)
                        {
                            z = z.Parent;
                            RotateRight(z);
                        }
                        z.Parent.isRed = false;
                        z.Parent.Parent.isRed = true;
                        RotateLeft(z.Parent.Parent);
                        break;
                    }
                }
            }
            root.isRed = false;
        }

        private void DeleteFixup(Node x, Node xParent, bool xIsLeft)
        {
            while (x != root && (x == null || !x.isRed))
            {
                Node w;
                if (xIsLeft && xParent != null)
                {
                    w = xParent.Right;
                    if (w != null && w.isRed)
                    {
                        w.isRed = false;
                        xParent.isRed = true;
                        RotateLeft(xParent);
                        w = xParent.Right;
                    }
                    if (w == null) break;
                    bool wLeftBlack = (w.Left == null || !w.Left.isRed);
                    bool wRightBlack = (w.Right == null || !w.Right.isRed);
                    if (wLeftBlack && wRightBlack)
                    {
                        w.isRed = true;
                        x = xParent;
                        xParent = x != null ? x.Parent : null;
                        xIsLeft = (x != null && xParent != null && x == xParent.Left);
                    }
                    else
                    {
                        if (w.Right == null || !w.Right.isRed)
                        {
                            if (w.Left != null) w.Left.isRed = false;
                            w.isRed = true;
                            RotateRight(w);
                            w = xParent != null ? xParent.Right : null;
                        }
                        if (w != null && xParent != null)
                        {
                            w.isRed = xParent.isRed;
                            xParent.isRed = false;
                            if (w.Right != null) w.Right.isRed = false;
                            RotateLeft(xParent);
                        }
                        break;
                    }
                }
                else if (!xIsLeft && xParent != null)
                {
                    w = xParent.Left;
                    if (w != null && w.isRed)
                    {
                        w.isRed = false;
                        xParent.isRed = true;
                        RotateRight(xParent);
                        w = xParent.Left;
                    }
                    if (w == null) break;
                    bool wLeftBlack2 = (w.Left == null || !w.Left.isRed);
                    bool wRightBlack2 = (w.Right == null || !w.Right.isRed);
                    if (wLeftBlack2 && wRightBlack2)
                    {
                        w.isRed = true;
                        x = xParent;
                        xParent = x != null ? x.Parent : null;
                        xIsLeft = (x != null && xParent != null && x == xParent.Left);
                    }
                    else
                    {
                        if (w.Left == null || !w.Left.isRed)
                        {
                            if (w.Right != null) w.Right.isRed = false;
                            w.isRed = true;
                            RotateLeft(w);
                            w = xParent != null ? xParent.Left : null;
                        }
                        if (w != null && xParent != null)
                        {
                            w.isRed = xParent.isRed;
                            xParent.isRed = false;
                            if (w.Left != null) w.Left.isRed = false;
                            RotateRight(xParent);
                        }
                        break;
                    }
                }
                else
                    break;
            }
            if (x != null)
                x.isRed = false;
        }

        private Node FindNode(K key)
        {
            Node p = root;
            while (p != null)
            {
                int c = Cmp(key, p.Key);
                if (c == 0) return p;
                if (c < 0)
                    p = p.Left;
                else
                    p = p.Right;
            }
            return null;
        }

        private Node Minimum(Node x)
        {
            while (x.Left != null)
                x = x.Left;
            return x;
        }

        private void Transplant(Node u, Node v)
        {
            if (u.Parent == null)
                root = v;
            else if (u == u.Parent.Left)
                u.Parent.Left = v;
            else
                u.Parent.Right = v;
            if (v != null)
                v.Parent = u.Parent;
        }

        public void Put(K key, V value)
        {
            Node parent = null;
            Node current = root;
            while (current != null)
            {
                parent = current;
                int c = Cmp(key, current.Key);
                if (c == 0)
                {
                    current.Value = value;
                    return;
                }
                if (c < 0)
                    current = current.Left;
                else
                    current = current.Right;
            }
            Node z = new Node(key, value, true); 
            z.Parent = parent;
            if (parent == null)
                root = z;
            else if (Cmp(key, parent.Key) < 0)
                parent.Left = z;
            else
                parent.Right = z;
            size++;
            InsertFixup(z);
        }
        
        /// <summary>
        /// Возвращает значение, связанное с указанным ключом.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Ключ не найден.</exception>
        public V Get(K key)
        {
            Node node = FindNode(key);
            if (node == null)
                throw new KeyNotFoundException($"Key '{key}' not found.");
            return node.Value;
        }

        /// <summary>
        /// Индексатор для доступа к значению по ключу (аналог Get).
        /// </summary>
        public V this[K key] => Get(key);

        public void Remove(K key)
        {
            Node z = FindNode(key);
            if (z == null) return;

            Node y = z;
            bool yWasRed = y.isRed;
            Node x = null;
            Node xParent = null;
            bool xIsLeft = false;

            if (z.Left == null)
            {
                x = z.Right;
                xParent = z.Parent;
                xIsLeft = (z.Parent != null && z == z.Parent.Left);
                Transplant(z, z.Right);
            }
            else if (z.Right == null)
            {
                x = z.Left;
                xParent = z.Parent;
                xIsLeft = (z.Parent != null && z == z.Parent.Left);
                Transplant(z, z.Left);
            }
            else
            {
                y = Minimum(z.Right);
                yWasRed = y.isRed;
                x = y.Right;
                xParent = y;
                xIsLeft = false;
                if (y.Parent == z)
                {
                    if (x != null) x.Parent = y;
                }
                else
                {
                    Transplant(y, y.Right);
                    y.Right = z.Right;
                    if (y.Right != null) y.Right.Parent = y;
                    xParent = y.Parent;
                    xIsLeft = (y.Parent != null && y == y.Parent.Left);
                }
                Transplant(z, y);
                y.Left = z.Left;
                if (y.Left != null) y.Left.Parent = y;
                y.isRed = z.isRed;
            }
            size--;
            if (!yWasRed)
                DeleteFixup(x, xParent, xIsLeft);
        }

        // Обход дерева слева направо (возвращает ключи по возрастанию)
        private void InOrderKeys(Node node, List<K> list)
        {
            if (node == null) return;
            InOrderKeys(node.Left, list);
            list.Add(node.Key);
            InOrderKeys(node.Right, list);
        }

        public List<K> KeySet()
        {
            List<K> list = new List<K>(size);
            InOrderKeys(root, list);
            return list;
        }

        public void Clear()
        {
            root = null;
            size = 0;
        }

        public bool ContainsKey(K key)
        {
            return FindNode(key) != null;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public int Size()
        {
            return size;
        }

        public K FirstKey()
        {
            if (root == null) throw new InvalidOperationException("Map is empty");
            return Minimum(root).Key;
        }

        public K LastKey()
        {
            if (root == null) throw new InvalidOperationException("Map is empty");
            Node p = root;
            while (p.Right != null)
                p = p.Right;
            return p.Key;
        }

        //наименьший ключ строго больше или равен данного
        public K CeilingKey(K key)
        {
            Node p = root;
            Node candidate = null;
            while (p != null)
            {
                int c = Cmp(key, p.Key);
                if (c == 0) return p.Key;
                if (c < 0) { candidate = p; p = p.Left; }
                else p = p.Right;
            }
            if (candidate == null) throw new KeyNotFoundException();
            return candidate.Key;
        }

        //наибольший ключ строго меньше или равен данного
        public K FloorKey(K key)
        {
            Node p = root;
            Node candidate = null;
            while (p != null)
            {
                int c = Cmp(key, p.Key);
                if (c == 0) return p.Key;
                if (c > 0) { candidate = p; p = p.Right; }
                else p = p.Left;
            }
            if (candidate == null) throw new KeyNotFoundException();
            return candidate.Key;
        }

        //наименьший ключ строго больше данного
        public K HigherKey(K key)
        {
            Node p = root;
            Node candidate = null;
            while (p != null)
            {
                int c = Cmp(key, p.Key);
                if (c < 0) { candidate = p; p = p.Left; }
                else p = p.Right;
            }
            if (candidate == null) throw new KeyNotFoundException();
            return candidate.Key;
        }

        //наибольший ключ строго меньше данного
        public K LowerKey(K key)
        {
            Node p = root;
            Node candidate = null;
            while (p != null)
            {
                int c = Cmp(key, p.Key);
                if (c > 0) { candidate = p; p = p.Right; }
                else p = p.Left;
            }
            if (candidate == null) throw new KeyNotFoundException();
            return candidate.Key;
        }

        public KeyValuePair<K, V> PollFirstEntry()
        {
            if (root == null) throw new InvalidOperationException("Map is empty");
            Node min = Minimum(root);
            KeyValuePair<K, V> kv = new KeyValuePair<K, V>(min.Key, min.Value);
            Remove(min.Key);
            return kv;
        }

        public KeyValuePair<K, V> PollLastEntry()
        {
            if (root == null) throw new InvalidOperationException("Map is empty");
            Node p = root;
            while (p.Right != null)
                p = p.Right;
            KeyValuePair<K, V> kv = new KeyValuePair<K, V>(p.Key, p.Value);
            Remove(p.Key);
            return kv;
        }
    }


    public class MyTreeSet<E> where E : IComparable<E>
    {
        private MyTreeMap<E, object> m;
        private static readonly object Dummy = new object();

        // 1. Конструктор MyTreeSet() для создания пустого множества, размещающего элементы согласно естественному порядку сортировки.
        public MyTreeSet()
        {
            m = new MyTreeMap<E, object>();
        }

        // 2. Конструктор MyTreeSet(MyTreeMap<E, object> m) для создания множества, использующего указанный объект MyTreeMap для хранения элементов.
        public MyTreeSet(MyTreeMap<E, object> map)
        {
            m = map != null ? map : new MyTreeMap<E, object>();
        }

        // 3. Конструктор MyTreeSet(TreeMapComparator comparator) для создания пустого множества, размещающего элементы согласно указанному компаратору.
        public MyTreeSet(TreeMapComparator<E> comparator)
        {
            if (comparator == null) throw new ArgumentNullException(nameof(comparator));
            m = new MyTreeMap<E, object>(comparator);
        }

        // 4. Конструктор MyTreeSet(T[] a) для создания множества, содержащего элементы указанной коллекции.
        public MyTreeSet(E[] a)
        {
            m = new MyTreeMap<E, object>();
            if (a != null)
                AddAll(a);
        }

        // 5. Конструктор MyTreeSet(SortedSet<E> s) для создания множества, содержащего элементы указанного сортированного множества.
        public MyTreeSet(SortedSet<E> s)
        {
            m = new MyTreeMap<E, object>();
            if (s != null)
            {
                foreach (E e in s)
                    Add(e);
            }
        }

        private MyTreeSet(MyTreeMap<E, object> map, bool dummy)
        {
            m = map;
        }

        // 6. Метод add(T e) для добавления элемента в конец множества.
        public void Add(E e)
        {
            m.Put(e, Dummy);
        }

        // 7. Метод addAll(T[] a) для добавления элементов из массива.
        public void AddAll(E[] a)
        {
            if (a == null) return;
            for (int i = 0; i < a.Length; i++)
                Add(a[i]);
        }

        // 8. Метод clear() для удаления всех элементов из множества.
        public void Clear()
        {
            m.Clear();
        }

        // 9. Метод contains(object o) для проверки, находится ли указанный объект во множестве.
        public bool Contains(object o)
        {
            if (o == null) return false;
            if (o is E e)
                return m.ContainsKey(e);
            return false;
        }

        // 10. Метод containsAll(T[] a) для проверки, содержатся ли указанные объекты во множестве.
        public bool ContainsAll(E[] a)
        {
            if (a == null) return true;
            for (int i = 0; i < a.Length; i++)
                if (!Contains(a[i])) return false;
            return true;
        }

        // 11. Метод isEmpty() для проверки, является ли множество пустым.
        public bool IsEmpty()
        {
            return m.IsEmpty();
        }

        // 12. Метод remove(object o) для удаления указанного объекта из множества, если он есть там.
        public bool Remove(object o)
        {
            if (o == null) return false;
            if (!(o is E e)) return false;
            if (!m.ContainsKey(e)) return false;
            m.Remove(e);
            return true;
        }

        // 13. Метод removeAll(T[] a) для удаления указанных объектов из множества.
        public void RemoveAll(E[] a)
        {
            if (a == null) return;
            for (int i = 0; i < a.Length; i++)
                Remove(a[i]);
        }

        // 14. Метод retainAll(T[] a) для оставления во множестве только указанных объектов.
        public void RetainAll(E[] a)
        {
            if (a == null) return;
            HashSet<E> toKeep = new HashSet<E>();
            for (int i = 0; i < a.Length; i++)
                toKeep.Add(a[i]);
            List<E> keys = m.KeySet();
            for (int i = 0; i < keys.Count; i++)
                if (!toKeep.Contains(keys[i]))
                    m.Remove(keys[i]);
        }

        // 15. Метод size() для получения размера множества в элементах.
        public int Size()
        {
            return m.Size();
        }

        // 16. Метод toArray() для возвращения массива объектов, содержащего все элементы множества.
        public E[] ToArray()
        {
            return m.KeySet().ToArray();
        }

        // 17. Метод toArray(T[] a) для возвращения массива объектов, содержащего все элементы множества. Если аргумент a равен null, то создаётся новый массив, в который копируются элементы.
        public E[] ToArray(E[] a)
        {
            List<E> keys = m.KeySet();
            if (a == null)
                return keys.ToArray();
            if (a.Length < keys.Count)
                return keys.ToArray();
            for (int i = 0; i < keys.Count; i++)
                a[i] = keys[i];
            if (a.Length > keys.Count)
                a[keys.Count] = default(E);
            return a;
        }

        // 18. Метод first() для возврата первого (наименьшего) элемента множества.
        public E First()
        {
            if (m.IsEmpty()) throw new InvalidOperationException("Set is empty");
            return m.FirstKey();
        }

        // 19. Метод last() для возврата последнего (наивысшего) элемента множества.
        public E Last()
        {
            if (m.IsEmpty()) throw new InvalidOperationException("Set is empty");
            return m.LastKey();
        }

        // 20. Метод subSet(E fromElement, E toElement) для возврата подмножества элементов из диапазона [fromElement; toElement).
        public MyTreeSet<E> SubSet(E fromElement, E toElement)
        {
            MyTreeSet<E> result = new MyTreeSet<E>(new MyTreeMap<E, object>(m.GetComparator()), false);
            List<E> keys = m.KeySet();
            TreeMapComparator<E> comp = m.GetComparator();
            for (int i = 0; i < keys.Count; i++)
            {
                E k = keys[i];
                if (comp.Compare(k, fromElement) >= 0 && comp.Compare(k, toElement) < 0)
                    result.Add(k);
            }
            return result;
        }

        // 21. Метод headSet(E toElement) для возврата множества элементов, меньших чем указанный элемент.
        public MyTreeSet<E> HeadSet(E toElement)
        {
            MyTreeSet<E> result = new MyTreeSet<E>(new MyTreeMap<E, object>(m.GetComparator()), false);
            List<E> keys = m.KeySet();
            TreeMapComparator<E> comp = m.GetComparator();
            for (int i = 0; i < keys.Count; i++)
            {
                if (comp.Compare(keys[i], toElement) < 0)
                    result.Add(keys[i]);
            }
            return result;
        }

        // 22. Метод tailSet(E fromElement) для возврата части множества из элементов, больших или равных указанному элементу.
        public MyTreeSet<E> TailSet(E fromElement)
        {
            MyTreeSet<E> result = new MyTreeSet<E>(new MyTreeMap<E, object>(m.GetComparator()), false);
            List<E> keys = m.KeySet();
            TreeMapComparator<E> comp = m.GetComparator();
            for (int i = 0; i < keys.Count; i++)
            {
                if (comp.Compare(keys[i], fromElement) >= 0)
                    result.Add(keys[i]);
            }
            return result;
        }

        // 23. Метод ceiling(E obj) для поиска в наборе наименьшего элемента e, для которого e >= obj. Если такой элемент найден, он возвращается. В противном случае возвращается null.
        public E Ceiling(E obj)
        {
            try { return m.CeilingKey(obj); }
            catch (KeyNotFoundException) { return default(E); }
        }

        // 24. Метод floor(E obj) для поиска в наборе наибольшего элемента e, для которого e <= obj. Если такой элемент найден, он возвращается. В противном случае возвращается null.
        public E Floor(E obj)
        {
            try { return m.FloorKey(obj); }
            catch (KeyNotFoundException) { return default(E); }
        }

        // 25. Метод higher(E obj) для поиска в наборе наибольшего элемента e, для которого e > obj. Если такой элемент найден, он возвращается. В противном случае возвращается null.
        public E Higher(E obj)
        {
            try { return m.HigherKey(obj); }
            catch (KeyNotFoundException) { return default(E); }
        }

        // 26. Метод lower(E obj) для поиска в наборе наименьшего элемента e, для которого e < obj. Если такой элемент найден, он возвращается. В противном случае возвращается null.
        public E Lower(E obj)
        {
            try { return m.LowerKey(obj); }
            catch (KeyNotFoundException) { return default(E); }
        }

        // 27. Метод headSet(E upperBound, bool incl) для возврата множества, включающего все элементы вызывающего набора, меньшие upperBound. Результирующий набор поддерживается вызывающим набором.
        public MyTreeSet<E> HeadSet(E upperBound, bool incl)
        {
            MyTreeSet<E> result = new MyTreeSet<E>(new MyTreeMap<E, object>(m.GetComparator()), false);
            List<E> keys = m.KeySet();
            TreeMapComparator<E> comp = m.GetComparator();
            for (int i = 0; i < keys.Count; i++)
            {
                E k = keys[i];
                int c = comp.Compare(k, upperBound);
                if (c < 0 || (incl && c == 0))
                    result.Add(k);
            }
            return result;
        }

        // 28. Метод subSet(E lowerBound, bool lowIncl, E upperBound, bool highIncl) для возврата NavigableSet: все элементы больше lowerBound и меньше upperBound; если lowIncl — включается элемент, равный lowerBound; если highIncl — элемент, равный upperBound.
        public MyTreeSet<E> SubSet(E lowerBound, bool lowIncl, E upperBound, bool highIncl)
        {
            MyTreeSet<E> result = new MyTreeSet<E>(new MyTreeMap<E, object>(m.GetComparator()), false);
            List<E> keys = m.KeySet();
            TreeMapComparator<E> comp = m.GetComparator();
            for (int i = 0; i < keys.Count; i++)
            {
                E k = keys[i];
                int cl = comp.Compare(k, lowerBound);
                int ch = comp.Compare(k, upperBound);
                bool okLow = cl > 0 || (lowIncl && cl == 0);
                bool okHigh = ch < 0 || (highIncl && ch == 0);
                if (okLow && okHigh)
                    result.Add(k);
            }
            return result;
        }

        // 29. Метод tailSet(E fromElement, bool inclusive) для возврата множества из элементов, больших (или равных, если inclusive равно true) чем fromElement. Результирующий набор поддерживается вызывающим набором.
        public MyTreeSet<E> TailSet(E fromElement, bool inclusive)
        {
            MyTreeSet<E> result = new MyTreeSet<E>(new MyTreeMap<E, object>(m.GetComparator()), false);
            List<E> keys = m.KeySet();
            TreeMapComparator<E> comp = m.GetComparator();
            for (int i = 0; i < keys.Count; i++)
            {
                E k = keys[i];
                int c = comp.Compare(k, fromElement);
                if (c > 0 || (inclusive && c == 0))
                    result.Add(k);
            }
            return result;
        }

        // 30. Метод pollLast() для возврата последнего элемента, удаляя его в процессе (элемент с наибольшим значением). Возвращает null в случае пустого набора.
        public E PollLast()
        {
            if (m.IsEmpty()) return default(E);
            KeyValuePair<E, object> entry = m.PollLastEntry();
            return entry.Key;
        }

        // 31. Метод pollFirst() для возврата первого элемента, удаляя его в процессе (элемент с наименьшим значением). Возвращает null в случае пустого набора.
        public E PollFirst()
        {
            if (m.IsEmpty()) return default(E);
            KeyValuePair<E, object> entry = m.PollFirstEntry();
            return entry.Key;
        }

        // 32. Метод descendingIterator() для возврата итератора, перемещающегося от большего к меньшему (обратного итератора).
        public IEnumerator<E> DescendingIterator()
        {
            List<E> keys = m.KeySet();
            for (int i = keys.Count - 1; i >= 0; i--)
                yield return keys[i];
        }

        // 33. Метод descendingSet() для возврата множества, представляющего собой обратную версию вызывающего набора. Результирующий набор поддерживается вызывающим набором.
        public MyTreeSet<E> DescendingSet()
        {
            MyTreeSet<E> result = new MyTreeSet<E>(new MyTreeMap<E, object>(m.GetComparator()), false);
            List<E> keys = m.KeySet();
            for (int i = keys.Count - 1; i >= 0; i--)
                result.Add(keys[i]);
            return result;
        }
    }

    public class Program
    {
        public static void Main()
        {
            MyTreeSet<int> set = new MyTreeSet<int>();
            set.Add(5);
            set.Add(2);
            set.Add(8);
            set.Add(2);
            Console.WriteLine("Size: " + set.Size());
            Console.WriteLine("First: " + set.First());
            Console.WriteLine("Last: " + set.Last());
            Console.WriteLine("Contains 2: " + set.Contains(2));
            Console.WriteLine("Contains 10: " + set.Contains(10));
            int[] arr = set.ToArray();
            Console.Write("ToArray: ");
            for (int i = 0; i < arr.Length; i++)
                Console.Write(arr[i] + " ");
            Console.WriteLine();
        }
    }
}
