namespace MoogleEngine;

public enum Language
{
    Spanish,
    English
}
public static class Moogle
{
    public static SearchResult Query(string query)
    {
        float[] score;
        int top_results = 10;
        var results = SearchMethod.MakeQuery(query.ToLower(), top_results, out score);

        List<SearchItem> items = new List<SearchItem>();
        int min = Math.Min(top_results,results.Item1.Length);
        for (int i = 0; i < min; i++)
        {
            //Saving the results in a list
            string path = GlobalVariables.filesPath[GlobalVariables.fileNames.ToList().IndexOf(results.Item1[i])];
            items.Add(new SearchItem(results.Item1[i], results.Item2[i], (float)score[i], path));
        }
        string suggestion = "";
        //If query and suggestion are the same, set suggestion to empty
        var temp1 = SearchMethod.GetQueryArray(query);
        var temp2 = SearchMethod.GetQueryArray(results.Item3);
        min = Math.Min(temp1.Length, temp2.Length);
        for (int i = 0; i < min; i++)
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
