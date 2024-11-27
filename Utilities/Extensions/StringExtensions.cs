namespace Utilities.Extensions;

public static class StringExtensions
{
    public static string Reversed(this string s)
    {
        var charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}