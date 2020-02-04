using _9epoch_common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace _9epoch_common.PDF
{
    public class PDFDownloader
    {
        public string SavePath { get; set; }
        public PDFDownloader(String savePath = "C:\\Trading\\PDF\\")
        {
            this.SavePath = savePath;
        }

        public void DownloadListAnnPDF(List<AsxWebAnn> list, bool overwrite = false)
        {
            using (var client = new WebClient())
            {
                foreach (var headline in list)
                {
                    String date = headline.DateTime.ToString("yyyyMMdd");
                    String ticker = headline.Ticker;
                    String annID = headline.ASXId;
                    String pdfURL = "http://www.aspecthuntley.com.au/asxdata/" + date + "/pdf/" + annID + ".pdf";
                    String savePathFile = SavePath + ticker + "\\" + date + "_" + annID + ".pdf";

                    if (headline.PDFPath == null)
                    {
                        //Update db
                        if (File.Exists(savePathFile)) { headline.PDFPath = savePathFile; }
                    }

                    if (overwrite || headline.PDFPath == null)
                    {

                        Boolean PDFisok = false;

                        if (!Directory.Exists(SavePath + ticker + "\\"))
                        {
                            Directory.CreateDirectory(SavePath + ticker + "\\");
                        }

                        Int16 downloadAttempt = 0;
                        while (downloadAttempt < 2 && PDFisok == false && headline.NoPages < 100)
                        {
                            try
                            {
                                client.DownloadFile(pdfURL, @savePathFile);

                                long length = new System.IO.FileInfo(savePathFile).Length;

                                if (length > 500) { PDFisok = true; } else { Thread.Sleep(500); }
                            }
                            catch (Exception e) { }

                            downloadAttempt++;
                        }


                        if (PDFisok == true)
                        {
                            Console.WriteLine("SUCCESS: PDF for " + ticker);
                            headline.PDFPath = savePathFile;
                        }
                        else
                        {
                            Console.WriteLine("FAILURE: PDF for " + ticker);
                        }
                    }


                }

            }
        }

        public bool DownloadPDFFromModel(AsxWebAnn headline)
        {
            using (var client = new WebClient())
            {
                String date = headline.DateTime.ToString("yyyyMMdd");
                String ticker = headline.Ticker;
                String annID = headline.ASXId;
                String pdfURL = "http://www.aspecthuntley.com.au/asxdata/" + date + "/pdf/" + annID + ".pdf";
                String savePathFile = SavePath + ticker + "\\" + date + "_" + annID + ".pdf";
                Boolean PDFisok = false;

                if (!Directory.Exists(SavePath + ticker + "\\"))
                {
                    Directory.CreateDirectory(SavePath + ticker + "\\");
                }

                Int16 downloadAttempt = 0;
                while (downloadAttempt < 2 && PDFisok == false)
                {
                    try
                    {
                        client.DownloadFile(pdfURL, @savePathFile);

                        long length = new System.IO.FileInfo(savePathFile).Length;

                        if (length > 500) { PDFisok = true; } else { Thread.Sleep(500); }
                    }
                    catch (Exception e) { }

                    downloadAttempt++;
                }


                if (PDFisok == true)
                {
                    Console.WriteLine("SUCCESS: PDF for " + ticker);
                    headline.PDFPath = savePathFile;
                }
                else
                {
                    Console.WriteLine("FAILURE: PDF for " + ticker);
                    return false;
                }

                return true; ;
            }
        }

        public String DownloadPDFfromDetails(String date, String ticker, String annID)
        {
            using (var client = new WebClient())
            {

                String pdfURL = "http://www.aspecthuntley.com.au/asxdata/" + date + "/pdf/" + annID + ".pdf";
                String savePathFile = SavePath + "\\" + ticker + "_" + date + "_" + annID + ".pdf";
                Boolean PDFisok = false;

                Int16 downloadAttempt = 0;
                while (downloadAttempt < 5 && PDFisok == false)
                {
                    try
                    {
                        client.DownloadFile(pdfURL, @savePathFile);

                        long length = new System.IO.FileInfo(savePathFile).Length;

                        if (length > 500) { PDFisok = true; } else { Thread.Sleep(500); }
                    }
                    catch (Exception e) { }

                    downloadAttempt++;
                }


                if (PDFisok == false)
                {

                    Console.WriteLine("FAILURE: PDF for " + ticker);
                    savePathFile = "failed";
                }
                else
                {
                    Console.WriteLine("SUCCESS: PDF for " + ticker);

                }

                return savePathFile;
            }
        }



    }
}
