using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.UrlShortener.Shortener
{
    public interface ILongShortener
    {
        string Compress(long number);
        long Restore(string compressedString);
    }

    public class LongShortener : ILongShortener
    {
        //https://github.com/MrModest/LinkShortener/blob/master/LinkShortener/ServiceImpls/ShortenerService.cs
        private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";
        private static readonly IDictionary<char, int> AlphabetIndex;
        private static readonly int Base = Alphabet.Length;

        static LongShortener()
        {
            AlphabetIndex = Alphabet
                .Select((c, i) => new { Index = i, Char = c })
                .ToDictionary(c => c.Char, c => c.Index);
        }

        public string Compress(long number)
        {
            if (number < Base)
                return Alphabet[(int)number].ToString();

            var str = new StringBuilder();
            var i = number;

            while (i > 0)
            {
                var modulus = (int)(i % Base);
                str.Insert(0, Alphabet[modulus]);
                i /= Base;
            }

            return str.ToString();
        }

        public long Restore(string compressedString)
        {
            long zero = 0;
            return compressedString.Aggregate(zero, (current, c) => current * Base + AlphabetIndex[c]);
        }
    }
}
