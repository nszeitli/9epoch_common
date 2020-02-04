using System;
using System.Collections.Generic;
using System.Text;

namespace _9epoch_common.Models
{
    public class AsxWebAnn
    {
 
        public int Id { get; set; }

        public string ASXId { get; set; }
        public string Ticker { get; set; }
        public DateTime DateTime { get; set; }
        public bool PriceSensitive { get; set; }
        public string Headline { get; set; }
        public int NoPages { get; set; }
        public double FileSize { get; set; }
        public string PDFPath { get; set; }
        public string HTMLPath { get; set; }
        public string CSVPath { get; set; }



        
    }
}
