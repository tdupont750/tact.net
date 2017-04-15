using System;
using System.Linq;
using System.Text;

namespace Tact
{
    public static class ExceptionExtensions
    {
        private static readonly string[] NewLine = {Environment.NewLine};

        private static readonly string[] AsyncLines =
        {
            "   at System.ThrowHelper",
            "   at System.Runtime.ExceptionServices",
            "   at System.Runtime.CompilerServices.TaskAwaiter",
            "   at System.Threading.Tasks.Task",
            "--- End of stack trace from previous location where exception was thrown ---"
        };

        public static string GetCleanStackTrace(this Exception ex)
        {
            var stack = ex.StackTrace;
            if (string.IsNullOrWhiteSpace(stack))
                return string.Empty;

            var split = stack.Split((string[]) NewLine, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder(stack.Length);

            foreach (var s in split)
            {
                var add = Enumerable.All<string>(AsyncLines, a => !s.StartsWith(a));
                if (add)
                    sb.AppendLine(s);
            }

            if (sb.Length > 0)
                sb.Length -= Environment.NewLine.Length;

            return sb.ToString();
        }
    }
}