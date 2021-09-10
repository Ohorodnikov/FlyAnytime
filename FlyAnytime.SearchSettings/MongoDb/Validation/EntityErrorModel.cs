using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FlyAnytime.SearchSettings.MongoDb.Validation
{
    public class EntityErrorModel
    {
        public EntityErrorModel()
        {

        }

        public EntityErrorModel(Exception exception)
        {
            Exception = exception;
        }

        public bool HasErrors => HasValidationError || Exception != null;

        public bool HasValidationError => PropertyErrors.Any();

        public readonly Dictionary<string, List<string>> PropertyErrors = new Dictionary<string, List<string>>();

        public Exception Exception { get; }

        public void AddValidationError(string propName, string message)
        {
            PropertyErrors.TryGetValue(propName, out var errorList);

            errorList.Add(message);

            PropertyErrors[propName] = errorList;
        }

        public void AddValidationError<TEntity>(Expression<Func<TEntity, object>> property, string message)
        {
            AddValidationError(property.ToString(), message);
        }
    }
}
