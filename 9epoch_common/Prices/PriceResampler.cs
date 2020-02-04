using _9epoch_common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _9epoch_common.Prices
{
    class PriceResampler
    {

        public List<Price> ProcessPrices(List<Price> oldPrices, int minutesToResample)
        {
            List<Price> output = new List<Price>();
            string ticker = oldPrices[0].Ticker;
            oldPrices = oldPrices.OrderBy(i => i.DateTime).ToList();
            DateTime date = oldPrices[0].DateTime.Date;

            //Fix early opens
            try
            {
                var earlyOpen = oldPrices.Where(i => i.DateTime.Hour == 9).SingleOrDefault();
                if (earlyOpen != null)
                {
                    DateTime toSet = earlyOpen.DateTime.Date;
                    TimeSpan ts = new TimeSpan(10, 00, 0);
                    toSet = toSet.Date + ts;
                    earlyOpen.DateTime = toSet;
                }

            }
            catch (Exception e) { }


            //Bar length
            TimeSpan barLength = new TimeSpan(0, minutesToResample, 0);

            TimeSpan startTime = new TimeSpan(10, 0, 0);
            TimeSpan endTime = new TimeSpan(16, 20, 0);



            List<Price> currentBar = new List<Price>();
            //data = oldPrices.GroupBy(t => (int)(t.DateTime.Minutes / minutesToResample)));
            var grouped = oldPrices.GroupBy(x =>
            {
                var stamp = x.DateTime;
                stamp = stamp.AddMinutes(-(stamp.Minute % minutesToResample));
                stamp = stamp.AddMilliseconds(-stamp.Millisecond - 1000 * stamp.Second);
                return stamp;
            })
            .Select(g => new {
                dt = g.Key,
                Open = g.First().OpenPrice,
                High = g.Max(x => x.HighPrice),
                Low = g.Min(x => x.LowPrice),
                HighClose = g.Last().HighClose,
                LowClose = g.Last().LowClose,
                Volume = g.Sum(x => x.Volume),
                Value = g.Sum(x => x.Value),
                TradeCount = g.Sum(x => x.TradeCount),
                LastBid = g.Last().LastBid,
                LastAsk = g.Last().LastAsk,
                LastTick = g.Last().LastTick,
                AdjFactor = g.Last().AdjFactor
            })
            .ToList();


            foreach (var g in grouped)
            {
                TimeSpan startBar = g.dt.TimeOfDay;
                TimeSpan endBar = startTime.Add(barLength);

                output.Add(new Price
                {
                    Ticker = ticker,
                    DateTime = g.dt,
                    OpenPrice = g.Open,
                    HighPrice = g.High,
                    LowPrice = g.Low,
                    HighClose = g.HighClose,
                    LowClose = g.LowClose,
                    Volume = g.Volume,
                    Value = g.Value,
                    TradeCount = g.TradeCount,
                    LastAsk = g.LastAsk,
                    LastBid = g.LastBid,
                    LastTick = g.LastTick,
                    BarVWAP = (g.Value / g.Volume) / g.AdjFactor,
                    AdjFactor = g.AdjFactor
                });
            }

            return output;
        }
        
        public List<Price> ProcessPricesDaily(List<Price> oldPrices)
        {
            List<Price> output = new List<Price>();
            string ticker = oldPrices[0].Ticker;
            oldPrices = oldPrices.OrderBy(i => i.DateTime).ToList();
            DateTime date = oldPrices[0].DateTime.Date;



            //Bar length


            TimeSpan startTime = new TimeSpan(10, 0, 0);
            TimeSpan endTime = new TimeSpan(16, 20, 0);



            List<Price> currentBar = new List<Price>();
            //data = oldPrices.GroupBy(t => (int)(t.DateTime.Minutes / minutesToResample)));
            var grouped = oldPrices.GroupBy(x =>
            {
                var stamp = x.DateTime.Date;
                return stamp;
            })
            .Select(g => new {
                dt = g.Key,
                Open = g.First().OpenPrice,
                High = g.Max(x => x.HighPrice),
                Low = g.Min(x => x.LowPrice),
                HighClose = g.Last().HighClose,
                Volume = g.Sum(x => x.Volume),
                Value = g.Sum(x => x.Value),
            })
            .ToList();


            foreach (var g in grouped)
            {
                DateTime startBar = g.dt;


                output.Add(new Price
                {
                    Ticker = ticker,
                    DateTime = g.dt,
                    OpenPrice = g.Open,
                    HighPrice = g.High,
                    LowPrice = g.Low,
                    HighClose = g.HighClose,
                    Volume = g.Volume,
                    Value = g.Value,
                });
            }

            return output;
        }


        public List<Price> DailyVWAP(List<Price> yearOfPrices)
        {
            List<Price> output = new List<Price>();

            double dailyVal = 0;
            double dailyVol = 0;
            DateTime currentDate = DateTime.MinValue;
            foreach (var p in yearOfPrices)
            {
                if (p.DateTime.Date > currentDate)
                {
                    //New day
                    dailyVal = 0;
                    dailyVol = 0;

                    currentDate = p.DateTime.Date;
                }
                dailyVal += p.Value;
                dailyVol += p.Volume;
                p.ProgressiveDailyVWAP = (dailyVal / dailyVol) / p.AdjFactor;
                output.Add(p);
            }
            return output;
        }
        public List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (System.Exception excpt)
            {

            }

            return files;
        }
    }
}
