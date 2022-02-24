using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public static class Tools
    {
        public static string? directory = "";

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
        public static int GetIndex(float value, Dictionary<int, float> dict)
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
        public static string[] SearchSynonyms(string word)
        {
            string english_synonyms = "synonyms.txt";
            string spanish_synonyms = "sinónimos.txt";
            string language = "";
            if (Moogle.language == Language.English)
            {
                language = english_synonyms;
                En_Stemmer stem = new En_Stemmer();
                word = stem.Execute(word);
            }
            else
            {
                language = spanish_synonyms;
                Es_Stemmer stem = new Es_Stemmer();
                word = stem.Execute(word);
            }
            if (language == "")
                return new string[0];
            string? path = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
            var read = File.ReadAllLines(Path.Join(path, language));
            string[][] synonyms = new string[read.GetLength(0)][];
            for (int i = 0; i < synonyms.Length; i++)
            {
                synonyms[i] = read[i].Split(';');
            }
            int index = -1;
            for (int i = 0; i < synonyms.Length; i++)
            {
                for (int j = 0; j < synonyms[i].Length; j++)
                {
                    if (Equal(word, synonyms[i][j].ToLower())>0)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                    break;
            }
            if (index == -1)
                return new string[0];
            else
                return synonyms[index];
        }
        public static int Equal(string s1, string s2)
        {
            int index = Math.Min(s1.Length, s2.Length);
            int count = 0;
            if (index == 0)
                return 0;
            for (int i = 0; i < index; i++)
            {
                if (s1[i] != s2[i])
                    return 0;
                else
                    count++;
            }
            return count;
        }
        /// <summary>
        /// Recieve a list with all the words with its TF_IDF
        /// </summary>
        /// <param name="tf_idf"></param>
        /// <param name="allwords"></param>
        /// <returns>Returns an array of array with all the TF_IDF</returns>
        public static float[][] CreateMatrix(List<Dictionary<string, float>> tf_idf, List<string> allwords)
        {
            float[][] matrix = new float[tf_idf.Count][];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                matrix[i] = new float[allwords.Count];
                for (int j = 0; j < allwords.Count; j++)
                {
                    if (tf_idf[i].ContainsKey(allwords[j]))
                        matrix[i][j] = tf_idf[i][allwords[j]];
                }
            }
            return matrix;
        }
        /// <summary>
        /// Get the minimum number of transformations to make one word equal to another
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static int Levenshtein(string s1, string s2)
        {
            int coste = 0;
            int n1 = s1.Length;
            int n2 = s2.Length;
            int[,] m = new int[n1 + 1, n2 + 1];
            for (int i = 0; i <= n1; i++)
            {
                m[i, 0] = i;
            }
            for (int i = 1; i <= n2; i++)
            {
                m[0, i] = i;
            }
            for (int i1 = 1; i1 <= n1; i1++)
            {
                for (int i2 = 1; i2 <= n2; i2++)
                {
                    coste = (s1[i1 - 1] == s2[i2 - 1]) ? 0 : 1;
                    m[i1, i2] = Math.Min(Math.Min(m[i1 - 1, i2] + 1, m[i1, i2 - 1] + 1), m[i1 - 1, i2 - 1] + coste);
                }
            }
            return m[n1, n2];
        }
    }
}
