using System.Text;
using System.Text.RegularExpressions;

namespace MoogleEngine
{
    public static class TF_IDF
    {
        public static List<List<string>> Content = new();
        public static string[] filesPath;

        #region PreProcessing
        /// <summary>
        /// Get the name of the files without a full path
        /// </summary>
        /// <param name="directory">Name of the directory with the files</param>
        /// <returns></returns>
        public static string[] SetFilesNames(string? directory)
        {
            string target = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName + "\\" + directory;
            string[] files = Directory.GetFiles(target);
            filesPath = files;
            List<string> filesWithoutPath = new();

            for (int i = 0; i < files.Length; i++)
            {
                string name = "";
                for (int j = files[i].Length - 1; files[i][j] != '\\'; j--)
                {
                    name = files[i][j] + name;
                }
                if (name == ".gitignore")
                    continue;
                filesWithoutPath.Add(name);
            }
            return filesWithoutPath.ToArray();
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

                StreamReader sr = new StreamReader(filesPath[i]);
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
                        switch (temp)
                        {
                            case 'á':
                                word += 'a';
                                continue;
                            case 'é':
                                word += 'e';
                                continue;
                            case 'í':
                                word += 'i';
                                continue;
                            case 'ó':
                                word += 'o';
                                continue;
                            case 'ú':
                                word += 'u';
                                continue;
                            case 'ñ':
                                word += 'n';
                                continue;
                            case 'ü':
                                word += 'u';
                                continue;
                        }
                        Content[i].Add(word);
                        if (wordsInFiles[i].ContainsKey(word)) wordsInFiles[i][word]++;
                        else
                        {
                            wordsInFiles[i].Add(word, 1);
                            if (!allwords.Contains(word)) allwords.Add(word);
                        }
                        word = "";
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

                        switch (temp)
                        {
                            case 'á':
                                word += 'a';
                                continue;
                            case 'é':
                                word += 'e';
                                continue;
                            case 'í':
                                word += 'i';
                                continue;
                            case 'ó':
                                word += 'o';
                                continue;
                            case 'ú':
                                word += 'u';
                                continue;
                            case 'ñ':
                                word += 'n';
                                continue;
                            case 'ü':
                                word += 'u';
                                continue;
                        }

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
            allwords.Sort();
            return (wordsInFiles, wordsInTitles);
        }
        #endregion

        #region Setting TF_IDF

        /// <summary>
        /// Calculate TF_IDF ;)
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns>Returns a list of dictionaries with Key=word and Value=TF_IDF</returns>
        public static List<Dictionary<string, float>> Calculate_TF_IDF((List<Dictionary<string, int>>, List<Dictionary<string, int>>) dataset)
        {
            List<Dictionary<string, float>> tf_idf = new();
            List<Dictionary<string, float>> tf_idf_title = new();

            float alpha = 0.3f;

            //TF_IDF of the document
            for (int i = 0; i < dataset.Item1.Count; i++)
            {
                tf_idf.Add(new());
                foreach (var word in dataset.Item1[i].Keys)
                {
                    float DF = 0;
                    float TF = dataset.Item1[i][word] + ((dataset.Item2[i].ContainsKey(word)) ? dataset.Item2[i][word] : 0);
                    foreach (var doc in dataset.Item1)
                    {
                        if (doc.ContainsKey(word))
                            DF++;
                    }
                    float IDF = (float)Math.Log(dataset.Item1.Count / DF);
                    tf_idf[i].Add(word, TF * IDF * alpha);
                }
            }

            //TF_IDF of the title
            for (int i = 0; i < dataset.Item2.Count; i++)
            {
                tf_idf_title.Add(new());
                foreach (var word in dataset.Item2[i].Keys)
                {
                    float DF = 0;
                    float TF = dataset.Item2[i][word] + ((dataset.Item1[i].ContainsKey(word)) ? dataset.Item1[i][word] : 0);
                    foreach (var title in dataset.Item2)
                    {
                        if (title.ContainsKey(word))
                            DF++;
                    }
                    float IDF = (float)Math.Log(dataset.Item2.Count / DF);
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
        public static List<Dictionary<string, float>> Calculate_TF_IDF_Query(List<string> query_content, Dictionary<int, int> op4, (List<Dictionary<string, int>>, List<Dictionary<string, int>>) dataset)
        {
            Dictionary<string, float> query = new();
            Dictionary<string, float> tf_idf_query = new();
            foreach (var word in query_content)
            {
                if (query.ContainsKey(word))
                    query[word]++;
                else
                    query.Add(word, 1);
            }
            for (int i = 0; i < op4.Count; i++)
            {
                query[query_content[op4.Keys.ToArray()[i]]] += op4[op4.Keys.ToArray()[i]];
            }

            foreach (var word in query.Keys)
            {
                float TF = query[word];
                float DF = 0;
                for (int i = 0; i < dataset.Item1.Count; i++)
                {
                    int temp = 0;
                    temp = ((dataset.Item1[i].ContainsKey(word)) ? 1 : 0);
                    if (temp == 0) temp = ((dataset.Item2[i].ContainsKey(word)) ? 1 : 0);
                    DF += temp;
                }
                float idf = (float)Math.Log(dataset.Item1.Count / DF);
                tf_idf_query.Add(word, TF * idf);
            }
            List<Dictionary<string, float>> tf_idf = new();
            tf_idf.Add(tf_idf_query);

            return tf_idf;
        }
        #endregion
    }
}
