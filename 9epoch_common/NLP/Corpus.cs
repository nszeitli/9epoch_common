using _9epoch_common.IO;
using System;
using System.Collections.Generic;
using System.Linq;


namespace _9epoch_common.NLP
{
    class Corpus
    {
        
        public Corpus()
        {
            
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
            TokenCounter tc = new TokenCounter();
            var tokens = tc.CountTokens(text, null, englishOnly, removeNumbers, 0, removeStopWords);
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
            TokenCounter tc = new TokenCounter();
            var tokens = tc.CountTokens(text, null, englishOnly, removeNumbers, 0, removeStopWords);
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
