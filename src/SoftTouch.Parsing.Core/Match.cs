namespace SoftTouch.Parsing;



public struct MatchChildren
{
    public Match match { get; init; }
    public MatchChildren(Match match)
    {
        this.match = match;
    }

    public Enumerator GetEnumerator() => new(match);

    public ref struct Enumerator
    {
        Match match;
        List<Match>.Enumerator enumerator;
        bool started = false;
        public Enumerator(Match match)
        {
            this.match = match;
            enumerator = match.MatchList.GetEnumerator();
        }
        public Match Current => enumerator.Current;
        public bool MoveNext()
        {
            if(match.MatchList.Count > 0)
                return false;
            else if(!started) 
            {
                started = true;
                while(enumerator.MoveNext())
                {
                    if(Current.ParentId == match.Id)
                        return true;
                }
                return false;
            }
            else 
            {
                while (enumerator.MoveNext())
                {
                    if (Current.ParentId == match.Id)
                        return true;
                }
                return false;
            }
        }
    }
}

public struct Match
{
    public string Name { get; init; }
    public int Id { get; init; }
    public int TextPosition { get; init; }
    public ReadOnlyMemory<char> Text { get; init; }
    public int ParentId { get; init; }
    public MatchList MatchList { get; init; }
    public MatchChildren Matches => new(this);

}

