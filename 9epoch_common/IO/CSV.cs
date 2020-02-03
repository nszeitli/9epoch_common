using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _9epoch_common.IO
{
    public class CSV
    {
        public List<List<string>> ReadCsv(string path, bool includeHeader = true, int startCol = 0, int endCol = 0)
        {
            var reader = new CSVReader(new FileStream(path, FileMode.Open));
            List<List<string>> list = new List<List<string>>();

            foreach (string[] item in reader.RowEnumerator)
            {
                try
                {
                    if (item.Length > 1)
                    {
                        list.Add(item.ToList());
                    }

                }
                catch (Exception e) { }
            }

            //Check if data
            if (list.Count == 0) { return list; }

            //Check headers
            if (!includeHeader) { list.RemoveAt(0); }

            //Check if specified columns
            if (startCol == 0 && endCol == 0) { return list; }

            else 
            {
                try
                {
                    if (endCol > list[0].Count - 1) { endCol = list[0].Count - 1; }

                    var table = Enumerable.Range(startCol, endCol)
                            .Select(i => list.Select(x => x[i])
                            .ToList()
                        ).ToList();

                    return table;
                }
                catch (Exception e) { Console.WriteLine("Error in table columns ranges"); }
            }

            return list;
        }


        public bool WriteCsv(string path, List<List<string>> data, bool append = false)
        {
            return CsvWriter.WriteToCSV(data, path, append);

        }

    }

    
    
    
}
