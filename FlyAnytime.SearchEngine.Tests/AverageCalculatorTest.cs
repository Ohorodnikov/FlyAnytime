using FlyAnytime.SearchEngine.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace FlyAnytime.SearchEngine.Tests
{
    public class AverageCalculatorTest
    {
        [Fact]
        public void AverageTest1()
        {
            var price2Count = new Dictionary<decimal, int>
            {
                { 13, 2 },
                { 14, 15 },
                { 14.5M, 25 },
                { 15, 10 },
                { 16, 3 }
            };

            var calculator = new AverageCalculator();

            var prices = new List<decimal>();
            foreach (var setup in price2Count)
                for (int i = 0; i < setup.Value; i++)
                    prices.Add(setup.Key);

            var av = AverageCalculator.GetAvarege(prices, 0.7M);

            Assert.Equal(14.3M, av, 1);
        }

        [Fact]
        public void AverageTest2()
        {
            var price2Count = new Dictionary<decimal, int>
            {
                { 13, 20 },
                { 14, 150 },
                { 14.5M, 250 },
                { 15, 100 },
                { 16, 30 }
            };

            var calculator = new AverageCalculator();

            var prices = new List<decimal>();
            foreach (var setup in price2Count)
                for (int i = 0; i < setup.Value; i++)
                    prices.Add(setup.Key);

            var av = AverageCalculator.GetAvarege(prices, 0.7M);

            Assert.Equal(14.3M, av, 1);
        }

        [Fact]
        public void AverageTestLargeData()
        {
            var totalCountPrices = 10_000_000;

            //3% of values in diapason 100-200
            //4% of values in diapason 200-220
            //6% of values in diapason 220-240
            //15% of values in diapason 240-250 //avarege is 245 //1500000 items
            //40% of values in diapason 250-255 //average is 252.5 //4000000 items
            //20% of values in diapason 255-260 //average is 257.5 //2000000 items
            //7% of values in diapason 260-280 //average is 270 //700000 items
            //4% of values in diapason 280-300
            //1% of values in diapason 300-400

            //Expected for accuracy 70% = (245*1500000 + 252.5*4000000 + 257.5*2000000)/(1500000+4000000+2000000)
            //= (245*1.5+252.5*4+257.5*2)/(1.5+4+2) = 252.33

            //Expected for accuracy 80% = (245*1.5+252.5*4+257.5*2+270*0.7)/(1.5+4+2+0.7) = 253.84

            int GetCountForPercent(int percent) => totalCountPrices * percent / 100;

            var prices = new List<decimal>();
            prices.AddRange(GetRandomValues(100, 200, GetCountForPercent(3)));
            prices.AddRange(GetRandomValues(200, 220, GetCountForPercent(4)));
            prices.AddRange(GetRandomValues(220, 240, GetCountForPercent(6)));
            prices.AddRange(GetRandomValues(240, 250, GetCountForPercent(15)));
            prices.AddRange(GetRandomValues(250, 255, GetCountForPercent(40)));
            prices.AddRange(GetRandomValues(255, 260, GetCountForPercent(20)));
            prices.AddRange(GetRandomValues(260, 280, GetCountForPercent(7)));
            prices.AddRange(GetRandomValues(280, 300, GetCountForPercent(4)));
            prices.AddRange(GetRandomValues(300, 400, GetCountForPercent(1)));

            var arr = prices.ToArray();

            Shuffle(rnd, arr);

            prices = arr.ToList();

            var calculator = new AverageCalculator();

            void DoAssert(decimal expected, decimal accuracy)
            {
                var av = AverageCalculator.GetAvarege(prices, accuracy);

                var diff = Math.Abs(expected - av);
                var allowedDiff = expected * (decimal)0.005; //0.5% of expected
                Assert.True(allowedDiff > diff);
            }

            DoAssert(252.33M, 0.7M);
            DoAssert(253.84M, 0.8M);
            
        }

        Random rnd = new Random();
        private static void Shuffle<T>(Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
        private IEnumerable<decimal> GetRandomValues(int min, int max, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var doub = rnd.NextDouble() * (max - min) + min;
                var d = new decimal(doub);
                yield return d;
            }
        }
    }
}
