using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PlantFarm.Core.Exceptions;
using PlantFarm.Core.Helpers;

namespace PlantFarm.Core.Dictionaries
{
    internal class ConstructorDictionary 
    {
        private readonly IConstructorHelper _constructorHelper;
        private readonly IBluePrintKeyHelper _bluePrintKeyHelper;
        private readonly Dictionary<string, Func<object>> _constructors = new Dictionary<string, Func<object>>();

        public ConstructorDictionary(IConstructorHelper constructorHelper, IBluePrintKeyHelper bluePrintKeyHelper)
        {
            _constructorHelper = constructorHelper;
            _bluePrintKeyHelper = bluePrintKeyHelper;
        }

        public T CreateIstance<T>(string variation)
        {
            if (_constructors.All(a => a.Key != _bluePrintKeyHelper.GetBluePrintKey<T>(variation)))
            {
                throw new TypeNotSetupException(typeof(T));
            }

            return (T)_constructors[_bluePrintKeyHelper.GetBluePrintKey<T>(variation)]();
        }

        public void Add<T>(string variation, NewExpression newExpression)
        {
            Func<object> costructor = () => _constructorHelper.CreateInstance<T>(newExpression);

            _constructors.Add(_bluePrintKeyHelper.GetBluePrintKey<T>(variation), costructor);
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