﻿namespace MoogleEngine;

public static class SearchMethod
{

    public static void Start()
    {
        Console.WriteLine("------------Calculating TF_IDF------------");
        Console.WriteLine("Getting files names....");
        GlobalVariables.fileNames = TF_IDF.SetFilesNames(GlobalVariables.Directory);
        Console.WriteLine("Reading files....");
        GlobalVariables.dataset = TF_IDF.ReadInside(GlobalVariables.fileNames, out GlobalVariables.allwords);
        Console.WriteLine("Calculate TF_IDF....");
        var document_tf_idf = TF_IDF.Calculate_TF_IDF(GlobalVariables.dataset);
        Console.WriteLine("Convert to matriz....");
        GlobalVariables.documents_matrix = Tools.CreateMatrix(document_tf_idf, GlobalVariables.allwords);
        Console.WriteLine("Finished!");
    }

    //OPs = '!', '^', '~', '*'
    //   !little ~ pig ~ *wolf
    /// <summary>
    /// Make the search
    /// </summary>
    /// <param name="query"></param>
    /// <param name="k">Max of matched documents</param>
    /// <param name="score">the score of all documents</param>
    /// <param name="check"></param>
    /// <returns>Return a tuple with Item1=Matched documents, Item2=Snipped of the documents, Item3=Suggestion</returns>
    public static (string[], string[], string) MakeQuery(string query, int k, out float[] score, bool check = false)
    {
        if (GlobalVariables.dataset.Item1.Count == 0)
        {
            score = new float[0];
            return (new string[0], new string[0], query);
        }

        //Deleting innecesary things
        var deleteinn = query.Split();
        List<string> querySplit = new();
        for (int i = 0; i < deleteinn.Length; i++)
        {
            if (deleteinn[i] != "" && deleteinn[i] != " ")
                querySplit.Add(deleteinn[i]);
        }

        //Operators
        var op1 = SearchOP1(querySplit.ToArray());
        var op2 = SearchOP2(querySplit.ToArray());
        var op3 = SearchOP3(querySplit.ToArray());
        var op4 = SearchOP4(querySplit.ToArray());

        if (op1.Count > 0)
            query = DeleteOp1(query);
        //Convert query into an array
        var query_array = GetQueryArray(query);

        //Add synonyms
        query_array = AddSynonyms(query_array);

        string[] query_array_copy = new string[query_array.Length];
        query_array.CopyTo(query_array_copy, 0);
        string[] temp = new string[query_array.Length];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = query_array[i];
        }

        //Find a suggestion
        string[] suggestion = SuggestQuery(temp, op4);
        //Deleting missing words
        query_array = ChangeQuery(query_array);

        //Converting suggestion to a string
        string true_suggestion = "";
        for (int i = 0; i < suggestion.Length; i++)
        {
            if (i == suggestion.Length - 1)
                true_suggestion += suggestion[i];
            else
                true_suggestion += suggestion[i] + " ";
        }
        if (query_array.Length == 0)
        {
            score = new float[0];
            return (new string[0], new string[0], true_suggestion);
        }

        //Making calculations
        var query_tf_idf = TF_IDF.Calculate_TF_IDF_Query(query_array.ToList(), op4, GlobalVariables.dataset);
        var query_matrix = Tools.CreateMatrix(query_tf_idf, GlobalVariables.allwords)[0];
        var coisine_sim = Coisine_Sim.GetCoisineSim(query_matrix, GlobalVariables.documents_matrix);
        //Sorting results
        float[] result = coisine_sim.Values.ToArray();
        float[] sort_result = result.ToArray();
        Array.Sort(sort_result);
        Array.Reverse(sort_result);

        //Setting the score param
        score = result;

        string[] files = new string[result.Length];
        List<int> document_index = new();

        //Setting the files names
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i] == 0f)
                document_index.Add(i);
            else
            {
                document_index.Add(Tools.GetIndex(result[i], coisine_sim));
            }
            files[i] = GlobalVariables.fileNames[document_index[i]];
        }

        if (!check)
        {
            //op1
            if (op1.Count != 0)
            {
                List<string> new_files = new();
                for (int i = 0; i < document_index.Count; i++)
                {
                    bool not_contains = true;
                    for (int j = 0; j < op1.Count; j++)
                    {
                        if (GlobalVariables.Content[document_index[i]].Contains(query_array_copy[op1[j]]))
                        {
                            not_contains = false;
                            break;
                        }
                    }
                    if (not_contains) new_files.Add(files[i]);
                }
                files = new_files.ToArray();
            }
            //op2
            if (op2.Count != 0)
            {
                List<string> new_files = new();
                for (int i = 0; i < document_index.Count; i++)
                {
                    bool contains = false;
                    for (int j = 0; j < op2.Count; j++)
                    {
                        if (GlobalVariables.Content[document_index[i]].Contains(query_array_copy[op2[j]]))
                        {
                            contains = true;
                            break;
                        }
                    }
                    if (contains) new_files.Add(files[i]);
                }
                files = new_files.ToArray();
            }
            //op3
            if (op3.Count != 0)
            {
                for (int i = 0; i < op3.Count; i++)
                {
                    for (int j = 0; j < document_index.Count; j++)
                    {
                        var words_index = Tools.GetIndexArray(query_array_copy[op3[i]], query_array_copy[op3[i] + 1], GlobalVariables.Content[document_index[j]]);
                        int distance = Tools.Distance(words_index.Item1, words_index.Item2);
                        float raise = (1f / (float)distance);

                        int word1_index = GlobalVariables.allwords.BinarySearch(query_array_copy[op3[i]]);
                        int word2_index = -1;
                        try
                        {
                            word2_index = GlobalVariables.allwords.BinarySearch(query_array_copy[op3[i + 1]]);
                            GlobalVariables.documents_matrix[j][word1_index] += raise;
                            GlobalVariables.documents_matrix[j][word2_index] += raise;
                        }
                        catch (Exception)
                        {
                            break;
                        }
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
        return (files, SearchSnipped(query_array, files), true_suggestion);
    }
    public static string DeleteOp1(string query)
    {
        string[] temp = query.Split();
        List<string> list = new();
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i][0] != '!')
            {
                list.Add(temp[i]);
            }
        }
        string result = "";
        for (int i = 0; i < list.Count; i++)
        {
            if (i == list.Count - 1)
                result += list[i];
            result += list[i] + " ";
        }
        return result;
    }
    /// <summary>
    /// Delete all words that not appear in the vacabulary
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static string[] ChangeQuery(string[] query)
    {
        List<string> list = query.ToList();
        for (int i = 0; i < query.Length; i++)
        {
            if (GlobalVariables.allwords.BinarySearch(query[i]) < 0)
                list.Remove(query[i]);
        }
        return list.ToArray();
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
        int count = 0;
        for (int i = 0; i < query.Length; i++)
        {
            if (query[i] == "~")
            {
                index.Add(i - (++count));
            }
        }
        if (index.Count > 0 && index[0] < 0)
            return new();
        return index;
    }
    static Dictionary<int, int> SearchOP4(string[] query)
    {
        Dictionary<int, int> index = new();
        int op_count = 0;
        for (int i = 0; i < query.Length; i++)
        {
            if (query[i] == "~")
                op_count++;
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
                index.Add(i - op_count, count);
            }
        }
        return index;
    }

    /// <summary>
    /// Preproces the query and put it in an array 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static string[] GetQueryArray(string query)
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

    /// <summary>
    /// Search for a similar query to suggest
    /// </summary>
    /// <param name="query"></param>
    /// <param name="op4"></param>
    /// <returns></returns>
    public static string[] SuggestQuery(string[] query, Dictionary<int, int> op4)
    {
        string[] tokens = query;
        List<string> missing_word = new();

        //Searching missing words
        foreach (var word in tokens)
        {
            if (!GlobalVariables.allwords.Contains(word))
                missing_word.Add(word);
        }
        if (missing_word.Count > 0)
        {
            List<List<string>> replacements = new();
            //Adding similary words
            for (int j = 0; j < missing_word.Count; j++)
            {
                replacements.Add(new());
                List<int> list = new();
                for (int i = 0; i < GlobalVariables.allwords.Count; i++)
                {
                    list.Add(Tools.Levenshtein(missing_word[j], GlobalVariables.allwords[i]));
                }
                int min = list.Min();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == min)
                        replacements[j].Add(GlobalVariables.allwords[i]);
                }
            }
            if (missing_word.Count == 1)
                return BestWord(query, missing_word[0], replacements[0], op4).Item1;
            else
            {
                return FindBestQuery(query, missing_word, replacements, op4).Item1;
            }
        }
        else
            return query;
    }

    /// <summary>
    /// select the word that produces the best search
    /// </summary>
    /// <param name="query"></param>
    /// <param name="missing_word"></param>
    /// <param name="replacements"></param>
    /// <param name="op4"></param>
    /// <returns></returns>
    public static (string[], float) BestWord(string[] query, string missing_word, List<string> replacements, Dictionary<int, int> op4)
    {
        List<string> new_query = new();
        List<float> results = new();
        string[] temp_query = query;
        for (int i = 0; i < replacements.Count; i++)
        {
            for (int j = 0; j < query.Length; j++)
            {
                if (query[j] == missing_word)
                    temp_query[j] = replacements[i];
            }
            var query_tf_idf = TF_IDF.Calculate_TF_IDF_Query(query.ToList(), op4, GlobalVariables.dataset);
            var query_matrix = Tools.CreateMatrix(query_tf_idf, GlobalVariables.allwords)[0];
            var coisine_sim = Coisine_Sim.GetCoisineSim(query_matrix, GlobalVariables.documents_matrix);
            results.Add(coisine_sim.Values.Max());
        }
        float value = results.Max();
        int index = results.IndexOf(value);
        for (int j = 0; j < query.Length; j++)
        {
            if (query[j] == missing_word)
                query[j] = replacements[index];
        }
        return (query, value);
    }

    /// <summary>
    /// Find the query with the most precision/accuracy search
    /// </summary>
    /// <param name="query"></param>
    /// <param name="missing_word"></param>
    /// <param name="replacements"></param>
    /// <param name="op4"></param>
    /// <returns></returns>
    public static (string[], float) FindBestQuery(string[] query, List<string> missing_word, List<List<string>> replacements, Dictionary<int, int> op4)
    {
        if (replacements.Count == 1)
        {
            return BestWord(query, missing_word[0], replacements[0], op4);
        }
        (string[], float) result = (new string[query.Length], 0f);
        for (int i = 0; i < replacements[0].Count; i++)
        {
            if (i != replacements[0].Count - 1)
            {
                string[] query1 = query.ToArray();
                string[] query2 = query.ToArray();
                for (int j = 0; j < query.Length; j++)
                {
                    if (query[j] == missing_word[0])
                        query1[j] = replacements[0][i];
                }
                for (int j = 0; j < query.Length; j++)
                {
                    if (query[j] == missing_word[0])
                        query2[j] = replacements[0][i + 1];
                }
                var result1 = FindBestQuery(query1, missing_word.ToArray()[1..].ToList(), replacements.ToArray()[1..].ToList(), op4);
                var result2 = FindBestQuery(query2, missing_word.ToArray()[1..].ToList(), replacements.ToArray()[1..].ToList(), op4);
                result = (result1.Item2 > result2.Item2) ? result1 : result2;
            }
            else
            {
                string[] query1 = query.ToArray();
                for (int j = 0; j < query.Length; j++)
                {
                    if (query[j] == missing_word[0])
                        query1[j] = replacements[0][i];
                }
                result = FindBestQuery(query1, missing_word.ToArray()[1..].ToList(), replacements.ToArray()[1..].ToList(), op4);
            }
        }
        return result;

    }

    /// <summary>
    /// Stemm the missing word trying to find a similary stemmer word
    /// </summary>
    /// <param name="orig_word"></param>
    /// <returns></returns>
    public static List<(int, int)> StemmMissingWord(string orig_word)
    {
        En_Stemmer stemmEN = new En_Stemmer();
        Es_Stemmer stemmES = new Es_Stemmer();

        if (GlobalVariables.Language == Language.Spanish)
        {
            orig_word = stemmES.Execute(orig_word);
        }
        if (GlobalVariables.Language == Language.English)
        {
            orig_word = stemmEN.Execute(orig_word);
        }
        List<(int, int)> result_index = new();
        int index = Tools.GetIndex<string>("pig", GlobalVariables.allwords.ToArray());
        for (int i = 0; i < GlobalVariables.allwords.Count; i++)
        {
            string s = GlobalVariables.allwords[i];
            if (GlobalVariables.Language == Language.Spanish)
            {
                s = stemmES.Execute(GlobalVariables.allwords[i]);
            }
            if (GlobalVariables.Language == Language.English)
            {
                s = stemmEN.Execute(GlobalVariables.allwords[i]);
            }
            int count = Tools.Equal(orig_word, s);
            if (count > 0)
                result_index.Add((i, count));

        }
        return result_index;
    }

    /// <summary>
    /// Search Snipped in each file obtained
    /// </summary>
    /// <param name="query"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    static string[] SearchSnipped(string[] query, string[] files)
    {
        int[] files_index = new int[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            for (int j = 0; j < GlobalVariables.fileNames.Length; j++)
            {
                if (files[i] == GlobalVariables.fileNames[j])
                {
                    files_index[i] = j;
                    break;
                }
            }
        }

        List<List<string>> files_content = new();
        List<List<string>> true_files_content = new();
        for (int i = 0; i < files_index.Length; i++)
        {
            files_content.Add(GlobalVariables.Content[files_index[i]]);
            true_files_content.Add(GlobalVariables.TrueContent[files_index[i]]);
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
                    snipped[i] += true_files_content[i][j] + " ";
                }
                else
                    snipped[i] += true_files_content[i][j];
            }
        }
        return snipped;
    }

    /// <summary>
    /// Obtain the most value word in a file
    /// </summary>
    /// <param name="query"></param>
    /// <param name="file_index"></param>
    /// <returns></returns>
    static string GetMostValueWord(string[] query, int file_index)
    {
        float[] query_values = new float[query.Length];
        int[] words_index = new int[query.Length];
        for (int i = 0; i < words_index.Length; i++)
        {
            words_index[i] = GlobalVariables.allwords.BinarySearch(query[i]);//Tools.GetIndex(query[i], allwords);
        }
        for (int i = 0; i < query_values.Length; i++)
        {
            query_values[i] = GlobalVariables.documents_matrix[file_index][words_index[i]];
        }
        return query[query_values.ToList().IndexOf(query_values.Max())];
    }

    /// <summary>
    /// Add synonyms to the query for better search
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    static string[] AddSynonyms(string[] query)
    {
        List<string> words = query.ToList();
        for (int i = 0; i < query.Length; i++)
        {
            words.Union(Tools.SearchSynonyms(query[i]).ToList());
        }
        string[] check = words.ToArray();
        foreach (var word in check)
        {
            if (!query.Contains(word) && GlobalVariables.allwords.BinarySearch(word) < 0)
                words.Remove(word);
        }
        return words.ToArray();
    }
}