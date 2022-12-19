using System.Text;

namespace Core;

public static class PhoneTranslator
{
    public static string ToNumber(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "";
        else
            raw = raw.ToUpperInvariant();

        var newNumber = new StringBuilder();
        foreach (var c in raw)
        {
            if (" -0123456789".Contains(c))
                newNumber.Append(c);
            else
            {
                var result = TranslateToNumber(c);
                if (result != null)
                    newNumber.Append(result);
            }
            // otherwise we've skipped a non-numeric char
        }
        return newNumber.ToString();
    }

    static int? TranslateToNumber(char c)
    {
        if ('A' <= c && c <= 'C')
            return 2;
        if ('D' <= c && c <= 'F')
            return 3;
        if ('G' <= c && c <= 'I')
            return 4;
        if ('J' <= c && c <= 'L')
            return 5;
        if ('M' <= c && c <= 'O')
            return 6;
        if ('P' <= c && c <= 'S')
            return 7;
        if ('T' <= c && c <= 'V')
            return 8;
        if ('W' <= c && c <= 'Z')
            return 9;
        return null;
    }
}