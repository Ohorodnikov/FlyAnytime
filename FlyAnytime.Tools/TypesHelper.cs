using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FlyAnytime.Tools
{
    public class TypesHelper
    {
        private static List<Type> allTypes;
        public static IEnumerable<Type> GetAllTypes()
        {
            if (allTypes != null)
                return allTypes;

            allTypes = Assembly
                .GetEntryAssembly()
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Append(Assembly.GetEntryAssembly())
                .SelectMany(x => x.DefinedTypes)
                .Select(x => x.AsType())
                .ToList();

            return allTypes;
        }
    }
}
