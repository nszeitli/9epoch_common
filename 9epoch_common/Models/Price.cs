using System;
using System.Collections.Generic;
using System.Text;

namespace _9epoch_common.Models
{
    public class Price
    {
        public string Ticker { get; set; }
        public DateTime DateTime { get; set; }
        public double OpenPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double ClosePrice { get; set; }
        public double HighClose { get; set; }
        public double LowClose { get; set; }

        public double Volume { get; set; }
        public double Value { get; set; }
        public long TradeCount { get; set; }

        public double LastTick { get; set; }
        public double LastBid { get; set; }
        public double LastAsk { get; set; }

        public double AdjFactor { get; set; }
        //VWAP
        public double BarVWAP { get; set; }
        public double ProgressiveDailyVWAP { get; set; }
    }
}
