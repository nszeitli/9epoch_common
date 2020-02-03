using _9epoch_common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WeCantSpell.Hunspell;

namespace _9epoch_common.NLP
{
    class Corpus
    {
        private WordList _spellCheck;
        private List<string> _stopWords;
        public Corpus()
        {
            _spellCheck = WordList.CreateFromFiles(@"en_au.dic");
            _stopWords = new List<string> { "i", "me", "my", "myself", "we", "our", "ours", "ourselves", "you", "your", "yours", "yourself", "yourselves", "he", "him", "his", "himself", "she", "her", "hers", "herself", "it", "its", "itself", "they", "them", "their", "theirs", "themselves", "what", "which", "who", "whom", "this", "that", "these", "those", "am", "is", "are", "was", "were", "be", "been", "being", "have", "has", "had", "having", "do", "does", "did", "doing", "a", "an", "the", "and", "but", "if", "or", "because", "as", "until", "while", "of", "at", "by", "for", "with", "about", "against", "between", "into", "through", "during", "before", "after", "above", "below", "to", "from", "up", "down", "in", "out", "on", "off", "over", "under", "again", "further", "then", "once", "here", "there", "when", "where", "why", "how", "all", "any", "both", "each", "few", "more", "most", "other", "some", "such", "no", "nor", "not", "only", "own", "same", "so", "than", "too", "very", "s", "t", "can", "will", "just", "don", "should", "now", "d", "ll", "m", "o", "re", "ve", "y", "ain", "aren", "couldn", "didn", "doesn", "hadn", "hasn", "haven", "isn", "ma", "mightn", "mustn", "needn", "shan", "shouldn", "wasn", "weren", "won", "wouldn" };
        }

        public List<string> LoadCorpusFromFile(string path)
        {
            var csv = new CSV();
            List<string> output = new List<string>();
            var txt = csv.ReadCsv(path, true);
            foreach (var line in txt)
            {
                output.Add(line[0]);
            }
            return output;
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
            if(corpus != null)
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


        public string ListToString(List<string> data)
        {
            string output = "";
            foreach (var item in data)
            {
                try
                {
                    output = output + " " + item;

                }
                catch (Exception e) { }
            }
            return output;
        }

        public string CsvToString(List<List<string>> data)
        {
            string output = "";
            foreach (var item in data)
            {
                try
                {
                        foreach (var s in item)
                        {
                            output = output + " " + s;
                        }

                }
                catch (Exception e) { }
            }
                return output;
        }
        public List<string> BuildNewCorpus(string text, double removeTopPercent = 0.05, int minimumCount = 5, bool englishOnly = true, bool removeNumbers = true, bool removeStopWords = true)
        {
            var tokens = CountTokens(text, null, englishOnly, removeNumbers, 0, removeStopWords);
            //Compile a dictionary of unique works
            List<string> output = new List<string>();

            int removeTop = Convert.ToInt32(tokens.Count * removeTopPercent);
            var getPercentile = tokens.Values.ToList().OrderByDescending(p => p).ToList();
            int maxCount = getPercentile.ElementAt(removeTop);

            foreach (var kvp in tokens)
            {
                if(kvp.Value >= minimumCount && kvp.Value <= maxCount)
                {
                    output.Add(kvp.Key);
                }
            }

            return output;
        }

        public void SaveNewCorpusToFile(string path, string text, double removeTopPercent = 0.05, int minimumCount = 5, bool englishOnly = true, bool removeNumbers = true, bool removeStopWords = true)
        {
            var data = BuildNewCorpus(text, removeTopPercent, minimumCount, englishOnly, removeNumbers, removeStopWords);

            List<List<string>> rows = new List<List<string>>();
            foreach (var item in data)
            {
                rows.Add(new List<string> { item });
            }

            CSV csv = new CSV();
            csv.WriteCsv(path, rows);
        }

        public void SaveNewCorpusToCSV(string path, string text, double removeTopPercent = 0.05, int minimumCount = 5, bool englishOnly = true, bool removeNumbers = true, bool removeStopWords = true)
        {
            var tokens = CountTokens(text, null, englishOnly, removeNumbers, 0, removeStopWords);
            //Compile a dictionary of unique works
            List<string> output = new List<string>();

            int removeTop = Convert.ToInt32(tokens.Count * removeTopPercent);
            var getPercentile = tokens.Values.ToList().OrderByDescending(p => p).ToList();
            int maxCount = getPercentile.ElementAt(removeTop);

            foreach (var kvp in tokens)
            {
                if (kvp.Value >= minimumCount && kvp.Value <= maxCount)
                {
                    output.Add(kvp.Key);
                }
            }

            //Order new corpus
            List<List<String>> rows = new List<List<string>>();

            List<Tuple<string, int>> tupleList = new List<Tuple<string, int>>();
            foreach (var item in tokens)
            {
                tupleList.Add(new Tuple<string, int>(item.Key, item.Value));
            }
            tupleList = tupleList.OrderByDescending(i => i.Item2).ToList();

            int iRow = 0;
            foreach (var tuple in tupleList)
            {
                rows.Add(new List<string>
                {
                    tuple.Item1, tuple.Item2.ToString()
                });

                iRow++;
            }

            CSV csv = new CSV();
            csv.WriteCsv(path, rows);

        }
    }
}
