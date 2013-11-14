using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Plant.Core.Exceptions;

namespace Plant.Core.Impl
{
    public class ConstructorDictionary
    {
        private readonly Dictionary<Type, Func<object>> _constructors = new Dictionary<Type, Func<object>>();

        public T CreateIstance<T>()
        {
            if (_constructors.All(a => a.Key != typeof(T)))
            {
                throw new TypeNotSetupException(typeof(T));
            }

            return (T)_constructors[typeof(T)]();
        }

        public void Add<T>(NewExpression newExpression)
        {
            Func<object> costructor = () => newExpression.Constructor.Invoke(newExpression.Arguments.Select(a => ((ConstantExpression)a).Value).ToArray());

            _constructors.Add(typeof(T), costructor);
        }

        public bool ContainsType<T>()
        {
            return _constructors.ContainsKey(typeof(T));
        }

        public bool ContainsType(Type type)
        {
            return _constructors.ContainsKey(type);
        }
    }
}