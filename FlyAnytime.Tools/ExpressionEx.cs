using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FlyAnytime.Tools
{
    public static class ExpressionEx
    {
        public static string GetStringBody<TEntity, TResult>(this Expression<Func<TEntity, TResult>> property)
        {
            var propName = property.ToString();
            var parts = propName.Split("=>");
            var paramName = parts[0].Trim();

            var body = parts[1].Trim();
            body = body.Remove(0, paramName.Length + 1); //paramName.Prop.Prop2 become Prop.Prop2

            return body;
        }
    }
}
