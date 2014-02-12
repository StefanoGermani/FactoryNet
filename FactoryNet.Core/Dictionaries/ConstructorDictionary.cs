using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FactoryNet.Core.Exceptions;
using FactoryNet.Core.Helpers;

namespace FactoryNet.Core.Dictionaries
{
    internal interface IConstructorDictionary
    {
        NewExpression Get<T>(string variation);
        void Add<T>(string variation, NewExpression newExpression);
        bool ContainsType<T>(string variation);
        bool ContainsType(string variation, Type type);
    }

    internal class ConstructorDictionary : IConstructorDictionary
    {
        private readonly IBluePrintKeyHelper _bluePrintKeyHelper;
        private readonly Dictionary<string, NewExpression> _constructors = new Dictionary<string, NewExpression>();

        public ConstructorDictionary(IBluePrintKeyHelper bluePrintKeyHelper)
        {
            _bluePrintKeyHelper = bluePrintKeyHelper;
        }

        public NewExpression Get<T>(string variation)
        {
            if (_constructors.All(a => a.Key != _bluePrintKeyHelper.GetBluePrintKey<T>(variation)))
            {
                throw new TypeNotSetupException(typeof(T));
            }

            return _constructors[_bluePrintKeyHelper.GetBluePrintKey<T>(variation)];
        }

        public void Add<T>(string variation, NewExpression newExpression)
        {
            _constructors.Add(_bluePrintKeyHelper.GetBluePrintKey<T>(variation), newExpression);
        }

        public bool ContainsType<T>(string variation)
        {
            return _constructors.ContainsKey(_bluePrintKeyHelper.GetBluePrintKey<T>(variation));
        }

        public bool ContainsType(string variation, Type type)
        {
            return _constructors.ContainsKey(_bluePrintKeyHelper.GetBluePrintKey(variation, type));
        }
    }
}