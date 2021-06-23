using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SkiaLyrica.Subrip
{
    /**
     * Format :
     * 
     * index
     * hh:mm:ss.mss --> hh:mm:ss.mss
     * Text Line 1
     * Text Line 2
     * **/
    public class SubtitleText
    {
        public TimeSpan Start;
        public TimeSpan End;
        public string Text;
        public bool valid = false;

        private static TimeSpan ParseTime(string source)
        {
            int hour = 0, minute = 0, second = 0, ms = 0;
            var format = source.Split(new[] { ':', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
            int.TryParse(format[0], out hour);
            int.TryParse(format[1], out minute);
            int.TryParse(format[2], out second);
            int.TryParse(format[3], out ms);
            return new TimeSpan((int)Math.Floor(hour /24f), hour % 24, minute, second, ms);
        }

        public static SubtitleText Parse(string source)
        {
            var retval = new SubtitleText();
            try
            {
                var format = source.Replace("\r", "").Split('\n');

                string timedata = format[0].Contains("-->") ? format[0] : format[1];
                var timeformat = timedata.Split("-->");
                retval.Start = ParseTime(timeformat[0].Trim());
                retval.End = ParseTime(timeformat[1].Trim());
                int textStart = format[0].Contains("-->") ? 1 : 2;

                var sb = new StringBuilder();
                for(int i = textStart; i < format.Length; i++)
                {
                    sb.AppendLine(format[i]);
                }
                retval.Text = sb.ToString();
                retval.valid = true;
            }
            catch (Exception e) {
                Log.X(e);
            }
            
            return retval;
        }
    }
      
    public static class SubripParser
    {
        private static Regex numericRegex = new Regex("[0-9]{1,10}", RegexOptions.Compiled);
        public static List<SubtitleText> Parse(string source)
        {
            List<SubtitleText> texts = new List<SubtitleText>();
            var lines = source.Replace("\r","").Split('\n');
            StringBuilder sb = new StringBuilder();
            string lastLine = "";

            void Put()
            {
                var text = SubtitleText.Parse(sb.ToString());
                if (text.valid) { texts.Add(text); }
                sb.Clear();
            }

            foreach(var line in lines)
            {
                if (lastLine.Length == 0 && numericRegex.IsMatch(line)) Put();
                sb.AppendLine(line);
                lastLine = line;
            }
            Put();

            return texts;
        }

        public static List<SubtitleText> LoadFile(string file)
        {
            return Parse(File.ReadAllText(file));
        }
    }
}
