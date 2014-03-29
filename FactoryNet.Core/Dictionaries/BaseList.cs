using System;
using System.Collections.Generic;
using System.Linq;
using FactoryNet.Core.Exceptions;

namespace FactoryNet.Core.Dictionaries
{
    internal class BaseList<TProperty> : List<Tuple<Type, string, TProperty>>
    {
        public virtual TProperty Get<T>(string variation)
        {
            if (!this.Any(a => a.Item1 == typeof(T) && a.Item2 == variation))
            {
                throw new TypeNotSetupException(typeof(T));
            }

            return this.First(a => a.Item1 == typeof(T) && a.Item2 == variation).Item3;
        }

        public virtual void Add<T>(string variation, TProperty newExpression)
        {
            Add(new Tuple<Type, string, TProperty>(typeof(T), variation, newExpression));
        }

        public virtual bool ContainsKey<T>(string variation)
        {
            return this.Any(a => a.Item1 == typeof(T) && a.Item2 == variation);
        }

        public virtual bool ContainsKey(string variation, Type type)
        {
            return this.Any(a => a.Item1 == type && a.Item2 == variation);
        }
    }
}