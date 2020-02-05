using _9epoch_common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _9epoch_common
{
    public static class Financial
    {
        public static DateTime FindNextOpen()
        {
            if ((DateTime.Now.DayOfWeek != DayOfWeek.Saturday || DateTime.Now.DayOfWeek != DayOfWeek.Sunday) && DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 10)
            {
                //pre open
                return DateTime.Today.AddHours(9).AddMinutes(57);
            }
            DateTime nextOpen = DateTime.Today.AddDays(1);
            nextOpen = nextOpen.AddHours(9).AddMinutes(57);

            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                //if its a friday then next open is monday
                nextOpen = nextOpen.AddDays(2);
            }

            return nextOpen;
        }
        public static double CalcDailySharpe(List<double> dailyReturns)
        {
            double sr = 0;
            sr = dailyReturns.Average() / dailyReturns.StdDev() * Math.Sqrt(252);
            return sr;
        }
        public static List<Price> GetPriceRange(List<Price> prices, DateTime start, DateTime end)
        {
            List<Price> output = new List<Price>();

            try
            {
                int startIndex = prices.BinarySearch(new Price { DateTime = start }, new PriceComparer());
                if (startIndex < -1)
                {
                    //handle non exact match
                    int newIndex = startIndex * -1;

                    //If binary search returns neg count, there is none found
                    if (newIndex > prices.Count)
                    {
                        return output;
                    }

                    //If binary search returns neg thats the nearest index
                    startIndex = newIndex;

                }
                if (startIndex >= 0)
                {

                    for (int i = startIndex; i < prices.Count; i++)
                    {
                        if (prices[i].DateTime > end)
                        {
                            break;
                        }
                        output.Add(prices[i]);
                    }
                }
            }
            catch (Exception e) { }
                

            return output;
        }
        public static SortedList<DateTime, double> GetReturnsSeries(List<Price> prices)
        {
            //Calc daily returns series
            SortedList<DateTime, double> returnsSeries = new SortedList<DateTime, double>();
            for (int i = 1; i < prices.Count; i++)
            {
                if (i > 0)
                {
                    bool returnOK = true;
                    if (prices[i - 1].ClosePrice == 0 || prices[i].ClosePrice == 0) { returnOK = false; }
                    if (prices[i - 1].Volume == 0 || prices[i].Volume == 0) { returnOK = false; }
                    try
                    {
                        if (returnOK)
                        {
                            Double dailyReturn = ((prices[i - 1].ClosePrice - prices[i].ClosePrice) / prices[i].ClosePrice);
                            if (Double.IsNaN(dailyReturn) || Double.IsInfinity(dailyReturn)) { dailyReturn = 0; }
                            DateTime date = prices[i].DateTime.Date;
                            returnsSeries.Add(date, dailyReturn);
                        }
                    }
                    catch (Exception e) { }
                }
            }

            return returnsSeries;
        }
        public static double GetSingleReturn(DateTime start, DateTime end, List<Price> prices, bool reverseStart = false)
        {
            if (end > DateTime.Today) { end = DateTime.Today; }
            double priceReturn = 0;
            try
            {
                int maxTries = ((end - start).Days - 1);
                if (maxTries <= 0) { maxTries = 1; }
                double startPrice = 0;
                double endPrice = 0;
                Price startObj = null;
                Price endObj = null;

                bool s = false;
                if (reverseStart)
                {
                    int tries = 0;
                    while (!s && tries <= maxTries)
                    {
                        startObj = prices.Where(x => x.DateTime == start).SingleOrDefault();
                        if (startObj == null) { s = false; start = start.AddDays(1); } else { s = true; }
                        tries++;
                    }

                }
                else
                {
                    int tries = 0;
                    while (!s && tries <= maxTries)
                    {
                        startObj = prices.Where(x => x.DateTime == start).SingleOrDefault();
                        if (startObj == null) { s = false; start = start.AddDays(1); } else { s = true; }
                        tries++;
                    }
                }


                bool e = false;
                endObj = prices.Where(x => x.DateTime == end).SingleOrDefault();
                if (endObj == null) { e = false; } else { e = true; }
                int endTries = 0;
                while (!e && endTries <= maxTries)
                {
                    endObj = prices.Where(x => x.DateTime == end).SingleOrDefault();
                    if (endObj == null) { e = false; end = end.AddDays(-1); } else { e = true; }
                    endTries++;
                }

                if (startObj != null && endObj != null)
                {
                    startPrice = startObj.ClosePrice;
                    endPrice = endObj.ClosePrice;
                    priceReturn = ((endPrice - startPrice) / startPrice);
                }
            }
            catch (Exception e) { }
            if (Double.IsNaN(priceReturn) || Double.IsInfinity(priceReturn)) { priceReturn = 0; }
            return priceReturn;
        }

        public static GicsSector ClassifySector(string code)
        {
            GicsSector output = GicsSector.Unknown;

            string sectorCode = code.Substring(0, 2);

            if (sectorCode == "10") { return GicsSector.Energy; }
            if (sectorCode == "15") { return GicsSector.Materials; }
            if (sectorCode == "20") { return GicsSector.Industrials; }
            if (sectorCode == "25") { return GicsSector.Discretionary; }
            if (sectorCode == "30") { return GicsSector.Staples; }
            if (sectorCode == "35") { return GicsSector.Healthcare; }
            if (sectorCode == "40") { return GicsSector.Financials; }
            if (sectorCode == "45") { return GicsSector.InfoTech; }
            if (sectorCode == "50") { return GicsSector.Communications; }
            if (sectorCode == "55") { return GicsSector.Utilities; }
            if (sectorCode == "60") { return GicsSector.RealEstate; }

            return output;
        }
    }
    public class PriceComparer : IComparer<Price>
    {

        public int Compare(Price x, Price y)
        {
            return x.DateTime.CompareTo(y.DateTime);
        }


    }
    public enum GicsSector
    {
        Energy,
        Materials,
        Communications,
        Discretionary,
        Staples,
        Financials,
        Healthcare,
        Industrials,
        InfoTech,
        RealEstate,
        Utilities,
        Unknown
    }


    public class SimpleDrawDown
    {
        public double Peak { get; set; }
        public double Trough { get; set; }
        public double MaxDrawDown { get; set; }

        public SimpleDrawDown()
        {
            Peak = double.NegativeInfinity;
            Trough = double.PositiveInfinity;
            MaxDrawDown = 0;
        }

        public void Calculate(double newValue)
        {
            if (newValue > Peak)
            {
                Peak = newValue;
                Trough = Peak;
            }
            else if (newValue < Trough)
            {
                Trough = newValue;
                var tmpDrawDown = Peak - Trough;
                if (tmpDrawDown > MaxDrawDown)
                    MaxDrawDown = tmpDrawDown;
            }
        }
    }
}
