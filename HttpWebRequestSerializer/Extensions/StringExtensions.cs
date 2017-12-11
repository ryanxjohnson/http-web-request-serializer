using System.Text.RegularExpressions;

namespace HttpWebRequestSerializer.Extensions
{
    public static class StringExtensions
    {
        public static string CleanHeader(this string input)
        {
            // machine and extended characters
            var regex = new Regex(@"[\x00-\x1F\u007F-\uFFFF]", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            
            return regex.Replace(input, string.Empty).Replace("\r", string.Empty).Trim();
        }

        public static string SubstituteText(this string input, string newText, string pattern)
        {
            var match = Regex.Match(input, pattern);
            var substring = match.Groups["QueryString"].Value;
            return input.Replace("#{{" + substring + "}}", newText);    
        }
    }
}
