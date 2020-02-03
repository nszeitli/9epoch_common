using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WeCantSpell.Hunspell;

namespace _9epoch_common.NLP
{
    public class TokenCounter
    {
        private WordList _spellCheck;
        private List<string> _stopWords;
        public TokenCounter()
        {
            _spellCheck = WordList.CreateFromFiles(@"en_au.dic");
            _stopWords = new List<string> { "i", "me", "my", "myself", "we", "our", "ours", "ourselves", "you", "your", "yours", "yourself", "yourselves", "he", "him", "his", "himself", "she", "her", "hers", "herself", "it", "its", "itself", "they", "them", "their", "theirs", "themselves", "what", "which", "who", "whom", "this", "that", "these", "those", "am", "is", "are", "was", "were", "be", "been", "being", "have", "has", "had", "having", "do", "does", "did", "doing", "a", "an", "the", "and", "but", "if", "or", "because", "as", "until", "while", "of", "at", "by", "for", "with", "about", "against", "between", "into", "through", "during", "before", "after", "above", "below", "to", "from", "up", "down", "in", "out", "on", "off", "over", "under", "again", "further", "then", "once", "here", "there", "when", "where", "why", "how", "all", "any", "both", "each", "few", "more", "most", "other", "some", "such", "no", "nor", "not", "only", "own", "same", "so", "than", "too", "very", "s", "t", "can", "will", "just", "don", "should", "now", "d", "ll", "m", "o", "re", "ve", "y", "ain", "aren", "couldn", "didn", "doesn", "hadn", "hasn", "haven", "isn", "ma", "mightn", "mustn", "needn", "shan", "shouldn", "wasn", "weren", "won", "wouldn" };
        }
        public Dictionary<string, int> CountTokens(string textToCount, List<string> corpus = null, bool englishOnly = true, bool cleanNumbers = true, int maxCount = 0, bool removeStopWords = true)
        {
            //get tokens to check
            Dictionary<string, int> tokenCounts = new Dictionary<string, int>();

            List<string> stopWords = _stopWords;
            if (!removeStopWords) { stopWords = new List<string>(); }

            //Clean
            string regexExpr = @"[^a-zA-Z0-9]";
            if (cleanNumbers) { regexExpr = @"[^a-zA-Z]"; }
            string cleanText = Regex.Replace(textToCount, regexExpr, " ");

            //split
            List<string> tokens = cleanText.Split(' ').ToList();

            //Count
            foreach (var item in tokens)
            {
                string adjToken = item.ToLower();
                if (adjToken.Length >= 3 && adjToken.Length < 18 && !stopWords.Contains(adjToken))
                {
                    if (!tokenCounts.ContainsKey(adjToken))
                    {
                        tokenCounts.Add(adjToken, 1);
                    }
                    else
                    {
                        if (!(tokenCounts[adjToken] >= maxCount)) { tokenCounts[adjToken]++; }

                    }
                }
            }

            //check tokens
            int iWord = 0;
            if (corpus != null)
            {
                Dictionary<string, int> filtered = new Dictionary<string, int>();
                foreach (string word in corpus)
                {
                    if (tokenCounts.ContainsKey(word))
                    {
                        filtered.Add(word, tokenCounts[word]);
                    }
                    iWord++;
                }
                return filtered;
            }

            if (englishOnly)
            {
                Dictionary<string, int> filtered = new Dictionary<string, int>();
                foreach (var kvp in tokenCounts)
                {
                    if (_spellCheck.Check(kvp.Key))
                    {
                        filtered.Add(kvp.Key, kvp.Value);
                    }
                    iWord++;
                }
                return filtered;
            }

            return tokenCounts;
        }
        public SortedDictionary<string, RealTermFrequency> CountTokensToRTF(SortedList<int, string> textDataByPage, int minChars, int maxChars, int pages, Dictionary<string, int> exclusions = null)
        {
            SortedDictionary<string, RealTermFrequency> tokenList = new SortedDictionary<string, RealTermFrequency>();

  
            foreach (var page in textDataByPage)
            {
                string cleanText = Regex.Replace(page.Value, @"[^a-zA-Z \-]", " ");
                string[] tokens = cleanText.Split(' ');
                try
                {
                    foreach (string t in tokens)
                    {
                        if (t.Length > 2)
                        {
                            string token = t.Trim();
                            string adjToken = token.ToLower();
                            //bool excluded = exclusions.TryGetValue(adjToken, out int ex);
                            bool excluded = false;
                            if (token.Length >= minChars && token.Length <= maxChars && !excluded)
                            {

                                RealTermFrequency rtf;
                                bool exists = tokenList.TryGetValue(adjToken, out rtf);

                                //Create new if needed
                                if (!exists)
                                {
                                    rtf = new RealTermFrequency();
                                    rtf.Word = adjToken;
                                    tokenList.Add(rtf.Word, rtf);
                                }

                                bool properNoun = false;

                                //check proper
                                char[] charArray = token.ToCharArray();
                                if (char.IsUpper(charArray[0]) && !char.IsUpper(charArray[1]))
                                {
                                    properNoun = true;
                                }


                                //Categorise page number
                                if (page.Key > 15)
                                {
                                    rtf.CountLateDoc++;
                                    if (properNoun) { rtf.CountLateDocProper++; }
                                }
                                else if (page.Key > 5)
                                {
                                    rtf.CountMidDoc++;
                                    if (properNoun) { rtf.CountMidDocProper++; }
                                }
                                else
                                {
                                    rtf.CountEarlyDoc++;
                                    if (properNoun) { rtf.CountEarlyDocProper++; }
                                }

                                //Final count
                                rtf.CountTotal++;
                                if (properNoun) { rtf.CountProper++; }

                            }
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine("token add error"); }
            }

            return tokenList;
        }
    }


    public class RealTermFrequency
    {
        public string Word { get; set; }

        public bool RealWord { get; set; }

        public int CountTotal { get; set; }

        public int CountProper { get; set; }


        public int CountEarlyDoc { get; set; }
        public int CountEarlyDocProper { get; set; }

        public int CountMidDoc { get; set; }
        public int CountMidDocProper { get; set; }
        public int CountLateDoc { get; set; }
        public int CountLateDocProper { get; set; }
        public int MaxPage { get; set; }
    }
}
