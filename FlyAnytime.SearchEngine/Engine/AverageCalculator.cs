using FlyAnytime.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine
{
    class PriceAggregate
    {
        public PriceAggregate(decimal value, int totalVals)
        {
            Value = value;
            TotalValues = totalVals;
            Count = 0;
        }

        public decimal Value { get; set; }
        public int Count { get; set; }
        public int TotalValues { get; set; }
        public decimal Probability => (decimal)Count / TotalValues;
    }

    public class AverageCalculator
    {
        public static decimal GetAvarege(List<decimal> prices, decimal accuracy)
        {
            if (accuracy <= 0 || accuracy > 1)
                throw new ArgumentOutOfRangeException(nameof(accuracy), "Accuracy must be in (0, 1]");

            var totalCount = prices.Count;

            if (totalCount == 0)
                return 0;

            var aggregated = new Dictionary<decimal, PriceAggregate>(totalCount/5);
            foreach (var p in prices)
            {
                var rounded = Math.Round(p * 2, MidpointRounding.AwayFromZero) / 2;

                var val = aggregated.GetOrAdd(rounded, v => new PriceAggregate(v, totalCount));
                val.Count++;
            }

            var orderd = aggregated.Values.OrderByDescending(x => x.Probability).ToList();
            decimal currAcc = 0;
            decimal propabilityAccur = 0.05M;
            decimal totalPrice = 0;
            var pricesCount = 0;

            for (int i = 0; i < orderd.Count; i++)
            {
                var curr = orderd[i];

                currAcc += curr.Probability;
                totalPrice += curr.Value * curr.Count;
                pricesCount += curr.Count;
                if (currAcc - accuracy > 0)
                {
                    if (i != orderd.Count - 1)//Not last iteration
                    {
                        if (curr.Probability - orderd[i + 1].Probability < propabilityAccur)
                        {
                            totalPrice += curr.Value * curr.Count;
                            pricesCount += curr.Count;
                        }
                    }
                    break;
                }
            }

            return totalPrice / pricesCount;
        }
    }
}
