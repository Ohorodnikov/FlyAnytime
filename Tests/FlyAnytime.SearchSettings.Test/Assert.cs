using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchSettings.Test
{
    public static class Assert
    {
        public static void AreEqual<TItem>(TItem expected, TItem actual)
            where TItem: IEquatable<TItem>
        {
            if (!expected.Equals(actual))
            {
                throw new Exception();
            }
        }
    }
}
