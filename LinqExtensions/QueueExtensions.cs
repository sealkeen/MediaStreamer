using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtensions
{
    public static class QueueExtensions
    {
        public static string ToLine(this Queue<string> value)
        {
            string result = "";
            while (value.Count != 0)
            {
                result += value.Dequeue();
            }
            return result;
        }
        public static T Dequeue<T>(this LinkedList<T> list)
        {
            if (!list.Empty())
            {
                var item = list.First.Value;
                list.RemoveFirst();
                return item;
            }
            return default(T);
        }

        public static void Enqueue<T>(this LinkedList<T> list, T item)
        {
            list.AddLast(item);
        }

        public static void Push<T>(this LinkedList<T> list, T item)
        {
            list.AddFirst(item);
        }

        public static bool Empty<T>(this LinkedList<T> list)
        {
            if (list.Count == 0)//queue not empty
                return true;
            return false;
        }
        public static bool Empty<T>(this Queue<T> queue)
        {
            if (queue.Any())
            {
                //queue not empty
                return false;
            }
            return true;
        }
    }
}
