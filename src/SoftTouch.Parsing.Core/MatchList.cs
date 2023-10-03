namespace SoftTouch.Parsing;

public class MatchList
{
    public string Text { get; init; }
    List<Match> matches;
    
    public int Count => matches.Count;

    public MatchList(string text)
    {
        Text = text;
        matches = new();
    }
    public List<Match>.Enumerator GetEnumerator() => matches.GetEnumerator();

}