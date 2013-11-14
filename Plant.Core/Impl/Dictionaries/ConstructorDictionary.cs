using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Plant.Core.Exceptions;
using Plant.Core.Impl.Dictionaries;

namespace Plant.Core.Impl
{
    internal class ConstructorDictionary : BaseDictionary
    {
        private readonly Dictionary<string, Func<object>> _constructors = new Dictionary<string, Func<object>>();

        public T CreateIstance<T>(string variation)
        {
            if (_constructors.All(a => a.Key != BluePrintKey<T>(variation)))
            {
                throw new TypeNotSetupException(typeof(T));
            }

            return (T)_constructors[BluePrintKey<T>(variation)]();
        }

        public void Add<T>(string variation, NewExpression newExpression)
        {
            Func<object> costructor = () => newExpression.Constructor.Invoke(newExpression.Arguments.Select(a => ExecuteExpression(a)).ToArray());

            _constructors.Add(BluePrintKey<T>(variation), costructor);
        }

        private object ExecuteExpression(Expression expression)
        {
            LambdaExpression lambda = Expression.Lambda(expression);
            Delegate compiled = lambda.Compile();

            return compiled.DynamicInvoke(null);
        }

        public bool ContainsType<T>(string variation)
        {
            return _constructors.ContainsKey(BluePrintKey<T>(variation));
        }

        public bool ContainsType(string variation, Type type)
        {
            return _constructors.ContainsKey(BluePrintKey(variation, type));
        }
    }
}