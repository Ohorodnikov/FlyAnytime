using FlyAnytime.Tools;
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

        public Dictionary<string, List<string>> PropertyErrors { get; } = new Dictionary<string, List<string>>();

        public Exception Exception { get; }

        public void AddValidationError(string propName, string message)
        {
            PropertyErrors.TryGetValue(propName, out var errorList);

            errorList = errorList ?? new List<string>();

            errorList.Add(message);

            PropertyErrors[propName] = errorList;
        }
    }

    public class EntityErrorModel<TEntity> : EntityErrorModel
    {
        public void AddValidationError<TResult>(Expression<Func<TEntity, TResult>> property, string message)
        {
            AddValidationError(property.GetStringBody(), message);
        }
    }
}
