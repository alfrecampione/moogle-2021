namespace MoogleEngine;

public enum Language
{
    Spanish,
    English
}
public static class Moogle
{

    public static string[] files;
    public static Language language = Language.English;
    public static SearchResult Query(string query)
    {
        Console.WriteLine("Call");
        float[] score;
        int top_results = 5;
        var results = SearchMethod.MakeQuery(query.ToLower(), top_results, out score);
        files = results.Item1;
        //if ((results.Item1.Length / top_results) <= 0.4)
        //{
        //    var stemm_query = query.ToLower().Split().ToList();
        //    //Implementando un stemmin de ingles
        //    if (language == Language.English)
        //    {
        //        for (int i = 0; i < stemm_query.Count; i++)
        //        {
        //            En_Stemmer stem = new En_Stemmer();
        //            stem.add(stemm_query[i].ToArray(), stemm_query[i].Length);
        //            stem.stem();
        //            stemm_query[i] = stem.ToString();
        //        }
        //    }
        //    if (language == Language.Spanish)
        //    {
        //        for (int i = 0; i < stemm_query.Count; i++)
        //        {
        //            Es_Stemmer stem = new Es_Stemmer();
        //            stemm_query[i] = stem.Execute(stemm_query[i]);
        //        }
        //    }
        //    string s = "";
        //    foreach (var word in stemm_query)
        //    {
        //        s += word + " ";
        //    }
        //    query = s;
        //    results = SearchMethod.MakeQuery(query, top_results, out score);
        //}
        List<SearchItem> items = new List<SearchItem>();
        for (int i = 0; i < results.Item1.Length; i++)
        {
            items.Add(new SearchItem(results.Item1[i], results.Item2[i], (float)score[i]));
        }
        string suggestion = "";

        var temp1 = SearchMethod.GetQueryArray(query);
        var temp2 = SearchMethod.GetQueryArray(results.Item3);
        for (int i = 0; i < temp1.Length; i++)
        {
            if (temp1[i] != temp2[i])
            {
                suggestion = results.Item3;
                break;
            }
        }
        return new SearchResult(items.ToArray(), suggestion);

    }
}
