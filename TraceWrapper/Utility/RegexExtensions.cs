using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TraceWrapper
{
    public static class RegexExtensions
    {
        public static string ReplaceGroup(this Regex regex, string input, string groupName, string replacement, Func<string, bool> IgnoredMatch)
        {
            return regex.Replace(input, m =>
            {
                return ReplaceNamedGroup(input, groupName, replacement, m, IgnoredMatch);
            }
            );
        }
        public static string ReplaceGroup(this Regex regex, string input, string groupName, string replacement)
        {
            return ReplaceGroup(regex, input, groupName, replacement, (s) => true);
        }

        private static string ReplaceNamedGroup(string input, string groupName, string replacement, Match m, Func<string, bool> IgnoredMatch)
        {

            var capt = m.Groups[groupName].Captures.OfType<Capture>().FirstOrDefault();
            if (capt == null)
                return m.Value;
            //System.Diagnostics.Trace.WriteLine(input.Substring(capt.Index, capt.Length));
            if (IgnoredMatch(input.Substring(capt.Index, capt.Length)) || input.Substring(capt.Index, capt.Length).Trim() == replacement) return m.Value;
            //var sb = new StringBuilder(input);
            //sb.Remove(capt.Index, capt.Length);
            //sb.Insert(capt.Index, replacement);
            var sb = new StringBuilder(m.Value);
            sb.Remove(capt.Index - m.Index, capt.Length);
            sb.Insert(capt.Index - m.Index, replacement);
            return sb.ToString();
        }
    }
}
