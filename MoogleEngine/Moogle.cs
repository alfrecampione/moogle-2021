namespace MoogleEngine;


public static class Moogle
{
    public enum Lenguaje
    {
        Español,
        Inglés
    }
    public static string[] files;
    public static SearchResult Query(string query)
    {
        Console.WriteLine("Call");
        float[] score;
        int top_results = 5;
        Lenguaje lenguaje = Lenguaje.Inglés;
        var results = SearchMethod.MakeQuery(query, top_results, out score);
        files = results.Item1;
        if (false && (results.Item1.Length / top_results) <= 0.4)
        {
            var stemm_query = query.ToLower().Split().ToList();
            //Implementando un stemmin de ingles
            if (lenguaje == Lenguaje.Inglés)
            {
                //TF_IDF.Stem(stemm_query);
            }
            if (lenguaje == Lenguaje.Español)
            {

            }
            string s = "";
            foreach (var word in stemm_query)
            {
                s += word + " ";
            }
            query = s;
            results = SearchMethod.MakeQuery(query, top_results, out score);
        }
        List<SearchItem> items = new List<SearchItem>();
        for (int i = 0; i < results.Item1.Length; i++)
        {
            items.Add(new SearchItem(results.Item1[i], results.Item2[i], (float)score[i]));
        }
        return new SearchResult(items.ToArray(), results.Item3);

    }
}
