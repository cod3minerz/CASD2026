using System;
using System.Collections.Generic;
using task17;
using Task25;

namespace Task28
{
    public class MyIteratorException : Exception
    {
        public MyIteratorException(string message) : base(message) { }
    }

    public class NoSuchElementException : MyIteratorException
    {
        public NoSuchElementException() : base("Нет следующего элемента") { }
    }

    public class IllegalStateException : MyIteratorException
    {
        public IllegalStateException() : base("Недопустимое состояние итератора") { }
    }

    public class IndexOutOfBoundsException : MyIteratorException
    {
        public IndexOutOfBoundsException(string message) : base(message) { }
    }

    public interface IMyIterator<T>
    {
        bool HasNext();
        T Next();
        void Remove();
    }

    public interface IMyListIterator<T> : IMyIterator<T>
    {
        bool HasPrevious();
        T Previous();
        int NextIndex();
        int PreviousIndex();
        void Set(T element);
        void Add(T element);
    }

    public class MyPriorityQueue<T> : global::MyPriorityQueue<T>
    {
        private class MyItr : IMyIterator<T>
        {
            private T[] data;
            private int cursor;
            private int lastIdx;
            private MyPriorityQueue<T> owner;

            public MyItr(MyPriorityQueue<T> owner)
            {
                this.owner = owner;
                data = owner.ToArray();
                cursor = 0;
                lastIdx = -1;
            }

            public bool HasNext()
            {
                return cursor < data.Length;
            }

            public T Next()
            {
                if (!HasNext())
                    throw new NoSuchElementException();
                lastIdx = cursor;
                return data[cursor++];
            }

            public void Remove()
            {
                if (lastIdx < 0)
                    throw new IllegalStateException();
                owner.Remove(data[lastIdx]);
                lastIdx = -1;
            }
        }

        public MyPriorityQueue() : base() { }
        public MyPriorityQueue(T[] a) : base(a) { }
        public MyPriorityQueue(int initialCapacity) : base(initialCapacity) { }

        public IMyIterator<T> Iterator()
        {
            return new MyItr(this);
        }
    }

    public class MyArrayDeque<T> : global::MyArrayDeque<T>
    {
        private class MyItr : IMyIterator<T>
        {
            private T[] data;
            private int cursor;
            private int lastIdx;
            private MyArrayDeque<T> owner;

            public MyItr(MyArrayDeque<T> owner)
            {
                this.owner = owner;
                data = owner.ToArray(new T[owner.Size()]);
                cursor = 0;
                lastIdx = -1;
            }

            public bool HasNext()
            {
                return cursor < data.Length;
            }

            public T Next()
            {
                if (!HasNext())
                    throw new NoSuchElementException();
                lastIdx = cursor;
                return data[cursor++];
            }

            public void Remove()
            {
                if (lastIdx < 0)
                    throw new IllegalStateException();
                owner.Remove(data[lastIdx]);
                lastIdx = -1;
            }
        }

        public MyArrayDeque() : base() { }
        public MyArrayDeque(T[] a) : base(a) { }

        public IMyIterator<T> Iterator()
        {
            return new MyItr(this);
        }
    }

    public class MyHashSet<E> : Task25.MyHashSet<E> where E : IComparable<E>
    {
        private class MyItr : IMyIterator<E>
        {
            private E[] data;
            private int cursor;
            private int lastIdx;
            private MyHashSet<E> owner;

            public MyItr(MyHashSet<E> owner)
            {
                this.owner = owner;
                data = owner.ToArray();
                cursor = 0;
                lastIdx = -1;
            }

            public bool HasNext()
            {
                return cursor < data.Length;
            }

            public E Next()
            {
                if (!HasNext())
                    throw new NoSuchElementException();
                lastIdx = cursor;
                return data[cursor++];
            }

            public void Remove()
            {
                if (lastIdx < 0)
                    throw new IllegalStateException();
                owner.Remove(data[lastIdx]);
                lastIdx = -1;
            }
        }

        public MyHashSet() : base() { }
        public MyHashSet(E[] a) : base(a) { }

        public IMyIterator<E> Iterator()
        {
            return new MyItr(this);
        }
    }

    public class MyTreeSet<E> where E : IComparable<E>
    {
        private task19.MyTreeMap<E, object> map;
        private static readonly object Dummy = new object();

        private class MyItr : IMyIterator<E>
        {
            private E[] data;
            private int cursor;
            private int lastIdx;
            private MyTreeSet<E> owner;

            public MyItr(MyTreeSet<E> owner)
            {
                this.owner = owner;
                data = owner.ToArray();
                cursor = 0;
                lastIdx = -1;
            }

            public bool HasNext()
            {
                return cursor < data.Length;
            }

            public E Next()
            {
                if (!HasNext())
                    throw new NoSuchElementException();
                lastIdx = cursor;
                return data[cursor++];
            }

            public void Remove()
            {
                if (lastIdx < 0)
                    throw new IllegalStateException();
                owner.Remove(data[lastIdx]);
                lastIdx = -1;
            }
        }

        public MyTreeSet()
        {
            map = new task19.MyTreeMap<E, object>();
        }

        public MyTreeSet(E[] a) : this()
        {
            for (int i = 0; i < a.Length; i++)
                Add(a[i]);
        }

        public bool Add(E e)
        {
            if (map.ContainsKey(e))
                return false;
            map.Put(e, Dummy);
            return true;
        }

        public bool Contains(E e)
        {
            return map.ContainsKey(e);
        }

        public bool Remove(E e)
        {
            if (!map.ContainsKey(e))
                return false;
            map.Remove(e);
            return true;
        }

        public int Size()
        {
            return map.Size();
        }

        public bool IsEmpty()
        {
            return map.IsEmpty();
        }

        public E[] ToArray()
        {
            List<E> keys = map.KeySet();
            E[] result = new E[keys.Count];
            for (int i = 0; i < keys.Count; i++)
                result[i] = keys[i];
            return result;
        }

        public IMyIterator<E> Iterator()
        {
            return new MyItr(this);
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder("{");
            E[] arr = ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append(arr[i]);
            }
            sb.Append("}");
            return sb.ToString();
        }
    }

    public class MyArrayList<T> : task17.MyArrayList<T>
    {
        private class MyItr : IMyListIterator<T>
        {
            private MyArrayList<T> list;
            private int cursor;
            private int lastReturned;

            public MyItr(MyArrayList<T> list, int index)
            {
                this.list = list;
                cursor = index;
                lastReturned = -1;
            }

            public bool HasNext()
            {
                return cursor < list.Size();
            }

            public T Next()
            {
                if (!HasNext())
                    throw new NoSuchElementException();
                lastReturned = cursor;
                return list.Get(cursor++);
            }

            public bool HasPrevious()
            {
                return cursor > 0;
            }

            public T Previous()
            {
                if (!HasPrevious())
                    throw new NoSuchElementException();
                lastReturned = --cursor;
                return list.Get(cursor);
            }

            public int NextIndex()
            {
                return cursor;
            }

            public int PreviousIndex()
            {
                return cursor - 1;
            }

            public void Remove()
            {
                if (lastReturned < 0)
                    throw new IllegalStateException();
                list.RemoveAt(lastReturned);
                if (lastReturned < cursor)
                    cursor--;
                lastReturned = -1;
            }

            public void Set(T element)
            {
                if (lastReturned < 0)
                    throw new IllegalStateException();
                list.Set(lastReturned, element);
            }

            public void Add(T element)
            {
                list.Add(cursor, element);
                cursor++;
                lastReturned = -1;
            }
        }

        public MyArrayList() : base() { }
        public MyArrayList(T[] a) : base(a) { }

        public IMyListIterator<T> ListIterator()
        {
            return new MyItr(this, 0);
        }

        public IMyListIterator<T> ListIterator(int index)
        {
            if (index < 0 || index > Size())
                throw new IndexOutOfBoundsException("Индекс " + index + " за пределами [0, " + Size() + "]");
            return new MyItr(this, index);
        }
    }

    public class MyVector<T> : global::MyVector.MyVector<T>
    {
        private class MyItr : IMyListIterator<T>
        {
            private MyVector<T> vec;
            private int cursor;
            private int lastReturned;

            public MyItr(MyVector<T> vec, int index)
            {
                this.vec = vec;
                cursor = index;
                lastReturned = -1;
            }

            public bool HasNext()
            {
                return cursor < vec.Size();
            }

            public T Next()
            {
                if (!HasNext())
                    throw new NoSuchElementException();
                lastReturned = cursor;
                return vec.Get(cursor++);
            }

            public bool HasPrevious()
            {
                return cursor > 0;
            }

            public T Previous()
            {
                if (!HasPrevious())
                    throw new NoSuchElementException();
                lastReturned = --cursor;
                return vec.Get(cursor);
            }

            public int NextIndex()
            {
                return cursor;
            }

            public int PreviousIndex()
            {
                return cursor - 1;
            }

            public void Remove()
            {
                if (lastReturned < 0)
                    throw new IllegalStateException();
                vec.Remove(lastReturned);
                if (lastReturned < cursor)
                    cursor--;
                lastReturned = -1;
            }

            public void Set(T element)
            {
                if (lastReturned < 0)
                    throw new IllegalStateException();
                vec.Set(lastReturned, element);
            }

            public void Add(T element)
            {
                vec.Add(cursor, element);
                cursor++;
                lastReturned = -1;
            }
        }

        public MyVector() : base() { }
        public MyVector(T[] a) : base(a) { }

        public IMyListIterator<T> ListIterator()
        {
            return new MyItr(this, 0);
        }

        public IMyListIterator<T> ListIterator(int index)
        {
            if (index < 0 || index > Size())
                throw new IndexOutOfBoundsException("Индекс " + index + " за пределами [0, " + Size() + "]");
            return new MyItr(this, index);
        }
    }

    public class MyLinkedList<T> : task17.MyLinkedList<T>
    {
        private class MyItr : IMyListIterator<T>
        {
            private MyLinkedList<T> list;
            private int cursor;
            private int lastReturned;

            public MyItr(MyLinkedList<T> list, int index)
            {
                this.list = list;
                cursor = index;
                lastReturned = -1;
            }

            public bool HasNext()
            {
                return cursor < list.Size();
            }

            public T Next()
            {
                if (!HasNext())
                    throw new NoSuchElementException();
                lastReturned = cursor;
                return list.Get(cursor++);
            }

            public bool HasPrevious()
            {
                return cursor > 0;
            }

            public T Previous()
            {
                if (!HasPrevious())
                    throw new NoSuchElementException();
                lastReturned = --cursor;
                return list.Get(cursor);
            }

            public int NextIndex()
            {
                return cursor;
            }

            public int PreviousIndex()
            {
                return cursor - 1;
            }

            public void Remove()
            {
                if (lastReturned < 0)
                    throw new IllegalStateException();
                list.RemoveAt(lastReturned);
                if (lastReturned < cursor)
                    cursor--;
                lastReturned = -1;
            }

            public void Set(T element)
            {
                if (lastReturned < 0)
                    throw new IllegalStateException();
                list.Set(lastReturned, element);
            }

            public void Add(T element)
            {
                list.Add(cursor, element);
                cursor++;
                lastReturned = -1;
            }
        }

        public MyLinkedList() : base() { }
        public MyLinkedList(T[] a) : base(a) { }

        public IMyListIterator<T> ListIterator()
        {
            return new MyItr(this, 0);
        }

        public IMyListIterator<T> ListIterator(int index)
        {
            if (index < 0 || index > Size())
                throw new IndexOutOfBoundsException("Индекс " + index + " за пределами [0, " + Size() + "]");
            return new MyItr(this, index);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== MyPriorityQueue + итератор ===");
            MyPriorityQueue<int> pq = new MyPriorityQueue<int>(new int[] { 5, 3, 8, 1 });
            IMyIterator<int> pqIter = pq.Iterator();
            while (pqIter.HasNext())
                Console.Write(pqIter.Next() + " ");
            Console.WriteLine();

            Console.WriteLine("\n=== MyArrayDeque + итератор ===");
            MyArrayDeque<int> deque = new MyArrayDeque<int>();
            deque.AddLast(10);
            deque.AddLast(20);
            deque.AddLast(30);
            IMyIterator<int> dequeIter = deque.Iterator();
            while (dequeIter.HasNext())
                Console.Write(dequeIter.Next() + " ");
            Console.WriteLine();

            Console.WriteLine("\n=== MyHashSet + итератор ===");
            MyHashSet<string> hashSet = new MyHashSet<string>();
            hashSet.Add("яблоко");
            hashSet.Add("банан");
            hashSet.Add("вишня");
            IMyIterator<string> hashIter = hashSet.Iterator();
            while (hashIter.HasNext())
                Console.Write(hashIter.Next() + " ");
            Console.WriteLine();

            Console.WriteLine("\n=== MyTreeSet + итератор ===");
            MyTreeSet<int> treeSet = new MyTreeSet<int>(new int[] { 5, 2, 8, 1, 9 });
            IMyIterator<int> treeIter = treeSet.Iterator();
            while (treeIter.HasNext())
                Console.Write(treeIter.Next() + " ");
            Console.WriteLine();

            Console.WriteLine("\n=== Удаление через итератор (MyTreeSet) ===");
            IMyIterator<int> removeIter = treeSet.Iterator();
            while (removeIter.HasNext())
            {
                int val = removeIter.Next();
                if (val % 2 == 0)
                    removeIter.Remove();
            }
            Console.WriteLine("После удаления чётных: " + treeSet);

            Console.WriteLine("\n=== MyArrayList + ListIterator ===");
            MyArrayList<int> arrList = new MyArrayList<int>(new int[] { 1, 2, 3, 4, 5 });
            IMyListIterator<int> listIter = arrList.ListIterator();
            Console.Write("Вперёд: ");
            while (listIter.HasNext())
                Console.Write(listIter.Next() + " ");
            Console.WriteLine();
            Console.Write("Назад:  ");
            while (listIter.HasPrevious())
                Console.Write(listIter.Previous() + " ");
            Console.WriteLine();

            IMyListIterator<int> modIter = arrList.ListIterator();
            modIter.Next();
            modIter.Set(100);
            modIter.Next();
            modIter.Add(99);
            Console.Write("После Set(100) и Add(99): ");
            IMyListIterator<int> printIter = arrList.ListIterator();
            while (printIter.HasNext())
                Console.Write(printIter.Next() + " ");
            Console.WriteLine();

            Console.WriteLine("\n=== MyVector + ListIterator ===");
            MyVector<int> vec = new MyVector<int>(new int[] { 10, 20, 30 });
            IMyListIterator<int> vecIter = vec.ListIterator();
            while (vecIter.HasNext())
                Console.Write(vecIter.Next() + " ");
            Console.WriteLine();

            Console.WriteLine("\n=== MyLinkedList + ListIterator ===");
            MyLinkedList<int> linked = new MyLinkedList<int>(new int[] { 7, 14, 21 });
            IMyListIterator<int> linkedIter = linked.ListIterator();
            while (linkedIter.HasNext())
                Console.Write(linkedIter.Next() + " ");
            Console.WriteLine();

            Console.WriteLine("\n=== Иерархия исключений (полиморфизм) ===");
            MyIteratorException[] exceptions = new MyIteratorException[]
            {
                new NoSuchElementException(),
                new IllegalStateException(),
                new IndexOutOfBoundsException("Индекс 10 за пределами [0, 3]")
            };
            for (int i = 0; i < exceptions.Length; i++)
            {
                try
                {
                    throw exceptions[i];
                }
                catch (NoSuchElementException ex)
                {
                    Console.WriteLine("NoSuchElementException: " + ex.Message);
                }
                catch (IllegalStateException ex)
                {
                    Console.WriteLine("IllegalStateException: " + ex.Message);
                }
                catch (IndexOutOfBoundsException ex)
                {
                    Console.WriteLine("IndexOutOfBoundsException: " + ex.Message);
                }
                catch (MyIteratorException ex)
                {
                    Console.WriteLine("MyIteratorException: " + ex.Message);
                }
            }

            Console.WriteLine("\n=== Исключения при неверном использовании ===");
            try
            {
                MyPriorityQueue<int> emptyPQ = new MyPriorityQueue<int>();
                emptyPQ.Iterator().Next();
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine("Ожидаемое: " + ex.Message);
            }

            try
            {
                MyArrayList<int> emptyList = new MyArrayList<int>();
                emptyList.ListIterator().Remove();
            }
            catch (IllegalStateException ex)
            {
                Console.WriteLine("Ожидаемое: " + ex.Message);
            }

            try
            {
                MyArrayList<int> small = new MyArrayList<int>(new int[] { 1, 2, 3 });
                small.ListIterator(10);
            }
            catch (IndexOutOfBoundsException ex)
            {
                Console.WriteLine("Ожидаемое: " + ex.Message);
            }
        }
    }
}
