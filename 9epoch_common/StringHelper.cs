using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace _9epoch_common
{
    public static class StringHelper
    {
        public static String TrimSpacesAndNewLines(String s)
        {
            s = s.Replace("\n", "");
            s = s.Replace(",", "");
            s = RemoveAllDoubleSpaces(s);
            s = s.Trim();
            return s;
        }

        public static Double PriceToDouble(String s)
        {
            s = s.Replace("$", "");
            //try { s = if(s.IndexOf("\n")) s.Remove(s.IndexOf("\n")); } catch (Exception e) { }
            s = s.Trim();
            Double sDbl = Convert.ToDouble(s);
            return sDbl;
        }

        public static int ParsePagesConverted(string s)
        {
            s = GetSpaceDelimitedNumbers(s);
            string[] pages = s.Split(' ');
            int maxpage = 0;
            foreach (String str in pages)
            {
                int pageNo = 0;
                try { pageNo = Convert.ToInt16(str); if (pageNo > maxpage) { maxpage = pageNo; } } catch (Exception e) { }
            }
            return maxpage;
        }

        public static String GetSpaceDelimitedNumbers(String text)
        {
            text = ChangeLettersToSpaces(text).Trim();
            text = RemoveAllDoubleSpaces(text);
            return text;
        }

        public static String ChangeLettersToSpaces(String text)
        {
            text = Regex.Replace(text, "[^0-9]", " ");
            return text;
        }
        public static String RemoveSpacesAndSymbols(String text)
        {
            text = Regex.Replace(text, @"[^a-zA-Z\d]", "-");
            return text;
        }
        public static String RemoveAllDoubleSpaces(String text)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            text = regex.Replace(text, " ");
            return text;
        }

        public static string FixSymbolsInHTML(string s)
        {
            s = s.Replace("&amp;", "&");
            return s.Replace("&#039;", "'");
        }

    }
}
