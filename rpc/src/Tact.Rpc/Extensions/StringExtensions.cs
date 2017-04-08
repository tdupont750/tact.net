using System.Text.RegularExpressions;

namespace Tact.Rpc
{
    public static class StringExtensions
    {
        private const string Suffix = "Async";

        private static readonly Regex PrefixRegex = new Regex("^I[A-Z]", RegexOptions.Compiled);

        public static string GetRpcName(this string name)
        {
            var startIndex = PrefixRegex.IsMatch(name) ? 1 : 0;

            var length = name.EndsWith(Suffix)
                ? name.Length - Suffix.Length - startIndex
                : name.Length - startIndex;

            return name.Substring(startIndex, length);
        }
    }
}
