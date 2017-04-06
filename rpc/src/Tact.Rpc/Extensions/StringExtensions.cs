using System.Text.RegularExpressions;

namespace Tact.Rpc
{
    public static class StringExtensions
    {
        private const string Suffix = "Async";

        private static readonly Regex PrefixRegex = new Regex("^I[A-Z]", RegexOptions.Compiled);

        public static string GetRpcName(this string name)
        {
            if (PrefixRegex.IsMatch(name))
                name = name.Substring(1);

            if (name.EndsWith(Suffix))
                name = name.Substring(0, name.Length - Suffix.Length);

            return name;
        }
    }
}
