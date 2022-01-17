using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public static class Tools
    {

        public static bool Find<T>(T element, T[] list) where T : IComparable<T>
        {
            int value = BinarySearch(list, element, 0);
            if (value == -1)
                return false;
            return true;
        }

        public static int GetIndex<T>(T element, T[] content) where T : IComparable<T>
        {
            int index = BinarySearch(content, element, 0);
            return index;
        }
        private static int BinarySearch<T>(T[] list, T element, int count) where T : IComparable<T>
        {
            int mid = (list.Length) / 2;
            if (list[mid].Equals(element))
                return count + mid;
            if (list.Length == 1 && !list[0].Equals(element))
                return -1;
            int index = 0;
            if (list[mid].CompareTo(element) < 0)
            {
                index = BinarySearch(list[mid..], element, count + mid);
            }
            else
            {
                if (list[mid].CompareTo(element) > 0)
                {
                    index = BinarySearch(list[..mid], element, count);
                }
            }
            return index;
        }

        public static int GetIndex(double value, Dictionary<int, double> dict)
        {
            for (int i = 0; i < dict.Keys.Count; i++)
            {
                if (dict[i] == value)
                    return i;
            }
            return -1;
        }

        public static (int[], int[]) GetIndexArray(string word1, string word2, List<string> content)
        {
            List<int> list1 = new();
            List<int> list2 = new();
            for (int i = 0; i < content.Count; i++)
            {
                if (content[i] == word1)
                    list1.Add(i);
                if (content[i] == word2)
                    list2.Add(i);
            }
            return (list1.ToArray(), list2.ToArray());
        }

        public static int Distance(int[] index1, int[] index2)
        {
            int min = int.MaxValue;
            for (int i = 0; i < index1.Length; i++)
            {
                for (int j = 0; j < index2.Length; j++)
                {
                    int temp = Math.Abs(index1[i] - index2[j]);
                    if (temp < min)
                    {
                        min = temp;
                    }
                }
            }
            return min;
        }
    }
}
