using _9epoch_common.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace _9epoch_common.HTML
{
    public class HTMLParser
    {
        public List<AsxWebAnn> ParseASXWebData(string html)
        {
            List<AsxWebAnn> annList = new List<AsxWebAnn>();

            //Load html into agility pack
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var annNode = doc.DocumentNode.SelectSingleNode("//announcement_data");

            if (annNode != null)
            {
                //get all rows in table
                var allRows = annNode.SelectNodes(".//tr");
                int iRow = 0;
                foreach (var row in allRows)
                {
                    //Skip header
                    if (iRow > 0)
                    {
                        try
                        {
                            var dataTd = row.SelectNodes(".//td");

                            //Parse date
                            string dtString = Regex.Replace(dataTd[1].InnerText, @"\t|\n|\r", "");
                            bool ok = DateHelper.ParseExact(dtString, out DateTime dt, "d/MM/yyyyh:mm tt");


                            //Check price sensitive
                            bool ps = false;
                            if (dataTd[2].InnerHtml.Length > 10) { ps = true; }


                            //Headline is first line of text
                            string innerHtml = dataTd[3].SelectSingleNode(".//a").InnerHtml;
                            string headline = Regex.Replace(innerHtml.Remove(innerHtml.IndexOf("<br>")), @"\t|\n|\r", "");
                            headline = StringHelper.FixSymbolsInHTML(headline);
                            //ASX id is in the link
                            string link = dataTd[3].SelectSingleNode(".//a").Attributes["href"].Value;
                            string asxid = link.Remove(0, link.IndexOf("idsId")).Replace("idsId=", "");
                            //pages and file size in its own span
                            int pages = 0;
                            string s = dataTd[3].SelectSingleNode(".//span[@class='page']").InnerText.Replace("pages", "");
                            bool pagesOk = int.TryParse(Regex.Replace(s, @"\t|\n|\r", ""), out pages);

                            double filesize = 0;
                            bool fsOk = double.TryParse(dataTd[3].SelectSingleNode(".//span[@class='filesize']").InnerText.Replace("MB", "").Replace("KB", ""), out filesize);

                            //Save to model
                            if (ok)
                            {
                                annList.Add(new AsxWebAnn
                                {
                                    Ticker = dataTd[0].InnerText,
                                    DateTime = dt,
                                    PriceSensitive = ps,
                                    Headline = headline,
                                    NoPages = pages,
                                    FileSize = filesize,
                                    ASXId = asxid
                                });
                            }
                        }
                        catch (Exception e) { Console.WriteLine("Warning: row parse failed from ASX announcements"); }
                    }
                    iRow++;
                }
            }
            return annList;
        }

        public List<string> ParseASXForeignEntityDataLinks(string html)
        {
            List<string> links = new List<string>();

            //Load html into agility pack
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var annNode = doc.DocumentNode.SelectSingleNode(@"//*[@class=""primary""]");

            if (annNode != null)
            {
                //get all rows in table
                var allRows = annNode.SelectNodes(".//tr");
                int iRow = 0;
                foreach (var row in allRows)
                {
                    //Skip header
                    if (iRow > 0)
                    {
                        try
                        {
                            var dataTd = row.SelectNodes(".//td");

                            var a = dataTd[1];
                            string link = a.SelectSingleNode(".//a").Attributes["href"].Value;

                            if(link.Length > 0) { links.Add(link); }
                            
                        }
                        catch (Exception e) { Console.WriteLine("Warning: row parse failed from ASX announcements"); }
                    }
                    iRow++;
                }
            }
            return links;
        }
    }
}
