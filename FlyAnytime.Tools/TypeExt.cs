using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Tools
{
    public static class TypeExt
    {
        public static bool IsImplementInterface(this Type type, Type interfaceType)
        {
            return interfaceType.IsAssignableFrom(type);
        }
    }
}
