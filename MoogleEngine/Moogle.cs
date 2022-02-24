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
        int top_results = 10;
        var results = SearchMethod.MakeQuery(query.ToLower(), top_results, out score);
        files = results.Item1;
        List<SearchItem> items = new List<SearchItem>();
        for (int i = 0; i < results.Item1.Length; i++)
        {
            string path = TF_IDF.filesPath[SearchMethod.fileNames.ToList().IndexOf(results.Item1[i])];
            items.Add(new SearchItem(results.Item1[i], results.Item2[i], (float)score[i], path));
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
