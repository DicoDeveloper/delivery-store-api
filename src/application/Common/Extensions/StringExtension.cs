using System.Globalization;
using System.Text;

namespace Application.Common.Extensions;

public static class StringExtension
{
    public static string ClearToCompare(this string text)
    {
        string normalizedString = text.Normalize(NormalizationForm.FormD);

        var stringBuilder = new StringBuilder();

        foreach (char c in normalizedString)
        {
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

            if (unicodeCategory != UnicodeCategory.NonSpacingMark && (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLower();
    }
}