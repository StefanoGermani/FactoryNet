using System.Collections.Generic;
using FactoryNet.Core.Helpers;

namespace FactoryNet.Core.Dictionaries
{
    internal interface ICreatedBlueprintsDictionary
    {
        void Add<T>(string variation, T createdObject);
        bool ContainsKey<T>(string variation);
        T Get<T>(string varation);
    }

    internal class CreatedBlueprintsDictionary : ICreatedBlueprintsDictionary
    {
        private readonly IBluePrintKeyHelper _bluePrintKeyHelper;

        private readonly Dictionary<string, object> _createdBluePrints = new Dictionary<string, object>();

        public CreatedBlueprintsDictionary(IBluePrintKeyHelper bluePrintKeyHelper)
        {
            _bluePrintKeyHelper = bluePrintKeyHelper;
        }

        public void Add<T>(string variation, T createdObject)
        {
            _createdBluePrints.Add(_bluePrintKeyHelper.GetBluePrintKey<T>(variation), createdObject);
        }

        public bool ContainsKey<T>(string variation)
        {
            return _createdBluePrints.ContainsKey(_bluePrintKeyHelper.GetBluePrintKey<T>(variation));
        }

        public T Get<T>(string varation)
        {
            return (T)_createdBluePrints[_bluePrintKeyHelper.GetBluePrintKey<T>(varation)];
        }
    }
}