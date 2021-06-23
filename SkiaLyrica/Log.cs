using System;
using System.Text;

namespace SkiaLyrica
{
    public static class Log
    {
        public static string SafeSubstring(this string source, int start, int length = -1)
        {
            if (length < 0) length = source.Length - start;
            if (start + length < source.Length && length > 0) 
                return source.Substring(start, length);
            else
                return source;
        }
        private static void Write(ConsoleColor c, char type, string tag, string message)
        {
#if DEBUG
            var prevCol = Console.ForegroundColor;
            Console.ForegroundColor = c;
            var sb = new StringBuilder();
            var prefix = $"{DateTime.Now:G} {type}/{tag}";
            sb.Append(prefix.SafeSubstring(0, 32));
            sb.Append(' ', Math.Max(32 - prefix.Length, 0));
            sb.Append(message);
            Console.WriteLine(sb.ToString());
            Console.ForegroundColor = prevCol;
#endif
        }

        public static void D(string tag, string message) => Write(ConsoleColor.White, 'D', tag, message);
        public static void E(string tag, string message) => Write(ConsoleColor.Red, 'E', tag, message);
        public static void I(string tag, string message) => Write(ConsoleColor.Blue, 'I', tag, message);
        public static void W(string tag, string message) => Write(ConsoleColor.Yellow, 'W', tag, message);
        public static void X(Exception e) => Write(ConsoleColor.DarkRed, 'X', e.GetType().Name, e.Message);
    }
}
