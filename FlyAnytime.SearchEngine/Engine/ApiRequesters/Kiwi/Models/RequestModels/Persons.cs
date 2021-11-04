using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    /// <summary>
    /// Used to specify the number of adults. Please note, that children are considered adults in our search engine. The default passengers' value is 1. The sum of adults, children and infants cannot be greater than 9.
    /// </summary>
    public class Adults : BaseNumericParam
    {
        public Adults(long value) : base("adults", value) { }
    }

    /// <summary>
    /// It specifies the number of children. The default value is 0. The sum of adults, children and the infants cannot be greater than 9. At the moment, children are considered as adults in most of the provided content. Whenever we have the child fare available for some content it will be visible in the response.
    /// </summary>
    public class Children : BaseNumericParam
    {
        public Children(long value) : base("children", value) { }
    }

    /// <summary>
    /// Parameter used to specify the number of infants. The default value is 0. The sum of adults, children and infants cannot be greater than 9.
    /// </summary>
    public class Infants : BaseNumericParam
    {
        public Infants(long value) : base("infants", value) { }
    }

    public static class PersonsParamHelper
    {
        public static IRequestDescriptor AddPersons(this IRequestDescriptor setter, int adults, int children, int infants)
        {
            if (adults < 0 || children < 0 || infants < 0 || adults + children + infants > 9)
                throw new ArgumentOutOfRangeException("Max count of adults + children + infants must be less than 9");

            setter.AddParam(new Adults(adults));
            setter.AddParam(new Children(children));
            setter.AddParam(new Infants(infants));

            return setter;
        }
    }
}
