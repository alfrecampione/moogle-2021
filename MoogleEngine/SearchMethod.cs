﻿namespace MoogleEngine;

public static class SearchMethod
{
    static string[] fileNames;
    public static string[] allwords;
    static (List<Dictionary<string, int>>, List<Dictionary<string, int>>) dataset = new();
    public static double[][] documents_matrix;
    public static string path;
    public static Thread thread_start = new Thread(new ThreadStart(Start));

    public static bool first_search = true;

    public static void Start()
    {
        //TF_IDF
        Console.WriteLine("Calculating TF_IDF");
        fileNames = TF_IDF.SetFilesNames(path);
        List<string> vocabulary = new();
        dataset = TF_IDF.ReadInside(fileNames, out vocabulary);
        allwords = vocabulary.ToArray();
        var document_tf_idf = TF_IDF.Calculate_TF_IDF(dataset);
        documents_matrix = TF_IDF.CreateMatrix(document_tf_idf, allwords);
        Console.WriteLine("Finished");
    }

    #region MakeQuery
    //OPs = '!', '^', '~', '*'
    public static (string[], string[], string) MakeQuery(string query, int k, out double[] score, bool check = false)
    {
        //Operators
        var op1 = SearchOP1(query.Split());
        var op2 = SearchOP2(query.Split());
        var op3 = SearchOP3(query.Split());
        var op4 = SearchOP4(query.Split());

        var query_array = GetQueryArray(query);
        var query_array_copy = query_array;

        bool query_changed = false;
        if (!check)
        {
            //op1
            if (op1.Count != 0)
            {
                var list = query_array.ToList();
                for (int i = 0; i < op1.Count; i++)
                {
                    list.RemoveAt(op1[i]);
                }
                query_array = list.ToArray();
            }

            query_array = ChangeQuery(query_array, op4, out query_changed);
        }

        query = "";
        foreach (var word in query_array)
        {
            query += word + " ";
        }
        query = query.Remove(query.Length - 1);

        var query_tf_idf = TF_IDF.Calculate_TF_IDF_Query(query_array.ToList(), op4, dataset);
        var query_matrix = TF_IDF.CreateMatrix(query_tf_idf, allwords)[0];
        var coisine_sim = GetCoisineSim(query_matrix);

        double[] result = coisine_sim.Values.ToArray();
        Array.Sort(result);
        Array.Reverse(result);

        int length = 0;
        for (int i = 0; i < k; i++)
        {
            if (result[i] <= 0.01) break;
            else length++;
        }
        if (length == 0)
            length = k;

        score = new double[length];
        for (int i = 0; i < score.Length; i++)
        {
            score[i] = result[i];
        }

        string[] files = new string[length];
        List<int> document_index = new();

        for (int i = 0; i < length; i++)
        {
            if (result[i] == 0) files[i] = "";
            else { document_index.Add(Tools.GetIndex(result[i], coisine_sim)); files[i] = fileNames[document_index[i]]; }
        }

        if (!check)
        {
            //op1
            if (op1.Count != 0)
            {
                List<string> new_files = new();
                for (int i = 0; i < document_index.Count; i++)
                {
                    bool contains = false;
                    for (int j = 0; j < op1.Count; j++)
                    {
                        if (TF_IDF.Content[document_index[i]].Contains(query_array_copy[op1[j]]))
                        {
                            contains = true;
                            break;
                        }
                    }
                    if (!contains) new_files.Add(files[i]);
                }
                if (!query_changed)
                    return (new_files.ToArray(), SearchSnipped(query.Split(), files), "");
                return (new_files.ToArray(), SearchSnipped(query.Split(), files), query);
            }
            //op2
            if (op2.Count != 0)
            {
                List<string> new_files = new();
                for (int i = 0; i < document_index.Count; i++)
                {
                    bool contains = true;
                    for (int j = 0; j < op1.Count; j++)
                    {
                        if (!TF_IDF.Content[document_index[i]].Contains(query_array_copy[op1[j]]))
                        {
                            contains = false;
                            break;
                        }
                    }
                    if (contains) new_files.Add(files[i]);
                }
                if (!query_changed)
                    return (new_files.ToArray(), SearchSnipped(query.Split(), files), "");
                return (new_files.ToArray(), SearchSnipped(query.Split(), files), query);
            }
            //op3
            if (op3.Count != 0)
            {
                for (int i = 0; i < op3.Count; i++)
                {
                    for (int j = 0; j < document_index.Count; j++)
                    {
                        var words_index = Tools.GetIndexArray(query_array[op3[i]], query_array[op3[i] + 1], TF_IDF.Content[document_index[j]]);
                        int distance = Tools.Distance(words_index.Item1, words_index.Item2) - 1;
                        int raise = (TF_IDF.Content[document_index[j]].Count - distance) / TF_IDF.Content[document_index[j]].Count;

                        int word1_index = Tools.GetIndex(query_array[op3[i]], allwords);
                        int word2_index = Tools.GetIndex(query_array[op3[i] + 1], allwords);

                        documents_matrix[j][word1_index] += raise;
                        documents_matrix[j][word2_index] += raise;
                    }
                }
                query = "";
                for (int i = 0; i < query_array_copy.Length; i++)
                {
                    if (i == query_array_copy.Length - 1)
                        query += query_array_copy[i];
                    else
                        query += query_array_copy[i] + " ";
                }
                return MakeQuery(query, k, out score, true);
            }
        }
        if (!query_changed)
            return (files, SearchSnipped(query.Split(), files), "");
        return (files, SearchSnipped(query.Split(), files), query);
    }
    static List<int> SearchOP1(string[] query)
    {
        List<int> index = new();
        for (int i = 0; i < query.Length; i++)
        {
            if (query[i][0] == '!')
            {
                index.Add(i);
            }
        }
        return index;
    }
    static List<int> SearchOP2(string[] query)
    {
        List<int> index = new();
        for (int i = 0; i < query.Length; i++)
        {
            if (query[i][0] == '^')
            {
                index.Add(i);
            }
        }
        return index;
    }
    static List<int> SearchOP3(string[] query)
    {
        List<int> index = new();
        for (int i = 0; i < query.Length; i++)
        {
            if (query[i] == "~")
            {
                index.Add(i - 1);
            }
        }
        return index;
    }
    static Dictionary<int, int> SearchOP4(string[] query)
    {
        Dictionary<int, int> index = new();
        for (int i = 0; i < query.Length; i++)
        {
            if (query[i][0] == '*')
            {
                int count = 1;
                for (int j = 1; j < query[i].Length; j++)
                {
                    if (query[i][j] == '*')
                        count++;
                    else
                        break;
                }
                index.Add(i, count);
            }
        }
        return index;
    }

    static string[] GetQueryArray(string query)
    {
        List<string> processed_query = new List<string>();
        string word = "";
        for (int j = 0; j < query.Length; j++)
        {
            var temp = Char.ToLower(query[j]);
            if (Char.IsLetterOrDigit(temp))
            {
                word += temp;
            }
            else
            {
                if (word == "" || word == " ") { word = ""; continue; }
                processed_query.Add(word);
                word = "";
            }
            if (j == query.Length - 1 && word.Length != 0)
            {
                processed_query.Add(word);
                word = "";
            }
        }
        return processed_query.ToArray();
    }
    public static string[] ChangeQuery(string[] query, Dictionary<int, int> op4, out bool change)
    {
        change = false;
        string[] tokens = query;
        List<string> missing_word = new();

        List<string> results = new();

        //Searching missing words
        foreach (var word in tokens)
        {
            if (!allwords.Contains(word))
                missing_word.Add(word);
        }

        if (missing_word.Count > 0)
        {
            change = true;
            List<Dictionary<string, int>> sim_val = new();
            int index = 0;
            foreach (var word in missing_word)
            {
                sim_val.Add(new());
                for (int i = 0; i < allwords.Length; i++)
                {
                    sim_val[index].Add(allwords[i], Levenshtein(word, allwords[i]));
                }
                index++;
            }

            var values = sim_val[0].Values.ToList();
            values.Sort();
            index = 0;
            List<List<string>> replacements = new();
            replacements.Add(new());
            foreach (var key in sim_val[0].Keys)
            {
                if (sim_val[0][key] == values[index])
                {
                    replacements[0].Add(key);
                }
            }
            List<Dictionary<string, double>> replacements_TF_IDF = new();
            List<string> selected = new();
            replacements_TF_IDF.Add(new Dictionary<string, double>());
            foreach (var word in replacements[0])
            {
                string replacement_query = "";
                for (int i = 0; i < tokens.Length; i++)
                {
                    if (tokens[i] == missing_word[0])
                        replacement_query += word + " ";
                    else replacement_query += tokens[i] + " ";
                    if (i == tokens.Length - 1)
                        replacement_query = replacement_query[0..(replacement_query.Length - 1)];
                }
                replacements_TF_IDF[0].Add(word, GetCoisineSim(TF_IDF.CreateMatrix((TF_IDF.Calculate_TF_IDF_Query(GetQueryArray(replacement_query).ToList(), op4, dataset)), allwords)[0])[0]);
            }
            List<double> max = new();
            foreach (var dict in replacements_TF_IDF)
            {
                max.Add(dict.Values.Max());
            }
            for (int j = 0; j < max.Count; j++)
            {
                for (int i = 0; i < replacements_TF_IDF[j].Keys.Count; i++)
                {
                    if (max[j] == replacements_TF_IDF[j].Values.ToArray()[i])
                    {
                        selected.Add(replacements_TF_IDF[j].Keys.ToArray()[i]); break;
                    }
                }
            }
            index = 0;
            results.Add("");
            for (int i = 0; i < tokens.Length; i++)
            {
                if (missing_word[index] == tokens[i])
                {
                    if (i == tokens.Length - 1)
                        results[0] += selected[index];
                    else
                        results[0] += selected[index] + " ";
                }

                else
                {
                    if (i == tokens.Length - 1)
                        results[0] += tokens[i];
                    else
                        results[0] += tokens[i] + " ";
                }
            }
            return results[0].Split();
        }
        else
            return query;
    }
    private static int Levenshtein(string s1, string s2)
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
    public static string[] SearchSnipped(string[] query, string[] files)
    {
        int[] files_index = new int[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            for (int j = 0; j < fileNames.Length; j++)
            {
                if (files[i] == fileNames[j])
                {
                    files_index[i] = j;
                    break;
                }
            }
        }

        List<List<string>> files_content = new();
        for (int i = 0; i < files_index.Length; i++)
        {
            files_content.Add(TF_IDF.Content[files_index[i]]);
        }

        List<string> words = new();

        for (int i = 0; i < files_index.Length; i++)
        {
            words.Add(GetMostValueWord(query, files_index[i]));
        }
        string[] snipped = new string[files.Length];
        for (int i = 0; i < snipped.Length; i++)
        {
            snipped[i] = "";
            int index = files_content[i].IndexOf(words[i]);
            for (int j = (index <= 10) ? 0 : index - 10; j <= index + 10; j++)
            {
                if (index != index + 10)
                {
                    snipped[i] += files_content[i][j] + " ";
                }
                else
                    snipped[i] += files_content[i][j];
            }
        }
        return snipped;
    }
    static string GetMostValueWord(string[] query, int file_index)
    {
        double[] query_values = new double[query.Length];
        int[] words_index = new int[query.Length];
        for (int i = 0; i < words_index.Length; i++)
        {
            words_index[i] = Tools.GetIndex(query[i], allwords);
        }
        for (int i = 0; i < query_values.Length; i++)
        {
            query_values[i] = documents_matrix[file_index][words_index[i]];
        }
        return query[query_values.ToList().IndexOf(query_values.Max())];
    }

    #endregion


    #region Coisine_sim
    private static double Magnitude(Vector v1)
    {
        return Math.Sqrt(v1 * v1);
    }
    public static double CoisineSimilarity(Vector general, Vector query)
    {
        if ((Magnitude(general) * Magnitude(query)) == 0)
            return 0;
        return (general * query) / (Magnitude(general) * Magnitude(query));
    }
    private static Dictionary<int, double> GetCoisineSim(double[] query_array)
    {
        List<Vector> document_vectors = new List<Vector>();
        foreach (var array in documents_matrix)
        {
            document_vectors.Add(new Vector(array));
        }
        Vector query_vector = new(query_array);
        Dictionary<int, double> coisine_sim = new();
        for (int i = 0; i < document_vectors.Count; i++)
        {
            coisine_sim.Add(i, CoisineSimilarity(document_vectors[i], query_vector));
        }
        return coisine_sim;
    }
    #endregion
}