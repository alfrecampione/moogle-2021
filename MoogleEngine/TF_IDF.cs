﻿using System.Text;
using System.Text.RegularExpressions;

namespace MoogleEngine
{
    public static class TF_IDF
    {
        public static List<List<string>> Content = new();


        #region PreProcessing
        /// <summary>
        /// Get the name of the files without a full path
        /// </summary>
        /// <param name="directory">Name of the directory with the files</param>
        /// <returns></returns>
        public static string[] SetFilesNames(string directory)
        {
            Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetCurrentDirectory()).FullName);
            string target = Directory.GetCurrentDirectory() + "\\" + directory;
            Directory.SetCurrentDirectory(target);  

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
            string[] filesWithoutPath = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                for (int j = target.Length + 1; j < files[i].Length; j++)
                {
                    filesWithoutPath[i] += files[i][j];
                }
            }
            return filesWithoutPath;
        }

        
        public static List<string> PreProcessingText(List<string> content)
        {

            for (int j = 0; j < content.Count; j++)
            {
                //Delete words with tilde
                content[j] = Regex.Replace(content[j].Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");

                StringBuilder stringBuilder = new StringBuilder();
                for (int k = 0; k < content[j].Length; k++)
                {
                    //Delete any char different of a latter or a digit
                    if (Char.IsLetterOrDigit(content[j][k]))
                        stringBuilder = stringBuilder.Append(content[j][k]);
                }
                content[j] = stringBuilder.ToString();
            }
            // Delete any word with size==1
            for (int j = 0; j < content.Count; j++)
            {
                if (content[j].Length <= 1)
                    content.Remove(content[j]);
            }
            return content;
        }

        /// <summary>
        /// Realize a stemming to de list of the words
        /// </summary>
        /// <param name="content"></param>
        public static void Stem(List<string> content)
        {
            for (int i = 0; i < content.Count; i++)
            {
                Stemmer stemmer = new Stemmer();
                stemmer.add(content[i].ToArray(), content[i].Length);
                stemmer.stem();
                content[i] = stemmer.ToString();
            }
        }

        /// <summary>
        /// Read all documents and save all of words in a tuple
        /// Item1 = a dict that save the words in a document with its frecuency
        /// Item2 = a dict that save the words in a tittle document with its frecuency
        /// </summary>
        /// <param name="filesName"></param>
        /// <returns>A List that contains a tuple with the words in the title and the words in the file</returns>
        public static (List<Dictionary<string, int>>, List<Dictionary<string, int>>) ReadInside(string[] filesName, out List<string> allwords)
        {
            allwords = new();
            List<Dictionary<string, int>> wordsInFiles = new();
            List<Dictionary<string, int>> wordsInTitles = new();
            for (int i = 0; i < filesName.Length; i++)
            {
                wordsInFiles.Add(new Dictionary<string, int>());
                wordsInTitles.Add(new Dictionary<string, int>());

                StreamReader sr = new StreamReader(filesName[i]);
                Content.Add(new());
                string word = "";
                while (!sr.EndOfStream)
                {
                    var temp = ((char)sr.Read());
                    temp = Char.ToLower(temp);
                    if (Char.IsLetterOrDigit(temp))
                        word += temp;
                    else
                    {
                        if (word == "" || word == " ") { word = ""; continue; }
                        Content[i].Add(word);
                        word = "";
                    }

                }
                foreach (var item in Content[i])
                {
                    if (wordsInFiles[i].ContainsKey(item)) wordsInFiles[i][item]++;
                    else
                    {
                        wordsInFiles[i].Add(item, 1);
                        if (!allwords.Contains(item)) allwords.Add(item);
                    }
                }

                string filename = "";
                for (int k = 0; k < filesName[i].Length; k++)
                {
                    if (filesName[i][k] == '.') break;
                    filename += filesName[i][k];
                }
                word = "";
                for (int j = 0; j < filename.Length; j++)
                {
                    var temp = Char.ToLower(filename[j]);
                    if (Char.IsLetterOrDigit(temp))
                    {
                        word += temp;
                    }
                    else
                    {
                        if (word == "" || word == " ") { word = ""; continue; }

                        if (wordsInTitles[i].ContainsKey(word)) wordsInTitles[i][word]++;
                        else
                        {
                            wordsInTitles[i].Add(word, 1);
                            if (!allwords.Contains(word)) allwords.Add(word);
                        }
                    }
                    if (j == filename.Length - 1 && word.Length != 0)
                    {
                        if (wordsInTitles[i].ContainsKey(word)) wordsInTitles[i][word]++;
                        else
                        {
                            wordsInTitles[i].Add(word, 1);
                            if (!allwords.Contains(word)) allwords.Add(word);
                        }
                    }
                }
            }
            return (wordsInFiles, wordsInTitles);
        }
        #endregion

        #region Setting TF_IDF
        /// <summary>
        /// Calculate TF_IDF ;)
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns>Returns a list of dictionaries with Key=word and Value=TF_IDF</returns>
        public static List<Dictionary<string, double>> Calculate_TF_IDF((List<Dictionary<string, int>>, List<Dictionary<string, int>>) dataset)
        {
            List<Dictionary<string, double>> tf_idf = new();
            List<Dictionary<string, double>> tf_idf_title = new();

            double alpha = 0.3;

            //TF_IDF of the document
            for (int i = 0; i < dataset.Item1.Count; i++)
            {
                tf_idf.Add(new());
                foreach (var word in dataset.Item1[i].Keys)
                {
                    double DF = 0;
                    double TF = dataset.Item1[i][word] + ((dataset.Item2[i].ContainsKey(word)) ? dataset.Item2[i][word] : 0);
                    foreach (var doc in dataset.Item1)
                    {
                        if (doc.ContainsKey(word))
                            DF++;
                    }
                    double IDF = Math.Log((double)dataset.Item1.Count / DF);
                    tf_idf[i].Add(word, TF * IDF * alpha);
                }
            }

            //TF_IDF of the title
            for (int i = 0; i < dataset.Item2.Count; i++)
            {
                tf_idf_title.Add(new());
                foreach (var word in dataset.Item2[i].Keys)
                {
                    double DF = 0;
                    double TF = dataset.Item2[i][word] + ((dataset.Item1[i].ContainsKey(word)) ? dataset.Item1[i][word] : 0);
                    foreach (var title in dataset.Item2)
                    {
                        if (title.ContainsKey(word))
                            DF++;
                    }
                    double IDF = Math.Log((double)dataset.Item2.Count / DF);
                    tf_idf_title[i].Add(word, TF * IDF);
                }
            }


            //Merge TF_IDF of the documents with its title
            for (int i = 0; i < tf_idf_title.Count; i++)
            {
                foreach (var word in tf_idf_title[i].Keys)
                {
                    if (tf_idf[i].ContainsKey(word))
                        tf_idf[i][word] = tf_idf_title[i][word];
                }
            }

            return tf_idf;
        }

        /// <summary>
        /// Calculate the TF_IDF of the query ;)
        /// </summary>
        /// <param name="query_content"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static List<Dictionary<string, double>> Calculate_TF_IDF_Query(List<string> query_content, (List<Dictionary<string, int>>, List<Dictionary<string, int>>) dataset)
        {
            Dictionary<string, double> query = new();
            Dictionary<string, double> tf_idf_query = new();
            foreach (var word in query_content)
            {
                if (query.ContainsKey(word))
                    query[word]++;
                else
                    query.Add(word, 1);
            }

            foreach (var word in query.Keys)
            {
                double TF = query[word];
                double DF = 0;
                for (int i = 0; i < dataset.Item1.Count; i++)
                {
                    int temp = 0;
                    temp = ((dataset.Item1[i].ContainsKey(word)) ? 1 : 0);
                    if (temp == 0) temp = ((dataset.Item2[i].ContainsKey(word)) ? 1 : 0);
                    DF += temp;
                }
                double idf = Math.Log(dataset.Item1.Count / DF);
                tf_idf_query.Add(word, TF * idf);
            }
            List<Dictionary<string, double>> tf_idf = new();
            tf_idf.Add(tf_idf_query);

            return tf_idf;
        }
        #endregion


        /// <summary>
        /// Recieve a list with all the words with its TF_IDF
        /// </summary>
        /// <param name="tf_idf"></param>
        /// <param name="allwords"></param>
        /// <returns>Returns an array of array with all the TF_IDF</returns>
        public static double[][] CreateMatrix(List<Dictionary<string, double>> tf_idf, List<string> allwords)
        {
            double[][] matrix = new double[tf_idf.Count][];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                matrix[i] = new double[allwords.Count];
                for (int j = 0; j < allwords.Count; j++)
                {
                    if (tf_idf[i].ContainsKey(allwords[j]))
                        matrix[i][j] = tf_idf[i][allwords[j]];
                }
            }
            return matrix;

        }
    }
}
