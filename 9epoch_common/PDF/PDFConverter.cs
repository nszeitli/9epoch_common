using _9epoch_common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace _9epoch_common.PDF
{
    public class PDFConverter
    {
        public string PDFroot { get; set; }
        public string HTMLroot { get; set; }

        public int Timeout { get; set; }


        public PDFConverter(string PDFroot = @"C:\Trading\PDF\", string HTMLroot = @"C:\Trading\HTML\")
        {
            this.PDFroot = PDFroot;
            this.HTMLroot = HTMLroot;
            Timeout = 30;

        }
        public async void ConvertASXAnns(List<AsxWebAnn> list)
        {
            List<AsxWebAnn> batchToExecute = new List<AsxWebAnn>();
            int count = 1;
            foreach (AsxWebAnn headline in list)
            {

                Stopwatch sw = new Stopwatch();
                sw.Start();

                if (headline.PDFPath.Length > 3)
                {
                    string htmlPath = HTMLroot + headline.Ticker + "\\" + headline.DateTime.ToString("yyyyMMdd") + "_" + StringHelper.RemoveSpacesAndSymbols(headline.Headline) + "_" + headline.ASXId + "\\";
                    (new FileInfo(htmlPath)).Directory.Create();
                    headline.HTMLPath = htmlPath;
                    batchToExecute.Add(headline);

                    //Do a batch
                    if (batchToExecute.Count == 5)
                    {
                        var output = await ConvertBatch(batchToExecute);
                        sw.Stop();
                        //Console.WriteLine("Converting no " + count + " of " + list.Count + " after " + (sw.ElapsedMilliseconds/1000/60) + " minutes");
                        string[] lines = { batchToExecute[0].Id.ToString() + " : " + batchToExecute[0].DateTime.ToShortDateString() + "Converting no " + count + " of " + list.Count + " after " + (sw.ElapsedMilliseconds / 1000 / 60) + " minutes" };
                        System.IO.File.WriteAllLines(@"D:\Trading\log.txt", lines);
                        for (int i = 0; i < 5; i++)
                        {
                            int pagesConverted = StringHelper.ParsePagesConverted(output[i]);
                            if (pagesConverted == 0)
                            {
                                batchToExecute[i].HTMLPath = null;
                            }
                        }
                    }
                }
                count++;
            }

            //clean up
            foreach (AsxWebAnn headline in batchToExecute)
            {
                string output = PdftoHTML(headline.PDFPath, headline.HTMLPath);
                int pagesConverted = StringHelper.ParsePagesConverted(output);
                if (pagesConverted == 0)
                {
                    headline.HTMLPath = null;
                }
            }
        }
        // Parallel execution
        private async Task<List<string>> ConvertBatch(List<AsxWebAnn> batchToExecute)
        {
            var first = Task.Run(() =>
            {
                return PdftoHTML(batchToExecute[0].PDFPath, batchToExecute[0].HTMLPath);
            });

            var second = Task.Run(() =>
            {
                return PdftoHTML(batchToExecute[1].PDFPath, batchToExecute[1].HTMLPath);
            });

            var third = Task.Run(() =>
            {
                return PdftoHTML(batchToExecute[2].PDFPath, batchToExecute[2].HTMLPath);
            });

            var fourth = Task.Run(() =>
            {
                return PdftoHTML(batchToExecute[3].PDFPath, batchToExecute[3].HTMLPath);
            });

            var fifth = Task.Run(() =>
            {
                return PdftoHTML(batchToExecute[4].PDFPath, batchToExecute[4].HTMLPath);
            });

            return new List<string>() { first.Result, second.Result, third.Result, fourth.Result, fifth.Result };
        }
        private String PdftoHTML(String pdfPath, String htmlPath)
        {
            //Console.WriteLine("Converting to " + htmlPath);
            string parms = " -c -i " + @pdfPath + " " + @htmlPath;
            var task = Task.Run(() => RunPDFtoHTMLProcess(parms));
            if (task.Wait(TimeSpan.FromSeconds(15)))
                return task.Result;
            else
                Console.WriteLine("HTML convert timed out on " + pdfPath);
            return "TIMEOUT";
        }
        private String RunPDFtoHTMLProcess(string parms)
        {

            string output = "";

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = @"C:\Users\nszei\source\repos\PSAIS\PSAIS\Data\Convert\pdftohtml.exe";
            psi.WorkingDirectory = System.IO.Path.GetDirectoryName(psi.FileName);
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.Arguments = parms;
            psi.UseShellExecute = false;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            psi.CreateNoWindow = true;
            System.Diagnostics.Process reg;

            reg = System.Diagnostics.Process.Start(psi);
            try
            {
                using (System.IO.StreamReader myOutput = reg.StandardOutput)
                {
                    output = myOutput.ReadToEnd();
                }
                using (System.IO.StreamReader myError = reg.StandardError)
                {
                    string error = myError.ReadToEnd();
                    if (error.Contains("PDF file")) { Debug.WriteLine(error); }
                }
                reg.WaitForExit();
            }
            catch (Exception e) { Console.WriteLine("PDF convert failed"); }
            string[] lines = { parms };
            System.IO.File.WriteAllLines(@"D:\Trading\log.txt", lines);
            return output;
        }
    }
}
