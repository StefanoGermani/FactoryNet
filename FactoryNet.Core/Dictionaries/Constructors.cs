using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FactoryNet.Core.Exceptions;

namespace FactoryNet.Core.Dictionaries
{
    internal interface IConstructors
    {
        NewExpression Get<T>(string variation);
        void Add<T>(string variation, NewExpression newExpression);
        bool ContainsKey<T>(string variation);
        bool ContainsKey(string variation, Type type);
    }

    internal class Constructors : BaseList<NewExpression>, IConstructors
    {
    }
}