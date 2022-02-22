namespace MoogleEngine;

public class SearchItem
{
    public SearchItem(string title, string snippet, float score, string path)
    {
        this.Title = title;
        this.Snippet = snippet;
        this.Score = score;
        this.Path = path;
    }

    public string Title { get; private set; }

    public string Snippet { get; private set; }

    public float Score { get; private set; }
    public string Path { get; private set; }
}
