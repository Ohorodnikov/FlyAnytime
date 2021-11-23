using FlyAnytime.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine
{
    public class PriceAggregate
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
        public static List<PriceAggregate> AggregatePrices(List<decimal> prices)
        {
            var totalCount = prices.Count;

            if (totalCount == 0)
                return new List<PriceAggregate>();

            var aggregated = new Dictionary<decimal, PriceAggregate>(totalCount / 5);
            foreach (var p in prices)
            {
                var rounded = Math.Round(p, MidpointRounding.AwayFromZero);

                var val = aggregated.GetOrAdd(rounded, v => new PriceAggregate(v, totalCount));
                val.Count++;
            }

            var orderd = aggregated.Values.ToList();

            return orderd;
        }

        private static decimal CalculateQuantileValue(List<PriceAggregate> prices, decimal chance)
        {
            if (chance <= 0 || chance > 1)
                throw new ArgumentOutOfRangeException(nameof(chance), "Accuracy must be in (0, 1]");

            PriceAggregate prev = null, next = null;

            prices = prices.OrderBy(x => x.Value).ToList();

            if (prices.Count == 0)
                return 0;

            var currChance = 0M;
            decimal propabilityAccur = chance / 100;

            for (int i = 0; i < prices.Count; i++)
            {
                currChance += prices[i].Probability;

                if (Math.Abs(currChance - chance) <= propabilityAccur)
                {
                    prev = prices[i];

                    if (i == prices.Count - 1)
                        next = prices[i];
                    else
                        next = prices[i + 1];

                    break;
                }

                if (currChance + propabilityAccur > chance)
                    return prices[i].Value;
            }

            return (prev.Value + next.Value) / 2;
        }

        public static List<PriceAggregate> GetQuantileArray(List<decimal> prices, decimal chance)
        {
            var orderd = AggregatePrices(prices);

            var val = CalculateQuantileValue(orderd, chance);

            return orderd.Where(x => x.Value <= val).ToList();
        }

        public static decimal GetQuantile(List<decimal> prices, decimal chance)
        {
            if (chance <= 0 || chance > 1)
                throw new ArgumentOutOfRangeException(nameof(chance), "Accuracy must be in (0, 1]");

            var orderd = AggregatePrices(prices);

            return CalculateQuantileValue(orderd, chance);
        }

        public static decimal GetAvarege(List<decimal> prices, decimal accuracy)
        {
            if (accuracy <= 0 || accuracy > 1)
                throw new ArgumentOutOfRangeException(nameof(accuracy), "Accuracy must be in (0, 1]");

            var aggregated = AggregatePrices(prices);

            var orderd = aggregated.OrderByDescending(x => x.Probability).ToList();
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
