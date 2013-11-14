using System.Collections.Generic;

namespace Plant.Core.Impl.Dictionaries
{
    internal class CreatedBlueprintsDictionary : BaseDictionary
    {
        private readonly Dictionary<string, object> _createdBluePrints = new Dictionary<string, object>();

        public void Add<T>(string variation, T createdObject)
        {
            _createdBluePrints.Add(BluePrintKey<T>(variation), createdObject);
        }

        public bool ContainsKey<T>(string variation)
        {
            return _createdBluePrints.ContainsKey(BluePrintKey<T>(variation));
        }

        public T Get<T>(string varation)
        {
            return (T)_createdBluePrints[BluePrintKey<T>(varation)];
        }
    }
}