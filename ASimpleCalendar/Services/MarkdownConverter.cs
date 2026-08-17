using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ASimpleCalendar.Services;

public static class MarkdownConverter
{
    private static readonly Regex InlinePattern = new(@"(\*\*.+?\*\*|\*.+?\*|`[^`]+`)", RegexOptions.Compiled);

    public static IEnumerable<Inline> ToInlines(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            yield break;
        }

        var lines = markdown.Replace("\r\n", "\n").Split('\n');
        foreach (var line in lines)
        {
            yield return ParseLine(line);
            yield return new LineBreak();
        }
    }

    private static Inline ParseLine(string line)
    {
        var trimmed = line.TrimEnd();

        if (trimmed.StartsWith("### "))
        {
            return new Run(trimmed[4..]) { FontSize = 15, FontWeight = FontWeights.SemiBold };
        }

        if (trimmed.StartsWith("## "))
        {
            return new Run(trimmed[3..]) { FontSize = 17, FontWeight = FontWeights.SemiBold };
        }

        if (trimmed.StartsWith("# "))
        {
            return new Run(trimmed[2..]) { FontSize = 20, FontWeight = FontWeights.Bold };
        }

        var text = trimmed;
        if (text.StartsWith("- ") || text.StartsWith("* "))
        {
            text = "•  " + text[2..];
        }

        return ParseInline(text);
    }

    private static Inline ParseInline(string text)
    {
        var span = new Span();
        var last = 0;

        foreach (Match match in InlinePattern.Matches(text))
        {
            if (match.Index > last)
            {
                span.Inlines.Add(new Run(text[last..match.Index]));
            }

            var token = match.Value;

            if (token.StartsWith("**") && token.EndsWith("**") && token.Length > 4)
            {
                span.Inlines.Add(new Run(token[2..^2]) { FontWeight = FontWeights.Bold });
            }
            else if (token.StartsWith("`") && token.EndsWith("`") && token.Length > 2)
            {
                span.Inlines.Add(new Run(token[1..^1])
                {
                    FontFamily = new FontFamily("Consolas"),
                    Background = new SolidColorBrush(Color.FromArgb(0x22, 0x7F, 0x7F, 0x7F))
                });
            }
            else if (token.StartsWith("*") && token.EndsWith("*") && token.Length > 2)
            {
                span.Inlines.Add(new Run(token[1..^1]) { FontStyle = FontStyles.Italic });
            }
            else
            {
                span.Inlines.Add(new Run(token));
            }

            last = match.Index + match.Length;
        }

        if (last < text.Length)
        {
            span.Inlines.Add(new Run(text[last..]));
        }

        return span;
    }
}
