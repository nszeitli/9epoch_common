using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _9epoch_common
{
    public static class MathExt
    {
        //Check for NaN
        public static bool NanCheck(double d)
        {
            if (Double.IsNaN(d) || Double.IsInfinity(d))
            {
                return true;
            }
            return false; 
        }
        public static double NoNaN(double d)
        {
            if (Double.IsNaN(d) || Double.IsInfinity(d))
            {
                return 0;
            }
            return d;
        }

        //For distributions
        public static double[] GetDynamicBins(List<double> data)
        {
            double sd = data.StdDev();
            //double ave = hm.GetMedian(data.ToArray());
            double ave = data.Average();
            double min = data.Min();
            double max = data.Max();

            //get new bounds without outliers
            List<double> noTails = new List<double>();
            foreach (var item in data)
            {
                if ((item < ave - (2 * sd)) || (item > ave + (2 * sd)))
                {
                    //remove
                }
                else
                {
                    noTails.Add(item);
                }
            }

            //work out rounding
            int rounding = 10;
            if (sd < 100) { rounding = 0; }
            if (sd < 1) { rounding = 1; }
            if (sd < 0.1) { rounding = 2; }
            if (sd < 0.01) { rounding = 3; }

            double noTailMin = noTails.Min();
            double noTailMax = noTails.Max();
            //double binInterval = (noTailMax - noTailMin) / 7;
            double binInterval = (sd) / 4;
            double topFour = 0;
            double halfsd = 0;
            double binStart = (ave - (3 * binInterval));
            if (rounding < 10)
            {
                ave = Math.Round(ave, rounding);
                max = Math.Round(max, rounding);
                noTailMin = Math.Round(noTailMin, rounding);
                noTailMax = Math.Round(noTailMax, rounding);
                binInterval = Math.Round(binInterval, rounding);
                binStart = Math.Round(binStart, rounding);


                //9 bins

                var b = new double[] { noTailMin, binStart , (binStart + binInterval),
                    (binStart + binInterval *2), ave, (ave + binInterval),
                    (ave + binInterval * 2), (ave + binInterval * 3),max };

                List<double> o = new List<double>();
                foreach (var item in b)
                {
                    o.Add(Math.Round(item, rounding));
                }
                return o.OrderBy(d => d).ToArray();
            }
            min = Math.Round(min / rounding, 0) * rounding;
            max = Math.Round(max / rounding, 0) * rounding;
            noTailMin = Math.Round(noTailMin / rounding, 0) * rounding;
            noTailMax = Math.Round(noTailMax / rounding, 0) * rounding;
            binInterval = Math.Round(binInterval / rounding, 0) * rounding;
            binStart = Math.Round(binStart / rounding, 0) * rounding;

            //9 bins

            var bins = new double[] { noTailMin, binStart , (binStart + binInterval),
                    (binStart + binInterval *2), ave, (ave + binInterval),
                    (ave + binInterval * 2), (ave + binInterval * 3),max };

            List<double> output = new List<double>();
            foreach (var item in bins)
            {
                if (!output.Contains(item))
                {
                    output.Add(Math.Round(item / rounding, 0) * rounding);
                }
            }
            return output.OrderBy(d => d).ToArray();

        }

        public static int[] GetDynamicBinsDiscrete(List<int> data)
        {
            var output = data.Distinct().OrderBy(d => d).ToList();

            return output.ToArray();

        }
        public static string Bin(double x, double[] intervals)
        {
            int i = 0;
            foreach (var bin in intervals)
            {
                if (x <= intervals[i])
                    return bin.ToString();
                i++;
            }
            return intervals[intervals.Length - 1].ToString();
        }
    }
}
