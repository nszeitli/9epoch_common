using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace _9epoch_common.IO
{
    public static class CsvWriter
    {
        public static object _locked = new object();

        public static bool WriteToCSV(List<List<String>> rows, string path, bool append = false)
        {
            lock (_locked)
            {
                try
                {
                    using (var writer = new StreamWriter(path, append))
                    {
                        foreach (List<String> row in rows)
                        {
                            string toWrite = "";
                            foreach (String st in row)
                            {
                                string s = st.Replace(",", " ");
                                toWrite = toWrite + s + ",";
                            }
                            toWrite = toWrite.Remove(toWrite.Length - 2);
                            writer.WriteLine(toWrite);
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine("Something went wrong writing to CSV " + path); return false; }
                return true;
            }
        }
    }
}
