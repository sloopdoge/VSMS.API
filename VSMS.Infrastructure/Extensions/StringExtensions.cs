using System.Globalization;
using System.Text;

namespace VSMS.Infrastructure.Extensions;

public static class StringExtensions
{
    public static string NormalizeText(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return input
            .Trim()
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }
}