namespace SoftTouch.Parsing;


public static class FluentExtensions
{
    public static void WithName<T>(this T p, string name)
        where T : Parser
    {
        p.DescriptiveName = name;
    }
}