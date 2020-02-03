using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace _9epoch_common.IO
{
    internal sealed class CSVReader : System.IDisposable
    {
        public CSVReader(string fileName)
            : this(new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
        }

        public CSVReader(Stream stream)
        {
            __reader = new StreamReader(stream);
        }

        public System.Collections.IEnumerable RowEnumerator
        {
            get
            {
                if (null == __reader)
                    throw new System.ApplicationException("I can't start reading without CSV input.");

                __rowno = 0;
                string sLine;
                string sNextLine;

                while (null != (sLine = __reader.ReadLine()))
                {
                    while (rexRunOnLine.IsMatch(sLine) && null != (sNextLine = __reader.ReadLine()))
                        sLine += "\n" + sNextLine;

                    __rowno++;
                    string[] values = rexCsvSplitter.Split(sLine);

                    for (int i = 0; i < values.Length; i++)
                        values[i] = CsvData.Unescape(values[i]);

                    yield return values;
                }

                __reader.Close();
            }

        }

        public long RowIndex { get { return __rowno; } }

        public void Dispose()
        {
            if (null != __reader) __reader.Dispose();
        }

        //============================================


        private long __rowno = 0;
        private TextReader __reader;
        private static Regex rexCsvSplitter = new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
        private static Regex rexRunOnLine = new Regex(@"^[^""]*(?:""[^""]*""[^""]*)*""[^""]*$");

    }
    
}
